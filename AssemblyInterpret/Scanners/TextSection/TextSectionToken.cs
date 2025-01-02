namespace AssemblyInterpret.Scanners.TextSection;

public record TextSectionToken(TextSectionTokenType Type, string Lexeme, int Line, int Column);