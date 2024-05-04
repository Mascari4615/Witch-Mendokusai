using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class InputManager : MonoBehaviour
	{
		// TODO: InputManager를 통해 모든 입력을 받아서 처리하도록 한다.

		[SerializeField] private InputActionAsset inputActionAsset;

		private void Update()
		{
			if (SOManager.Instance.IsChatting.RuntimeValue)
				return;

			// UIManager
			if (Input.GetKeyDown(KeyCode.Tab))
				UIManager.Instance.ToggleOverlayUI_Tab();
			if (inputActionAsset["UI/Cancel"].triggered)
				UIManager.Instance.ToggleOverlayUI_Setting();
			
			// Player
			if (inputActionAsset["UI/Submit"].triggered)
				Player.Instance.TryInteract();
			if (Input.GetKeyDown(KeyCode.Space))
				Player.Instance.TryUseSkill(0);
			if (SOManager.Instance.IsMouseOnUI.RuntimeValue || SOManager.Instance.IsPaused.RuntimeValue)
				return;
			if (Input.GetMouseButton(0))
				Player.Instance.TryUseSkill(1);
			if (Input.GetMouseButton(1))
				Player.Instance.TryUseSkill(2);
		}
	}
}