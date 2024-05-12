using UnityEngine;

namespace Mascari4615
{
	public abstract class SkillComponent : MonoBehaviour
	{
		public abstract void InitContext(SkillObject skillObject);
	}
}