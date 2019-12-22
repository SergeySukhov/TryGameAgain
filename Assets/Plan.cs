using System.Collections.Generic;
using UnityEngine;


public class Plan
{
    private List<ICommand> _commands = new List<ICommand>();
    private int _curIdx = 0;
    private GameObject gameObject = null;
    public void AddCommand(ICommand cmd)
    {
        _commands.Add(cmd);
    }
    public bool IsComplete { get; private set; } = false;
    public void Execute()
    {
        if (_curIdx < _commands.Count)
        {
            if (!_commands[_curIdx].IsComplete)
            {
                _commands[_curIdx].Execute();
            }
            else
            {
                _curIdx++;
            }
        }
        else
        {
            IsComplete = true;
        }
    }
}
