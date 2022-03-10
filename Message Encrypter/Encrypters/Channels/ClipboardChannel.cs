using TextCopy;

namespace Message_Encrypter.Encrypters.Channels;
public class ClipboardChannel : IChannel
{
    public void SendDencrypted(string value) => ClipboardService.SetText(value);
    public void SendEncrypted(string value) => ClipboardService.SetText(value);
}
