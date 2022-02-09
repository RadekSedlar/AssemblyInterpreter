namespace AssemblyInterpret;
public abstract class InstructionArgument
{
    
}

public class MemoryArgument : InstructionArgument
{
    public UInt32 Address  { get; init; }

    public MemoryArgument(UInt32 address)
    {
        Address = address;
    }
}

public class ConstantArgument : InstructionArgument
{
    public UInt32 Value  { get; init; }

    public ConstantArgument(UInt32 value)
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