using AssemblyInterpret;

namespace Tests;

public class GlobalMemoryTests
{
    [Fact]
    public void Set_Byte_Value_To_Global_Memory_Should_Have_Value_In_One_Cell()
    {
        GlobalMemory memory = new GlobalMemory(20);
        UInt32 address = 5;
        byte value = 255;
        
        memory.SetByte(address, value);

        Assert.Equal(0, memory.ReadByte(address + 1));
        Assert.Equal(value, memory.ReadByte(address));
        Assert.Equal(0, memory.ReadByte(address - 1));
    }
    
    [Fact]
    public void Set_Word_Value_To_Global_Memory_Should_Have_Value_In_Two_Cells()
    {
        GlobalMemory memory = new GlobalMemory(20);
        UInt32 address = 5;
        UInt16 value = 0b1100011010010110;
        
        memory.SetWord(address, value);
        
        Assert.Equal(0, memory.ReadByte(address - 1));
        
        Assert.Equal(0b11000110, memory.ReadByte(address));
        Assert.Equal(0b10010110, memory.ReadByte(address + 1));
        
        Assert.Equal(0, memory.ReadByte(address + 2));
        
        Assert.Equal(0b1100011010010110, memory.ReadWord(address));
    }
    
    [Fact]
    public void Set_DWord_Value_To_Global_Memory_Should_Have_Value_In_Four_Cells()
    {
        GlobalMemory memory = new GlobalMemory(20);
        UInt32 address = 5;
        UInt32 value = 0b11101110011010110010011011010000;
        
        memory.SetDoubleWord(address, value);

        Assert.Equal(0, memory.ReadByte(address - 1));
        
        Assert.Equal(0b11101110, memory.ReadByte(address));
        Assert.Equal(0b01101011, memory.ReadByte(address + 1));
        Assert.Equal(0b00100110, memory.ReadByte(address + 2));
        Assert.Equal(0b11010000, memory.ReadByte(address + 3));
        
        Assert.Equal(0, memory.ReadByte(address + 4));
        
        Assert.Equal(0b1110111001101011, memory.ReadWord(address));
        Assert.Equal(0b0010011011010000, memory.ReadWord(address + 2));
        
        Assert.Equal(0b11101110011010110010011011010000, memory.ReadDoubleWord(address));
    }

    [Fact]
    public void Set_Global_Variable_Should_Retrieve_Same_Value()
    {
        GlobalMemory memory = new GlobalMemory(20);
        uint value = 5;
        string nameOfVariable = "var22";
        memory.AddGlobalVar(new MemoryCell(ByteCount.DW, nameOfVariable, 0));
        
        memory.SetGlobalVar(nameOfVariable, value);
        
        Assert.Equal((uint)0, memory.GetGlobalVar(nameOfVariable)!.Address);
        Assert.Equal(value, memory.ReadDoubleWord(memory.GetGlobalVar(nameOfVariable)!.Address));
    }
}