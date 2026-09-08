using UnityEngine;

namespace WitchMendokusai
{
	public sealed class TraversalProbe
	{
		private const int BUFFER_SIZE = 32;
		private readonly CapsuleCollider capsule;
		private readonly TraversalTuning tuning;
		private readonly RaycastHit[] hits = new RaycastHit[BUFFER_SIZE];
		private readonly Collider[] overlaps = new Collider[BUFFER_SIZE];

		public TraversalProbe(CapsuleCollider capsule, TraversalTuning tuning)
		{
			this.capsule = capsule;
			this.tuning = tuning;
		}

		public float Radius => capsule.radius * Mathf.Max(Mathf.Abs(capsule.transform.lossyScale.x), Mathf.Abs(capsule.transform.lossyScale.z));
		private Vector3 CenterOffset => capsule.transform.TransformVector(capsule.center);
		private float Height => Mathf.Max(Radius * 2f, capsule.height * Mathf.Abs(capsule.transform.lossyScale.y));

		public bool Wall(Vector3 position, Vector3 direction, out RaycastHit hit)
		{
			if (direction.sqrMagnitude == 0f || Cast(position + CenterOffset, direction.normalized, Radius + tuning.GrabReach, out hit) == false)
			{
				hit = default;
				return false;
			}
			return Mathf.Abs(hit.normal.y) <= tuning.WallNormalLimit && hit.collider.GetComponentInParent<ClimbableSurface>() != null;
		}

		public bool GlideClearance(Vector3 position)
		{
			Vector3 feet = position + CenterOffset - Vector3.up * (Height * 0.5f);
			return Cast(feet + Vector3.up * tuning.Clearance, Vector3.down, tuning.GlideMinHeight + tuning.Clearance, out _) == false;
		}

		public bool Mantle(Vector3 position, Vector3 normal, out Vector3 target)
		{
			float reach = tuning.MantleReach;
			Vector3 inside = position - normal * (Radius * 2f + tuning.GrabReach + tuning.Clearance);
			Vector3 origin = inside + Vector3.up * (reach + tuning.Clearance);
			if (Cast(origin, Vector3.down, reach + tuning.Clearance, out RaycastHit top) && top.normal.y > 0.7f)
			{
				target = new Vector3(inside.x, top.point.y + tuning.Clearance, inside.z);
				Vector3 above = new(position.x, target.y, position.z);
				return HasRoom(target) && HasRoom(above);
			}
			target = default;
			return false;
		}

		public bool HasRoom(Vector3 position)
		{
			Vector3 center = position + CenterOffset;
			float segment = Mathf.Max(0f, Height * 0.5f - Radius);
			int count = Physics.OverlapCapsuleNonAlloc(center - Vector3.up * segment, center + Vector3.up * segment,
				Mathf.Max(tuning.Clearance, Radius - tuning.Clearance), overlaps, ~0, QueryTriggerInteraction.Ignore);
			if (count == overlaps.Length)
				return false;
			for (int i = 0; i < count; i++)
			{
				if (Accept(overlaps[i]))
					return false;
			}
			return true;
		}

		private bool Cast(Vector3 origin, Vector3 direction, float distance, out RaycastHit nearest)
		{
			int count = Physics.RaycastNonAlloc(origin, direction, hits, distance, ~0, QueryTriggerInteraction.Ignore);
			nearest = default;
			float nearestDistance = float.PositiveInfinity;
			for (int i = 0; i < count; i++)
			{
				if (Accept(hits[i].collider) && hits[i].distance < nearestDistance)
				{
					nearest = hits[i];
					nearestDistance = hits[i].distance;
				}
			}
			return nearest.collider != null;
		}

		private bool Accept(Collider other)
		{
			return other != null && other.transform.IsChildOf(capsule.transform) == false &&
				Physics.GetIgnoreLayerCollision(capsule.gameObject.layer, other.gameObject.layer) == false &&
				Physics.GetIgnoreCollision(capsule, other) == false;
		}
	}
}
