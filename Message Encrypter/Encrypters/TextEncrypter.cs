using System.Security.Cryptography;

using Message_Encrypter.Encrypters.Channels;

namespace Message_Encrypter.Encrypters;
public class TextEncrypter : IEncrypter
{
    public const string SEPARATOR = @"§";
    IChannel _channel;

    public TextEncrypter(IChannel channel) => _channel = channel;

    public byte[] Encrypt(string message)
    {
        using var crypt = Aes.Create();
        crypt.GenerateKey();
        crypt.GenerateIV();

        var encryptedValue = crypt.EncryptCbc(message.ToBytes(), crypt.IV, PaddingMode.Zeros);
        string encryptedValueInText = $"{crypt.Key.ToBase64String()}{SEPARATOR}{crypt.IV.ToBase64String()}{SEPARATOR}{encryptedValue.ToBase64String()}";

        crypt.Clear();
        _channel.SendEncrypted(encryptedValueInText);
        return encryptedValue;
    }

    public byte[] Decrypt(string encryptedValue)
    {
        using var crypt = Aes.Create();

        string[] input = encryptedValue.Split(SEPARATOR) ?? Array.Empty<string>();
        byte[] key = Convert.FromBase64String(input[0]);
        byte[] IV = Convert.FromBase64String(input[1]);
        byte[] value = Convert.FromBase64String(string.Join(string.Empty, input[2..]));

        crypt.IV = IV;
        crypt.Key = key;
        var decryptedValue = crypt.DecryptCbc(value, IV, PaddingMode.Zeros);
        crypt.Clear();

        string decryptedValueInText = decryptedValue.ToUnicodeString();
        _channel.SendDencrypted(decryptedValueInText);
        return decryptedValue;
    }
}
