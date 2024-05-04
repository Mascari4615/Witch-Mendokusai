using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Mascari4615
{
	public class ExpManager : MonoBehaviour
	{
		private const int REQUIRE_EXP_INCREASEMENT = 50;
		
		[SerializeField] private GameObject levelUpEffect;
		
		private Stat PlayerStat => Player.Instance.Stat;

		private void Start()
		{
			PlayerStat.AddListener(StatType.EXP_CUR, UpdateLevel);
			Init();
		}

		public void Init()
		{
			PlayerStat[StatType.EXP_MAX] = REQUIRE_EXP_INCREASEMENT;
			PlayerStat[StatType.EXP_CUR] = 0;
			PlayerStat[StatType.LEVEL_CUR] = 0;
			// Debug.Log(nameof(Init) + PlayerStat[StatType.EXP_CUR] + " / " + PlayerStat[StatType.EXP_MAX]);
		}

		public void UpdateLevel()
		{
			// Debug.Log("Try Level Up" + PlayerStat[StatType.EXP_CUR] + " / " + PlayerStat[StatType.EXP_MAX]);
			if (PlayerStat[StatType.EXP_CUR] >= PlayerStat[StatType.EXP_MAX])
			{
				// Debug.Log("Level Up");
				RuntimeManager.PlayOneShot("event:/SFX/LevelUp", transform.position);

				PlayerStat[StatType.EXP_CUR] -= PlayerStat[StatType.EXP_MAX];
				PlayerStat[StatType.EXP_MAX] += REQUIRE_EXP_INCREASEMENT;
				PlayerStat[StatType.LEVEL_CUR]++;
				
				SOManager.Instance.OnLevelUp.Raise();

				GameObject l = ObjectPoolManager.Instance.Spawn(levelUpEffect);
				l.transform.position = Player.Instance.transform.position;
				l.SetActive(true);
			}
		}
	}
}