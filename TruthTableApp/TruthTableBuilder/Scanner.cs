using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/**
 * and, or, not, xor, ->, <->, 
 */
namespace UnitedProjectApp.TruthTableBuilder
{
    public class Scanner
    {
        public PreliminaryScanResult? Scan(string input)
        {
            if (!CheckIntegrity(input))
            {
                return null;
            }

            var preliminary = PreliminaryScan(input);

            return preliminary;//numberVariables(preliminary);
        }

        private bool CheckIntegrity(string input)
        {
            var okayCharsRegex = "[A-Za-z]|[0-9]|-|<|>|\\)|\\(| *";

            foreach (var ch in input)
            {
                if (!Regex.IsMatch(ch.ToString(), okayCharsRegex))
                {
                    return false;
                }
            }

            return true;
        }

        private string ScanVariable(string input, int index, PreliminaryScanResult result)
        {
            var name = TryReadVariable(input, index);
            result.Variables[name] = true;
            return name;
        }

        private string? TryReadVariable(string input, int index)
        {
            var isCharRegex = $"[A-Za-z]";
            var isCharOrNumberRegex = $"[A-Za-z]|[0-9]";
            if (!Regex.IsMatch(input[index].ToString(), isCharRegex))
            {
                return null;
            }

            var result = string.Empty;

            while (Regex.IsMatch(input[index].ToString(), isCharOrNumberRegex))
            {
                result += input[index].ToString();
                ++index;
            }

            var reservedWords = new List<string>() { "T", "F", "and", "or", "not", "iff", "implies", "true", "false", "xor" };
            return reservedWords.Contains(result) ? null : result;
        }

        private PreliminaryScanResult? PreliminaryScan(string input)
        {
            input += "$";
            var result = new PreliminaryScanResult();
            var error = false;
            int i = 0;

            while (true)
            {
                var curr = input[i].ToString();

                if (curr == "$")
                {
                    result.Tokens.Add(new Token(TokenType.Operator, curr, i));
                    return result;
                }
                else if (IsVariableStart(input, i))
                {
                    var variable = ScanVariable(input, i, result);
                    result.Tokens.Add(new Token(TokenType.Variable, variable, i));
                    i += variable.Length;
                }
                else if (IsOperatorStart(input, i))
                {
                    var operatorName = TryReadOperator(input, i);
                    result.Tokens.Add(new Token(TokenType.Operator, operatorName, i));
                    i += operatorName.Length;
                }
                else if (string.IsNullOrWhiteSpace(curr))
                {
                    ++i;
                }
                else
                {
                    error = true;
                    break;
                }
            }

            return error ? null :  result;
        }

        private bool IsVariableStart(string input, int index)
        {
            return TryReadVariable(input, index) != null;
        }

        private bool IsOperatorStart(string input, int index)
        {
            return TryReadOperator(input, index) != null;
        }

        private string? TryReadOperator(string input, int index)
        {
            var operators = new List<string>() { "and", "or", "not", "xor", "->", "<->", "(", ")", "T", "F" };
            var result = string.Empty;

            if (index < input.Length - 3)
            {
                result = input.Substring(index, 3);

                if (operators.Contains(result))
                {
                    return result;
                }
            }

            if (index < input.Length - 2)
            {
                result = input.Substring(index, 2);

                if (operators.Contains(result))
                {
                    return result;
                }
            }

            if (index < input.Length - 1)
            {
                result = input.Substring(index, 1);

                if (operators.Contains(result))
                {
                    return result;
                }
            }

            return null;
        }
    }

    public class PreliminaryScanResult
    {
        public Dictionary<string, bool> Variables { get; set; } = new Dictionary<string, bool>();

        public List<Token> Tokens { get; set; } = new List<Token>();
    }

    public class Variable
    {
        public string Name { get; set; }

        public bool Value { get; set; }
    }

    public class Token
    {
        public Token(TokenType type, string value, int start)
        {
            Type = type;
            Start = start;
            Value = value;
        }

        private string _value = string.Empty;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                End = _value.Length;
            }
        }

        public TokenType Type { get; set; }

        public int Start { get; set; } = 0;

        public int End { get; private set; }
    }

    public enum TokenType
    {
        Operator,
        Variable
    }
}
