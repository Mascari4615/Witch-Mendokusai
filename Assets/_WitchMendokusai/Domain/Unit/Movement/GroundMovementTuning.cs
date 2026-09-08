using System;
using UnityEngine;

namespace WitchMendokusai
{
	[Serializable]
	public sealed class GroundMovementTuning
	{
		[SerializeField] private bool enabled = false;
		[SerializeField, Min(0f)] private float acceleration = 18f;
		[SerializeField, Min(0f)] private float deceleration = 24f;
		[SerializeField, Min(0f)] private float airAcceleration = 6f;
		[SerializeField, Range(0f, 1f)] private float crouchSpeedMultiplier = 0.4f;
		[SerializeField, Range(0.1f, 1f)] private float crouchHeightFraction = 0.6f;

		public bool Enabled => enabled;
		public float Acceleration => acceleration;
		public float Deceleration => deceleration;
		public float AirAcceleration => airAcceleration;
		public float CrouchSpeedMultiplier => crouchSpeedMultiplier;
		public float CrouchHeightFraction => crouchHeightFraction;
	}
}
