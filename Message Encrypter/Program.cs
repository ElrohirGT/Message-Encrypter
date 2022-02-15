using System.Text;

using BlowFishCS;

using Message_Encrypter;

using TextCopy;

using static ConsoleUtilitiesLite.ConsoleUtilities;

string[] _title = new string[]
{
    "█▄─▄▄─█▄─▀█▄─▄█─▄▄▄─█▄─▄▄▀█▄─█─▄█▄─▄▄─█─▄─▄─█▄─▄▄─█▄─▄▄▀█",
    "██─▄█▀██─█▄▀─██─███▀██─▄─▄██▄─▄███─▄▄▄███─████─▄█▀██─▄─▄█",
    "▀▄▄▄▄▄▀▄▄▄▀▀▄▄▀▄▄▄▄▄▀▄▄▀▄▄▀▀▄▄▄▀▀▄▄▄▀▀▀▀▄▄▄▀▀▄▄▄▄▄▀▄▄▀▄▄▀"
};
byte[] IV = new byte[] { 232, 164, 157, 217, 142, 215, 223, 69 };

ShowTitle(_title);
ShowVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

Console.WriteLine("1) Decrypt.");
Console.WriteLine("2) Encrypt.");

ConsoleMenu mainMenu = new();
mainMenu.AddCommand("1", Decrypt);
mainMenu.AddCommand("2", Encrypt);

while (true)
{
    string answer = Console.ReadLine() ?? string.Empty;
    if (answer == "q")
        break;
    await mainMenu.Execute(answer);
    SubDivision();
}

void Decrypt()
{
    Console.WriteLine("Write the private key (write nothing to use the saved one).");
    string? inputKey = Console.ReadLine();
    byte[] key = GetKey(inputKey);

    BlowFish crypt = new(key);
    crypt.IV = IV;

    string value = ClipboardService.GetText() ?? string.Empty;
    var decryptedValue = crypt.Decrypt_CBC(Encoding.Unicode.GetBytes(value));
    string decryptedValueInText = Encoding.Unicode.GetString(decryptedValue);

    ClipboardService.SetText(decryptedValueInText);
    LogInfoMessage(decryptedValueInText);
}

void Encrypt()
{
    Console.WriteLine("Write the public key (write nothing to use the saved one).");
    string? inputKey = Console.ReadLine();
    byte[] key = GetKey(inputKey);

    BlowFish crypt = new(key);
    crypt.IV = IV;

    string value = ClipboardService.GetText() ?? string.Empty;
    var encryptedValue = crypt.Encrypt_CBC(Encoding.Unicode.GetBytes(value));
    string encryptedValueInText = Encoding.Unicode.GetString(encryptedValue);

    ClipboardService.SetText(encryptedValueInText);
    LogInfoMessage(encryptedValueInText);
}

byte[] GetKey(string? inputKey)
{
    byte[] key;
    if (!string.IsNullOrEmpty(inputKey))
    {
        key = Encoding.UTF8.GetBytes(inputKey);
        File.WriteAllBytes(".pKey", key);
    }
    else
    {
        if (!File.Exists(".pKey"))
            File.Create(".pKey");
        key = File.ReadAllBytes(".pKey");
    }
    return key;
}