using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class ExpManager : MonoBehaviour
	{
		[SerializeField] private IntVariable curExp;
		[SerializeField] private IntVariable maxExp;
		[SerializeField] private IntVariable curLevel;
		[SerializeField] private GameEvent onLevelUp;
		[SerializeField] private GameObject levelUpEffect;

		private int initMaxExp;
		private int initLevel;

		private void Awake()
		{
			initMaxExp = maxExp.RuntimeValue;
			initLevel = curLevel.RuntimeValue;
		}

		public void Init()
		{
			curExp.RuntimeValue = 0;
			maxExp.RuntimeValue = initMaxExp;
			curLevel.RuntimeValue = initLevel;
		}

		public void UpdateLevel()
		{
			if (curExp.RuntimeValue >= maxExp.RuntimeValue)
			{
				curExp.RuntimeValue -= maxExp.RuntimeValue;
				maxExp.RuntimeValue += 10;
				curLevel.RuntimeValue++;
				onLevelUp.Raise();

				GameObject l = ObjectManager.Instance.PopObject(levelUpEffect);
				l.transform.position = PlayerController.Instance.transform.position;
				l.SetActive(true);
			}
		}
	}
}