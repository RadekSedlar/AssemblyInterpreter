namespace AssemblyInterpret;

public class MemoryCell(ByteCount byteCount, string name, UInt32 address)
{
    public string Name { get; init; } = name;
    public UInt32 Address { get; init; } = address;
    public ByteCount ByteCount { get; set; } = byteCount;
}