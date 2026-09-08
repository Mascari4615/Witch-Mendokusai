using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace WitchMendokusai
{
	public sealed class TraversalCourse : MonoBehaviour
	{
		[SerializeField] private Transform[] checkpoints;
		[SerializeField] private Collider[] checkpointPlatforms;
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
		private Label instruction;
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
			if (Finished == false && movement.IsGrounded() && IsOnCheckpointPlatform(CheckpointIndex + 1, position))
				CheckpointIndex++;
			status.text = $"이동 시험 {CheckpointIndex + 1}/{checkpoints.Length} | {movement.Traversal.Mode} | 기력 {movement.Traversal.Stamina:0} | 복귀 {RecoveryCount}";
			if (Finished)
				status.text += " | 도착";
			instruction.text = Finished ? "도착! 체크포인트 복귀로 다시 서거나 이전 Stage로 돌아가기."
				: movement.Traversal.Mode == TraversalMode.Glide
				? "활공 중: WASD로 방향 조절. 노란 착지대 위에서 C로 접기."
				: CheckpointIndex == 0
					? "청록 벽을 향해 W로 등반. 꼭대기를 넘어 발판에 서면 복귀 지점 저장."
					: movement.IsGrounded()
						? "활공: W로 발판 밖으로 나간 뒤 Space 한 번. 점프했다면 Space를 놓고 다시 누르기."
						: "공중: Space를 눌러 활공. 누르고 있기만 하면 펼쳐지지 않음. 바닥에 가깝거나 기력이 소진되면 전개 불가.";
		}

		public bool IsOnCheckpointPlatform(int index, Vector3 position)
		{
			Bounds bounds = checkpointPlatforms[index].bounds;
			return position.x >= bounds.min.x && position.x <= bounds.max.x &&
				position.z >= bounds.min.z && position.z <= bounds.max.z &&
				Mathf.Abs(position.y - checkpoints[index].position.y) <= checkpointRadius;
		}

		private void BuildHud()
		{
			hud = new VisualElement();
			hud.AddToClassList("traversal-hud");
			hud.styleSheets.Add(hudStyle);
			status = new Label();
			hud.Add(status);
			hud.Add(new Label("WASD 이동 / Ctrl 질주 / Space 점프, 등반 도약, 활공 전환 / C 놓기, 접기"));
			instruction = new Label();
			hud.Add(instruction);
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
