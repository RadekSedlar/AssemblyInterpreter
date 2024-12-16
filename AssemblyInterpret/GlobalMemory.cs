namespace AssemblyInterpret;

public class GlobalMemory
{
    private readonly byte[] _memory;
    public UInt32 MemorySize { get; set; }

    public GlobalMemory(UInt32 memorySize)
    {
        MemorySize = memorySize;
        _memory = new byte[memorySize];
    }
    
    public UInt32 TopPointer { get; set; }
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
    
    public void SetGlobalVar(string name, byte value) => SetByte(GetMemoryCellByName(name).Address, value);
    public void SetGlobalVar(string name, UInt16 value) => SetWord(GetMemoryCellByName(name).Address, value);
    public void SetGlobalVar(string name, UInt32 value) => SetDoubleWord(GetMemoryCellByName(name).Address, value);


    private MemoryCell GetMemoryCellByName(string name)
    {
        var memoryCell = MemoryCells.FirstOrDefault(x => x.Name == name);
        
        if (memoryCell is null)
            throw new Exception($"Global variable '{name}' does not exists");
        
        return memoryCell;
    }
    
    public MemoryCell? GetGlobalVar(string name)
    {
        var toBeReturned = MemoryCells.Find(x => x.Name == name);
        return toBeReturned;
    }

    private void ThrowIfOutOfMemory(UInt32 address)
    {
        if (address >= _memory.Length)
        {
            throw new OutOfMemoryException();
        }
    }
    
    public void SetByte(UInt32 address, byte value)
    {
        ThrowIfOutOfMemory(address);
        _memory[address] = value;
    }
    
    public byte ReadByte(UInt32 address)
    {
        ThrowIfOutOfMemory(address);
        return _memory[address];
    }
 
    public void SetWord(UInt32 address, UInt16 value)
    {
        ThrowIfOutOfMemory(address + 1);
        _memory[address] = (byte) (value >> 8);
        _memory[address + 1] = (byte)value;
    }
    
    public UInt16 ReadWord(UInt32 address)
    {
        ThrowIfOutOfMemory(address + 1);
        UInt16 retVal = _memory[address];
        retVal <<= 8;
        retVal += _memory[address + 1];
        return retVal;
    }
    
    public void SetDoubleWord(UInt32 address, UInt32 value)
    {
        ThrowIfOutOfMemory(address + 3);
        _memory[address + 3] = (byte) value;
        _memory[address + 2] = (byte) (value >> 8);
        _memory[address + 1] = (byte) (value >> 16);
        _memory[address] = (byte) (value >> 24);

    }
    
    public UInt32 ReadDoubleWord(UInt32 address)
    {
        ThrowIfOutOfMemory(address + 3);
        UInt32 retVal = _memory[address];
        retVal <<= 8;
        retVal += _memory[address + 1];
        retVal <<= 8;
        retVal += _memory[address + 2];
        retVal <<= 8;
        retVal += _memory[address + 3];
        return retVal;
    }
    
}