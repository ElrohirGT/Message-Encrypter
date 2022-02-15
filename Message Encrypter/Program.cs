﻿using System.Text;

using BlowFishCS;

using ConsoleUtilitiesLite;

using TextCopy;

using static ConsoleUtilitiesLite.ConsoleUtilities;

string[] _title = new string[]
{
    "█▄─▄▄─█▄─▀█▄─▄█─▄▄▄─█▄─▄▄▀█▄─█─▄█▄─▄▄─█─▄─▄─█▄─▄▄─█▄─▄▄▀█",
    "██─▄█▀██─█▄▀─██─███▀██─▄─▄██▄─▄███─▄▄▄███─████─▄█▀██─▄─▄█",
    "▀▄▄▄▄▄▀▄▄▄▀▀▄▄▀▄▄▄▄▄▀▄▄▀▄▄▀▀▄▄▄▀▀▄▄▄▀▀▀▀▄▄▄▀▀▄▄▄▄▄▀▄▄▀▄▄▀"
};
byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

Console.Clear();

ShowTitle(_title);
ShowVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

bool shouldStop = false;
CommandObserver observer = new();
observer.Add(new ConsoleCommand(ConsoleKey.D, Decrypt, "decrypt message"));
observer.Add(new ConsoleCommand(ConsoleKey.E, Encrypt, "encrypt message"));
observer.Add(new ConsoleCommand(ConsoleKey.C, ChangeKey, "change key"));
observer.Add(new ConsoleCommand(ConsoleKey.Q, ()=>shouldStop = true, "quit"));

var t = observer.StartObserving();
foreach (var command in observer.Commands)
    LogInfoMessage($"Press {command.ActivatorKey} to {command.Description}.");
SubDivision();

while (!shouldStop)
    await Task.Delay(1000);

void Decrypt()
{
    byte[] key = GetKey();

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
    byte[] key = GetKey();

    BlowFish crypt = new(key);
    crypt.IV = IV;

    string value = ClipboardService.GetText() ?? string.Empty;
    var encryptedValue = crypt.Encrypt_CBC(Encoding.Unicode.GetBytes(value));
    string encryptedValueInText = Encoding.Unicode.GetString(encryptedValue);

    ClipboardService.SetText(encryptedValueInText);
    LogInfoMessage(encryptedValueInText);
}
void ChangeKey()
{
    Console.WriteLine("Write the private key (write nothing to use the saved one).");
    string? inputKey = Console.ReadLine() ?? string.Empty;
    byte[] key = Encoding.UTF8.GetBytes(inputKey);
    File.WriteAllBytes(".pKey", key);
}

byte[] GetKey() => File.ReadAllBytes(".pKey");