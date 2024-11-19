namespace Mascari4615
{
	public class IntCriteria : NumCriteria
	{
		public IntVariable IntVariable { get; private set; }

		public IntCriteria(CriteriaInfo criteriaInfo, IntVariable intVariable) : base(criteriaInfo)
		{
			IntVariable = intVariable;
		}

		public override int GetCurValue()
		{
			return IntVariable.RuntimeValue;
		}
	}
}