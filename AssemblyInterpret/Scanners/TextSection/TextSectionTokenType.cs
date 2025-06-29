namespace AssemblyInterpret.Scanners.TextSection;

public enum TextSectionTokenType
{
    Separator,
    SqBracketOpen,
    SqBracketClose,
    DataTypeByte,
    DataTypeWord,
    DataTypeDouble,
    Number,
    Word,
    Plus,
    Minus,
    Times,
    Register,
    Eof,
    NewLine,
    Label
}