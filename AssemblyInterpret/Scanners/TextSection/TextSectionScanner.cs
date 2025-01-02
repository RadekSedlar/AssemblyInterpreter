using System.Text;
using System.Text.RegularExpressions;

namespace AssemblyInterpret.Scanners.TextSection;

public class TextSectionScanner
{
    private string _originalText;
    private int _currentLocation;
    private int _currentLine = 1;
    private int _currentColumn;
    private List<TextSectionToken> _tokens = [];
    
    
    public TextSectionScanner(string originalText)
    {
        _originalText = Regex.Replace(originalText, "[;].*$", "");
    }

    public TextSectionToken GetToken()
    {
        if (_tokens.Count == 0)
        {
            return new TextSectionToken(TextSectionTokenType.Eof, "", _currentLine, _currentColumn);
        }

        var first = _tokens.First();

        _tokens.Remove(first);
        return first;
    }

    public void ReturnToken(TextSectionToken token)
    {
        _tokens.Add(token);
    }

    private char? GetCharacter()
    {
        if (_currentLocation >= _originalText.Length)
        {
            return null;
        }
        var currentCharacter = _originalText[_currentLocation];
        _currentLocation++;
        _currentColumn++;
        return currentCharacter;
    }
    
    private void ReturnCharacter()
    {
        _currentLocation--;
        _currentColumn--;
    }

    public void Scan()
    {
        while (true)
        {
            var currentCharacter = GetCharacter();

            switch (currentCharacter)
            {
                case ' ':
                    break;
                case null:
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.Eof, "", _currentLine, _currentColumn));
                    return;
                case '\r': // TODO \r\n
                    var newLine = GetCharacter();
                    if (newLine != '\n')
                    {
                        return;
                    }
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.NewLine, "\r\n", _currentLine, _currentColumn));
                    _currentLine++;
                    _currentColumn = 0;
                    break;
                case ',': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.Separator, ",", _currentLine, _currentColumn));
                    break;
                case '-': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.Minus, "-", _currentLine, _currentColumn));
                    break;
                case '+': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.Plus, "+", _currentLine, _currentColumn));
                    break;
                case '*': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.Times, "*", _currentLine, _currentColumn));
                    break;
                case '[': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.SqBracketOpen, "[", _currentLine, _currentColumn));
                    break;
                case ']': 
                    _tokens.Add(new TextSectionToken(TextSectionTokenType.SqBracketClose, "]", _currentLine, _currentColumn));
                    break;
                case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'): 
                    _tokens.Add(GenerateWord(currentCharacter.Value));
                    break;
                case (>= '0' and <= '9'): 
                    _tokens.Add(GenerateNumber(currentCharacter.Value));
                    break;
                default:
                    return;
            }
        }
    }

    

    private TextSectionToken GenerateWord(char startingChar)
    {
        var sb = new StringBuilder($"{startingChar}");

        int literalStartingPosition = _currentColumn;

        while (true)
        {
            var nextChar = GetCharacter();
            
            if (nextChar is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9')))
            {
                if (nextChar is not null) ReturnCharacter();

                string lexeme = sb.ToString();

                return lexeme switch {
                    ("eax" or "ebx" or "ecx" or "edx") or ("ax" or "bx" or "cx" or "dx") or ("ah" or "al" or "bh" or "bl" or "ch" or "cl" or "dh" or "dl") 
                        => new TextSectionToken(TextSectionTokenType.Register, lexeme, _currentLine,
                            literalStartingPosition),
                    "BYTE" => new TextSectionToken(TextSectionTokenType.DataTypeByte, lexeme, _currentLine, literalStartingPosition),
                    "WORD" => new TextSectionToken(TextSectionTokenType.DataTypeWord, lexeme, _currentLine, literalStartingPosition),
                    "DWORD" => new TextSectionToken(TextSectionTokenType.DataTypeDouble, lexeme, _currentLine, literalStartingPosition),
                    _ => new TextSectionToken(TextSectionTokenType.Word, lexeme, _currentLine, literalStartingPosition)
                };
                
            }
            
            sb.Append(nextChar);
        }
    }
    
    private TextSectionToken GenerateNumber(char startingChar)
    {
        var sb = new StringBuilder($"{startingChar}");

        int literalStartingPosition = _currentColumn;

        while (true)
        {
            var nextChar = GetCharacter();
            
            if (nextChar is not (>= '0' and <= '9'))
            {
                if (nextChar is not null) ReturnCharacter();
                
                return new TextSectionToken(TextSectionTokenType.Number, sb.ToString(), _currentLine,
                    literalStartingPosition);
            }
            
            sb.Append(nextChar);
        }
    }
}