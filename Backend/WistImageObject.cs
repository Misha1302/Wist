namespace Backend;

using System.Text;

[Serializable]
public record WistImageObject(List<WistConst> Consts, List<WistConst> Consts2, List<WistOp> Ops)
{
    public override string ToString()
    {
        StringBuilder sb = new();
        for (var i = 0; i < Ops.Count; i++)
            sb.AppendLine($"{Ops[i]} : {Consts[i]} : {Consts2[i]}");

        return sb.ToString();
    }
}