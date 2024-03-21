using UnityEngine;

namespace Mascari4615
{
	public enum ArithmeticOperator
	{
		Set,
		Add,
		Subtract,
		Multiply,
		Divide,
		Remainder,
		Power
	}

	public abstract class Effect : Artifact
	{
		public abstract void OnEffect();
		public abstract void Cancle();
	}
}