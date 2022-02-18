using System.Security.Cryptography;
using System.Text;

using ConsoleUtilitiesLite;

using Message_Encrypter;

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
observer.Add(new ConsoleCommand(ConsoleKey.C, ChangeKey, "change key"));
observer.Add(new ConsoleCommand(ConsoleKey.Q, observer.StopObserving, "quit"));

var t = observer.StartObserving();
foreach (var command in observer.Commands)
    LogInfoMessage($"Press {command.ActivatorKey} to {command.Description}.");
SubDivision();

await t;

void Decrypt()
{
    using var crypt = Aes.Create();

    string[] input = ClipboardService.GetText()?.Split(SEPARATOR) ?? Array.Empty<string>();
    byte[] key = Convert.FromBase64String(input[0]);
    byte[] IV = Convert.FromBase64String(input[1]);
    byte[] value = Convert.FromBase64String(string.Join(string.Empty, input[2..]));
    
    crypt.IV = IV;
    crypt.Key = key;
    var decryptedValue = crypt.DecryptCbc(value, IV, PaddingMode.Zeros);
    string decryptedValueInText = decryptedValue.ToUnicodeString();

    ClipboardService.SetText(decryptedValueInText);
    LogInfoMessage($"{decryptedValueInText}: {decryptedValue.Length}");
    crypt.Clear();
}

void Encrypt()
{
    using var crypt = Aes.Create();
    crypt.GenerateKey();
    crypt.GenerateIV();

    string value = ClipboardService.GetText() ?? string.Empty;
    var encryptedValue = crypt.EncryptCbc(value.ToBytes(), crypt.IV, PaddingMode.Zeros);
    string encryptedValueInText = $"{crypt.Key.ToBase64String()}{SEPARATOR}{crypt.IV.ToBase64String()}{SEPARATOR}{encryptedValue.ToBase64String()}";

    ClipboardService.SetText(encryptedValueInText);
    LogInfoMessage($"{encryptedValueInText}: {encryptedValue.Length}");
    crypt.Clear();
}
void ChangeKey()
{
    Console.WriteLine("Write the private key (write nothing to use the saved one).");
    string? inputKey = Console.ReadLine() ?? string.Empty;
    byte[] key = Encoding.UTF8.GetBytes(inputKey);
    File.WriteAllBytes(".pKey", key);
}

byte[] GetKey() => File.ReadAllBytes(".pKey");