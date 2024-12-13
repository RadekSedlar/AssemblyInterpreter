using System.Text.RegularExpressions;
namespace AssemblyInterpret.Interpreters;


public class DataInterpret
{
    public GlobalMemory Memory { get; init; }
    public DataInterpret(GlobalMemory memory)
    {
        Memory = memory;
    }

    internal void InterpretBytes(string name, string values)
    {
        Memory.AddGlobalVar(new MemoryCell(ByteCount.DB, name, Memory.TopPointer));


        switch (values)
        {
            case [var singleValue]:

                
                
                break;
            case [var singleValue, var dupExpression]:
                break;
        }
    }

    private byte[] ConvertTextToByteValues(string valueText)
    {
        if (byte.TryParse(valueText, out byte singleValue))
        {
            return new[] {singleValue};
        }

        if (valueText is "?")
        {
            return new[] {byte.MinValue};
        }
        
        return Array.Empty<byte>();
    }
    
    
    internal void InterpretWords(string name, string line)
    {
        Memory.AddGlobalVar(new MemoryCell(ByteCount.DW, name, Memory.TopPointer));
        line = line.Replace(',',' ');
        string[] values = line.Split(' ');

        foreach (var token in values)
        {
            if (token.StartsWith('\'') && token.EndsWith('\''))
            {
                var literal = token.Trim('\'');
                foreach (var character in literal)
                {
                    Memory.SetWord(Memory.TopPointer, (UInt16)character);
                    Memory.TopPointer += 2;
                }
            }
            
            if (token == "?")
            {
                Memory.SetWord(Memory.TopPointer, 0);
                Memory.TopPointer += 2;
            }
            
            UInt16 wordValue;
            if (UInt16.TryParse(token, out wordValue))
            {
                Memory.SetWord(Memory.TopPointer, wordValue);
                Memory.TopPointer += 2;
            }
            else
            {
                throw new Exception($"'{token}' is not word value");
            }
        }
    }
    
    internal void InterpretDoubleWords(string name, string line)
    {
        Memory.AddGlobalVar(new MemoryCell(ByteCount.DD, name, Memory.TopPointer));
        line = line.Replace(',',' ');
        string[] values = line.Split(' ');

        foreach (var token in values)
        {
            if (token.StartsWith('\'') && token.EndsWith('\''))
            {
                var literal = token.Trim('\'');
                foreach (var character in literal)
                {
                    Memory.SetDoubleWord(Memory.TopPointer, character);
                    Memory.TopPointer += 4;
                }
            }
            
            if (token == "?")
            {
                Memory.SetDoubleWord(Memory.TopPointer, 0);
                Memory.TopPointer += 4;
            }
            
            UInt32 doubleWordValue;
            if (UInt32.TryParse(token, out doubleWordValue))
            {
                Memory.SetDoubleWord(Memory.TopPointer, doubleWordValue);
                Memory.TopPointer += 4;
            }
            else
            {
                throw new Exception($"'{token}' is not double word value");
            }
        }
    }

    private string StripCommentsAndTrimRawLineText(string rawLineText) =>
        Regex.Replace(rawLineText, @"[;].*", "").Trim();
    
    public void InterpretLine(string rawLineText)
    {
        var line = StripCommentsAndTrimRawLineText(rawLineText);
        
        if (line.Length == 0)
        {
            return;
        }

        var lineWords = line.Split(' ');

        
        
        switch (lineWords)
        {
            case [var variableName, "DB", .. var tail]:
                InterpretBytes(variableName, line.TrimStart());
                break;
            case "DW":
                break;
            case "DD":
                break;
            default:
                throw new Exception($"'{varSizeMatch.Value}' is not var size.");
        }
        
    }
}