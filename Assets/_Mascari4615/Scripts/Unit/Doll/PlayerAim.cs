using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerAim : MonoBehaviour
	{
		[SerializeField] private CinemachineTargetGroup targetGroup;

		[SerializeField] private float maxDistance;
		// [SerializeField] private float maxWeight;
		// [SerializeField] private float blendSpeed;

		[SerializeField] private GameObject targetMarker;
		[SerializeField] private Transform targetPos;
		[SerializeField] private Animator targetMarkerAnimator;

		private GameObject curNearestAutoTarget;
		private Coroutine changeTarget;

		private void Start()
		{
			StartCoroutine(AimLoop());
			StartCoroutine(AutoAimLoop());
		}

		private void Update()
		{
			if (curNearestAutoTarget)
				targetMarker.transform.position = curNearestAutoTarget.transform.position;
		}

		private IEnumerator AutoAimLoop()
		{
			while (true)
			{
				GameObject aimTarget = null;
			
				List<GameObject> targets = ObjectBufferManager.Instance.GetObjectsWithDistance(ObjectType.Monster, transform.position, maxDistance);
				if (targets != null)
				{
					foreach (GameObject target in targets)
					{
						if (Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit hit, maxDistance))
						{
							if (hit.collider.gameObject == target)
								aimTarget = target;
						}
					}
				}

				if (aimTarget == null)
				{
					curNearestAutoTarget = null;
					// targetGroup.m_Targets[1].target = null;
					// SOManager.Instance.PlayerAutoAimPosition.RuntimeValue = Vector3.zero;

					if (targetMarker.activeSelf)
						targetMarker.SetActive(false);

					// targetGroup.m_Targets[1].weight = .5f;
				}
				else
				{
					bool targetChanged = false;

					if (curNearestAutoTarget != aimTarget)
					{
						targetChanged = true;
						curNearestAutoTarget = aimTarget;

						if (changeTarget != null)
						{
							// StopCoroutine(changeTarget);
						}

						// changeTarget = StartCoroutine(ChangeTarget());

						targetPos.position = curNearestAutoTarget.transform.position;
						// targetGroup.m_Targets[1].target = curNearestTarget.transform;
					}

					if (!targetMarker.activeSelf)
						targetMarker.SetActive(true);

					if (targetChanged)
						targetMarkerAnimator.SetTrigger("ON");

					// targetMarker.transform.position = curNearestTarget.transform.position;
					SOManager.Instance.PlayerAutoAimPosition.RuntimeValue = curNearestAutoTarget.transform.position;
				}

				targetGroup.m_Targets[1].target = null;
				//	Vector3.Distance(PlayerController.Instance.transform.position, targetPos.transform.position) > maxDistance
				//		? null
				//		: targetPos;

				yield return new WaitForSeconds(.3f);
			}
		}

		/*public IEnumerator ChangeTarget()
        {
            while (targetGroup.m_Targets[1].weight > 0)
            {
                targetGroup.m_Targets[1].weight -= Time.deltaTime * blendSpeed;
                yield return null;
            }

            targetGroup.m_Targets[1].target = curNearestTarget.transform;

            while (targetGroup.m_Targets[1].weight < maxWeight)
            {
                targetGroup.m_Targets[1].weight += Time.deltaTime * blendSpeed;
                yield return null;
            }
        }*/

		private IEnumerator AimLoop()
		{
			while (true)
			{
				// if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("GROUND")))
				{
					// Debug.Log($"충돌된 물체 이름 : {hit.transform.name}, Position : {hit.point}");
					Vector3 mouseWorldPosition = hit.point;
					SOManager.Instance.PlayerAimDirection.RuntimeValue = (mouseWorldPosition - transform.position).normalized;
				}
				else
				{
					SOManager.Instance.PlayerAimDirection.RuntimeValue = Vector3.zero;
				}

				yield return null;
			}
		}
	}
}