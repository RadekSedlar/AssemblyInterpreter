using System;
using AssemblyInterpret;
using AssemblyInterpret.Interpreters;
using Xunit;

namespace IterpretTests;

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
        uint endValue = 1111;
        Registers.Eax = UInt32.MaxValue - 1;
        Registers.Ecx = endValue;
        
        InstructionInterpreter.InterpretLine("mov eax ecx");
        Assert.Equal(endValue, Registers.Ecx);
        Assert.Equal(Registers.Ecx, Registers.Eax);
    }
    
    [Fact]
    public void Mov_Reg16ToReg16_ValueMoved()
    {
        UInt16 endValue = 1111;
        Registers.Ax = UInt16.MaxValue - 1;
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
    
    [Fact]
    public void Mov_ByteFromRegToMemory_ValueMoved()
    {
        // Arrange
        uint address = 3;
        byte endValue = 33;
        Registers.Cl = endValue;

        // Act
        InstructionInterpreter.InterpretLine($"mov [{address}] cl");
        
        // Assert
        Assert.Equal(endValue, Memory.ReadByte(address));
    }
    
    
}