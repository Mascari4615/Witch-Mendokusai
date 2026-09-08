using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace WitchMendokusai.EditorTools
{
	// World Play에서 가상 키보드로 실제 입력 경로와 Stage 왕복 검증
	public static class TraversalPlayVerify
	{
		private static Keyboard keyboard;
		private static Stage previousStage;
		private static Vector3 previousPosition;
		private static Vector3 previousOrigin;
		private static double deadline;
		private static int phase;
		private static bool climbed;
		private static bool mantled;
		private static bool glided;
		private static bool canopy;
		public static string Result { get; private set; } = "Not run";

		public static void Start(double timeoutSeconds = 90)
		{
			if (EditorApplication.isPlaying == false || PlayerProvider.Instance == null || StageManager.Instance == null)
				throw new InvalidOperationException("World Play에서 실행");
			Stop();
			previousStage = StageManager.Instance.CurStage;
			previousPosition = PlayerProvider.Instance.CurrentObject.UnitMovement.Position;
			previousOrigin = StageManager.Instance.CurStageObject.transform.position;
			if (StageManager.Instance.CurStageObject.GetComponent<TraversalCourse>() != null)
				throw new InvalidOperationException("시험 Stage 밖에서 시작");
			keyboard = InputSystem.AddDevice<Keyboard>("TraversalVerifyKeyboard");
			phase = 0;
			climbed = mantled = glided = canopy = false;
			deadline = EditorApplication.timeSinceStartup + timeoutSeconds;
			Result = "Running";
			DevWindowController.Instance.EnterTraversalCourse();
			EditorApplication.update += Tick;
		}

		public static void Stop()
		{
			EditorApplication.update -= Tick;
			if (keyboard != null && keyboard.added)
				InputSystem.RemoveDevice(keyboard);
			keyboard = null;
		}

		private static void Tick()
		{
			try
			{
				if (EditorApplication.isPlaying == false)
					throw new InvalidOperationException("Play 종료");
				if (EditorApplication.timeSinceStartup > deadline)
					throw new InvalidOperationException("시간 초과: " + Result);
				Player player = PlayerProvider.Instance.Current;
				UnitMovement movement = player.Object.UnitMovement;
				StageManager stages = StageManager.Instance;
				TraversalCourse course = stages.CurStageObject.GetComponent<TraversalCourse>();
				bool forward = false;
				bool jump = false;
				bool fold = false;
				if (course != null)
				{
					Vector3 local = course.transform.InverseTransformPoint(movement.Position);
					climbed |= movement.Traversal.Mode == TraversalMode.Climb;
					mantled |= movement.Traversal.Mode == TraversalMode.Mantle;
					glided |= movement.Traversal.Mode == TraversalMode.Glide;
					canopy |= player.transform.Find("TraversalGlider").gameObject.activeSelf;
					Result = $"Running phase={phase}, local={local}, mode={movement.Traversal.Mode}, checkpoint={course.CheckpointIndex}, recover={course.RecoveryCount}";
					if (phase == 0 && course.transform.position.y >= 100f)
						phase = 1;
					if (phase == 1)
					{
						forward = true;
						if (course.RecoveryCount > 0)
						{
							Require(climbed && mantled && course.CheckpointIndex == 1 && glided == false, "등반 후 활공 없는 추락 복귀");
							phase = 2;
						}
					}
					else if (phase == 2)
					{
						forward = true;
						jump = local.z > 22.6f && movement.IsGrounded() == false;
						if (movement.Traversal.Mode == TraversalMode.Glide)
							phase = 3;
					}
					else if (phase == 3)
					{
						forward = local.z < 35.5f;
						fold = forward == false;
						if (course.Finished)
						{
							Require(glided && canopy && course.RecoveryCount == 1, "활공 완주와 시각 표시");
							forward = false;
							movement.Teleport(course.transform.position + Vector3.down * 10f);
							phase = 4;
						}
					}
					else if (phase == 4 && course.RecoveryCount == 2)
					{
						Require(Vector3.Distance(movement.Position, course.CheckpointPosition) < 0.3f, "마지막 체크포인트 복귀");
						Require(movement.Traversal.Mode == TraversalMode.None && movement.Traversal.Stamina == 100f, "복귀 시 상태 초기화");
						course.Return();
						phase = 5;
					}
					else if (phase == 6 && course.transform.position.y >= 100f && course.CheckpointIndex == 0)
					{
						Require(course.RecoveryCount == 0, "재입장 체크포인트 초기화");
						course.Return();
						phase = 7;
					}
				}
				else if (phase == 5 || phase == 7)
				{
					Require(stages.CurStage == previousStage, "이전 Stage 복원");
					Require(Vector3.Distance(stages.CurStageObject.transform.position, previousOrigin) < 0.1f, "이전 Stage 원점 복원");
					Require(Vector3.Distance(movement.Position, previousPosition) < 0.3f, "이전 플레이어 위치 복원");
					if (phase == 7)
					{
						Result = "PASS: input climb/mantle, no-glide fall recovery, glide/canopy/landing, final checkpoint recovery, Stage return/reentry/return";
						Debug.Log("[TraversalPlayVerify] " + Result);
						Stop();
						return;
					}
					DevWindowController.Instance.EnterTraversalCourse();
					phase = 6;
				}
				KeyboardState state = forward ? (jump ? new KeyboardState(Key.W, Key.Space) : new KeyboardState(Key.W)) : (fold ? new KeyboardState(Key.C) : new KeyboardState());
				InputSystem.QueueStateEvent(keyboard, state);
			}
			catch (Exception error)
			{
				Result = "FAIL: " + error.Message;
				Debug.LogError("[TraversalPlayVerify] " + Result);
				Stop();
			}
		}

		private static void Require(bool passed, string description)
		{
			if (passed == false)
				throw new InvalidOperationException(description);
		}
	}
}
