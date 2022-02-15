using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Encrypter;
public class ConsoleMenu
{
    readonly IDictionary<string, Action> _commands = new Dictionary<string, Action>();
    readonly IDictionary<string, Func<Task>> _asyncCommands = new Dictionary<string, Func<Task>>();

    public bool CaseSensitive { get; set; }

    public bool AddCommand(string key, Action command) => _commands.TryAdd(key, command);
    public bool AddAsyncCommand(string key, Func<Task> command) => _asyncCommands.TryAdd(key, command);
    public async ValueTask Execute(string key)
    {
        key = !CaseSensitive ? key.ToLower() : key;
        if (_commands.ContainsKey(key))
            _commands[key]();
        else if (_asyncCommands.ContainsKey(key))
            await _asyncCommands[key]();
    }
}
