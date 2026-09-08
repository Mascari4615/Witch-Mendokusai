using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace WitchMendokusai.EditorTools
{
	public static class ExplorationPlayVerify
	{
		private static Keyboard keyboard;
		private static InputSettings savedInputSettings;
		private static InputSettings verificationInputSettings;
		private static double started;
		private static int phase;
		private static bool climbed;
		private static bool glided;
		private static CameraControlMode control;
		private static CameraPerspective perspective;
		private static Vector2 angles;
		private static OrbitCameraProfile profile;
		private static Stage previousStage;
		private static Vector3 previousPosition;
		private static int cameraSamples;
		private static int climbCameraSamples;
		public static string Result { get; private set; } = "Not run";

		public static void Start()
		{
			if (EditorApplication.isPlaying == false || PlayerProvider.Instance == null || CameraManager.Instance == null || StageManager.Instance == null)
				throw new InvalidOperationException("World Play에서 시작");
			Stop();
			CameraManager cameras = CameraManager.Instance;
			control = cameras.ControlMode;
			perspective = cameras.Perspective;
			angles = cameras.OrbitAngles;
			profile = cameras.OrbitProfile;
			previousStage = StageManager.Instance.CurStage;
			previousPosition = PlayerProvider.Instance.CurrentObject.UnitMovement.Position;
			// Use a temporary settings instance: automated input must not depend on editor focus.
			savedInputSettings = InputSystem.settings;
			verificationInputSettings = UnityEngine.Object.Instantiate(savedInputSettings);
			verificationInputSettings.hideFlags = HideFlags.HideAndDontSave;
			verificationInputSettings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
			verificationInputSettings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
			InputSystem.settings = verificationInputSettings;
			keyboard = InputSystem.AddDevice<Keyboard>("ExplorationVerifyKeyboard");
			phase = cameraSamples = climbCameraSamples = 0;
			climbed = glided = false;
			started = EditorApplication.timeSinceStartup;
			Result = "Running";
			DevWindowController.Instance.EnterExplorationCourse();
			EditorApplication.update += Tick;
			AssemblyReloadEvents.beforeAssemblyReload += Stop;
		}

		public static void Stop()
		{
			EditorApplication.update -= Tick;
			AssemblyReloadEvents.beforeAssemblyReload -= Stop;
			if (keyboard != null && keyboard.added)
				InputSystem.RemoveDevice(keyboard);
			keyboard = null;
			if (savedInputSettings != null)
				InputSystem.settings = savedInputSettings;
			savedInputSettings = null;
			if (verificationInputSettings != null)
				UnityEngine.Object.DestroyImmediate(verificationInputSettings);
			verificationInputSettings = null;
		}

		private static void Tick()
		{
			try
			{
				if (EditorApplication.isPlaying == false || EditorApplication.timeSinceStartup - started > 240d)
					throw new InvalidOperationException("Play 종료 또는 240초 초과");
				StageManager stages = StageManager.Instance;
				CameraManager cameras = CameraManager.Instance;
				UnitMovement movement = PlayerProvider.Instance.CurrentObject.UnitMovement;
				TraversalCourse course = stages.CurStageObject.GetComponent<TraversalCourse>();
				if (phase == 8)
				{
					Require(stages.CurStage == previousStage, "Stage 복원");
					Require(cameras.ControlMode == control && cameras.Perspective == perspective && cameras.OrbitProfile == profile, "카메라 모드 복원");
					Require(Vector2.Distance(cameras.OrbitAngles, angles) < 0.01f, "시점 각도 복원");
					Require(Vector3.Distance(movement.Position, previousPosition) < 0.2f, "귀환 위치");
					Result = $"PASS: route, climb, glide, camera samples={cameraSamples}, climb camera samples={climbCameraSamples}, return, seconds={EditorApplication.timeSinceStartup - started:0.0}";
					Stop();
					return;
				}
				if (course == null || course.transform.position.y < 100f)
					return;
				Vector3 local = course.transform.InverseTransformPoint(movement.Position);
				climbed |= movement.Traversal.Mode == TraversalMode.Climb;
				glided |= movement.Traversal.Mode == TraversalMode.Glide;
				bool jump = false;
				bool fold = false;
				if (phase == 0 && local.z >= 49.5f) Turn(1, 90f);
				if (phase == 1 && local.x >= 89.5f) Turn(2, 0f);
				if (phase == 2 && local.z >= 99.5f) Turn(3, 270f);
				if (phase == 3 && local.x <= 0.5f) Turn(4, 0f);
				if (phase == 4 && local.z > 156.6f && movement.IsGrounded() == false)
				{
					jump = true;
					if (movement.Traversal.Mode == TraversalMode.Glide)
						phase = 5;
				}
				if (phase == 5 && local.z > 170f)
				{
					fold = true;
					phase = 6;
				}
				if (course.Finished)
				{
					Require(climbed && glided && cameraSamples > 10 && climbCameraSamples > 10, "등반, 활공과 카메라 검사");
					Require(course.RecoveryCount == 0, "의도하지 않은 추락 없음");
					InputSystem.QueueStateEvent(keyboard, new KeyboardState());
					course.Return();
					phase = 8;
					return;
				}
				// 좁은 벽길에서 실제 가상 카메라의 시선 구간을 PhysX로 재검사
				if (phase == 0 && local.z > 12f && local.z < 29f)
				{
					MCamera view = cameras.GetComponentsInChildren<MCamera>().First(item => item.ContentCameraMode == ContentCameraMode.Normal);
					Unity.Cinemachine.CameraState state = view.CinemachineCamera.State;
					Vector3 origin = state.ReferenceLookAt;
					Vector3 end = state.RawPosition + state.PositionCorrection;
					foreach (RaycastHit hit in Physics.RaycastAll(origin, (end - origin).normalized, Vector3.Distance(origin, end), ~0, QueryTriggerInteraction.Ignore))
						Require(hit.collider.GetComponentInParent<GroundSurface>() == null, "벽길 카메라 가림");
					cameraSamples++;
				}
				if (movement.Traversal.Mode == TraversalMode.Climb)
				{
					MCamera view = cameras.GetComponentsInChildren<MCamera>().First(item => item.ContentCameraMode == ContentCameraMode.Normal);
					Unity.Cinemachine.CameraState state = view.CinemachineCamera.State;
					Vector3 end = state.RawPosition + state.PositionCorrection;
					Vector3 chest = movement.Position + Vector3.up * 0.5f;
					Require(Vector3.Distance(chest, end) > 1f, "등반 중 카메라 거리 붕괴");
					foreach (RaycastHit hit in Physics.RaycastAll(chest, (end - chest).normalized, Vector3.Distance(chest, end), ~0, QueryTriggerInteraction.Ignore))
						Require(hit.collider.GetComponentInParent<GroundSurface>() == null, "등반 중 캐릭터 시선 가림");
					climbCameraSamples++;
				}
				InputSystem.QueueStateEvent(keyboard, jump ? new KeyboardState(Key.W, Key.Space) : fold ? new KeyboardState(Key.W, Key.C) : new KeyboardState(Key.W));
				Result = $"Running phase={phase}, local={local}, mode={movement.Traversal.Mode}, checkpoint={course.CheckpointIndex}, recovery={course.RecoveryCount}";
			}
			catch (Exception exception)
			{
				Result = "FAIL: " + exception.Message + " | " + Result;
				Stop();
			}
		}

		private static void Turn(int nextPhase, float yaw)
		{
			phase = nextPhase;
			CameraManager.Instance.SetOrbitAngles(new Vector2(yaw, 0f));
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
