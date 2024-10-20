using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations;

namespace Mascari4615
{
	public enum CameraType
	{
		Normal,
		Dungeon,
		Dialogue,
	}

	public class CameraManager : Singleton<CameraManager>
	{
		[SerializeField] private CinemachineBrain cinemachineBrain;
		[SerializeField] private CinemachineCamera[] cinemachineCameras;
		[SerializeField] private PositionConstraint[] posDelegates;
		[SerializeField] private CinemachineImpulseSource impulseSource;
		[SerializeField] private float xDiff = .3f;

		private Coroutine chatXCoroutine;
		private CinemachinePositionComposer chatPositionTransposer;

		protected override void Awake()
		{
			base.Awake();
			Init();
		}

		private void Init()
		{
			cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.FixedUpdate;
			chatPositionTransposer = cinemachineCameras[2].GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
		}

		private void Start()
		{
			posDelegates[0].SetSource(0, new ConstraintSource { sourceTransform = Player.Instance.Object.CameraPosition, weight = 1 });
			posDelegates[1].SetSource(0, new ConstraintSource { sourceTransform = Player.Instance.Object.SpritePosition, weight = 1 });
		}

		public void SetCamera(CameraType cameraType)
		{
			cinemachineBrain.DefaultBlend.Style = (int)cameraType == (int)MCanvasType.Dungeon ? CinemachineBlendDefinition.Styles.Cut : CinemachineBlendDefinition.Styles.EaseInOut;

			for (int i = 0; i < cinemachineCameras.Length; i++)
			{
				cinemachineCameras[i].Priority = i == (int)cameraType ? 10 : 0;
			}

			if (chatXCoroutine != null)
				StopCoroutine(chatXCoroutine);
			chatXCoroutine = StartCoroutine(ChatXCoroutine(0));
		}

		// public void SetChatCamera(bool isLeft)
		public void SetChatCamera()
		{
			if (chatXCoroutine != null)
				StopCoroutine(chatXCoroutine);
			// chatXCoroutine = StartCoroutine(ChatXCoroutine(0.5f + xDiff * (isLeft ? 1f : -1f)));
			chatXCoroutine = StartCoroutine(ChatXCoroutine(-xDiff));
		}

		private IEnumerator ChatXCoroutine(float diff)
		{
			const float lerpSpeed = 0.03f;
			while (true)
			{
				chatPositionTransposer.Composition.ScreenPosition.x = Mathf.Lerp(chatPositionTransposer.Composition.ScreenPosition.x, diff, lerpSpeed);
				if (Mathf.Abs(chatPositionTransposer.Composition.ScreenPosition.x - diff) < 0.01f)
					break;
				yield return null;
			}
		}

		private void LateUpdate()
		{
			Vector3 direction = (Player.Instance.transform.position - cinemachineBrain.transform.position).normalized;
			// RaycastHit[] hits = Physics.RaycastAll(cinemachineBrain.transform.position, direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnvironmentObject"));
			RaycastHit[] hits = Physics.RaycastAll(cinemachineBrain.transform.position, direction, Mathf.Infinity);

			for (int i = 0; i < hits.Length; i++)
			{
				TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

				for (int j = 0; j < obj.Length; j++)
				{
					obj[j]?.UpdateTransparent();
				}
			}
		}

		public void 뽀삐뽀삐뽀()
		{
			impulseSource.GenerateImpulse();
		}
	}
}