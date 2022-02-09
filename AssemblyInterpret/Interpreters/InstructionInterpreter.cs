using System.Text.RegularExpressions;

namespace AssemblyInterpret.Interpreters;

public class InstructionInterpreter
{
    private Dictionary<string, Action<string>> _instructions;
    private GlobalMemory _memory;

    private bool _sizeExplicitlyStated = false;
    private ByteCount _instructionSize = ByteCount.DD;
    private Registers _registers;

    public InstructionInterpreter(GlobalMemory memory, Registers registers)
    {
        _registers = registers;
        _instructions = new Dictionary<string, Action<string>>
        {
            {"add", Add},
            {"mov", Mov},
            {"push", Push}
        };
        _memory = memory;

        _registers.Esp = _memory.MemorySize - 1;
    }

    private (UInt32, bool) ValueOfAddressPart(string addressPart)
    {
        var registerFindingResult = _registers.FindRegister(addressPart);
        if (registerFindingResult.Item1)
        {
            if (registerFindingResult.Item2 == ByteCount.DB)
                return (_registers.GetByteRegister(addressPart), true);
            if (registerFindingResult.Item2 == ByteCount.DW)
                return (_registers.GetWordRegister(addressPart), true);
            if (registerFindingResult.Item2 == ByteCount.DD)
                return (_registers.GetDoubleWordRegister(addressPart), true);
        }

        var globalVarFindingResult = _memory.GetGlobalVar(addressPart);
        if (globalVarFindingResult is not null)
        {
            return (_memory.ReadDoubleWord(globalVarFindingResult.Address), false);
        }
        
        if (UInt32.TryParse(addressPart, out UInt32 constantValue))
        {
            return (constantValue, false);
        }

        throw new Exception($"'{addressPart}' could not be translated to address.");
    }
    
    private UInt32 ResolveMemoryAddress(string addressText)
    {
        string[] addressParts = addressText.Split('+', '-');
        if (addressParts.Length == 1)
        {
            return ValueOfAddressPart(addressParts[0]).Item1;
        }

        if (addressParts.Length == 2)
        {
            var addressValue1 = ValueOfAddressPart(addressParts[0]);
            var addressValue2 = ValueOfAddressPart(addressParts[1]);

            var addition = addressText.Contains('+');

            if (!addition)
            {
                if (addressValue1.Item2 && addressValue2.Item2)
                {
                    throw new Exception($"Cannot subtract two registers. '{addressText}'");
                }

                return addressValue1.Item1 - addressValue2.Item1;
            }
            return addressValue1.Item1 + addressValue2.Item1;
        }

        throw new Exception($"Address cannot be composed from {addressParts.Length} parts");
    }
    
    private List<InstructionArgument> TokenizeArguments(string arguments)
    {
        var separatedArguments = arguments.Split(' ', ',').ToList();
        separatedArguments.RemoveAll(string.IsNullOrWhiteSpace);
        

        List<InstructionArgument> instructionArguments = new List<InstructionArgument>();
        foreach (var argument in separatedArguments)
        {
            var memoryMatch = Regex.Match(argument, @"(?<=\[).*(?=\])");
            if (memoryMatch.Success)
            {
                // Is memory
                instructionArguments.Add(new MemoryArgument(ResolveMemoryAddress(memoryMatch.Value)));
                continue;
            }

            var findRegisterResult =_registers.FindRegister(argument);
            if (findRegisterResult.Item1)
            {
                // Is register
                instructionArguments.Add(new RegisterArgument(argument, findRegisterResult.Item2));
                continue;
            }

            var findGlobalVariableResult = _memory.GetGlobalVar(argument);
            if (findGlobalVariableResult is not null)
            {
                // Is global variable
                instructionArguments.Add(new VariableArgument(findGlobalVariableResult));
                continue;
            }

            if (UInt32.TryParse(argument, out UInt32 constantValue))
            {
                // Is constant
                instructionArguments.Add(new ConstantArgument(constantValue));
                continue;
            }

            throw new Exception($"'{argument}' cannot be interpreted as argument.");
        }

        return instructionArguments;
    }


