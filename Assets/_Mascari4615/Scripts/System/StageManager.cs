using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		public void LoadStage(Stage stage, int spawnPortalIndex = -1, bool isBackToLastStage = false, Action action = null)
		{
			UIManager.Instance.Transition.Transition(cDuringTransition: LoadStage_(), aWhenEnd: () => UIManager.Instance.StagePopup(stage));

			IEnumerator LoadStage_()
			{
				// 플레이어는 가만히 있고, 스테이지가 위치에 따라 생성됨
				// SpawnPortalIndex가 -1이면, 포탈 타기 전 마지막 위치로 돌아간다는 뜻

				// https://gall.dcinside.com/mgallery/board/view/?id=game_dev&no=99368
				// 플레이어가 더 이상 충돌로 Interactive 오브젝트들을 찾지 않음
				// PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(NoCollision));

				// TimeScale 0이면 FixedUpdate가 호출되지 않기 때문에 임의로 조정
				TimeManager.Instance.Resume();
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();

				// 마지막 스테이지를 현재 스테이지로 갱신하고, 비활성화 
				Vector3 newLastPosDiff = Player.Instance.transform.position - CurStageObject.gameObject.transform.position;
				// Debug.Log(newLastPosDiff);
				CurStageObject.gameObject.SetActive(false);
				LastStage = CurStage;

				// 새로운 스테이지 생성 (비활성화 상태)
				GameObject targetStage = stage.Prefab.gameObject;
				CurStageObject = ObjectPoolManager.Instance.Spawn(targetStage).GetComponent<StageObject>();

				// 새로운 스테이지 위치 변환
				// TODO: 
				// Vector3 portalTPPos = stage.Prefab.Portals[spawnPortalIndex].TpPos.position;
				Vector3 portalTPPos = stage.Prefab.Portals.Where(p => p.TargetStage == LastStage).First().TpPos.position;
				Vector3 newStagePos = isBackToLastStage
					? Player.Instance.transform.position - lastPosDiff
					: Player.Instance.transform.position - portalTPPos;
				CurStageObject.transform.position = newStagePos;

				// 새로운 스테이지 활성화
				CurStageObject.gameObject.SetActive(true);
				CurStage = stage;

				lastPosDiff = newLastPosDiff;

				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				// 플레이어가 더 이상 충돌로 Interactive 오브젝트들을 찾지 않음
				// PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(Unit));
				TimeManager.Instance.Pause();

				action?.Invoke();
			}
		}
	}
}