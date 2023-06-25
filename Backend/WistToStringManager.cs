namespace Backend;

using System.Globalization;
using System.Text;

public static class WistToStringManager
{
    public static string ToStr(WistConst wistConst)
    {
        return wistConst.Type switch
        {
            WistType.Number => $"{wistConst.GetNumber().ToString(CultureInfo.InvariantCulture)}",
            WistType.Bool => $"{wistConst.GetBool()}",
            WistType.InternalInteger => $"{wistConst.GetInternalInteger()}",
            WistType.Pointer => $"{wistConst.GetInternalPtr()}",
            WistType.String => $"{wistConst.GetString()}",
            WistType.None => "<<Undefined>>",
            WistType.Null => "None",
            WistType.List => $"[{string.Join(", ", wistConst.GetList())}]",
            WistType.Class => $"{{{wistConst.GetClass()}}}",
            _ => throw new WistException($"Unknown type - {wistConst.Type}")
        };
    }

    public static string ToStr(WistImageObject wistImageObject)
    {
        StringBuilder sb = new();
        for (var i = 0; i < wistImageObject.Ops.Count; i++)
            if (wistImageObject.Consts2[i].Type != WistType.None)
                sb.AppendLine($"{wistImageObject.Ops[i]} : {wistImageObject.Consts[i]} : {wistImageObject.Consts2[i]}");
            else sb.AppendLine($"{wistImageObject.Ops[i]} : {wistImageObject.Consts[i]}");

        return sb.ToString();
    }

    public static string ToStr(WistClass wistClass)
    {
        return string.Join(", ", wistClass.GetAllFields().Select(x => x.Item2));
    }
}