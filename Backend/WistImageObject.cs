namespace Backend;

using System.Text;

[Serializable]
public record WistImageObject(List<WistConst> Consts, List<WistConst> Consts2, List<WistOp> Ops)
{
    public override string ToString() => WistToStringManager.ToStr(this);
}