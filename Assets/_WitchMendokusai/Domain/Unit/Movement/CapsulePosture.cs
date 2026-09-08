using UnityEngine;

namespace WitchMendokusai
{
	public sealed class CapsulePosture
	{
		private const int OVERLAP_CAPACITY = 32;
		private const float CONTACT_INSET = 0.005f;
		private readonly CapsuleCollider capsule;
		private readonly float standingHeight;
		private readonly Vector3 standingCenter;
		private readonly Collider[] overlaps = new Collider[OVERLAP_CAPACITY];

		public bool IsCrouching { get; private set; }

		public CapsulePosture(CapsuleCollider capsule)
		{
			this.capsule = capsule;
			standingHeight = capsule.height;
			standingCenter = capsule.center;
		}

		public void SetCrouching(bool requested, float heightFraction)
		{
			float height = requested ? Mathf.Max(capsule.radius * 2f, standingHeight * heightFraction) : standingHeight;
			Vector3 center = standingCenter + Vector3.up * ((height - standingHeight) * 0.5f);
			if (height > capsule.height && HasRoom(height, center) == false)
				return;
			capsule.height = height;
			capsule.center = center;
			IsCrouching = requested;
		}

		private bool HasRoom(float height, Vector3 center)
		{
			Transform body = capsule.transform;
			float radius = capsule.radius * Mathf.Max(Mathf.Abs(body.lossyScale.x), Mathf.Abs(body.lossyScale.z));
			float halfSegment = Mathf.Max(0f, height * Mathf.Abs(body.lossyScale.y) * 0.5f - radius);
			Vector3 worldCenter = body.TransformPoint(center);
			int count = Physics.OverlapCapsuleNonAlloc(worldCenter - body.up * halfSegment,
				worldCenter + body.up * halfSegment, Mathf.Max(CONTACT_INSET, radius - CONTACT_INSET),
				overlaps, ~0, QueryTriggerInteraction.Ignore);
			if (count == overlaps.Length)
				return false;
			for (int i = 0; i < count; i++)
			{
				Collider other = overlaps[i];
				if (other == capsule || other.transform.IsChildOf(body))
					continue;
				if (Physics.GetIgnoreLayerCollision(capsule.gameObject.layer, other.gameObject.layer) || Physics.GetIgnoreCollision(capsule, other))
					continue;
				return false;
			}
			return true;
		}
	}
}
