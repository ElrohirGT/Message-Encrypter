namespace Message_Encrypter.Encrypters;

public interface IEncrypter
{
    byte[] Decrypt(string data);
    byte[] Encrypt(string data);
}