namespace AssemblyInterpret.Scanners.DataSection;

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