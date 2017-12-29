using SimpleC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleC.CodeGeneration
{
    /// <summary>
    /// Code emitter for SimpleC machine code.
    /// </summary>
    abstract class CodeEmitter
    {
        List<CodeInstruction> emittedCode = new List<CodeInstruction>();

        public abstract void ParseAST(AstNode ast);

        public void Emit(CodeInstruction instruction)
        {
            emittedCode.Add(instruction);
        }

        public CodeInstruction[] GetEmittedCode()
        {
            return emittedCode.ToArray();
        }
    }
}
