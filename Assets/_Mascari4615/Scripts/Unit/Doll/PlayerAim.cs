using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerAim
	{
		private const float MaxAimDistance = 100f;

		private readonly Transform transform;
		public PlayerAim(Transform transform)
		{
			this.transform = transform;
		}

		public GameObject GetNearestTarget()
		{
			List<GameObject> targets = ObjectBufferManager.Instance.GetObjectsWithDistance(ObjectType.Monster, transform.position, MaxAimDistance);
			if (targets == null)
				return null;

			GameObject curNearestAutoTarget = null;
			int layerMask = 1 << LayerMask.NameToLayer("UNIT");

			float minDistance = MaxAimDistance;
			foreach (GameObject target in targets)
			{
				float distance = Vector3.Distance(transform.position, target.transform.position);
				if (distance >= minDistance)
					continue;

				Vector3 direction = target.transform.position - transform.position;
				if (Physics.Raycast(transform.position, direction, out RaycastHit hit, MaxAimDistance, layerMask) == false)
					continue;

				if (hit.collider.gameObject != target)
					continue;

				curNearestAutoTarget = target;
				minDistance = distance;
			}

			return curNearestAutoTarget;
		}

		public Vector3 CalcAutoAim()
		{
			GameObject nearestTarget = GetNearestTarget();
			return nearestTarget == null ? Vector3.zero : nearestTarget.transform.position;
		}

		public Vector3 CalcMouseAimDriection()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance = 100f;
			int layerMask = 1 << LayerMask.NameToLayer("GROUND");
			
			if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
			{
				// Debug.Log($"충돌된 물체 이름 : {hit.transform.name}, Position : {hit.point}");
				Vector3 mouseWorldPosition = hit.point;
				return (mouseWorldPosition - transform.position).normalized;
			}
			else
			{
				return Vector3.zero;
			}
		}
	}
}