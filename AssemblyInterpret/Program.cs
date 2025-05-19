// See https://aka.ms/new-console-template for more information
using AssemblyInterpret;
using AssemblyInterpret.Interpreters;

class Program
{
    public static void WriteRegisters(GpRegister register)
    {
        Console.WriteLine(new String('+', 45));
        Console.WriteLine($" EAX => {register.RegisterText} | {register.Register}");
        Console.WriteLine($" AX  => {new String(' ', 16)}{register.HalfText} | {register.Half}");
        Console.WriteLine($" AL  => {new String(' ', 24)}{register.LowerHalfText} | {register.LowerHalf}");
        Console.WriteLine($" AH  => {new String(' ', 16)}{register.HigherHalfText}{new String(' ', 8)} | {register.HigherHalf}");
        Console.WriteLine(new String('+', 40));
    }

    public static void Main(string[] args)
    {
        Registers registers = new Registers();
        registers.GPRegisters["eax"].Register = 65794;
        WriteRegisters(registers.GPRegisters["eax"]);
        registers.GPRegisters["eax"].HigherHalf = 55;
        WriteRegisters(registers.GPRegisters["eax"]);
        GlobalMemory memory = new GlobalMemory(20);
        memory.SetByte(0, 5);
        memory.SetWord(1, 255);
        memory.SetDoubleWord(3, 67188482);
        memory.PrintMemory();
        
        Console.WriteLine($"\n{memory.ReadByte(0)}");
        Console.WriteLine(memory.ReadWord(1));
        Console.WriteLine(memory.ReadDoubleWord(3));

        memory = new GlobalMemory(40);
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
        memory.PrintMemory();


        uint strAddress = memory.GetGlobalVar("str")!.Address;
        Console.WriteLine();
        for (byte i = 0; i < 5; i++)
        {
            Console.Write("{0}", (char)memory.ReadByte(strAddress + i));
        }
        Console.WriteLine();
        
        
        
        registers.Eax = 1;
        registers.Ebx = 2;
        dataInterpret.InterpretDataSection();
        TextInterpreter textInterpreter = new TextInterpreter(memory, """
                                                                      add eax ebx
                                                                      add ebx [14]
                                                                      """,registers.GPRegisters);
        textInterpreter.InterpretSection();
        WriteRegisters(registers.GPRegisters["eax"]);
        Console.WriteLine("END");
    }
}
