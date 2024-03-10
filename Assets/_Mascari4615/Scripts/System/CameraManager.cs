using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Mascari4615
{
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

		public void SetCamera(int index)
		{
			cinemachineBrain.m_DefaultBlend.m_Style = index == (int)PlayerState.Combat ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseInOut;

			for (int i = 0; i < virtualCameras.Length; i++)
			{
				virtualCameras[i].Priority = i == index ? 10 : 0;
			}
		}

		private void LateUpdate()
		{
			// Player´Â ½Ì±ÛÅæÀÌ±â¿¡ Àü¿ªÀûÀ¸·Î Á¢±ÙÇÒ ¼ö ÀÖ½À´Ï´Ù.
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
		
		public void »Ç»ß»Ç»ß»Ç()
		{
			impulseSource.GenerateImpulse();
		}
	}
}