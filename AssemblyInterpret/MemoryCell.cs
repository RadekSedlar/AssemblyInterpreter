namespace AssemblyInterpret;

public class MemoryCell
{
    public string Name { get; init; }
    public int Address { get; init; }
    public ByteCount ByteCount { get; set; }
    

    public MemoryCell(ByteCount byteCount, string name, int address)
    {
        ByteCount = byteCount;
        Name = name;
        Address = address;
    }
}