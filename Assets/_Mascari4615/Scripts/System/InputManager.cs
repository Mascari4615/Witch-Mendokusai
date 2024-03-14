using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class InputManager : MonoBehaviour
	{
		// TODO : InputManager를 통해 모든 입력을 받아서 처리하도록 한다.

		private void Update()
		{
			// UIManager
			if (Input.GetKeyDown(KeyCode.Tab))
				UIManager.Instance.Tab.ToggleTabMenu();
			if (Input.GetKeyDown(KeyCode.Escape))
				UIManager.Instance.ToggleSetting();

			// Player
			if (Input.GetKeyDown(KeyCode.F))
				PlayerController.Instance.TryInteract();

			if (SOManager.Instance.IsMouseOnUI.RuntimeValue || SOManager.Instance.IsPaused.RuntimeValue)
				return;
			if (Input.GetMouseButton(0))
				PlayerController.Instance.TryUseSkill(0);
			if (Input.GetMouseButton(1))
				PlayerController.Instance.TryUseSkill(1);
		}
	}
}