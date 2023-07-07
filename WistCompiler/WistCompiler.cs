namespace WistCompiler;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

public static class WistCompiler
{
    private static List<string> _args = null!;

    private static List<WistCompilerClass> _classes = null!;
    private static WistCompilerClass _curClass = null!;
    private static Dictionary<string, GroboIL.Label> _labels = null!;

    private static Dictionary<string, GroboIL.Local> _locals = null!;

    public static WistExecutableObject Compile(WistImageObject image)
    {
        _classes = new List<WistCompilerClass>();
        _locals = new Dictionary<string, GroboIL.Local>();
        _labels = new Dictionary<string, GroboIL.Label>();

        foreach (var c in image.Classes) CreateClass(c.Fields, c.Name);

        foreach (var c in image.Classes)
        {
            foreach (var m in c.Methods)
                CreateMethod(m.Name, m.Args, m.Instructions);
        }

        return new WistExecutableObject(_classes.Select(x => x.Class).ToList());
    }

    private static void CreateMethod(string name, string[] args, List<WistInstruction> instructions)
    {
        var m = new DynamicMethod(name, typeof(WistConst), new[] { typeof(WistConst) });
        var il = new GroboIL(m);
        _curClass.Class.AddMethod(name.GenerateHashCode(), m);
        _args = args.ToList();

        foreach (var instr in instructions)
            CompileOneOp(il, instr.Op, instr.Arg!);

        il.Ret();
    }

    // ReSharper disable once ParameterTypeCanBeEnumerable.Local
    private static void CreateClass(string[] fields, string name)
    {
        _curClass = new WistCompilerClass(name,
            new WistClass(new List<(int id, WistConst value)>(), new List<(int id, DynamicMethod method)>()));
        _classes.Add(_curClass);
        WistCompilerHelper.Classes.Add(_curClass.Name.GenerateHashCode(), _curClass.Class);

        foreach (var f in fields)
            _curClass.Class.AddField(f.GenerateHashCode());
    }

    private static void CompileOneOp(GroboIL il, WistOp op, object const1)
    {
        var constStr = (const1 as string)!;
        GroboIL.Local? classLocal;
        switch (op)
        {
            case WistOp.Add:
                il.Call(typeof(WistConst).GetMethod(nameof(WistConst.Sum)));
                break;
            case WistOp.Sub:
                il.Call(typeof(WistConst).GetMethod(nameof(WistConst.Sub)));
                break;
            case WistOp.Mul:
                il.Call(typeof(WistConst).GetMethod(nameof(WistConst.Mul)));
                break;
            case WistOp.Div:
                il.Call(typeof(WistConst).GetMethod(nameof(WistConst.Div)));
                break;
            case WistOp.CreateClass:
                var cl = _classes.Find(x => x.Name == constStr) ??
                         throw new WistError($"Can't find class with name {constStr}");

                il.Ldc_I4(cl.Name.GenerateHashCode());
                il.Call(typeof(WistCompilerHelper).GetMethod(nameof(WistCompilerHelper.PushClass)));

                il.Newobj(typeof(WistConst).GetConstructor(
                        BindingFlags.Public | BindingFlags.Instance,
                        new[] { typeof(WistClass) }
                    )
                );
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