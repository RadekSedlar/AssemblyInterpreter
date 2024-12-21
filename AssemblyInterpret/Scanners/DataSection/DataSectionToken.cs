namespace AssemblyInterpret.Scanners.DataSection;

public record DataSectionToken(DataSectionTokenType Type, string Lexeme, int Line, int Column);