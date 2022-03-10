using System.Security.Cryptography;

using Message_Encrypter.Encrypters.Channels;

namespace Message_Encrypter.Encrypters;
public class FileEncrypter : IEncrypter
{
    readonly IChannel _channel;

    public FileEncrypter(IChannel channel) => _channel = channel;

    public byte[] Decrypt(string data)
    {
        using var crypt = Aes.Create();

        using var encryptedFileStream = File.OpenRead(data);

        var key = new byte[32];
        encryptedFileStream.ReadArray(ref key);

        var IV = new byte[16];
        encryptedFileStream.ReadArray(ref IV);

        var fileNameCount = new byte[4];
        encryptedFileStream.ReadArray(ref fileNameCount);
        int fileNameEncryptedLength = BitConverter.ToInt32(fileNameCount);
        
        var fileName = new byte[fileNameEncryptedLength];
        encryptedFileStream.ReadArray(ref fileName);


        crypt.IV = IV;
        crypt.Key = key;
        var decryptedFileName = crypt.DecryptCbc(fileName, IV).ToUnicodeString();

        Directory.CreateDirectory("Output");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Output", decryptedFileName);
        
        using var decryptedFile = File.Create(path);
        using var cryptoStream = new CryptoStream(encryptedFileStream, crypt.CreateDecryptor(key, IV), CryptoStreamMode.Read, true);
        cryptoStream.CopyTo(decryptedFile);

        crypt.Clear();
        _channel.SendDencrypted(path);
        return Array.Empty<byte>();
    }

    public byte[] Encrypt(string data)
    {
        const string ENCRYPTED_FILE_NAME = "output.txt";
        using var crypt = Aes.Create();
        crypt.GenerateKey();
        crypt.GenerateIV();

        var fileName = Path.GetFileName(data);
        var encryptedFileName = crypt.EncryptCbc(fileName.ToBytes(), crypt.IV);

        using var dataStream = File.OpenRead(data);
        using var encryptedFileStream = File.Create(ENCRYPTED_FILE_NAME);

        encryptedFileStream.Write(crypt.Key);
        encryptedFileStream.Write(crypt.IV);
        byte[] fileNameLengthInBytes = BitConverter.GetBytes(encryptedFileName.Length);
        encryptedFileStream.Write(fileNameLengthInBytes);
        encryptedFileStream.Write(encryptedFileName);
        
        using var cryptoStream = new CryptoStream(encryptedFileStream, crypt.CreateEncryptor(crypt.Key, crypt.IV), CryptoStreamMode.Write, true);
        dataStream.CopyTo(cryptoStream);


        var path = Path.Combine(Directory.GetCurrentDirectory(), ENCRYPTED_FILE_NAME);
        _channel.SendEncrypted(path);
        crypt.Clear();

        return Array.Empty<byte>();
    }
}
