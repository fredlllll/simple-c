namespace SimpleC.MuchAssemblyRequired
{
    class ImmediateInsArg : InstructionArg
    {
        public short Value { get; }

        public ImmediateInsArg(short val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
