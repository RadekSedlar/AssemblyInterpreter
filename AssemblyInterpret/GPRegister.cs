namespace AssemblyInterpret
{
    public class GPRegister
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
}
