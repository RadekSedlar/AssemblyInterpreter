using AssemblyInterpret;
using AssemblyInterpret.Interpreters;

namespace Tests;

public class TextInterpreterTests
{
    private readonly GlobalMemory _globalMemory;
    private readonly Registers _registers;
    private readonly uint _eaxInitialValue = 1;
    private const uint EbxInitialValue = 0b11101110011010110010011011010000;
    private readonly uint _bxInitialValue = EbxInitialValue & 0b00000000000000001111111111111111;
    private readonly uint _blInitialValue = EbxInitialValue & 0b00000000000000000000000011111111;
    private readonly uint _bhInitialValue = EbxInitialValue & 0b00000000000000001111111100000000;
    
    public TextInterpreterTests()
    {
        _globalMemory = new GlobalMemory(40);
        DataInterpret dataInterpret = new DataInterpret(_globalMemory, """
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
        _registers = new Registers()
        {
            Eax = _eaxInitialValue,
            Ebx = EbxInitialValue
        };
    }
    
    
    [Fact]
    public void Interpret_Add_Register_Register_Should_Have_Added_Value_In_First_Register()
    {
        string textSection = "add eax ebx";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal(_eaxInitialValue + EbxInitialValue, _registers.Eax);
    }
    
    [Fact]
    public void Interpret_Add_HalfRegister_HalfRegister_Should_Have_Added_Value_In_First_Register()
    {
        string textSection = "add ax bx";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal(_eaxInitialValue + _bxInitialValue, _registers.Eax);
    }
    
    [Fact]
    public void Interpret_Add_LowerHalfRegister_LowerHalfRegister_Should_Have_Added_Value_In_First_Register()
    {
        string textSection = "add al bl";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal(_eaxInitialValue + _blInitialValue, _registers.Eax);
    }
    
    [Fact]
    public void Interpret_Add_HigherHalfRegister_HigherHalfRegister_Should_Have_Added_Value_In_First_Register()
    {
        string textSection = "add ah bh";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal(_eaxInitialValue + _bhInitialValue, _registers.Eax);
    }
    
    [Fact]
    public void Interpret_Add_Register_Memory_Should_Have_Added_Value_In_Register()
    {
        string textSection = "add eax [14]";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal((uint)10, _registers.Eax);
    }
    
    [Fact]
    public void Interpret_Add_Memory_Register_Should_Have_Added_Value_In_Memory()
    {
        string textSection = "add [18] eax";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.Equal((uint)10, _globalMemory.ReadDoubleWord(18));
    }
    
    [Fact]
    public void Interpret_Label_Should_Contain_Label()
    {
        string textSection = ".label_name";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        textInterpreter.InterpretSection();
        
        Assert.True(textInterpreter.Labels.ContainsKey(".label_name"));
    }
    
    [Fact]
    public void Interpret_Jmp_Without_Defining_Label_First_Should_Throw_Exception()
    {
        string textSection = "jmp .label_name";
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        
        Assert.Throws<Exception>(() => textInterpreter.InterpretSection());
    }
    
    /*[Fact]
    public void DOOM_OF_CPUS()
    {
        string textSection = """
                             .label_name
                             jmp .label_name
                             """;
        TextInterpreter textInterpreter = new TextInterpreter(_globalMemory, textSection,
            _registers);
        
        
        Assert.Throws<Exception>(() => textInterpreter.InterpretSection());
    }*/
    
}