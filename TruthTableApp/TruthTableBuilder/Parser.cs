﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedProjectApp.TruthTableBuilder.AST;

namespace UnitedProjectApp.TruthTableBuilder
{
	public class ParserResult
	{
		public string Formula { get; set; }
		public Node AST { get; set; } = null!;

		public Dictionary<string, bool> Variables { get; set; } = new Dictionary<string, bool>();
	}

	public class Parser
	{
		public Scanner Scanner { get; set; }

		private Dictionary<string, bool> _variables = new Dictionary<string, bool>();

		public Parser(Scanner scanner)
		{
			Scanner = scanner;
		}

		public ParserResult Parse(string input)
		{
			var scanResult = Scanner.Scan(input);
			var tokens = scanResult.Tokens;
			_variables = scanResult.Variables;

			var operands = new Stack<Node>();
			var operators = new Stack<Token>();

			var needOperand = true;

			foreach (var currToken in tokens)
			{
				if (needOperand)
				{
					if (IsOperand(currToken))
					{
						AddOperand(WrapOperand(currToken), operands, operators);
						needOperand = false;
					}
					else if (currToken.Value == "(" || currToken.Value == "not")
					{
						operators.Push(currToken);
					}
					else if (currToken.Value == "$")
					{
						if (operators.Count == 0)
						{
							throw new Exception("No operators");
						}

						if (operators.Peek().Value == "(")
						{
							throw new Exception("Invalid brackets");
						}

						throw new Exception("This operator is missing an operand.");
					}
					else
					{
						throw new Exception("We were expecting a variable, constant, or open parenthesis here.");
					}
				}
				else
				{
					if (IsBinaryOperator(currToken) || currToken.Value == "$")
					{
						while (true)
						{
							if (operators.Count == 0)
							{
								break;
							}

							if (operators.Peek().Value == "(")
							{
								break;
							}

							if (PriorityOf(operators.Peek()) <= PriorityOf(currToken))
							{
								break;
							}

							var upOperator = operators.Pop();
							var rhs = operands.Pop();
							var lhs = operands.Pop();
							AddOperand(CreateOperatorNode(lhs, upOperator, rhs), operands, operators);
						}

						operators.Push(currToken);
						needOperand = true;

						if (currToken.Value == "$")
						{
							break;
						}
					}
					else if (currToken.Value == ")")
					{
						while (true)
						{
							if (operators.Count == 0)
							{
								throw new Exception("This close parenthesis doesn't match any open parenthesis.");
							}

							var currOp = operators.Pop();

							if (currOp.Value == "(")
							{
								break;
							}

							if (currOp.Value == "not")
							{
								throw new Exception("Nothing is negated by this operator.");
							}

							var rhs = operands.Pop();
							var lhs = operands.Pop();

							AddOperand(CreateOperatorNode(lhs, currOp, rhs), operands, operators);
						}

						var expr = operands.Pop();
						AddOperand(expr, operands, operators);
					}
					else
					{
						throw new Exception("We were expecting a close parenthesis or a binary connective here.");
					}
				}
			}

			return new ParserResult() { AST = operands.Pop(), Variables = scanResult.Variables, Formula = input };
		}

		private void AddOperand(Node node, Stack<Node> operands, Stack<Token> operators)
		{
			while (operators.Count > 0 && operators.Peek().Value == "not")
			{
				operators.Pop();
				node = new NegateNode(node);
			}

			operands.Push(node);
		}

		private Node WrapOperand(Token token)
		{
			if (token.Value == "T")
				return new TrueNode();
			if (token.Value == "F")
				return new FalseNode();
			if (token.Type == TokenType.Variable)
				return new VariableNode(token.Value, _variables);
			throw new Exception($"Invalid token: {token.Type} {token.Value}");
		}

		private Node CreateOperatorNode(Node lhs, Token token, Node rhs)
		{
			if (token.Value == "<->")
				return new IifNode(lhs, rhs);
			if (token.Value == "->")
				return new ImpliesNode(lhs, rhs);
			if (token.Value == "or")
				return new OrNode(lhs, rhs);
			if (token.Value == "and")
				return new AndNode(lhs, rhs);
			if (token.Value == "xor")
				return new XorNode(lhs, rhs);
			throw new Exception($"Should never need to create an operator node from {token.Value}");
		}

		private bool IsOperand(Token token)
		{
			return token.Value == "T" ||
				   token.Value == "F" ||
				   token.Type == TokenType.Variable;
		}

		private bool IsBinaryOperator(Token token)
		{
			return token.Value == "<->" ||
				   token.Value == "->" ||
				   token.Value == "and" ||
				   token.Value == "or" ||
				   token.Value == "xor";
		}

		private int PriorityOf(Token token)
		{
			return token.Value == "$" ? -1 :
				   token.Value == "<->" ? 0 :
				   token.Value == "->" ? 1 :
				   token.Value == "xor" ? 2 :
				   token.Value == "or" ? 3 :
				   token.Value == "and" ? 4 :
				   throw new Exception($"Should never need the priority of {token.Value}");
		}
	}
}
