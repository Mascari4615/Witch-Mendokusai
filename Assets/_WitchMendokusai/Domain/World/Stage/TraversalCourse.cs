using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace WitchMendokusai
{
	public sealed class TraversalCourse : MonoBehaviour
	{
		[SerializeField] private Transform[] checkpoints;
		[SerializeField] private StyleSheet hudStyle;
		[SerializeField] private float fallLimit = -6f;
		[SerializeField] private float checkpointRadius = 2f;
		[SerializeField] private float isolationHeight = 100f;
		private Vector3 returnPosition;
		private PlayerProvider players;
		private StageManager stages;
		private UIRoot uiRoot;
		private VisualElement hud;
		private Label status;
		private bool started;
		public int CheckpointIndex { get; private set; }
		public int RecoveryCount { get; private set; }
		public bool Finished => CheckpointIndex == checkpoints.Length - 1;
		public Vector3 CheckpointPosition => checkpoints[CheckpointIndex].position;

		[Inject]
		public void Construct(PlayerProvider players, StageManager stages, UIRoot uiRoot)
		{
			this.players = players;
			this.stages = stages;
			this.uiRoot = uiRoot;
		}

		private void OnEnable()
		{
			CheckpointIndex = 0;
			RecoveryCount = 0;
			started = false;
		}

		private void Update()
		{
			if (started == false)
			{
				returnPosition = players.CurrentObject.UnitMovement.Position;
				transform.position += Vector3.up * Mathf.Max(0f, isolationHeight - transform.position.y);
				players.CurrentObject.UnitMovement.Teleport(CheckpointPosition);
				BuildHud();
				started = true;
			}
			UnitMovement movement = players.CurrentObject.UnitMovement;
			Vector3 position = movement.Position;
			if (position.y < transform.position.y + fallLimit)
				Recover();
			if (Finished == false && movement.IsGrounded() && Vector3.Distance(position, checkpoints[CheckpointIndex + 1].position) <= checkpointRadius)
				CheckpointIndex++;
			status.text = $"이동 시험 {CheckpointIndex + 1}/{checkpoints.Length} | {movement.Traversal.Mode} | 기력 {movement.Traversal.Stamina:0} | 복귀 {RecoveryCount}";
			if (Finished)
				status.text += " | 도착";
		}

		private void BuildHud()
		{
			hud = new VisualElement();
			hud.AddToClassList("traversal-hud");
			hud.styleSheets.Add(hudStyle);
			status = new Label();
			hud.Add(status);
			hud.Add(new Label("WASD 이동 / Ctrl 질주 / Space 점프, 등반 도약, 활공 전환 / C 놓기, 접기"));
			hud.Add(new Label("청록 벽을 향해 이동하면 등반. W/S 상하, A/D 좌우. 꼭대기에서 앞으로 계속 이동."));
			VisualElement buttons = new();
			buttons.AddToClassList("traversal-buttons");
			buttons.Add(new Button(Recover) { text = "체크포인트 복귀" });
			buttons.Add(new Button(Return) { text = "이전 Stage로" });
			hud.Add(buttons);
			uiRoot.WindowsLayer.Add(hud);
		}

		public void Recover()
		{
			players.CurrentObject.UnitMovement.Teleport(CheckpointPosition);
			RecoveryCount++;
		}

		public void Return()
		{
			players.CurrentObject.UnitMovement.Teleport(returnPosition);
			stages.LoadStage(stages.LastStage, isBackToLastStage: true);
		}

		private void OnDisable()
		{
			hud?.RemoveFromHierarchy();
			hud = null;
		}
	}
}
