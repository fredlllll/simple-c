using SimpleC.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleC.Types.AstNodes
{
    class VariableAssignmentNode : AstNode
    {
        public string VariableName { get; private set; }
        public ExpressionNode ValueExpression { get; private set; }

        public VariableAssignmentNode(string name, ExpressionNode expr)
        {
            VariableName = name;
            ValueExpression = expr ?? throw new ParsingException("The assinged expression may not be null!");
        }
    }
}
