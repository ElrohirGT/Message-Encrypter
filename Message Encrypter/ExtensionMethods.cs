using System.Text;

namespace Message_Encrypter;
public static class ExtensionMethods
{
    public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes);
    public static byte[] ToBytes(this string str, Encoding? strEncoding = null)
    {
        strEncoding ??= Encoding.Unicode;
        var bytes  = strEncoding.GetBytes(str);
        return Convert.FromBase64String(bytes.ToBase64String());
    }
    public static string ToUnicodeString(this byte[] bytes) => Encoding.Unicode.GetString(bytes);
}
