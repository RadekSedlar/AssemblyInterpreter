namespace AssemblyInterpret;

public class MemoryCell
{
    public string Name { get; init; }
    public UInt32 Address { get; init; }
    public ByteCount ByteCount { get; set; }
    

    public MemoryCell(ByteCount byteCount, string name, UInt32 address)
    {
        ByteCount = byteCount;
        Name = name;
        Address = address;
    }
}