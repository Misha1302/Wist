namespace WistCompiler;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

public class WistCompiler
{
    private static WistCompiler? _instance;
    private List<string> _args = null!;

    private List<WistClass> _classes = null!;
    private WistClass _curClass = null!;
    private Dictionary<string, GroboIL.Label> _labels = null!;

    private Dictionary<string, GroboIL.Local> _locals = null!;
    public static WistCompiler Instance => _instance ??= new WistCompiler();

    public WistExecutableObject Compile(WistImageObject image)
    {
        _classes = new List<WistClass>();
        _locals = new Dictionary<string, GroboIL.Local>();
        _labels = new Dictionary<string, GroboIL.Label>();

        foreach (var c in image.Classes)
        {
            CreateClass(c.Fields);

            foreach (var m in c.Methods)
                CreateMethod(m.Name, m.Args, m.Instructions);
        }

        return new WistExecutableObject(_classes);
    }

    private void CreateMethod(string name, string[] args, List<WistInstruction> instructions)
    {
        var m = new DynamicMethod(name, typeof(WistConst), new[] { typeof(WistConst) });
        var il = new GroboIL(m);
        _curClass.AddMethod(name.GenerateHashCode(), m);
        _args = args.ToList();

        foreach (var instr in instructions)
            CompileOneOp(il, instr.Op, instr.Arg!);

        il.Ret();
    }

    // ReSharper disable once ParameterTypeCanBeEnumerable.Local
    private void CreateClass(string[] fields)
    {
        _curClass = new WistClass(new List<(int id, WistConst value)>(), new List<(int id, DynamicMethod method)>());
        _classes.Add(_curClass);

        foreach (var f in fields)
            _curClass.AddField(f.GenerateHashCode());
    }

    private void CompileOneOp(GroboIL il, WistOp op, object const1)
    {
        var constStr = (const1 as string)!;
        GroboIL.Local? classLocal;
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
            case WistOp.IsNotEquals:
                il.Call(typeof(WistConst).GetMethod("IsNotEquals"));
                break;
            case WistOp.LoadLocal:
                il.Ldloc(_locals[constStr]);
                break;
            case WistOp.SetLabel:
                var label = il.DefineLabel(constStr);
                il.MarkLabel(label);
                _labels.Add(constStr, label);
                break;
            case WistOp.GoTo:
                il.Br(_labels[constStr]);
                break;
            case WistOp.GoToIfTrue:
                var name = il.DeclareLocal(typeof(WistConst));
                il.Stloc(name);
                il.Ldloca(name);
                il.Call(typeof(WistConst).GetMethod("GetBool"));
                il.Brtrue(_labels[constStr]);
                break;
            case WistOp.LessThan:
                il.Call(typeof(WistConst).GetMethod("LessThan"));
                break;
            case WistOp.SetLocal:
                if (!_locals.ContainsKey(constStr))
                    _locals.Add(constStr, il.DeclareLocal(typeof(WistConst), constStr));
                il.Stloc(_locals[constStr]);
                break;
            case WistOp.SetField:
                var valueLocal = il.DeclareLocal(typeof(WistConst));
                classLocal = il.DeclareLocal(typeof(WistConst));
                il.Stloc(valueLocal); // value

                il.Stloc(classLocal); // class
                il.Ldloca(classLocal); // class
                il.Call(typeof(WistConst).GetMethod("GetClass")); // class

                il.Ldc_I4(constStr.GenerateHashCode()); // id
                il.Ldloc(valueLocal); // value
                il.Call(typeof(WistClass).GetMethod("SetField")); // int id, WistConst value
                break;
            case WistOp.LoadArg:
                il.Ldarg(_args.IndexOf(constStr));
                break;
            case WistOp.LoadField:
                classLocal = il.DeclareLocal(typeof(WistConst));
                il.Stloc(classLocal); // class
                il.Ldloca(classLocal); // class

                il.Call(typeof(WistConst).GetMethod("GetClass"));
                il.Ldc_I4(constStr.GenerateHashCode()); // id
                il.Call(typeof(WistClass).GetMethod("GetField"));
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