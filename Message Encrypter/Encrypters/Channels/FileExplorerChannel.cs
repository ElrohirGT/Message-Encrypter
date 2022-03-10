using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Message_Encrypter.Encrypters.Channels;
public class FileExplorerChannel : IChannel
{
    public const string SEPARATOR = @"§";
    public void SendDencrypted(string value)
    {
        var values = value.Split(SEPARATOR);
        var fileName = Convert.FromBase64String(values[0]).ToUnicodeString();
        var fileValue = Convert.FromBase64String(values[1]);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Output", fileName);

        Directory.CreateDirectory("Output");
        File.WriteAllBytes(filePath, fileValue);
        OpenFileExplorer(filePath);
    }

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

    public void SendEncrypted(string value)
    {
        File.WriteAllText("output.txt", value);
        var fullPath = Path.GetFullPath("output.txt");
        OpenFileExplorer(fullPath);
    }
}
