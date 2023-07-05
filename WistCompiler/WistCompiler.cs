namespace WistCompiler;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

public class WistCompiler
{
    private static WistCompiler? _instance;

    private readonly AssemblyBuilder _asm = AssemblyBuilder.DefineDynamicAssembly(
        new AssemblyName(Guid.NewGuid().ToString()),
        AssemblyBuilderAccess.Run
    );

    private TypeBuilder _curClass = null!;
    private Dictionary<string, FieldBuilder> _fields = null!;

    private Dictionary<string, GroboIL.Local> _locals = null!;
    public static WistCompiler Instance => _instance ??= new WistCompiler();

    public AssemblyBuilder Compile(WistImageObject image)
    {
        _locals = new Dictionary<string, GroboIL.Local>();
        _fields = new Dictionary<string, FieldBuilder>();

        foreach (var c in image.Classes)
        {
            CreateClass(c.Name);

            foreach (var m in c.Fields)
                CreateField(m);
            foreach (var m in c.Methods)
                CreateMethod(m.Name, m.Instructions);

            _curClass.CreateType();
        }

        return _asm;
    }

    private void CreateMethod(string name, List<WistInstruction> instrs)
    {
        var m1 = _curClass.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static, typeof(WistConst),
            new[] { typeof(WistConst) });
        var il = new GroboIL(m1);

        foreach (var i in instrs)
            CompileOneOp(il, i.Op, i.Arg!);

        il.Ret();
    }

    private void CreateField(string name)
    {
        var field = _curClass.DefineField(name, typeof(WistConst), FieldAttributes.Public);
        _fields.Add(name, field);
    }

    private void CreateClass(string name)
    {
        var moduleBuilder = _asm.DefineDynamicModule(name);
        var tb = moduleBuilder.DefineType(name,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            null);

        _curClass = tb;
    }

    private void CompileOneOp(GroboIL il, WistOp op, object const1)
    {
        switch (op)
        {
            case WistOp.Add:
                il.Call(typeof(WistConst).GetMethod("Sum"));
                break;
            case WistOp.Sub:
                il.Call(typeof(WistConst).GetMethod("Sub"));
                break;
            case WistOp.Mul:
                il.Call(typeof(WistConst).GetMethod("Mul"));
                break;
            case WistOp.Div:
                il.Call(typeof(WistConst).GetMethod("Div"));
                break;
            case WistOp.CreateLocal:
                _locals.Add((string)const1, il.DeclareLocal(typeof(WistConst), (string)const1));
                break;
            case WistOp.LoadLocal:
                il.Ldloc(_locals[(string)const1]);
                break;
            case WistOp.SetLocal:
                il.Stloc(_locals[(string)const1]);
                break;
            case WistOp.SetField:
                var loc = il.DeclareLocal(typeof(WistConst), Guid.NewGuid().ToString());
                il.Stloc(loc);
                il.Ldarga(0);
                il.Call(typeof(WistConst).GetMethod("GetClass"));
                il.Castclass(_curClass.CreateType());
                il.Ldloc(loc);

                il.Stfld(_curClass.CreateType().GetField((string)const1));
                break;
            case WistOp.LoadField:
                il.Ldarga(0);
                il.Call(typeof(WistConst).GetMethod("GetClass"));
                il.Castclass(_curClass.CreateType());
                il.Ldfld(_curClass.CreateType().GetField((string)const1));
                break;
            case WistOp.Push:
                Type type;
                switch (const1)
                {
                    case double d:
                        il.Ldc_R8(d);
                        type = typeof(double);
                        break;
                    case string s:
                        il.Ldstr(s);
                        type = typeof(string);
                        break;
                    case bool b:
                        il.Ldc_I4(b ? 1 : 0);
                        type = typeof(bool);
                        break;
                    default:
                        throw new WistError($"Unknown type {const1.GetType()}");
                }

                il.Newobj(typeof(WistConst).GetConstructor(new[] { type }));

                break;
            default:
                throw new WistError($"Unknown instruction {op}");
        }
    }
}