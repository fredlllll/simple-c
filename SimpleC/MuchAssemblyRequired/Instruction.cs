using System.Collections.Generic;
using System.Linq;
using SimpleC.Types;

namespace SimpleC.MuchAssemblyRequired
{
    class Instruction : CodeInstruction
    {
        public string OpCode { get; set; }
        public List<InstructionArg> Args { get; } = new List<InstructionArg>();

        public Instruction(string opcode, params InstructionArg[] args)
        {
            OpCode = opcode;
            Args.AddRange(args);
        }

        public override string ToString()
        {
            return OpCode + " " + string.Join(",", Args.Select(x => x.ToString()));
        }
    }
}