    string CheckForSize(string line)
    {
        var sizeMatch = Regex.Match(line, @"[A-Z]{4,5}[ ]{1,}PTR");
        if (!sizeMatch.Success)
        {
            _instructionSize = ByteCount.DD;
            _sizeExplicitlyStated = false;
            return line;
        }

        string sizeIdentifier = sizeMatch.Value.Split()[0];

        switch (sizeIdentifier)
        {
            case "BYTE":
                _instructionSize = ByteCount.DB;
                break;
            case "WORD":
                _instructionSize = ByteCount.DW;
                break;
            case "DWORD":
                _instructionSize = ByteCount.DD;
                break;
        }

        _sizeExplicitlyStated = true;
        return line.Replace(sizeMatch.Value, "").Trim();
    }
    
    public void InterpretLine(string line)
    {
        line = Regex.Replace(line, @"[;].*", ""); // remove comments
        if (string.IsNullOrWhiteSpace(line))
        {
            return; //line is empty
        }
        
        var matchName = Regex.Match(line, @"[a-z]{1,}");

        if (!matchName.Success)
        {
            throw new Exception($"No instruction found on line '{line}'.");
        }

        string instructionName = matchName.Value;

        if (!_instructions.ContainsKey(instructionName))
        {
            throw new Exception($"No instruction '{instructionName}' found.");
        }

        string arguments = line.Replace(instructionName, "").Trim();
        _instructions[instructionName](arguments);

    }

    private void Add(string line)
    {
        
    }
    
    private void Push(string line)
    {
        var arguments = TokenizeArguments(line);
        if (arguments.Count != 1)
        {
            throw new Exception($"There is no PUSH instruction with {arguments.Count} arguments.");
        }
        
        // push <reg32>
        if (arguments[0] is RegisterArgument)
        {
            var registerArgument = (arguments[0] as RegisterArgument) ?? new RegisterArgument("", ByteCount.DB);
            if (registerArgument.ByteCount < ByteCount.DD)
            {
                throw new Exception($"PUSH instruction needs double word register, not '{registerArgument.RegisterIdentifier}'.");
            }

            _registers.Esp -= 4;
            _memory.SetDoubleWord(_registers.Esp, _registers.GetDoubleWordRegister(registerArgument.RegisterIdentifier));
            return;
        }
        
        // push <mem>
        if (arguments[0] is MemoryArgument)
        {
            var memoryArgument = (arguments[0] as MemoryArgument) ?? new MemoryArgument(0);
            _registers.Esp -= 4;
            _memory.SetDoubleWord(_registers.Esp, _memory.ReadDoubleWord(memoryArgument.Address));
            return;
        }
        
        // push <con32>
        if (arguments[0] is ConstantArgument)
        {
            var constantArgument = (arguments[0] as ConstantArgument) ?? new ConstantArgument(0);

            _registers.Esp -= 4;
            _memory.SetDoubleWord(_registers.Esp, constantArgument.Value);
            return;
        }
        
    }
    
