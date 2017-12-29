using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleC.Types;
using SimpleC.Types.AstNodes;

namespace SimpleC.MuchAssemblyRequired
{
    class CodeEmitter : CodeGeneration.CodeEmitter
    {
        public override void ParseAST(AstNode ast)
        {
            Type disT = typeof(CodeEmitter);

            //converting recursive to iterative using the stack and pushing nodes backwards onto the stack
            Stack<AstNode> toVisit = new Stack<AstNode>();
            toVisit.Push(ast);
            while(toVisit.Count > 0)
            {
                AstNode node = toVisit.Pop();
                var visitMethod = disT.GetMethod("Visit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { node.GetType() }, null);
                if(visitMethod == null)
                {
                    throw new Exception("no visit method for AstNode type: " + node.GetType());
                }
                var result = visitMethod.Invoke(this, new object[] { node });
                if(result != null)
                {
                    IEnumerable<AstNode> nodes = (IEnumerable<AstNode>)result;
                    foreach(var n in nodes.Reverse())
                    {
                        toVisit.Push(n);
                    }
                }
            }
        }

        IEnumerable<AstNode> Visit(BinaryOperationNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(ExpressionNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(FunctionCallExpressionNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(FunctionDeclarationNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(IfStatementNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(LoopStatementNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(NumberLiteralNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(ParameterDeclarationNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(ProgramNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(ReturnStatementNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(StatementSequenceNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(UnaryOperationNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(VariableAssignmentNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(VariableDeclarationNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(VariableReferenceExpressionNode n)
        {
            return null;
        }

        IEnumerable<AstNode> Visit(WhileLoopNode n)
        {
            return null;
        }
    }
}
