namespace AssemblyInterpret
{
    public class GpRegister
    {

        private UInt32 _register = 0;
        public UInt32 Register
        {
            get => _register;
            set
            {
                _register = value;
            }
        }
        public string RegisterText => Convert.ToString(Register, 2).PadLeft(32, '0');
        public UInt16 Half
        {
            get
            {
                return (UInt16)_register;
            }
            set
            {
                _register = _register >> 16;
                _register = _register << 16;
                _register = _register | value;
            }
        }
        public string HalfText => Convert.ToString(Half, 2).PadLeft(16, '0');
        public byte LowerHalf
        {
            get
            {
                return (byte)_register;
            }
            set
            {
                _register >>= 8;
                _register <<= 8;
                _register |= value;
            }
        }
        public string LowerHalfText => Convert.ToString(LowerHalf, 2).PadLeft(8, '0');
        public byte HigherHalf
        {
            get
            {
                short temp = (short)_register;
                return (byte)(temp >> 8);
            }
            set
            {
                UInt32 temp = _register >> 16;
                temp <<= 8;
                temp += value;
                temp <<= 8;
                temp += (byte)_register;
                _register = temp;
            }
        }
        public string HigherHalfText => Convert.ToString(HigherHalf, 2).PadLeft(8, '0');
    }

    public class RegisterReader
    {
        private GpRegister _register;
        private string _registerName;

        public RegisterReader(GpRegister register, string registerName)
        {
            _register = register;
            _registerName = registerName;
        }

        public uint ReadRegister()
        {
            return _registerName switch
            {
                "eax" or "ebx" or "ecx" or "edx" => _register.Register,
                "ax" or "bx" or "cx" or "dx" => _register.Half,
                "al" or "bl" or "cl" or "dl" => _register.LowerHalf,
                "ah" or "bh" or "ch" or "dh" => _register.HigherHalf,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public void SetRegister(uint value)
        {
            if (_registerName is "eax" or "ebx" or "ecx" or "edx")
            {
                _register.Register = value;
                return;
            }
            
            if (_registerName is "ax" or "bx" or "cx" or "dx")
            {
                _register.Half = (ushort)value;
                return;
            }
            
            
            if (_registerName is "al" or "bl" or "cl" or "dl")
            {
                _register.LowerHalf = (byte)value;
                return;
            }
            
            if (_registerName is "ah" or "bh" or "ch" or "dh")
            {
                _register.HigherHalf = (byte)value;
                return;
            }
            
            throw new ArgumentOutOfRangeException();
        }
    }

    public static class GpRegistersExtensions
    {
        public static GpRegister GetGpRegisterByName(this Dictionary<string, GpRegister> registers, string name)
        {
            var wasRegisterFound = registers.TryGetValue(name, out GpRegister? register);

            if (!wasRegisterFound || register is null)
            {
                throw new KeyNotFoundException($"There is no such register: {name}");
            }
            
            return register;
        }
    }
}
