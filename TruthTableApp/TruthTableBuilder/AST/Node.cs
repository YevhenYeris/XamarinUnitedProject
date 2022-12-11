using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedProjectApp.TruthTableBuilder.AST
{
    public abstract class Node
    {
        public abstract bool Evaluate();

        public override abstract string ToString();
    }

    public class TrueNode : Node
    {
        public override bool Evaluate()
        {
            return true;
        }

        public override string ToString()
        {
            return "T";
        }
    }

    public class FalseNode : Node
    {
        public override bool Evaluate()
        {
            return false;
        }

        public override string ToString()
        {
            return "F";
        }
    }

    public class NegateNode : Node
    {
        public NegateNode(Node underlying)
        {
            Underlying = underlying;
        }

        public Node Underlying { get; set; }

        public override bool Evaluate()
        {
            return !Underlying.Evaluate();
        }

        public override string ToString()
        {
            return $"not {Underlying.ToString()}";
        }
    }

    public class AndNode : Node
    {
        public AndNode(Node lhs, Node rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public Node Lhs { get; set; }

        public Node Rhs { get; set; }

        public override bool Evaluate()
        {
            return Lhs.Evaluate() && Rhs.Evaluate();
        }

        public override string ToString()
        {
            return $"({Lhs.ToString()} and {Rhs.ToString()})";
        }
    }

    public class OrNode : Node
    {
        public OrNode(Node lhs, Node rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public Node Lhs { get; set; }

        public Node Rhs { get; set; }

        public override bool Evaluate()
        {
            return Lhs.Evaluate() || Rhs.Evaluate();
        }

        public override string ToString()
        {
            return $"({Lhs.ToString()} or {Rhs.ToString()})";
        }
    }

    public class ImpliesNode : Node
    {
        public ImpliesNode(Node lhs, Node rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public Node Lhs { get; set; }

        public Node Rhs { get; set; }

        public override bool Evaluate()
        {
            return !Lhs.Evaluate() || Rhs.Evaluate();
        }

        public override string ToString()
        {
            return $"({Lhs.ToString()} -> {Rhs.ToString()})";
        }
    }

    public class IifNode : Node
    {
        public IifNode(Node lhs, Node rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public Node Lhs { get; set; }

        public Node Rhs { get; set; }

        public override bool Evaluate()
        {
            return Lhs.Evaluate() == Rhs.Evaluate();
        }

        public override string ToString()
        {
            return $"({Lhs.ToString()} <-> {Rhs.ToString()})";
        }
    }

    public class XorNode : Node
    {
        public XorNode(Node lhs, Node rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public Node Lhs { get; set; }

        public Node Rhs { get; set; }

        public override bool Evaluate()
        {
            return Lhs.Evaluate() != Rhs.Evaluate();
        }

        public override string ToString()
        {
            return $"({Lhs.ToString()} xor {Rhs.ToString()})";
        }
    }

    public class VariableNode : Node
    {
        public VariableNode(string name, Dictionary<string, bool> variables)
        {
            Name = name;
            Variables = variables;
        }

        public string Name { get; set; } = string.Empty;

        public Dictionary<string, bool> Variables { get; set; } = new Dictionary<string, bool>();

        public override bool Evaluate()
        {
            return Variables[Name];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}