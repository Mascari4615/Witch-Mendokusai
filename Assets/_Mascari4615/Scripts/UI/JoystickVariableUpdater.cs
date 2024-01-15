using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class JoystickVariableUpdater : MonoBehaviour
	{
		[SerializeField] private VariableJoystick variableJoystick;

		[SerializeField] private FloatVariable joystickX;
		[SerializeField] private FloatVariable joystickY;

		private void Update()
		{
			joystickX.RuntimeValue = variableJoystick.Horizontal;
			joystickY.RuntimeValue = variableJoystick.Vertical;
		}
	}
}