    private void Mov(string line)
    {
        var arguments = TokenizeArguments(line);
        if (arguments.Count != 2)
        {
            throw new Exception($"There is no MOV instruction with {arguments.Count} arguments.");
        }
        
        
        // MOV TO FROM

        // mov <reg>,<reg>
        if (arguments[0] is RegisterArgument && arguments[1] is RegisterArgument)
        {
            var registerArgument1 = (arguments[0] as RegisterArgument);
            var registerArgument2 = (arguments[1] as RegisterArgument);
            
            
            if (registerArgument1?.ByteCount != registerArgument2?.ByteCount)
            {
                throw new Exception($"'{registerArgument1?.RegisterIdentifier}' has not same size as '{registerArgument2?.RegisterIdentifier}'.");
            }

            switch (registerArgument1?.ByteCount)
            {
                case ByteCount.DB:
                    _registers.
                        SetByteRegister(registerArgument1?.RegisterIdentifier ?? "", 
                            _registers.GetByteRegister(registerArgument2?.RegisterIdentifier ?? ""));
                    break;
                case ByteCount.DW:
                    _registers.
                        SetWordRegister(registerArgument1?.RegisterIdentifier ?? "", 
                            _registers.GetWordRegister(registerArgument2?.RegisterIdentifier ?? ""));
                    break;
                case ByteCount.DD:
                    _registers.
                        SetDoubleWordRegister(registerArgument1?.RegisterIdentifier ?? "", 
                            _registers.GetDoubleWordRegister(registerArgument2?.RegisterIdentifier ?? ""));
                    break;
                default:
                    throw new Exception("Invalid byte count.");
            }
            return;
        }
        
        // mov <reg>,<mem>
        if (arguments[0] is RegisterArgument && arguments[1] is MemoryArgument)
        {
            var registerArgument = (arguments[0] as RegisterArgument) ?? new RegisterArgument("eax", ByteCount.DD);
            var memoryArgument = (arguments[1] as MemoryArgument) ?? new MemoryArgument(0);
            

            switch (registerArgument.ByteCount)
            {
                case ByteCount.DB:
                    _registers.
                        SetByteRegister(registerArgument.RegisterIdentifier, 
                            _memory.ReadByte(memoryArgument.Address));
                    break;
                case ByteCount.DW:
                    _registers.
                        SetWordRegister(registerArgument.RegisterIdentifier, 
                            _memory.ReadWord(memoryArgument.Address));
                    break;
                case ByteCount.DD:
                    _registers.
                        SetDoubleWordRegister(registerArgument.RegisterIdentifier, 
                            _memory.ReadDoubleWord(memoryArgument.Address));
                    break;
                default:
                    throw new Exception("Invalid byte count.");
            }
            return;
        }
        
        // mov <mem>,<reg>
        if (arguments[0] is MemoryArgument && arguments[1] is RegisterArgument)
        {
            var memoryArgument = (arguments[0] as MemoryArgument) ?? new MemoryArgument(0);
            var registerArgument = (arguments[1] as RegisterArgument) ?? new RegisterArgument("eax", ByteCount.DD);   

            switch (registerArgument.ByteCount)
            {
                case ByteCount.DB:
                    _memory.SetByte(memoryArgument.Address, _registers.GetByteRegister(registerArgument.RegisterIdentifier));
                    break;
                case ByteCount.DW:
                    _memory.SetWord(memoryArgument.Address, _registers.GetWordRegister(registerArgument.RegisterIdentifier));
                    break;
                case ByteCount.DD:
                    _memory.SetDoubleWord(memoryArgument.Address, _registers.GetDoubleWordRegister(registerArgument.RegisterIdentifier));
                    break;
                default:
                    throw new Exception("Invalid byte count.");
            }
            return;
        }
        
        // mov <reg>,<const>
        if (arguments[0] is RegisterArgument && arguments[1] is ConstantArgument)
        {
            var registerArgument1 = (arguments[0] as RegisterArgument);
            var constantArgument2 = (arguments[1] as ConstantArgument);
            
            switch (registerArgument1?.ByteCount)
            {
                case ByteCount.DB:
                    _registers.
                        SetByteRegister(registerArgument1?.RegisterIdentifier ?? "", (byte) (constantArgument2?.Value ?? 0));
                    break;
                case ByteCount.DW:
                    _registers.
                        SetWordRegister(registerArgument1!.RegisterIdentifier,  (UInt16)(constantArgument2?.Value ?? 0));
                    break;
                case ByteCount.DD:
                    _registers.
                        SetDoubleWordRegister(registerArgument1?.RegisterIdentifier ?? "", (constantArgument2?.Value ?? 0));
                    break;
                default:
                    throw new Exception("Invalid byte count.");
            }
            return;
        }
        
        // mov <mem>,<const>
        if (arguments[0] is MemoryArgument && arguments[1] is ConstantArgument)
        {
            var memoryArgument = (arguments[0] as MemoryArgument) ?? new MemoryArgument(0);
            var constantArgument = (arguments[1] as ConstantArgument) ?? new ConstantArgument(0);   

            _memory.SetDoubleWord(memoryArgument.Address, constantArgument.Value);
        }
    }
}