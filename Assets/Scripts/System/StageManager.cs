using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class StageManager : Singleton<StageManager>
	{
		private const string NoCollision = "NO_CLLISION";
		private const string Unit = "Unit";

		[SerializeField] private UIStageResult uIStageResult;
		[SerializeField] private GameObject homeStageObject;
		[SerializeField] private Stage homeStage;

		public StageObject CurStageObject { get; private set; }
		public Stage LastStage { get; private set; }
		public Stage CurStage { get; private set; }

		private Vector3 lastPosDiff;

		private void Start()
		{
			CurStageObject = homeStageObject.GetComponent<StageObject>();
			CurStage = homeStage;
			uIStageResult.SetActive(false);
		}

		public void LoadStage(Stage stage, int spawnPortalIndex)
		{
			StartCoroutine(LoadStage_(stage, spawnPortalIndex));
		}

		private IEnumerator LoadStage_(Stage stage, int spawnPortalIndex)
		{
			// 플레이어는 가만히 있고, 스테이지가 위치에 따라 생성됨
			// SpawnPortalIndex가 -1이면, 포탈 타기 전 마지막 위치로 돌아간다는 뜻

			// https://gall.dcinside.com/mgallery/board/view/?id=game_dev&no=99368
			PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(NoCollision));
			yield return new WaitForFixedUpdate();

			// 마지막 스테이지를 현재 스테이지로 갱신하고, 비활성화 
			Vector3 newLastPosDiff = PlayerController.Instance.transform.position - CurStageObject.gameObject.transform.position;
			// Debug.Log(newLastPosDiff);
			CurStageObject.gameObject.SetActive(false);
			LastStage = CurStage;

			// 새로운 스테이지 생성 (비활성화 상태)
			GameObject targetStage = stage.Prefab.gameObject;
			CurStageObject = ObjectManager.Instance.PopObject(targetStage).GetComponent<StageObject>();

			// 새로운 스테이지 위치 변환
			Vector3 newStagePos = (spawnPortalIndex != -1)
				? PlayerController.Instance.transform.position - stage.Prefab.Portals[spawnPortalIndex].TpPos.position
				: PlayerController.Instance.transform.position - lastPosDiff;
			CurStageObject.transform.position = newStagePos;

			// 새로운 스테이지 활성화
			CurStageObject.gameObject.SetActive(true);
			CurStage = stage;

			lastPosDiff = newLastPosDiff;

			yield return new WaitForFixedUpdate();
			PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(Unit));
		}

		public void GameEnd()
		{
			TimeManager.Instance.Pause();

			uIStageResult.Init();
			uIStageResult.SetActive(true);
		}

		public void Continue()
		{
			// 집으로 돌아가기
			TimeManager.Instance.Resume();
			uIStageResult.SetActive(false);

			for (int i = GameManager.Instance.EnemyDataBuffer.RuntimeItems.Count - 1; i >= 0; i--)
				GameManager.Instance.EnemyDataBuffer.RuntimeItems[i].SetActive(false);
			for (int i = GameManager.Instance.DropItemDataBuffer.RuntimeItems.Count - 1; i >= 0; i--)
				GameManager.Instance.DropItemDataBuffer.RuntimeItems[i].SetActive(false);

			// Debug.Log(Instance.LastStage);
			LoadStage(Instance.LastStage, -1);

			GameManager.Instance.SetPlayerState(PlayerState.Peaceful);
		}
	}
}