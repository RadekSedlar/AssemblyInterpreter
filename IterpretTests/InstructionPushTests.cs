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
        int oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push 5");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(5, Memory.ReadDoubleWord(Registers.Esp));
    }
    
    [Fact]
    public void Push_32Register_EsiDecrementedAndRegisterPushed()
    {
        Registers.Ebx = 268;
        int oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push ebx");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(Registers.Ebx, Memory.ReadDoubleWord(Registers.Esp));
    }
    
    [Fact]
    public void Push_16Register_Throws()
    {
        Registers.Ebx = 268;
        int oldEsp = Registers.Esp;
        Assert.Throws<Exception>(() => InstructionInterpreter.InterpretLine("push cx"));
        // TODO create custom exception
    }
    
    [Fact]
    public void Push_8Register_Throws()
    {
        Registers.Ebx = 268;
        int oldEsp = Registers.Esp;
        Assert.Throws<Exception>(() => InstructionInterpreter.InterpretLine("push cx"));
        // TODO create custom exception
    }
    
    [Fact]
    public void Push_Memory_Throws()
    {
        Registers.Ebx = 5;
        Memory.SetDoubleWord(5, Int32.MaxValue);
        int oldEsp = Registers.Esp;
        InstructionInterpreter.InterpretLine("push [ebx]");
        
        Assert.Equal(Memory.MemorySize - 5, Registers.Esp); // Assert that ESP is one lower than MemorySize and decreased by 4 after push
        Assert.Equal(oldEsp - 4, Registers.Esp);
        Assert.Equal(Int32.MaxValue, Memory.ReadDoubleWord(Registers.Esp));
    }
}


public class InstructionMovTests
{
    public Registers Registers { get; set; }
    public GlobalMemory Memory { get; set; }
    public InstructionInterpreter InstructionInterpreter { get; set; }
    public InstructionMovTests()
    {
        Registers = new Registers();
        Memory = new GlobalMemory(10);
        InstructionInterpreter = new InstructionInterpreter(Memory, Registers);
    }

    [Fact]
    public void Mov_Reg32ToReg32_ValueMoved()
    {
        int endValue = 1111;
        Registers.Eax = Int32.MaxValue - 1;
        Registers.Ecx = endValue;
        
        InstructionInterpreter.InterpretLine("mov eax ecx");
        Assert.Equal(endValue, Registers.Ecx);
        Assert.Equal(Registers.Ecx, Registers.Eax);
    }
    
    [Fact]
    public void Mov_Reg16ToReg16_ValueMoved()
    {
        Int16 endValue = 1111;
        Registers.Ax = Int16.MaxValue - 1;
        Registers.Cx = endValue;
        
        InstructionInterpreter.InterpretLine("mov ax cx");
        Assert.Equal(endValue, Registers.Cx);
        Assert.Equal(Registers.Cx, Registers.Ax);
    }
    
    [Fact]
    public void Mov_Reg8ToReg8_ValueMoved()
    {
        byte endValue = 111;
        Registers.Ah = byte.MaxValue - 1;
        Registers.Cl = endValue;
        
        InstructionInterpreter.InterpretLine("mov ah cl");
        Assert.Equal(endValue, Registers.Cl);
        Assert.Equal(Registers.Cl, Registers.Ah);
    }
}