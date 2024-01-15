using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class SOManager : Singleton<SOManager>
	{
		public static SOManager I => Instance;

		[field: Space(10), Header("PlayerData")]
		[field: SerializeField] public IntVariable CurHp { get; private set; }
		[field: SerializeField] public FloatVariable InvincibleTime { get; private set; }
		[field: SerializeField] public FloatVariable JoystickX { get; private set; }
		[field: SerializeField] public FloatVariable JoystickY { get; private set; }
		[field: SerializeField] public FloatVariable MovementSpeed { get; private set; }
		[field: SerializeField] public FloatVariable DashDuration { get; private set; }
		[field: SerializeField] public FloatVariable DashSpeed { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerMoveDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerLookDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerAimDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerAutoAimDirection { get; private set; }
		[field: SerializeField] public BoolVariable IsChatting { get; private set; }
		[field: SerializeField] public BoolVariable IsDashing { get; private set; }

		[field: Space(10), Header("Buffer")]
		[field: SerializeField] public GameObjectBuffer SpawnCircleBuffer { get; private set; }

		[field: Space(10), Header("GameEvent")]
		[field: SerializeField] public GameEvent OnPlayerHit { get; private set; }
		[field: SerializeField] public GameEvent OnPlayerDied { get; private set; }
	}
}