using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private BoolVariable IsPaused;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			IsPaused.RuntimeValue = !IsPaused.RuntimeValue;
	}
}
