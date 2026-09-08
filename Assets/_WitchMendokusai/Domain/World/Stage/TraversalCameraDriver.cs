using UnityEngine;

namespace WitchMendokusai
{
	// 이동 상태를 카메라 프로필로 연결하는 어댑터. 카메라 코어는 이동 모드를 참조하지 않음
	public sealed class TraversalCameraDriver : MonoBehaviour
	{
		[SerializeField] private OrbitCameraProfile ground = new();
		[SerializeField] private OrbitCameraProfile climb = new();
		[SerializeField] private OrbitCameraProfile glide = new();
		private CameraManager cameras;
		private UnitMovement movement;
		private CameraControlMode previousControl;
		private CameraPerspective previousPerspective;
		private ContentCameraMode previousContent;
		private OrbitCameraProfile previousProfile;
		private Vector2 previousAngles;
		private bool active;

		public void Begin(UnitMovement actor)
		{
			movement = actor;
			cameras = CameraManager.Instance;
			previousControl = cameras.ControlMode;
			previousPerspective = cameras.Perspective;
			previousContent = cameras.CurrentContentMode;
			previousProfile = cameras.OrbitProfile;
			previousAngles = cameras.OrbitAngles;
			cameras.SetContentCameraMode(ContentCameraMode.Normal);
			cameras.SetPerspective(CameraPerspective.ThirdPerson);
			cameras.SetControlMode(CameraControlMode.MouseLook);
			cameras.SetOrbitAngles(Vector2.zero);
			active = true;
		}

		private void Update()
		{
			if (active == false)
				return;
			OrbitCameraProfile profile = movement.Traversal.Mode switch
			{
				TraversalMode.Climb or TraversalMode.Mantle => climb,
				TraversalMode.Glide => glide,
				_ => ground
			};
			if (cameras.OrbitProfile != profile)
				cameras.SetOrbitProfile(profile);
		}

		private void OnDisable()
		{
			if (active == false || cameras == null)
				return;
			active = false;
			cameras.SetOrbitProfile(previousProfile);
			cameras.SetContentCameraMode(previousContent);
			cameras.SetPerspective(previousPerspective);
			cameras.SetControlMode(previousControl);
			cameras.SetOrbitAngles(previousAngles);
		}
	}
}
