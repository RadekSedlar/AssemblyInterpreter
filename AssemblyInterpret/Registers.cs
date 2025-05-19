namespace AssemblyInterpret
{
    public class Registers
    {
        public UInt32 Esp
        {
            get => GPRegisters["esp"].Register;
            set => GPRegisters["esp"].Register = value;
        }
        public UInt32 Ebp
        {
            get => GPRegisters["ebp"].Register;
            set => GPRegisters["ebp"].Register = value;
        }
        
        #region EAX

        public UInt32 Eax
        {
            get => GPRegisters["eax"].Register;
            set => GPRegisters["eax"].Register = value;
        }

        public UInt16 Ax
        {
            get => GPRegisters["eax"].Half;
            set => GPRegisters["eax"].Half = value;
        }
        
        public byte Al
        {
            get => GPRegisters["eax"].LowerHalf;
            set => GPRegisters["eax"].LowerHalf = value;
        }
        
        public byte Ah
        {
            get => GPRegisters["eax"].HigherHalf;
            set => GPRegisters["eax"].HigherHalf = value;
        }

        #endregion
        #region EBX

        public UInt32 Ebx
        {
            get => GPRegisters["ebx"].Register;
            set => GPRegisters["ebx"].Register = value;
        }

        public UInt16 Bx
        {
            get => GPRegisters["ebx"].Half;
            set => GPRegisters["ebx"].Half = value;
        }
        
        public byte Bl
        {
            get => GPRegisters["ebx"].LowerHalf;
            set => GPRegisters["ebx"].LowerHalf = value;
        }
        
        public byte Bh
        {
            get => GPRegisters["ebx"].HigherHalf;
            set => GPRegisters["ebx"].HigherHalf = value;
        }

        #endregion
        #region ECX

        public UInt32 Ecx
        {
            get => GPRegisters["ecx"].Register;
            set => GPRegisters["ecx"].Register = value;
        }

        public UInt16 Cx
        {
            get => GPRegisters["ecx"].Half;
            set => GPRegisters["ecx"].Half = value;
        }
        
        public byte Cl
        {
            get => GPRegisters["ecx"].LowerHalf;
            set => GPRegisters["ecx"].LowerHalf = value;
        }
        
        public byte Ch
        {
            get => GPRegisters["ecx"].HigherHalf;
            set => GPRegisters["ecx"].HigherHalf = value;
        }

        #endregion
        #region EDX

        public UInt32 Edx
        {
            get => GPRegisters["edx"].Register;
            set => GPRegisters["edx"].Register = value;
        }

        public UInt16 Dx
        {
            get => GPRegisters["edx"].Half;
            set => GPRegisters["edx"].Half = value;
        }
        
        public byte Dl
        {
            get => GPRegisters["edx"].LowerHalf;
            set => GPRegisters["edx"].LowerHalf = value;
        }
        
        public byte Dh
        {
            get => GPRegisters["edx"].HigherHalf;
            set => GPRegisters["edx"].HigherHalf = value;
        }

        #endregion
        #region ESI

        public UInt32 Esi
        {
            get => GPRegisters["esi"].Register;
            set => GPRegisters["esi"].Register = value;
        }

        public UInt16 Si
        {
            get => GPRegisters["esi"].Half;
            set => GPRegisters["esi"].Half = value;
        }
        
        public byte Sil
        {
            get => GPRegisters["esi"].LowerHalf;
            set => GPRegisters["esi"].LowerHalf = value;
        }
        

        #endregion
        #region EDI

        public UInt32 Edi
        {
            get => GPRegisters["edi"].Register;
            set => GPRegisters["edi"].Register = value;
        }

        public UInt16 Di
        {
            get => GPRegisters["edi"].Half;
            set => GPRegisters["edi"].Half = value;
        }
        
        public byte Dil
        {
            get => GPRegisters["edi"].LowerHalf;
            set => GPRegisters["edi"].LowerHalf = value;
        }
        

        #endregion
        
        public Dictionary<string, GpRegister> GPRegisters = new Dictionary<string, GpRegister>()
        {
            {"eax", new GpRegister() },
            {"ebx", new GpRegister() },
            {"ecx", new GpRegister() },
            {"edx", new GpRegister() },
            {"esi", new GpRegister() },
            {"edi", new GpRegister() },
            {"esp", new GpRegister() },
            {"ebp", new GpRegister() }

        };

        public void SetDoubleWordRegister(string registerIdentifier, UInt32 value)
        {
            switch (registerIdentifier)
            {
                case "eax":
                    Eax = value;
                    break;
                case "ebx":
                    Ebx = value;
                    break;
                case "ecx":
                    Ecx = value;
                    break;
                case "edx":
                    Edx = value;
                    break;
                case "esi":
                    Esi = value;
                    break;
                case "edi":
                    Edi = value;
                    break;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }
        
        public UInt32 GetDoubleWordRegister(string registerIdentifier)
        {
            switch (registerIdentifier)
            {
                case "eax":
                    return Eax;
                case "ebx":
                    return Ebx;
                case "ecx":
                    return Ecx;
                case "edx":
                    return Edx;
                case "esi":
                    return Esi;
                case "edi":
                    return Edi;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }
        
        public void SetWordRegister(string registerIdentifier, UInt16 value)
        {
            switch (registerIdentifier)
            {
                case "ax":
                    Ax = value;
                    break;
                case "bx":
                    Bx = value;
                    break;
                case "cx":
                    Cx = value;
                    break;
                case "dx":
                    Dx = value;
                    break;
                case "si":
                    Si = value;
                    break;
                case "di":
                    Di = value;
                    break;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }
        
        public UInt16 GetWordRegister(string registerIdentifier)
        {
            switch (registerIdentifier)
            {
                case "ax":
                    return Ax;
                case "bx":
                    return Bx;
                case "cx":
                    return Cx;
                case "dx":
                    return Dx;
                case "si":
                    return Si;
                case "di":
                    return Di;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }
        
        public void SetByteRegister(string registerIdentifier, byte value)
        {
            switch (registerIdentifier)
            {
                case "ah":
                    Ah = value;
                    break;
                case "bh":
                    Bh = value;
                    break;
                case "ch":
                    Ch = value;
                    break;
                case "dh":
                    Dh = value;
                    break;
                case "al":
                    Al = value;
                    break;
                case "bl":
                    Bl = value;
                    break;
                case "cl":
                    Cl = value;
                    break;
                case "dl":
                    Dl = value;
                    break;
                case "sil":
                    Sil = value;
                    break;
                case "dil":
                    Dil = value;
                    break;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }
        
        public byte GetByteRegister(string registerIdentifier)
        {
            switch (registerIdentifier)
            {
                case "ah":
                    return Ah;
                case "bh":
                    return Bh;
                case "ch":
                    return Ch;
                case "dh":
                    return Dh;
                case "al":
                    return Al;
                case "bl":
                    return Bl;
                case "cl":
                    return Cl;
                case "dl":
                    return Dl;
                case "sil":
                    return Sil;
                case "dil":
                    return Dil;
                default:
                    throw new Exception($"Register '{registerIdentifier}' not found.");
            }
        }

        public (bool, ByteCount) FindRegister(string registerIdentifier)
        {
            switch (registerIdentifier)
            {
                case "eax":
                    return (true, ByteCount.DD);
                case "ebx":
                    return (true, ByteCount.DD);
                case "ecx":
                    return (true, ByteCount.DD);
                case "edx":
                    return (true, ByteCount.DD);
                case "esi":
                    return (true, ByteCount.DD);
                case "edi":
                    return (true, ByteCount.DD);
                case "ax":
                    return (true, ByteCount.DW);
                case "bx":
                    return (true, ByteCount.DW);
                case "cx":
                    return (true, ByteCount.DW);
                case "dx":
                    return (true, ByteCount.DW);
                case "si":
                    return (true, ByteCount.DW);
                case "di":
                    return (true, ByteCount.DW);
                case "ah":
                    return (true, ByteCount.DB);
                case "bh":
                    return (true, ByteCount.DB);
                case "ch":
                    return (true, ByteCount.DB);
                case "dh":
                    return (true, ByteCount.DB);
                case "al":
                    return (true, ByteCount.DB);
                case "bl":
                    return (true, ByteCount.DB);
                case "cl":
                    return (true, ByteCount.DB);
                case "dl":
                    return (true, ByteCount.DB);
                case "sil":
                    return (true, ByteCount.DB);
                case "dil":
                    return (true, ByteCount.DB);
            }
            return (false, ByteCount.DB);
        }
    }
}
