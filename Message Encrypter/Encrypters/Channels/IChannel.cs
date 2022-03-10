namespace Message_Encrypter.Encrypters.Channels;

public interface IChannel
{
    void SendEncrypted(string value);
    void SendDencrypted(string value);
}