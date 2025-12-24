using System.Text.RegularExpressions;

namespace AssemblyInterpret.Interpreters;

public enum InterpreterMode
{
    NotSet,
    Data,
    Text
}
public class AssemblyInterpreter
{
    public GlobalMemory Memory { get; init; }
    public Registers Registers { get; init; }
    public string OriginalText { get; init; }
    public Dictionary<string, int> Labels { get; init; }
    public List<string> Lines => _lines;
    public int CurrentLineIndex { get; set; }
    public InterpreterMode Mode { get; set; } =  InterpreterMode.NotSet;

    private List<string> _lines;
    private readonly int _maxLineIndex;

    public AssemblyInterpreter(GlobalMemory memory, Registers registers, string originalText)
    {
        Registers = registers;
        Memory = memory;
        Labels = new Dictionary<string, int>();

        OriginalText = originalText;
        _lines = originalText.Split(Environment.NewLine).ToList();
        _maxLineIndex = _lines.Count;
    }

    public void InterpretNextLine()
    {
        if (CurrentLineIndex >= _maxLineIndex)
        {
            return;
        }
        
        string currentLine = _lines[CurrentLineIndex];
        CurrentLineIndex++;
        
        
        if (currentLine.Trim() == "text:")
        {
            Mode = InterpreterMode.Text;
            return;
        }
        
        if (currentLine.Trim() == "data:")
        {
            Mode = InterpreterMode.Data;
            return;
        }

        InterpretLine(currentLine);
    }
    
    public void InterpretLine(string line)
    {
        if (Mode == InterpreterMode.Data)
        {
            DataInterpret dataInterpret = new DataInterpret(Memory, line);
            dataInterpret.InterpretDataSection();
        }
        
        if (Mode == InterpreterMode.Text)
        {
            TextInterpreter textInterpreter = new TextInterpreter(Memory, line, Registers, Labels, CurrentLineIndex);
            try
            {
                textInterpreter.InterpretSection();
            }
            catch (GlobalJumpException globalJumpException)
            {
                CurrentLineIndex = globalJumpException.GlobalLineJump;
            }
        }
    }
}