using System.Text;

namespace Message_Encrypter;
public static class ExtensionMethods
{
    public static string ToUnicodeString(this byte[] bytes) => Encoding.Unicode.GetString(bytes);
    public static byte[] ToBytes(this string str) => Encoding.Unicode.GetBytes(str);
}
