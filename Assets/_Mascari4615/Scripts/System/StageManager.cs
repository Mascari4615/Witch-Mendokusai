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
		}

		public void LoadStage(Stage stage, int spawnPortalIndex)
		{
			UIManager.Instance.Transition(LoadStage_());

			IEnumerator LoadStage_()
			{
				// 플레이어는 가만히 있고, 스테이지가 위치에 따라 생성됨
				// SpawnPortalIndex가 -1이면, 포탈 타기 전 마지막 위치로 돌아간다는 뜻

				// https://gall.dcinside.com/mgallery/board/view/?id=game_dev&no=99368
				PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(NoCollision));
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
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
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(Unit));
			}
		}
	}
}