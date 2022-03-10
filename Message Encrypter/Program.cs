
using ConsoleUtilitiesLite;

using Message_Encrypter.Encrypters;

using TextCopy;

using static ConsoleUtilitiesLite.ConsoleUtilities;

string[] _title = new string[]
{
    "█▄─▄▄─█▄─▀█▄─▄█─▄▄▄─█▄─▄▄▀█▄─█─▄█▄─▄▄─█─▄─▄─█▄─▄▄─█▄─▄▄▀█",
    "██─▄█▀██─█▄▀─██─███▀██─▄─▄██▄─▄███─▄▄▄███─████─▄█▀██─▄─▄█",
    "▀▄▄▄▄▄▀▄▄▄▀▀▄▄▀▄▄▄▄▄▀▄▄▀▄▄▀▀▄▄▄▀▀▄▄▄▀▀▀▀▄▄▄▀▀▄▄▄▄▄▀▄▄▀▄▄▀"
};
const string SEPARATOR = @"§";

Console.Clear();

ShowTitle(_title);
ShowVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

CommandObserver observer = new();
observer.Add(new ConsoleCommand(ConsoleKey.D, Decrypt, "decrypt message"));
observer.Add(new ConsoleCommand(ConsoleKey.E, Encrypt, "encrypt message"));
observer.Add(new ConsoleCommand(ConsoleKey.Q, observer.StopObserving, "quit"));

var t = observer.StartObserving();
foreach (var command in observer.Commands)
    LogInfoMessage($"Press {command.ActivatorKey} to {command.Description}.");
SubDivision();

await t;

void Decrypt()
{
    string message = ClipboardService.GetText() ?? string.Empty;
    IEncrypter encrypter = EncrypterFactory.GetFromEncryption(message);
    encrypter.Decrypt(message);
}

void Encrypt()
{
    string message = ClipboardService.GetText() ?? string.Empty;
    IEncrypter encrypter = EncrypterFactory.GetFromMessage(message);
    encrypter.Encrypt(message);
}