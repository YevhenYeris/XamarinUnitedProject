using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedProjectApp.TruthTableBuilder
{
    public class TruthTableCalculator
    {
		public TruthTable GenerateTruthTable(ParserResult parserResult)
        {
			var assignment = parserResult.Variables.Select(v => false).ToList();
			var truthTable = new TruthTable() { Formula = parserResult.Formula };

			do
			{
				for (int i = 0; i < parserResult.Variables.Count; ++i)
				{
					var varName = parserResult.Variables.ElementAt(i).Key;
					parserResult.Variables[varName] = assignment[i];
				}

				truthTable.Table[assignment] = parserResult.AST.Evaluate();
				assignment = NextAssignment(assignment);
			}
			while (assignment != null);

			if (truthTable.Table.Values.All(v => v))
			{
				truthTable.IsEquality = true;
				truthTable.IsExecutable = true;
			}
			else if (truthTable.Table.Values.Where(v => v).Any())
			{
				truthTable.IsExecutable = true;
			}

			truthTable.Variables = parserResult.Variables.Keys;

			return truthTable;
        }

		private List<bool> NextAssignment(List<bool> assignment)
		{
			var flipIndex = assignment.Count - 1;
			while (flipIndex >= 0 && assignment[flipIndex])
			{
				--flipIndex;
			}

			if (flipIndex == -1)
			{
				return null;
			}

			var newAssignment = assignment.Select(v => v).ToList();
			newAssignment[flipIndex] = true;

			for (int i = flipIndex + 1; i < assignment.Count; ++i)
            {
				newAssignment[i] = false;
            }

			return newAssignment;
		}
	}

	public class TruthTable
    {
		public Dictionary<List<bool>, bool> Table { get; set; } = new Dictionary<List<bool>, bool>();

		public IEnumerable<string> Variables { get; set; }

		public bool IsExecutable { get; set; } = false;

		public bool IsEquality { get; set; } = false;

		public string Formula { get; set; }
	}
}