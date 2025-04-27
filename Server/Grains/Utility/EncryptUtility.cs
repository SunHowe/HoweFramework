using System.Security.Cryptography;
using System.Text;

namespace Grains.Utility;

/// <summary>
/// 加密工具。
/// </summary>
public static class EncryptUtility
{
    /// <summary>
    /// MD5加密。
    /// </summary>
    public static string MD5Encrypt(string str)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(str));
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}