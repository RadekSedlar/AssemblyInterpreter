using AssemblyInterpret;
using AssemblyInterpret.Interpreters;

namespace Tests;

public class DataInterpreterTests
{
    [Fact]
    public void Interpret_Multiple_DataSection_Features()
    {
        GlobalMemory memory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(memory, """
                                                                var DB 64 ;Declare a byte containing the value 64. Label the
                                                                ; memory location “var”.
                                                                str DB 'hello',0 ; Declare 5 bytes starting at the address “str”
                                                                neco DB 9 
                                                                DB 9 
                                                                DB 9 
                                                                DW 9 
                                                                DW 9
                                                                DD 9
                                                                DD 9 
                                                                """);
        dataInterpret.InterpretDataSection();
        
        Assert.Equal(64, memory.ReadByte(0));
        Assert.Equal((byte)'h', memory.ReadByte(1));
        Assert.Equal((byte)'e', memory.ReadByte(2));
        Assert.Equal((byte)'l', memory.ReadByte(3));
        Assert.Equal((byte)'l', memory.ReadByte(4));
        Assert.Equal((byte)'o', memory.ReadByte(5));
        Assert.Equal(0, memory.ReadByte(6));
        Assert.Equal(9, memory.ReadByte(7));
        Assert.Equal(9, memory.ReadByte(8));
        Assert.Equal(9, memory.ReadByte(9));
        Assert.Equal(9, memory.ReadWord(10));
        Assert.Equal(9, memory.ReadWord(12));
        Assert.Equal((uint)9, memory.ReadDoubleWord(14));
        Assert.Equal((uint)9, memory.ReadDoubleWord(18));
        Assert.Equal(0, memory.ReadByte(22));
    }
    
    
    [Fact]
    public void Interpret_Global_Variable_Should_Set_One_Memory_Cell_And_Create_Variable()
    {
        GlobalMemory memory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(memory, """
                                                                var DB 64
                                                                """);
        dataInterpret.InterpretDataSection();
        
        Assert.Equal(64, memory.ReadByte(0));
        Assert.Equal(64, memory.ReadByte(memory.GetGlobalVar("var")!.Address));
    }
    
    [Fact]
    public void Interpret_Global_Variable_With_Comments_Should_Set_One_Memory_Cell_And_Create_Variable()
    {
        GlobalMemory memory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(memory, """
                                                                var DB 64 ;bababui
                                                                """);
        dataInterpret.InterpretDataSection();
        
        Assert.Equal(64, memory.ReadByte(0));
        Assert.Equal(64, memory.ReadByte(memory.GetGlobalVar("var")!.Address));
    }
    
    [Fact]
    public void Interpret_Global_String_Variable_Should_Set_One_Memory_Cell_For_Each_Character_And_Create_Variable()
    {
        GlobalMemory memory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(memory, """
                                                                strVar DB 'a6p'
                                                                """);
        dataInterpret.InterpretDataSection();
        
        Assert.Equal((byte)'a', memory.ReadByte(memory.GetGlobalVar("strVar")!.Address));
        Assert.Equal((byte)'6', memory.ReadByte(memory.GetGlobalVar("strVar")!.Address + 1));
        Assert.Equal((byte)'p', memory.ReadByte(memory.GetGlobalVar("strVar")!.Address + 2));
    }
    
    [Fact]
    public void Interpret_Global_Word_Array_Variable_Should_Set_Two_Memory_Cells_For_Each_Element_And_Create_Variable()
    {
        GlobalMemory memory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(memory, """
                                                                arr DW 69,42
                                                                """);
        dataInterpret.InterpretDataSection();
        
        Assert.Equal((UInt16)69, memory.ReadWord(memory.GetGlobalVar("arr")!.Address));
        Assert.Equal((UInt16)42, memory.ReadWord(memory.GetGlobalVar("arr")!.Address + 2));
    }
}