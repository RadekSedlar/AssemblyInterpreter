using System.Text;
using System.Text.RegularExpressions;
using AssemblyInterpret.Scanners.TextSection;

namespace AssemblyInterpret.Interpreters;


/// <summary>
/// Need to implement
/// 1. explicit instruction size, for memory to memory operations.
/// 2. labels - DONE
/// 3. jmp - DONE
/// 4. cmp and zero flags
/// 5. conditional jumps
/// </summary>
public class TextInterpreter
{
    public GlobalMemory Memory { get; init; }
    public TextSectionScanner SectionScanner { get; init; }
    public Registers Registers { get; init; }
    
    public Dictionary<string, int> LocalLabels { get; init; }
    public Dictionary<string, int> GlobalLabels { get; init; }
    public int CurrentSectionGlobalOffset { get; init; }
    public TextInterpreter(GlobalMemory memory, string textSectionText, Registers registers, Dictionary<string, int> globalLabels, int lineOffset)
    {
        Memory = memory;
        Registers = registers;
        GlobalLabels = globalLabels;
        LocalLabels = new ();
        CurrentSectionGlobalOffset = lineOffset;
        var sb = new StringBuilder();
        var lines = textSectionText.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            sb.AppendLine(StripCommentsAndTrimRawLineText(line));
        }

