using System.Text;
using System.Text.RegularExpressions;

namespace AssemblyInterpret;


public class UnexpectedCharacterException : Exception
{
    public UnexpectedCharacterException(
        DataSectionTokenType tokenTypeTryingToCreate,
        char unexpectedCharacter, 
        int line, 
        int column) : base($"There is unexpected character > {unexpectedCharacter} < on position ({line},{column}) while trying to create {tokenTypeTryingToCreate}.")
    {
        
    }
}

public enum DataSectionTokenType
{
    Newline,
    StringLiteral,
    Word,
    KeywordDataByte,
    KeywordDataWord,
    KeywordDataDoubleWord,
    KeywordDup,
    Number,
    EmptyValue,
    Separator,
    BracketStart,
    BracketEnd,
    Eof
}

public record DataSectionToken(DataSectionTokenType Type, string Lexeme, int Line, int Column);

public class DataSectionScanner
{
    private string _originalText;
    private int _currentLocation;
    private int _currentLine = 1;
    private int _currentColumn;
    private List<DataSectionToken> _tokens = [];
    
    
    public DataSectionScanner(string originalText)
    {
        _originalText = Regex.Replace(originalText, "[;].*$", "");
    }

    public DataSectionToken GetToken()
    {
        if (_tokens.Count == 0)
        {
            return new DataSectionToken(DataSectionTokenType.Eof, "", _currentLine, _currentColumn);
        }

        var first = _tokens.First();

        _tokens.Remove(first);
        return first;
    }

    public void ReturnToken(DataSectionToken token)
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
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.Eof, "", _currentLine, _currentColumn));
                    return;
                case '\n': // TODO \r\n
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.Newline, "\n", _currentLine, _currentColumn));
                    _currentLine++;
                    _currentColumn = 0;
                    break;
                case '?': 
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.EmptyValue, "?", _currentLine, _currentColumn));
                    break;
                case '(': 
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.BracketStart, "(", _currentLine, _currentColumn));
                    break;
                case ')': 
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.BracketEnd, ")", _currentLine, _currentColumn));
                    break;
                case ',': 
                    _tokens.Add(new DataSectionToken(DataSectionTokenType.Separator, ",", _currentLine, _currentColumn));
                    break;
                case '\'': 
                    _tokens.Add(GenerateStringLiteral('\''));
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

    private DataSectionToken GenerateStringLiteral(char startingChar)
    {
        var sb = new StringBuilder($"{startingChar}");

        int literalStartingPosition = _currentColumn;

        while (true)
        {
            var nextChar = GetCharacter();

            if (nextChar is null)
            {
                throw new UnexpectedCharacterException(DataSectionTokenType.StringLiteral, '\0', _currentLine,
                    _currentColumn);
            }
            
            if (nextChar is '\n')
            {
                throw new UnexpectedCharacterException(DataSectionTokenType.StringLiteral, nextChar.Value, _currentLine,
                    literalStartingPosition);
            }
            
            sb.Append(nextChar);
            if (nextChar is '\'')
            {
                return new DataSectionToken(DataSectionTokenType.StringLiteral, sb.ToString(), _currentLine,
                    literalStartingPosition);
            }
        }
    }

    private DataSectionToken GenerateWord(char startingChar)
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
                
                switch (lexeme)
                {
                    case "DB":
                        return new DataSectionToken(DataSectionTokenType.KeywordDataByte, sb.ToString(), _currentLine,
                            literalStartingPosition);
                    case "DW":
                        return new DataSectionToken(DataSectionTokenType.KeywordDataWord, sb.ToString(), _currentLine,
                            literalStartingPosition);
                    case "DD":
                        return new DataSectionToken(DataSectionTokenType.KeywordDataDoubleWord, sb.ToString(), _currentLine,
                            literalStartingPosition);
                    case "DUP":
                        return new DataSectionToken(DataSectionTokenType.KeywordDup, sb.ToString(), _currentLine,
                            literalStartingPosition);
                    default:
                        return new DataSectionToken(DataSectionTokenType.Word, sb.ToString(), _currentLine,
                            literalStartingPosition);
                }
            }
            
            sb.Append(nextChar);
        }
    }
    
    private DataSectionToken GenerateNumber(char startingChar)
    {
        var sb = new StringBuilder($"{startingChar}");

        int literalStartingPosition = _currentColumn;

        while (true)
        {
            var nextChar = GetCharacter();
            
            if (nextChar is not (>= '0' and <= '9'))
            {
                if (nextChar is not null) ReturnCharacter();
                
                return new DataSectionToken(DataSectionTokenType.Number, sb.ToString(), _currentLine,
                    literalStartingPosition);
            }
            
            sb.Append(nextChar);
        }
    }
}