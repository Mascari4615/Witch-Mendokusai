using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

		protected override void Awake()
		{
			base.Awake();
			cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
		}

		public void SetCamera(CameraType cameraType)
		{
			cinemachineBrain.m_DefaultBlend.m_Style = (int)cameraType == (int)MCanvasType.Dungeon ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseInOut;

			for (int i = 0; i < virtualCameras.Length; i++)
			{
				virtualCameras[i].Priority = i == (int)cameraType ? 10 : 0;
			}
		}

		private void LateUpdate()
		{
			Vector3 direction = (PlayerController.Instance.transform.position - cinemachineBrain.transform.position).normalized;
			// RaycastHit[] hits = Physics.RaycastAll(cinemachineBrain.transform.position, direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnvironmentObject"));
			RaycastHit[] hits = Physics.RaycastAll(cinemachineBrain.transform.position, direction, Mathf.Infinity);

			for (int i = 0; i < hits.Length; i++)
			{
				TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

				for (int j = 0; j < obj.Length; j++)
				{
					obj[j]?.BecomeTransparent();
				}
			}
		}
		
		public void 뽀삐뽀삐뽀()
		{
			impulseSource.GenerateImpulse();
		}
	}
}