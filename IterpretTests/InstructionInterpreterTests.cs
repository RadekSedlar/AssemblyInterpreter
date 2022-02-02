using AssemblyInterpret;
using AssemblyInterpret.Interpreters;
using Xunit;
namespace IterpretTests;

public class InstructionInterpreterTests
{
    public Registers Registers { get; set; }
    public GlobalMemory Memory { get; set; }
    public InstructionInterpreter InstructionInterpreter { get; set; }
    public InstructionInterpreterTests()
    {
        Registers = new Registers();
        Memory = new GlobalMemory(10);
        InstructionInterpreter = new InstructionInterpreter(Memory, Registers);
    }
    
    [Fact]
    public void Push_5Const_EsiDecrementedAnd5Pushed()
    {
        int oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push 5");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp);
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(5, Memory.ReadDoubleWord(Registers.Esp));
    }
    
    [Fact]
    public void Push_32Register_EsiDecrementedAndRegisterPushed()
    {
        Registers.Ebx = 268;
        int oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push ebx");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp);
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(Registers.Ebx, Memory.ReadDoubleWord(Registers.Esp));
    }
}