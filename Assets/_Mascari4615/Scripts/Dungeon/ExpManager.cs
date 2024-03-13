using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class ExpManager : MonoBehaviour
	{
		private const int REQUIRE_EXP_INCREASEMENT = 25;
		
		[SerializeField] private GameObject levelUpEffect;

		public void Init()
		{
			SOManager.Instance.CurExp.RuntimeValue = 0;
			SOManager.Instance.MaxExp.RuntimeValue = 100;
			SOManager.Instance.CurLevel.RuntimeValue = 0;
		}

		public void UpdateLevel()
		{
			if (SOManager.Instance.CurExp.RuntimeValue >= SOManager.Instance.MaxExp.RuntimeValue)
			{
				RuntimeManager.PlayOneShot("event:/SFX/LevelUp", transform.position);
				SOManager.Instance.CurExp.RuntimeValue -= SOManager.Instance.MaxExp.RuntimeValue;
				SOManager.Instance.MaxExp.RuntimeValue += REQUIRE_EXP_INCREASEMENT;
				SOManager.Instance.CurLevel.RuntimeValue++;

				GameObject l = ObjectManager.Instance.PopObject(levelUpEffect);
				l.transform.position = PlayerController.Instance.transform.position;
				l.SetActive(true);
			}
		}
	}
}