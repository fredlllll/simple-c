namespace SimpleC.MuchAssemblyRequired
{
    class StringInsArg : InstructionArg
    {
        public string Register { get; }

        public StringInsArg(string register)
        {
            Register = register;
        }

        public override string ToString()
        {
            return Register;
        }
    }
}
