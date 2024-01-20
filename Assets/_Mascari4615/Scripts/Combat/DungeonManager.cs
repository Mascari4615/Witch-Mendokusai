using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class DungeonManager : Singleton<DungeonManager>
	{
		[field: Header("_" + nameof(DungeonManager))]
		[SerializeField] private MasteryManager masteryManager;
		[SerializeField] private MonsterSpawner monsterSpawner;

		[SerializeField] private UIDungeonEntrance uiDungeonEntrance;
		[SerializeField] private UICombatCanvas uiCombatCanvas;
		[SerializeField] private UIDamage uiDamage;

		[SerializeField] private DateTimeVarialbe combatStartTime;
		[SerializeField] private ExpManager expChecker;

		private TimeSpan dungeonTime;

		public void OpenDungeonEntranceUI(List<Dungeon> dungeonDatas)
		{
			Debug.Log(nameof(OpenDungeonEntranceUI));
			uiDungeonEntrance.OpenCanvas(dungeonDatas);
		}

		public void PopDamage(Vector3 pos, int damge)
		{
			StartCoroutine(uiDamage.DamageTextUI(pos, damge));
		}

		public void CombatIntro()
		{
		}

		[ContextMenu(nameof(StartCombat))]
		public void StartCombat(Dungeon dungeon)
		{
			Debug.Log($"{nameof(StartCombat)}");

			StageManager.Instance.LoadStage(dungeon, 0);
			monsterSpawner.transform.position = PlayerController.Instance.transform.position;
			GameManager.Instance.SetPlayerState(PlayerState.Combat);

			combatStartTime.RuntimeValue = DateTime.Now;

			monsterSpawner.StartWave(dungeon);

			uiCombatCanvas.UpdateExp();
			uiCombatCanvas.SetActive(true);

			expChecker.Init();
			masteryManager.Init();
			PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

			foreach (var effect in DataManager.Instance.CurStuff(0)!.Effects)
				effect.OnEquip();
			foreach (var effect in DataManager.Instance.CurStuff(1)!.Effects)
				effect.OnEquip();
			foreach (var effect in DataManager.Instance.CurStuff(2)!.Effects)
				effect.OnEquip();

			StartCoroutine(CombatLoop());
			dungeonTime = new TimeSpan(0, 0, 15, 0, 0);
;		}

		private IEnumerator CombatLoop()
		{
			Debug.Log(nameof(CombatLoop));
			var ws01 = new WaitForSeconds(.1f);

			while (true)
			{
				dungeonTime -= new TimeSpan(0, 0, 0, 0, 100);
				uiCombatCanvas.UpdateTime(dungeonTime);

				yield return ws01;
			}
		}

		public void EndCombat()
		{
			Debug.Log($"{nameof(EndCombat)}");

			monsterSpawner.StopLoop();
			StageManager.Instance.GameEnd();
			uiCombatCanvas.SetActive(false);

			masteryManager.ClearMasteryEffect();
			PlayerController.Instance.PlayerObject.Init(PlayerController.Instance.PlayerObject.UnitData);

			foreach (var effect in DataManager.Instance.CurStuff(0)!.Effects)
				effect.OnRemove();
			foreach (var effect in DataManager.Instance.CurStuff(1)!.Effects)
				effect.OnRemove();
			foreach (var effect in DataManager.Instance.CurStuff(2)!.Effects)
				effect.OnRemove();

			StopAllCoroutines();
		}
	}
}