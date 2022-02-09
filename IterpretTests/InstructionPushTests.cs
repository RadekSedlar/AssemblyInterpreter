using System;
using AssemblyInterpret;
using AssemblyInterpret.Interpreters;
using Xunit;
namespace IterpretTests;

public class InstructionPushTests
{
    public Registers Registers { get; set; }
    public GlobalMemory Memory { get; set; }
    public InstructionInterpreter InstructionInterpreter { get; set; }
    public InstructionPushTests()
    {
        Registers = new Registers();
        Memory = new GlobalMemory(10);
        InstructionInterpreter = new InstructionInterpreter(Memory, Registers);
    }
    
    [Fact]
    public void Push_5Const_EsiDecrementedAnd5Pushed()
    {
        uint oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push 5");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal((uint)5, Memory.ReadDoubleWord(Registers.Esp));
    }
    
    [Fact]
    public void Push_32Register_EsiDecrementedAndRegisterPushed()
    {
        Registers.Ebx = 268;
        uint oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push ebx");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(Registers.Ebx, Memory.ReadDoubleWord(Registers.Esp));
    }
    
    [Fact]
    public void Push_16Register_Throws()
    {
        Registers.Ebx = 268;
        uint oldEsp = Registers.Esp;
        Assert.Throws<Exception>(() => InstructionInterpreter.InterpretLine("push cx"));
        // TODO create custom exception
    }
    
    [Fact]
    public void Push_8Register_Throws()
    {
        Registers.Ebx = 268;
        uint oldEsp = Registers.Esp;
        Assert.Throws<Exception>(() => InstructionInterpreter.InterpretLine("push cx"));
        // TODO create custom exception
    }
    
    [Fact]
    public void Push_Memory_Throws()
    {
        Registers.Ebx = 5;
        Memory.SetDoubleWord(5, UInt32.MaxValue);
        uint oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push [ebx]");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(UInt32.MaxValue, Memory.ReadDoubleWord(Registers.Esp));
    }
}