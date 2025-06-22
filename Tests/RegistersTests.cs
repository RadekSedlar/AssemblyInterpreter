using AssemblyInterpret;

namespace Tests;

public class RegistersTests
{
    [Fact]
    public void Eax_Register_Accessed_By_Dictionary_Access_Has_Expected_Values()
    {
        Registers registers = new Registers();
        registers.GPRegisters["eax"].Register = 3999999696;
        
        Assert.Equal(0b11101110011010110010011011010000, registers.GPRegisters["eax"].Register);
        Assert.Equal(0b0010011011010000, registers.GPRegisters["eax"].Half);
        Assert.Equal(0b11010000, registers.GPRegisters["eax"].LowerHalf);
        Assert.Equal(0b00100110, registers.GPRegisters["eax"].HigherHalf);
        
        Assert.Equal("11101110011010110010011011010000", registers.GPRegisters["eax"].RegisterText);
        Assert.Equal("0010011011010000", registers.GPRegisters["eax"].HalfText);
        Assert.Equal("11010000", registers.GPRegisters["eax"].LowerHalfText);
        Assert.Equal("00100110", registers.GPRegisters["eax"].HigherHalfText);
    }
    
    [Fact]
    public void Eax_Register_Accessed_By_Property_Access_Has_Expected_Values()
    {
        Registers registers = new Registers();
        registers.Eax = 3999999696;
        
        Assert.Equal(0b11101110011010110010011011010000, registers.Eax);
        Assert.Equal(0b0010011011010000, registers.Ax);
        Assert.Equal(0b11010000, registers.Al);
        Assert.Equal(0b00100110, registers.Ah);
    }
}