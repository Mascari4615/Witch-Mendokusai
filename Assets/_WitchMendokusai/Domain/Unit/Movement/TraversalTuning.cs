using System;
using UnityEngine;

namespace WitchMendokusai
{
	[Serializable]
	public sealed class TraversalTuning
	{
		[field: SerializeField] public bool Enabled { get; private set; }
		[field: SerializeField, Min(0.1f)] public float ClimbSpeed { get; private set; } = 2f;
		[field: SerializeField, Min(0.01f)] public float GrabReach { get; private set; } = 0.35f;
		[field: SerializeField, Range(0f, 1f)] public float WallNormalLimit { get; private set; } = 0.25f;
		[field: SerializeField, Min(0f)] public float RegrabDelay { get; private set; } = 0.5f;
		[field: SerializeField, Min(0.1f)] public float MantleReach { get; private set; } = 0.8f;
		[field: SerializeField, Min(0.1f)] public float MantleSpeed { get; private set; } = 2.5f;
		[field: SerializeField, Min(0.1f)] public float MantleTimeout { get; private set; } = 2f;
		[field: SerializeField, Min(0.01f)] public float Clearance { get; private set; } = 0.04f;
		[field: SerializeField, Min(0f)] public float ClimbJumpUpSpeed { get; private set; } = 5f;
		[field: SerializeField, Min(0f)] public float ClimbJumpOutSpeed { get; private set; } = 3f;
		[field: SerializeField, Min(0.1f)] public float GlideSpeed { get; private set; } = 6f;
		[field: SerializeField, Min(0.1f)] public float GlideSinkSpeed { get; private set; } = 1.5f;
		[field: SerializeField, Min(0.1f)] public float GlideAcceleration { get; private set; } = 8f;
		[field: SerializeField, Min(1f)] public float GlideTurnDegrees { get; private set; } = 120f;
		[field: SerializeField, Min(0f)] public float GlideMinHeight { get; private set; } = 1.2f;
		[field: SerializeField, Min(1f)] public float MaxStamina { get; private set; } = 100f;
		[field: SerializeField, Min(0f)] public float ClimbCostPerSecond { get; private set; } = 12f;
		[field: SerializeField, Min(0f)] public float GlideCostPerSecond { get; private set; } = 8f;
		[field: SerializeField, Min(0f)] public float ClimbJumpCost { get; private set; } = 15f;
		[field: SerializeField, Min(0f)] public float RecoveryPerSecond { get; private set; } = 25f;
		[field: SerializeField, Min(0f)] public float RecoveryDelay { get; private set; } = 0.5f;
		[field: SerializeField, Min(0f)] public float RestartStamina { get; private set; } = 20f;
	}
}
