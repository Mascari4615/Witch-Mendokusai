using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			GameManager.Instance.SkillObjects.Add(gameObject);
		}

		private void OnDisable()
		{
			GameManager.Instance.SkillObjects.Remove(gameObject);
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