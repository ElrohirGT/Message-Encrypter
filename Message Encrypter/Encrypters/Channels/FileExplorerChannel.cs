using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Message_Encrypter.Encrypters.Channels;
public class FileExplorerChannel : IChannel
{
    public const string SEPARATOR = @"§";
    public void SendDencrypted(string value) => OpenFileExplorer(value);
    public void SendEncrypted(string value) => OpenFileExplorer(value);

    private static void OpenFileExplorer(string filePath)
    {
        string containerDirectory = Path.GetDirectoryName(filePath) ?? "/";
        ProcessStartInfo psi = new();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            psi.FileName = "explorer";
            psi.Arguments = containerDirectory;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            psi.FileName = "open";
            psi.Arguments = $"-a finder {containerDirectory}";
        }
        else
        {
            psi.FileName = "cd";
            psi.Arguments = containerDirectory;            
        }
        Process.Start(psi);
    }

}
