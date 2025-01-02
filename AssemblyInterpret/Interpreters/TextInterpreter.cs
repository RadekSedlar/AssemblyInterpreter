using System.Text;
using System.Text.RegularExpressions;
using AssemblyInterpret.Scanners.TextSection;

namespace AssemblyInterpret.Interpreters;

public class TextInterpreter
{
    public GlobalMemory Memory { get; init; }
    public TextSectionScanner SectionScanner { get; init; }
    public TextInterpreter(GlobalMemory memory, string textSectionText)
    {
        Memory = memory;
        var sb = new StringBuilder();
        var lines = textSectionText.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            sb.AppendLine(StripCommentsAndTrimRawLineText(line));
        }

        SectionScanner = new TextSectionScanner(sb.ToString());
        SectionScanner.Scan();
    }
    
    private string StripCommentsAndTrimRawLineText(string rawLineText) =>
        Regex.Replace(rawLineText, @"[;].*", "").Trim();
    
    public void InterpretSection()
    {
        while (true)
        {
            var token = SectionScanner.GetToken();
            switch (token.Type)
            {
                case TextSectionTokenType.NewLine:
                    continue;
                case TextSectionTokenType.Word:
                    // INSTRUCTION HERE
                    break;
                case TextSectionTokenType.Eof:
                    return;
                default:
                    throw new UnexpectedTokenTypeException($"Instruction is expected. '{token.Type} was given.'");
            }
        }
    }
}

public class UnexpectedTokenTypeException : Exception
{
    public UnexpectedTokenTypeException(string message) : base(message) { }
}