using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

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
		[SerializeField] private CinemachineVirtualCamera[] virtualCameras;
		[SerializeField] private CinemachineImpulseSource impulseSource;
		[SerializeField] private float xDiff = .3f;

		private Coroutine chatXCoroutine;
		private CinemachineFramingTransposer chatFramingTransposer; 

		protected override void Awake()
		{
			base.Awake();
			cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.FixedUpdate;
			chatFramingTransposer = virtualCameras[2].GetCinemachineComponent<CinemachineFramingTransposer>();
		}

		public void SetCamera(CameraType cameraType)
		{
			cinemachineBrain.DefaultBlend.Style = (int)cameraType == (int)MCanvasType.Dungeon ? CinemachineBlendDefinition.Styles.Cut : CinemachineBlendDefinition.Styles.EaseInOut;

			for (int i = 0; i < virtualCameras.Length; i++)
			{
				virtualCameras[i].Priority = i == (int)cameraType ? 10 : 0;
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
			const float defaultX = 0.5f;
			while (true)
			{
				chatFramingTransposer.m_ScreenX = Mathf.Lerp(chatFramingTransposer.m_ScreenX, defaultX + diff, lerpSpeed);
				if (Mathf.Abs(chatFramingTransposer.m_ScreenX - (defaultX + diff)) < 0.01f)
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