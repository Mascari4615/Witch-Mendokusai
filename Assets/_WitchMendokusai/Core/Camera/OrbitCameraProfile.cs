using System;
using UnityEngine;

namespace WitchMendokusai
{
	[Serializable]
	public sealed class OrbitCameraProfile
	{
		[field: SerializeField, Min(0.1f)] public float Distance { get; private set; } = 7f;
		[field: SerializeField] public float Pitch { get; private set; } = 15f;
		[field: SerializeField, Range(20f, 100f)] public float FieldOfView { get; private set; } = 60f;
		[field: SerializeField] public Vector3 TargetOffset { get; private set; } = new(0f, 0.5f, 0f);
		[field: SerializeField] public Vector3 FollowDamping { get; private set; } = new(0.12f, 0.2f, 0.12f);
		[field: SerializeField, Min(0f)] public float BlendTime { get; private set; } = 0.25f;
	}
}
