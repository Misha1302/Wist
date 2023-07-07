namespace WistCompiler;

using System.Security.Cryptography;
using System.Text;

public static class WistHashGenerator
{
    private static readonly Dictionary<int, string> _dict = new();

    public static int GenerateHashCode(this string s)
    {
        var inputBytes = Encoding.Unicode.GetBytes(s);
        var hashBytes = MD5.HashData(inputBytes);

        var hash = BitConverter.ToInt32(hashBytes);
        _dict.TryAdd(hash, s);
        return hash;
    }

    public static string GetStringFromHash(int hash) => _dict[hash];
}