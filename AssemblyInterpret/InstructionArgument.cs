namespace AssemblyInterpret;
public abstract class InstructionArgument
{
    
}

public class MemoryArgument : InstructionArgument
{
    public int Address  { get; init; }

    public MemoryArgument(int address)
    {
        Address = address;
    }
}

public class ConstantArgument : InstructionArgument
{
    public int Value  { get; init; }

    public ConstantArgument(int value)
    {
        Value = value;
    }
}

public class RegisterArgument : InstructionArgument
{
    public string RegisterIdentifier { get; init; }
    public ByteCount ByteCount { get; init; }

    public RegisterArgument(string registerIdentifier, ByteCount byteCount)
    {
        RegisterIdentifier = registerIdentifier;
        ByteCount = byteCount;
    }
}

public class VariableArgument : InstructionArgument
{
    public VariableArgument(MemoryCell memoryCell)
    {
        MemoryCell = memoryCell;
    }

    public MemoryCell MemoryCell { get; init; }

}