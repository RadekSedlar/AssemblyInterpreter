using System.Text;
using System.Text.RegularExpressions;
using AssemblyInterpret.Scanners.TextSection;

namespace AssemblyInterpret.Interpreters;

public class TextInterpreter
{
    public GlobalMemory Memory { get; init; }
    public TextSectionScanner SectionScanner { get; init; }
    public Dictionary<string, GpRegister> Registers { get; init; }
    public TextInterpreter(GlobalMemory memory, string textSectionText, Dictionary<string, GpRegister> registers)
    {
        Memory = memory;
        Registers = registers;
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
                case TextSectionTokenType.Eof:
                    return;
                default:
                    throw new UnexpectedTokenTypeException($"Instruction is expected. '{token.Type} was given.'");
            }
        }
    }


    private void InterpretInstruction(string instruction)
    {
        switch (instruction.ToLower())
        {
            case "add":
                InterpretInstructionAdd();
                break;
            default:
                throw new UnexpectedTokenTypeException($"Instruction is expected. '{instruction}'.");
        }
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
            var finalRegister = Registers.GetGpRegisterByName(firstArgument.Lexeme);
            var addedRegister = Registers.GetGpRegisterByName(secondArgument.Lexeme);
            finalRegister.Register += addedRegister.Register;
            /*
             * "eax" or "ebx" or "ecx" or "edx" => (int)register.Register,
            "ax" or "bx" or "cx" or "dx" => register.Half,
            "al" or "bl" or "cl" or "dl"=> register.LowerHalf,
            "ah" or "bh" or "ch" or "dh" => register.HigherHalf,
             */
            
            return;
        }
        
        if (firstArgument.Type is ArgumentType.Register && secondArgument.Type is ArgumentType.Address)
        {
            var finalRegister = Registers.GetGpRegisterByName(firstArgument.Lexeme);
            var valueFromMemory = Memory.ReadDoubleWord((uint) secondArgument.Value);
            finalRegister.Register += valueFromMemory;
            return;
        }
        
    }

    private int InterpretValueOfAddress(
        int numberOfRegistersThatCanBeUsed,
        bool canBeConstantUsed,
        bool canBeRegisterPremultiplied,
        bool wasLastInterpretedAddressPartRegister,
        int tempValueOfAddress)
    {
        (bool wasRegisterUsed, bool wasConstantUsed, bool wasRegisterPremultiplied, int valueOfExpression, bool addressEnd) = InterpretPartOfAddress();

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
        int valueOfExpression,
        bool addressEnd) InterpretPartOfAddress()
    {
        var token = SectionScanner.GetToken();
        
        if (token.Type == TextSectionTokenType.Register)
        {
            return (true, false, false,GetValueFromRegisterOrThrow(token.Lexeme), CheckIfAddressEnded());
        }

        if (token.Type == TextSectionTokenType.Number)
        {
            var timesCheckToken = SectionScanner.GetToken();
            var registerCheckToken = SectionScanner.GetToken();
            if (timesCheckToken.Type == TextSectionTokenType.Times && registerCheckToken.Type == TextSectionTokenType.Register)
            {
                return (true, false, true, GetValueFromRegisterOrThrow(registerCheckToken.Lexeme), CheckIfAddressEnded());
            }
            
            SectionScanner.ReturnToken(registerCheckToken);
            SectionScanner.ReturnToken(timesCheckToken);
            
            return (false, true, false, int.Parse(token.Lexeme), CheckIfAddressEnded());
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
        
        SectionScanner.ReturnToken(token);
        return false;
    }

    private int GetValueFromRegisterOrThrow(string registerName)
    {
        var isRegisterFound = Registers.TryGetValue(registerName, out GpRegister? register);
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
            "eax" or "ebx" or "ecx" or "edx" => (int)register.Register,
            "ax" or "bx" or "cx" or "dx" => register.Half,
            "al" or "bl" or "cl" or "dl"=> register.LowerHalf,
            "ah" or "bh" or "ch" or "dh" => register.HigherHalf,
            _ => throw new ArgumentOutOfRangeException(nameof(registerName), registerName, null)
        };
    }
    
    private List<Argument> GetArguments()
    {
        List<Argument> arguments = [];
        while (true)
        {
            var token = SectionScanner.GetToken();

            switch (token.Type)
            {
                case TextSectionTokenType.Register:
                    arguments.Add(new Argument(GetValueFromRegisterOrThrow(token.Lexeme), ArgumentType.Register, token.Lexeme));
                    break;
                case TextSectionTokenType.Number:
                    arguments.Add(new Argument(int.Parse(token.Lexeme), ArgumentType.Value, token.Lexeme));
                    break;
                case TextSectionTokenType.SqBracketOpen:
                    arguments.Add(new Argument(InterpretValueOfAddress(2, true, true, false, 0), ArgumentType.Address, token.Lexeme));
                    break;
                case TextSectionTokenType.NewLine:
                    return arguments;
                default:
                    throw new UnexpectedTokenTypeException($"Instruction is expected. '{token.Type}' was given.");
            }
        }
    }
}

public class Argument
{
    public int Value { get; init; }
    public string Lexeme { get; init; }
    public ArgumentType Type { get; init; }

    public Argument(int value, ArgumentType type, string lexeme)
    {
        Value = value;
        Type = type;
        Lexeme = lexeme;
    }
}

public enum ArgumentType
{
    Address, Value, Register
}


public class UnexpectedTokenTypeException : Exception
{
    public UnexpectedTokenTypeException(string message) : base(message) { }
}