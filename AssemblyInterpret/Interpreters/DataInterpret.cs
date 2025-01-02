using System.Text;
using System.Text.RegularExpressions;
using AssemblyInterpret.Scanners.DataSection;

namespace AssemblyInterpret.Interpreters;

public class DataInterpret
{
    public GlobalMemory Memory { get; init; }
    public DataSectionScanner SectionScanner { get; init; }
    public DataInterpret(GlobalMemory memory, string dataSectionText)
    {
        Memory = memory;
        var sb = new StringBuilder();
        var lines = dataSectionText.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            sb.AppendLine(StripCommentsAndTrimRawLineText(line));
        }

        SectionScanner = new DataSectionScanner(sb.ToString());
        SectionScanner.Scan();
    }

    internal void InterpretBytes(string? variableName)
    {
        if (variableName is not null)
        {
            Memory.AddGlobalVar(new MemoryCell(ByteCount.DB, variableName, Memory.TopPointer));
        }
        
        var values = GetValues(false);

        foreach (var value in values)
        {
            Memory.SetByte(Memory.TopPointer, (byte)value);
            Memory.TopPointer++;
        }
    }
    
    internal void InterpretWords(string? variableName)
    {
        if (variableName is not null)
        {
            Memory.AddGlobalVar(new MemoryCell(ByteCount.DW, variableName, Memory.TopPointer));
        }
        
        var values = GetValues(false);

        foreach (var value in values)
        {
            Memory.SetWord(Memory.TopPointer, (UInt16)value);
            Memory.TopPointer += 2;
        }
    }
    
    internal void InterpretDoubleWords(string? variableName)
    {
        if (variableName is not null)
        {
            Memory.AddGlobalVar(new MemoryCell(ByteCount.DW, variableName, Memory.TopPointer));
        }
        
        var values = GetValues(false);

        foreach (var value in values)
        {
            Memory.SetDoubleWord(Memory.TopPointer, (uint)value);
            Memory.TopPointer += 4;
        }
    }

   

    private string StripCommentsAndTrimRawLineText(string rawLineText) =>
        Regex.Replace(rawLineText, @"[;].*", "").Trim();


    private void InterpretVariable(string variableNameLexeme)
    {
        var token = SectionScanner.GetToken();

        switch (token.Type)
        {
            case DataSectionTokenType.KeywordDataDoubleWord:
                InterpretDoubleWords(variableNameLexeme);
                break;
            case DataSectionTokenType.KeywordDataWord:
                InterpretWords(variableNameLexeme);
                break;
            case DataSectionTokenType.KeywordDataByte:
                InterpretBytes(variableNameLexeme);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private List<int> GetValues(bool firstLevelOfRecursion = true)
    {
        var firstToken = SectionScanner.GetToken();
        List<int> values = firstToken switch
        {
            {Type: DataSectionTokenType.EmptyValue} => [0],
            {Type: DataSectionTokenType.Number} => [int.Parse(firstToken.Lexeme ?? "0")],
            {Type: DataSectionTokenType.StringLiteral} => firstToken.Lexeme!
                .Skip(1)
                .Take(firstToken.Lexeme!.Length-2)
                .Select(x=>(int)x)
                .ToList(),
            _ => throw new ArgumentOutOfRangeException()
        };

        var secondToken = SectionScanner.GetToken();

        if (firstLevelOfRecursion && secondToken.Type is DataSectionTokenType.KeywordDup)
        {
            var probablyStartingBracket = SectionScanner.GetToken();
            var probablyNumber = SectionScanner.GetToken();
            var probablyEndingBracket = SectionScanner.GetToken();

            if (probablyStartingBracket.Type == DataSectionTokenType.BracketStart &&
                probablyNumber.Type == DataSectionTokenType.Number &&
                probablyEndingBracket.Type == DataSectionTokenType.BracketEnd)
            {
                var numberOfRepetitions = int.Parse(firstToken.Lexeme ?? "0");
                var valueOfRepetition = int.Parse(probablyNumber.Lexeme ?? "0");
                
                return Enumerable.Repeat(valueOfRepetition, numberOfRepetitions).ToList();
            }
            
            if (probablyStartingBracket.Type == DataSectionTokenType.BracketStart &&
                probablyNumber.Type == DataSectionTokenType.EmptyValue &&
                probablyEndingBracket.Type == DataSectionTokenType.BracketEnd)
            {
                var numberOfRepetitions = int.Parse(firstToken.Lexeme ?? "0");
                
                return Enumerable.Repeat(0, numberOfRepetitions).ToList();
            }
        }
        
        if (secondToken.Type is DataSectionTokenType.Separator)
        {
            values.AddRange(GetValues(false));
            return values;
        }

        SectionScanner.ReturnToken(secondToken);

        return values;
    }
    
    public void InterpretDataSection()
    {
        while (true)
        {
            var token = SectionScanner.GetToken();
            switch (token.Type)
            {
                case DataSectionTokenType.Newline:
                    break;
                case DataSectionTokenType.Word:
                    InterpretVariable(token.Lexeme ?? "");
                    break;
                case DataSectionTokenType.KeywordDataDoubleWord:
                    InterpretDoubleWords(null);
                    break;
                case DataSectionTokenType.KeywordDataWord:
                    InterpretWords(null);
                    break;
                case DataSectionTokenType.KeywordDataByte:
                    InterpretBytes(null);
                    break;
                case DataSectionTokenType.Eof:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}