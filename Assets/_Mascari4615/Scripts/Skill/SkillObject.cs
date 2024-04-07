using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.MHelper;

namespace Mascari4615
{
	public abstract class SkillComponent : MonoBehaviour
	{
		public abstract void InitContext(SkillObject skillObject);
	}

	public class SkillObject : MonoBehaviour
	{
		[field: Header("Context")]
		public UnitObject User { get; private set; }
		public bool UsedByPlayer { get; private set; }

		private SkillComponent[] skillComponents;

		private void OnEnable()
		{
			GameManager.Instance.AddObject(ObjectBufferType.Skill, gameObject);
		}

		private void OnDisable()
		{
			if (IsPlaying)
				GameManager.Instance.RemoveObject(ObjectBufferType.Skill, gameObject);
		}

		public void InitContext(UnitObject unitObject)
		{
			User = unitObject;
			UsedByPlayer = (unitObject is PlayerObject);

			skillComponents = GetComponentsInChildren<SkillComponent>(true);

			foreach (SkillComponent skillComponent in skillComponents)
				skillComponent.InitContext(this);
		}
	}
}