using System.Text.RegularExpressions;
namespace AssemblyInterpret.Interpreters;


public class DataInterpret
{
    public GlobalMemory Memory { get; init; }
    public DataInterpret(GlobalMemory memory)
    {
        Memory = memory;
    }

    internal void InterpretBytes(string name, string line)
    {
        Memory.AddGlobalVar(new MemoryCell(ByteCount.DB, name, Memory.TopPointer));
        line = line.Replace(',',' ');
        string[] values = line.Split(' ');

        foreach (var token in values)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                continue;
            }
            
            if (token.StartsWith('\'') && token.EndsWith('\''))
            {
                var literal = token.Trim('\'');
                foreach (var character in literal)
                {
                    Memory.SetByte(Memory.TopPointer, (byte)character);
                    Memory.TopPointer++;
                }
                continue;
            }
            
            if (token == "?")
            {
                Memory.SetByte(Memory.TopPointer, 0);
                Memory.TopPointer++;
                continue;
            }
            
            byte byteValue;
            if (byte.TryParse(token, out byteValue))
            {
                Memory.SetByte(Memory.TopPointer, byteValue);
                Memory.TopPointer++;
            }
            else
            {
                throw new Exception($"'{token}' is not byte value");
            }
        }
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
                    Memory.SetWord(Memory.TopPointer, (Int16)character);
                    Memory.TopPointer += 2;
                }
            }
            
            if (token == "?")
            {
                Memory.SetWord(Memory.TopPointer, 0);
                Memory.TopPointer += 2;
            }
            
            Int16 wordValue;
            if (Int16.TryParse(token, out wordValue))
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
                    Memory.SetDoubleWord(Memory.TopPointer, (Int32)character);
                    Memory.TopPointer += 4;
                }
            }
            
            if (token == "?")
            {
                Memory.SetDoubleWord(Memory.TopPointer, 0);
                Memory.TopPointer += 4;
            }
            
            Int32 doubleWordValue;
            if (Int32.TryParse(token, out doubleWordValue))
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
    
    public void InterpretLine(string line)
    {
        line = Regex.Replace(line, @"[;].*", "");

        line = line.TrimStart();
        if (line.Length == 0)
        {
            return;
        }

        var nameReg = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{0,}");
        var nameMatch = nameReg.Match(line);
        if (!nameMatch.Success)
        {
            throw new Exception($"Pattern '{line}' cannot be interpreted as variable name.");
        }

        var varName = nameMatch.Value;
        line = nameReg.Replace(line, "");

        line = line.TrimStart();
        var varSizeReg = new Regex(@"^[D][BWD]");
        var varSizeMatch = varSizeReg.Match(line);
        if (!varSizeMatch.Success)
        {
            throw new Exception($"Pattern '{line}' cannot be interpreted as variable size.");
        }

        line = varSizeReg.Replace(line, "");
        
        switch (varSizeMatch.Value)
        {
            case "DB":
                InterpretBytes(varName, line.TrimStart());
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