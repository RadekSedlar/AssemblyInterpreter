namespace AssemblyInterpret
{
    public class GPRegister
    {

        private int _register = 0;
        public int Register
        {
            get => _register;
            set
            {
                _register = value;
            }
        }
        public string RegisterText => Convert.ToString(Register, 2).PadLeft(32, '0');
        public short Half
        {
            get
            {
                return (short)_register;
            }
            set
            {
                _register = _register >> 16;
                _register = _register << 16;
                _register = _register | (int)value;
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
                _register = _register >> 8;
                _register = _register << 8;
                _register = _register | (int)value;
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
                int temp = value << 8;
                _register = _register ^ temp;
                _register = _register | temp;
            }
        }
        public string HigherHalfText => Convert.ToString(HigherHalf, 2).PadLeft(8, '0');
    }
}
