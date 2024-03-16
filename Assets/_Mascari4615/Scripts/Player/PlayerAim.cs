using System;
using System.Collections;
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
				var target = SOManager.Instance.MonsterObjectBuffer.RuntimeItems.Count > 0 ? NearestTarget() : null;

				if (target == null)
				{
					curNearestAutoTarget = null;
					// targetGroup.m_Targets[1].target = null;
					SOManager.Instance.PlayerAutoAimDirection.RuntimeValue = Vector3.zero;

					if (targetMarker.activeSelf)
						targetMarker.SetActive(false);

					// targetGroup.m_Targets[1].weight = .5f;
				}
				else
				{
					var targetChanged = false;

					if (curNearestAutoTarget != target)
					{
						targetChanged = true;
						curNearestAutoTarget = target;

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
					SOManager.Instance.PlayerAutoAimDirection.RuntimeValue = (curNearestAutoTarget.transform.position - transform.position).normalized;
				}

				targetGroup.m_Targets[1].target = null;
				//	Vector3.Distance(PlayerController.Instance.transform.position, targetPos.transform.position) > maxDistance
				//		? null
				//		: targetPos;

				yield return new WaitForSeconds(.3f);
			}
		}

		private GameObject NearestTarget()
		{
			GameObject nearest = null;

			var playerPos = PlayerController.Instance.transform.position;
			var minDistance = float.MaxValue;

			// TODO : 레이쏴서 벽 뒤에 있으면 Continue

			foreach (var target in SOManager.Instance.MonsterObjectBuffer.RuntimeItems)
			{
				// Debug.Log(target.gameObject.name);
				var distance = Vector3.Distance(playerPos, target.transform.position);

				if (distance > maxDistance)
					continue;

				if (distance < minDistance)
				{
					minDistance = distance;
					nearest = target;
				}
			}

			return nearest;
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