namespace AssemblyInterpret;

public class GlobalMemory
{
    private readonly byte[] _memory;
    public int MemorySize { get; set; }

    public GlobalMemory(int memorySize)
    {
        MemorySize = memorySize;
        _memory = new byte[memorySize];
    }
    
    public int TopPointer { get; set; }
    private List<MemoryCell> MemoryCells { get; set; } = new List<MemoryCell>();

    public void PrintMemory()
    {
        for (int i = 0; i < _memory.Length; i++)
        {
            if ((i % 16) == 0)
            {
                Console.Write("\n{0:X4} => ", i);
            }
            Console.Write(" {0:X2}", _memory[i]);
        }
    }
    
    public void AddGlobalVar(MemoryCell memoryCell)
    {
        if (MemoryCells.Exists(x => x.Name == memoryCell.Name))
            throw new Exception("Global variable already exists");
        
        MemoryCells.Add(memoryCell);
    }
    
    public MemoryCell? GetGlobalVar(string name)
    {
        var toBeReturned = MemoryCells.Find(x => x.Name == name);
        return toBeReturned;
    }

    private void ThrowIfOutOfMemory(int address)
    {
        if (address >= _memory.Length)
        {
            throw new OutOfMemoryException();
        }
    }
    
    public void SetByte(int address, byte value)
    {
        ThrowIfOutOfMemory(address);
        _memory[address] = value;
    }
    
    public byte ReadByte(int address)
    {
        ThrowIfOutOfMemory(address);
        return _memory[address];
    }
 
    public void SetWord(int address, Int16 value)
    {
        ThrowIfOutOfMemory(address + 1);
        _memory[address] = (byte) (value >> 8);
        _memory[address + 1] = (byte)value;
    }
    
    public Int16 ReadWord(int address)
    {
        ThrowIfOutOfMemory(address + 1);
        Int16 retVal = _memory[address];
        retVal = (short) (retVal << 8);
        retVal = (short)(retVal | (short)_memory[address + 1]);
        return retVal;
    }
    
    public void SetDoubleWord(int address, Int32 value)
    {
        ThrowIfOutOfMemory(address + 3);
        _memory[address + 3] = (byte) value;
        _memory[address + 2] = (byte) (value >> 8);
        _memory[address + 1] = (byte) (value >> 16);
        _memory[address] = (byte) (value >> 24);

    }
    
    public Int32 ReadDoubleWord(int address)
    {
        ThrowIfOutOfMemory(address + 3);
        Int32 firstHalf = (_memory[address] << 24) | (_memory[address + 1] << 16);
        Int32 secondHalf = (_memory[address + 2] << 8) | _memory[address + 3];
        return firstHalf | secondHalf;
    }
    
}