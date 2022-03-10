using System.Security.Cryptography;

using Message_Encrypter.Encrypters.Channels;

namespace Message_Encrypter.Encrypters;
public class FileEncrypter : IEncrypter
{
    public const string SEPARATOR = @"§";
    readonly IChannel _channel;

    public FileEncrypter(IChannel channel) => _channel = channel;

    public byte[] Decrypt(string data)
    {
        using var crypt = Aes.Create();

        var fileText = File.ReadAllText(data);

        string[] input = fileText.Split(SEPARATOR) ?? Array.Empty<string>();
        byte[] key = Convert.FromBase64String(input[0]);
        byte[] IV = Convert.FromBase64String(input[1]);
        byte[] fileName = Convert.FromBase64String(input[2]);
        byte[] value = Convert.FromBase64String(string.Join(string.Empty, input[3..]));

        crypt.IV = IV;
        crypt.Key = key;
        var decryptedFileName = crypt.DecryptCbc(fileName, IV);
        var decryptedValue = crypt.DecryptCbc(value, IV);

        crypt.Clear();
        _channel.SendDencrypted($"{decryptedFileName.ToBase64String()}{SEPARATOR}{decryptedValue.ToBase64String()}");
        return decryptedValue;
    }

    public byte[] Encrypt(string data)
    {
        using var crypt = Aes.Create();
        crypt.GenerateKey();
        crypt.GenerateIV();

        var dataBytes = File.ReadAllBytes(data);
        var encryptedData = crypt.EncryptCbc(dataBytes, crypt.IV);
        var fileName = Path.GetFileName(data);
        var encryptedFileName = crypt.EncryptCbc(fileName.ToBytes(), crypt.IV);

        //key - IV - fileName - data
        var encryptedValueInText = $"{crypt.Key.ToBase64String()}{SEPARATOR}{crypt.IV.ToBase64String()}{SEPARATOR}{encryptedFileName.ToBase64String()}{SEPARATOR}{encryptedData.ToBase64String()}";
        crypt.Clear();

        _channel.SendEncrypted(encryptedValueInText);
        return encryptedData;
    }
}
