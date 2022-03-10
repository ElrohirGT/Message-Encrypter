using Message_Encrypter.Encrypters.Channels;

namespace Message_Encrypter.Encrypters;

internal class EncrypterFactory
{
    internal static IEncrypter GetFromMessage(string message)
    {
        if (!File.Exists(message))
            return new TextEncrypter(new ClipboardChannel());
        return new FileEncrypter(new FileExplorerChannel());
    }

    internal static IEncrypter GetFromEncryption(string message)
    {
        if (!File.Exists(message))
            return new TextEncrypter(new ClipboardChannel());
        return new FileEncrypter(new FileExplorerChannel());
    }
}