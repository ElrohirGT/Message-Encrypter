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
const string IV_SEPARATOR = @"\_-_\";

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
    byte[] key = GetKey();
    crypt.KeySize = key.Length * 8;
    crypt.Key = key;

    string[] input = ClipboardService.GetText()?.Split(IV_SEPARATOR) ?? Array.Empty<string>();
    byte[] IV = input[^1].ToBytes();
    byte[] value = string.Join(string.Empty, input[..^1]).ToBytes();
    
    crypt.IV = IV;
    var decryptedValue = crypt.DecryptCbc(value, IV, PaddingMode.Zeros);
    string decryptedValueInText = decryptedValue.ToUnicodeString();

    ClipboardService.SetText(decryptedValueInText);
    LogInfoMessage($"{decryptedValueInText}: {decryptedValue.Length}");
    crypt.Clear();
}

void Encrypt()
{
    using var crypt = Aes.Create();
    byte[] key = GetKey();
    crypt.KeySize = key.Length * 8;
    crypt.Key = key;
    crypt.GenerateIV();

    string value = ClipboardService.GetText() ?? string.Empty;
    var encryptedValue = crypt.EncryptCbc(value.ToBytes(), crypt.IV, PaddingMode.Zeros);
    string encryptedValueInText = $"{encryptedValue.ToUnicodeString()}{IV_SEPARATOR}{crypt.IV.ToUnicodeString()}";

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