        SectionScanner = new TextSectionScanner(sb.ToString());
        SectionScanner.Scan();
    }
    
    private string StripCommentsAndTrimRawLineText(string rawLineText) =>
        Regex.Replace(rawLineText, @"[;].*", "").Trim();
    
    public void InterpretSection()
    {
        while (true)
        {
            var token = SectionScanner.GetToken();
            switch (token.Type)
            {
                case TextSectionTokenType.NewLine:
                    continue;
                case TextSectionTokenType.Word:
                    InterpretInstruction(token.Lexeme);
                    break;
                case TextSectionTokenType.Label:
                    RegisterLabel(token.Lexeme, SectionScanner.CurrentTokenIndex, token.Line);
                    break;
                case TextSectionTokenType.Eof:
                    return;
                default:
                    throw new UnexpectedTokenTypeException($"Instruction is expected. '{token.Type} was given.'");
            }
        }
    }

    private void RegisterLabel(string tokenLexeme, int sectionScannerCurrentTokenIndex,  int line)
    {
        if (GlobalLabels.TryGetValue(tokenLexeme, out int alreadyDefinedLabelTokenIndex))
        {
            throw new Exception("Label already defined.");
        }
        
        GlobalLabels[tokenLexeme] = line + CurrentSectionGlobalOffset - 1; // -1 because global lines are indexed from 0
        LocalLabels[tokenLexeme] = sectionScannerCurrentTokenIndex;
    }


    private void InterpretInstruction(string instruction)
    {
        switch (instruction.ToLower())
        {
            case "add":
                InterpretInstructionAdd();
                break;
            case "jmp":
                InterpretInstructionJmp();
                break;
            case "cmp":
                InterpretInstructionCmp();
                break;
            case "je":
                InterpretInstructionJe();
                break;
            default:
                throw new UnexpectedTokenTypeException($"Instruction is expected. '{instruction}'.");
        }
    }

    private void InterpretInstructionJe()
    {
        var arguments = GetArguments();

        if (GetArguments().Count != 0)
        {
            throw new Exception($"Unexpected number of arguments. Expected 0, got {arguments.Count}.");
        }
        
        
    }

    private void InterpretInstructionCmp()
    {
        var arguments = GetArguments();

        if (arguments.Count != 2)
        {
            throw new Exception($"Unexpected number of arguments. Expected 2, got {arguments.Count}.");
        }
        
        var firstArgument = arguments[0];
        var secondArgument = arguments[1];

        if (firstArgument.Type is ArgumentType.Register && secondArgument.Type is ArgumentType.Register)
        {
            if (firstArgument.Bytes != secondArgument.Bytes)
            {
                throw new Exception("Registers of different sizes were used");
            }

            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstRegisterValue = Registers.GetByteRegister(firstArgument.Lexeme);
                byte secondRegisterValue = Registers.GetByteRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstRegisterValue == secondRegisterValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstRegisterValue = Registers.GetWordRegister(firstArgument.Lexeme);
                ushort secondRegisterValue = Registers.GetWordRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstRegisterValue == secondRegisterValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstRegisterValue = Registers.GetDoubleWordRegister(firstArgument.Lexeme);
                uint secondRegisterValue = Registers.GetDoubleWordRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstRegisterValue == secondRegisterValue;
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Register && secondArgument.Type is ArgumentType.Address)
        {
            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstRegisterValue = Registers.GetByteRegister(firstArgument.Lexeme);
                byte secondMemoryValue = Memory.ReadByte(secondArgument.Value);
                
                Registers.ZeroFlag = firstRegisterValue == secondMemoryValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstRegisterValue = Registers.GetWordRegister(firstArgument.Lexeme);
                ushort secondMemoryValue = Memory.ReadWord(secondArgument.Value);
                
                Registers.ZeroFlag = firstRegisterValue == secondMemoryValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstRegisterValue = Registers.GetDoubleWordRegister(firstArgument.Lexeme);
                uint secondMemoryValue = Memory.ReadDoubleWord(secondArgument.Value);
                
                Registers.ZeroFlag = firstRegisterValue == secondMemoryValue;
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Address && secondArgument.Type is ArgumentType.Register)
        {
            if (secondArgument.Bytes is ByteCount.DB)
            {
                byte firstMemoryValue = Memory.ReadByte(firstArgument.Value);
                byte secondRegisterValue = Registers.GetByteRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstMemoryValue == secondRegisterValue;
                return;
            }
            
            if (secondArgument.Bytes is ByteCount.DW)
            {
                ushort firstMemoryValue = Memory.ReadWord(firstArgument.Value);
                ushort secondRegisterValue = Registers.GetWordRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstMemoryValue == secondRegisterValue;
                return;
            }
            
            if (secondArgument.Bytes is ByteCount.DD)
            {
                uint firstMemoryValue = Memory.ReadDoubleWord(firstArgument.Value);
                uint secondRegisterValue = Registers.GetDoubleWordRegister(secondArgument.Lexeme);
                
                Registers.ZeroFlag = firstMemoryValue == secondRegisterValue;
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Address && secondArgument.Type is ArgumentType.Address)
        {
            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstMemoryValue = Memory.ReadByte(firstArgument.Value);
                byte secondMemoryValue = Memory.ReadByte(secondArgument.Value);
                
                Registers.ZeroFlag = firstMemoryValue == secondMemoryValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstMemoryValue = Memory.ReadWord(firstArgument.Value);
                ushort secondMemoryValue = Memory.ReadWord(secondArgument.Value);
                
                Registers.ZeroFlag = firstMemoryValue == secondMemoryValue;
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstMemoryValue = Memory.ReadDoubleWord(firstArgument.Value);
                uint secondMemoryValue = Memory.ReadDoubleWord(secondArgument.Value);
                
                Registers.ZeroFlag = firstMemoryValue == secondMemoryValue;
                return;
            }
        }
        
        throw new NotImplementedException();
    }

    private void InterpretInstructionJmp()
    {
        var arguments = GetArguments();
        
        if (arguments.Count != 1)
        {
            throw new Exception($"Unexpected number of arguments. Expected 1, got {arguments.Count}.");
        }
        
        var firstArgument = arguments[0];

        if (firstArgument.Type is ArgumentType.Label)
        {
            if (!LocalLabels.TryGetValue(firstArgument.Lexeme, out int indexOfLabel))
            {
                if (!GlobalLabels.TryGetValue(firstArgument.Lexeme, out int globalLineIndex))
                {
                    throw new Exception("Label must be first defined before jumping to it.");    
                }

                throw new GlobalJumpException(globalLineIndex);
            }

            SectionScanner.CurrentTokenIndex = indexOfLabel;
            return;
        }
        
        throw new NotImplementedException();
    }
    
    private void InterpretInstructionAdd()
    {
        var arguments = GetArguments();

        if (arguments.Count != 2)
        {
            throw new Exception($"Unexpected number of arguments. Expected 2, got {arguments.Count}.");
        }
        
        var firstArgument = arguments[0];
        var secondArgument = arguments[1];

        if (firstArgument.Type is ArgumentType.Register && secondArgument.Type is ArgumentType.Register)
        {
            if (firstArgument.Bytes != secondArgument.Bytes)
            {
                throw new Exception("Registers of different sizes were used");
            }

            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstRegisterValue = Registers.GetByteRegister(firstArgument.Lexeme);
                byte secondRegisterValue = Registers.GetByteRegister(secondArgument.Lexeme);
                
                Registers.SetByteRegister(firstArgument.Lexeme, (byte)(firstRegisterValue + secondRegisterValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstRegisterValue = Registers.GetWordRegister(firstArgument.Lexeme);
                ushort secondRegisterValue = Registers.GetWordRegister(secondArgument.Lexeme);
                
                Registers.SetWordRegister(firstArgument.Lexeme, (ushort)(firstRegisterValue + secondRegisterValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstRegisterValue = Registers.GetDoubleWordRegister(firstArgument.Lexeme);
                uint secondRegisterValue = Registers.GetDoubleWordRegister(secondArgument.Lexeme);
                
                Registers.SetDoubleWordRegister(firstArgument.Lexeme, firstRegisterValue + secondRegisterValue);
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Register && secondArgument.Type is ArgumentType.Address)
        {
            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstRegisterValue = Registers.GetByteRegister(firstArgument.Lexeme);
                byte secondMemoryValue = Memory.ReadByte(secondArgument.Value);
                
                Registers.SetByteRegister(firstArgument.Lexeme, (byte)(firstRegisterValue + secondMemoryValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstRegisterValue = Registers.GetWordRegister(firstArgument.Lexeme);
                ushort secondMemoryValue = Memory.ReadWord(secondArgument.Value);
                
                Registers.SetWordRegister(firstArgument.Lexeme, (ushort)(firstRegisterValue + secondMemoryValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstRegisterValue = Registers.GetDoubleWordRegister(firstArgument.Lexeme);
                uint secondMemoryValue = Memory.ReadDoubleWord(secondArgument.Value);
                
                Registers.SetDoubleWordRegister(firstArgument.Lexeme, firstRegisterValue + secondMemoryValue);
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Address && secondArgument.Type is ArgumentType.Register)
        {
            if (secondArgument.Bytes is ByteCount.DB)
            {
                byte firstMemoryValue = Memory.ReadByte(firstArgument.Value);
                byte secondRegisterValue = Registers.GetByteRegister(secondArgument.Lexeme);
                
                Memory.SetByte(firstArgument.Value, (byte)(firstMemoryValue + secondRegisterValue));
                return;
            }
            
            if (secondArgument.Bytes is ByteCount.DW)
            {
                ushort firstMemoryValue = Memory.ReadWord(firstArgument.Value);
                ushort secondRegisterValue = Registers.GetWordRegister(secondArgument.Lexeme);
                
                Memory.SetWord(firstArgument.Value, (ushort)(firstMemoryValue + secondRegisterValue));
                return;
            }
            
            if (secondArgument.Bytes is ByteCount.DD)
            {
                uint firstMemoryValue = Memory.ReadDoubleWord(firstArgument.Value);
                uint secondRegisterValue = Registers.GetDoubleWordRegister(secondArgument.Lexeme);
                
                Memory.SetDoubleWord(firstArgument.Value, (uint)(firstMemoryValue + secondRegisterValue));
                return;
            }
        }
        
        if (firstArgument.Type is ArgumentType.Address && secondArgument.Type is ArgumentType.Address)
        {
            if (firstArgument.Bytes is ByteCount.DB)
            {
                byte firstMemoryValue = Memory.ReadByte(firstArgument.Value);
                byte secondMemoryValue = Memory.ReadByte(secondArgument.Value);
                
                Memory.SetByte(firstArgument.Value, (byte)(firstMemoryValue + secondMemoryValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DW)
            {
                ushort firstMemoryValue = Memory.ReadWord(firstArgument.Value);
                ushort secondMemoryValue = Memory.ReadWord(secondArgument.Value);
                
                Memory.SetWord(firstArgument.Value, (ushort)(firstMemoryValue + secondMemoryValue));
                return;
            }
            
            if (firstArgument.Bytes is ByteCount.DD)
            {
                uint firstMemoryValue = Memory.ReadDoubleWord(firstArgument.Value);
                uint secondMemoryValue = Memory.ReadDoubleWord(secondArgument.Value);
                
                Memory.SetDoubleWord(firstArgument.Value, (uint)(firstMemoryValue + secondMemoryValue));
                return;
            }
        }
        
        throw new NotImplementedException();
    }

    private uint InterpretValueOfAddress(
        int numberOfRegistersThatCanBeUsed,
        bool canBeConstantUsed,
        bool canBeRegisterPremultiplied,
        bool wasLastInterpretedAddressPartRegister,
        uint tempValueOfAddress)
    {
        (bool wasRegisterUsed, bool wasConstantUsed, bool wasRegisterPremultiplied, uint valueOfExpression, bool addressEnd) = InterpretPartOfAddress();

        if (canBeConstantUsed is false && wasConstantUsed)
        {
            throw new Exception("Constant was already used.");
        }
            
        if (canBeRegisterPremultiplied is false && wasRegisterPremultiplied)
        {
            throw new Exception("Register was already premultiplied.");
        }

        if (numberOfRegistersThatCanBeUsed == 0 && wasRegisterUsed)
        {
            throw new Exception("Only 2 registers can be used in addressing.");
        }

        if (wasConstantUsed)
        {
            if (addressEnd)
            {
                return valueOfExpression;
            }
            
            
            var additionOrSubtractionToken = SectionScanner.GetToken();
            if (additionOrSubtractionToken.Type is TextSectionTokenType.Plus)
            {
                return InterpretValueOfAddress(
                    numberOfRegistersThatCanBeUsed,
                    false,
                    canBeRegisterPremultiplied,
                    false,
                    valueOfExpression) + tempValueOfAddress;
            }
            if (additionOrSubtractionToken.Type is TextSectionTokenType.Minus)
            {
                return tempValueOfAddress - InterpretValueOfAddress(
                    numberOfRegistersThatCanBeUsed,
                    false,
                    canBeRegisterPremultiplied,
                    false,
                    valueOfExpression);
            }
            throw new UnexpectedTokenTypeException($"{nameof(TextSectionTokenType.Plus)} or {nameof(TextSectionTokenType.Minus)} was expected. '{additionOrSubtractionToken.Type} was given.");
        }
        
        if (wasRegisterUsed)
        {
            if (addressEnd)
            {
                return valueOfExpression;
            }
                
            var additionOrSubtractionToken = SectionScanner.GetToken();
            if (additionOrSubtractionToken.Type == TextSectionTokenType.Plus)
            {
                return InterpretValueOfAddress(
                    numberOfRegistersThatCanBeUsed - 1,
                    canBeConstantUsed,
                    canBeRegisterPremultiplied | wasRegisterPremultiplied,
                    wasRegisterUsed,
                    valueOfExpression) + tempValueOfAddress;
            }
            if (additionOrSubtractionToken.Type == TextSectionTokenType.Minus)
            {
                if (wasLastInterpretedAddressPartRegister)
                {
                    throw new Exception("Cannot subtract two registers");
                }
                
                return tempValueOfAddress - InterpretValueOfAddress(
                    numberOfRegistersThatCanBeUsed - 1,
                    canBeConstantUsed,
                    canBeRegisterPremultiplied | wasRegisterPremultiplied,
                    wasRegisterUsed,
                    valueOfExpression);
            }

            throw new UnexpectedTokenTypeException($"{nameof(TextSectionTokenType.Plus)} or {nameof(TextSectionTokenType.Minus)} was expected. '{additionOrSubtractionToken.Type} was given.");
        }
        throw new Exception($"I dont know. Debug me.");
    }

    private (
        bool wasRegisterUsed,
        bool wasConstantUsed,
        bool wasRegisterPremultiplied,
        uint valueOfExpression,
        bool addressEnd) InterpretPartOfAddress()
    {
        var token = SectionScanner.GetToken();
        
        if (token.Type == TextSectionTokenType.Register)
        {
            var (valueFromRegister, _) = GetValueFromRegisterOrThrow(token.Lexeme);
            return (true, false, false, valueFromRegister, CheckIfAddressEnded());
        }

        if (token.Type == TextSectionTokenType.Number)
        {
            var timesCheckToken = SectionScanner.GetToken();
            var registerCheckToken = SectionScanner.GetToken();
            if (timesCheckToken.Type == TextSectionTokenType.Times && registerCheckToken.Type == TextSectionTokenType.Register)
            {
                var (valueFromRegister, _) = GetValueFromRegisterOrThrow(registerCheckToken.Lexeme);
                return (true, false, true, valueFromRegister, CheckIfAddressEnded());
            }
            
            SectionScanner.ReturnToken();
            SectionScanner.ReturnToken();
            
            return (false, true, false, uint.Parse(token.Lexeme), CheckIfAddressEnded());
        }
        
        throw new UnexpectedTokenTypeException($"{nameof(TextSectionTokenType.Number)} or {nameof(TextSectionTokenType.Register)} was expected. '{token.Type} was given.");
    }

    private bool CheckIfAddressEnded()
    {
        var token = SectionScanner.GetToken();
        if (token.Type == TextSectionTokenType.SqBracketClose)
        {
            return true;
        }
        
        SectionScanner.ReturnToken();
        return false;
    }

    private (uint, ByteCount) GetValueFromRegisterOrThrow(string registerName)
    {
        var isRegisterFound = Registers.GPRegisters.TryGetValue(registerName, out GpRegister? register);
        if (!isRegisterFound)
        {
            throw new Exception($"Unknown register '{registerName}'.");
        }
        
        if (register is null)
        {
            throw new Exception($"Register '{registerName}' not initialized");
        }

        return registerName switch
        {
            "eax" or "ebx" or "ecx" or "edx" => (register.Register, ByteCount.DD),
            "ax" or "bx" or "cx" or "dx" => (register.Half, ByteCount.DW),
            "al" or "bl" or "cl" or "dl"=> (register.LowerHalf, ByteCount.DB),
            "ah" or "bh" or "ch" or "dh" => (register.HigherHalf,ByteCount.DB),
            _ => throw new ArgumentOutOfRangeException(nameof(registerName), registerName, null)
        };
    }
    
    private List<Argument> GetArguments()
    {
        List<Argument> arguments = [];
        while (true)
        {
            var token = SectionScanner.GetToken();

            uint value;
            ByteCount byteCount;
            
            switch (token.Type)
            {
                case TextSectionTokenType.Register:
                    (value, byteCount) = GetValueFromRegisterOrThrow(token.Lexeme);
                    arguments.Add(new Argument(value, ArgumentType.Register, token.Lexeme, byteCount));
                    break;
                case TextSectionTokenType.Number:
                    value = uint.Parse(token.Lexeme);
                    byteCount = value switch
                    {
                        <= 255 => ByteCount.DB,
                        65535 => ByteCount.DW,
                        _ => ByteCount.DD,
                    };
                    arguments.Add(new Argument(value, ArgumentType.Value, token.Lexeme, byteCount));
                    break;
                case TextSectionTokenType.SqBracketOpen:
                    arguments.Add(new Argument(InterpretValueOfAddress(2, true, true, false, 0), ArgumentType.Address, token.Lexeme, ByteCount.DB));
                    break;
                case TextSectionTokenType.Label:
                    arguments.Add(new Argument(0, ArgumentType.Label, token.Lexeme, ByteCount.DB));
                    break;
                case TextSectionTokenType.NewLine or TextSectionTokenType.Eof:
                    return arguments;
                default:
                    throw new UnexpectedTokenTypeException($"Instruction is expected. '{token.Type}' was given.");
            }
        }
    }
}

public class Argument
{
    public uint Value { get; init; }
    public string Lexeme { get; init; }
    public ArgumentType Type { get; init; }
    public ByteCount Bytes { get; init; }

    public Argument(uint value, ArgumentType type, string lexeme, ByteCount bytes)
    {
        Value = value;
        Type = type;
        Lexeme = lexeme;
        Bytes = bytes;
    }
}

public enum ArgumentType
{
    Address, Value, Register, Label
}


public class UnexpectedTokenTypeException : Exception
{
    public UnexpectedTokenTypeException(string message) : base(message) { }
}

public class GlobalJumpException : Exception
{
    public int GlobalLineJump { get; set; }

    public GlobalJumpException(int lineToJump) : base("Expected jump")
    {
        GlobalLineJump = lineToJump;
    }
}