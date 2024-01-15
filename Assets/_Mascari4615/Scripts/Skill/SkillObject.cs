using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class SkillObject : MonoBehaviour
	{
		[field: Header("Context")] public UnitObject User { get; private set; }
		public bool UsedByPlayer { get; private set; }

		[SerializeField] private SkillComponent[] _skillComponents;

		private void OnEnable()
		{
			GameManager.Instance?.SkillObjectBuffer.AddItem(gameObject);
		}

		private void OnDisable()
		{
			GameManager.Instance?.SkillObjectBuffer.RemoveItem(gameObject);
		}

		public void InitContext(UnitObject unitObject)
		{
			User = unitObject;
			UsedByPlayer = (unitObject is PlayerObject);

			foreach (var skillComponent in _skillComponents)
				skillComponent.InitContext(this);
		}
	}

	public abstract class SkillComponent : MonoBehaviour
	{
		public abstract void InitContext(SkillObject skillObject);
	}
}