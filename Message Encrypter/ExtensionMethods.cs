using System.Text;

namespace Message_Encrypter;
public static class ExtensionMethods
{
    public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes);
    public static byte[] ToBytes(this string str, Encoding? strEncoding = null)
    {
        strEncoding ??= Encoding.Unicode;
        var bytes  = strEncoding.GetBytes(str);
        return bytes;
    }
    public static string ToUnicodeString(this byte[] bytes) => Encoding.Unicode.GetString(bytes);

    public static void ReadArray(this FileStream stream, ref byte[] array)
    {
        while (stream.Read(array, 0, array.Length) != array.Length)
        {
            //It doesn't need to do anything here as it already sets the values to array.
        }
    }
}
