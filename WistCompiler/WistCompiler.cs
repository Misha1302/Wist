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

    private List<Type> _classes = null!;

    private TypeBuilder _curClass = null!;
    private Type _curType = null!;
    private Dictionary<string, FieldBuilder> _fields = null!;
    private Dictionary<string, GroboIL.Label> _labels = null!;

    private Dictionary<string, GroboIL.Local> _locals = null!;
    private List<DynamicMethod> _methods = null!;
    public static WistCompiler Instance => _instance ??= new WistCompiler();

    public WistExecutableObject Compile(WistImageObject image)
    {
        _methods = new List<DynamicMethod>();
        _classes = new List<Type>();
        _locals = new Dictionary<string, GroboIL.Local>();
        _labels = new Dictionary<string, GroboIL.Label>();
        _fields = new Dictionary<string, FieldBuilder>();

        foreach (var c in image.Classes)
        {
            CreateClass(c.Name, c.Fields);

            foreach (var m in c.Methods)
                CreateMethod(m.Name, m.Instructions);
        }

        return new WistExecutableObject(_methods, _classes);
    }

    private void CreateMethod(string name, List<WistInstruction> instructions)
    {
        var m = new DynamicMethod(name, typeof(WistConst), new[] { typeof(WistConst) });
        var il = new GroboIL(m);
        _methods.Add(m);

        foreach (var instr in instructions)
            CompileOneOp(il, instr.Op, instr.Arg!);

        il.Ret();
    }

    private void CreateField(string name)
    {
        var field = _curClass.DefineField(name, typeof(WistConst), FieldAttributes.Public);
        _fields.Add(name, field);
    }

    private void CreateClass(string name, string[] fields)
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

        foreach (var f in fields)
            CreateField(f);

        _curType = tb.CreateType();
        _classes.Add(_curType);
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
            case WistOp.CallExternMethod:
                il.Call((MethodInfo)const1);
                break;
            case WistOp.Drop:
                il.Pop();
                break;
            case WistOp.LoadLocal:
                il.Ldloc(_locals[(string)const1]);
                break;
            case WistOp.SetLabel:
                var label = il.DefineLabel((string)const1);
                il.MarkLabel(label);
                _labels.Add((string)const1, label);
                break;
            case WistOp.GoTo:
                il.Br(_labels[(string)const1]);
                break;
            case WistOp.GoToIfTrue:
                var name = il.DeclareLocal(typeof(WistConst));
                il.Stloc(name);
                il.Ldloca(name);
                il.Callnonvirt(typeof(WistConst).GetMethod("GetBool"));
                il.Brtrue(_labels[(string)const1]);
                break;
            case WistOp.LessThan:
                il.Call(typeof(WistConst).GetMethod("LessThan"));
                break;
            case WistOp.SetLocal:
                if (!_locals.ContainsKey((string)const1))
                    _locals.Add((string)const1, il.DeclareLocal(typeof(WistConst), (string)const1));
                il.Stloc(_locals[(string)const1]);
                break;
            case WistOp.SetField:
                var loc = il.DeclareLocal(typeof(WistConst), Guid.NewGuid().ToString());
                il.Stloc(loc);
                il.Ldarga(0);
                il.Call(typeof(WistConst).GetMethod("GetClass"));
                il.Castclass(_curType);
                il.Ldloc(loc);

                il.Stfld(_curType.GetField((string)const1));
                break;
            case WistOp.LoadField:
                il.Ldarga(0);
                il.Call(typeof(WistConst).GetMethod("GetClass"));
                il.Castclass(_curType);
                il.Ldfld(_curType.GetField((string)const1));
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