namespace AssemblyInterpret.Scanners.DataSection;

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