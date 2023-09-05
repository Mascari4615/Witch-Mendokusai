using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerAutoAim : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] private float maxDistance;
    // [SerializeField] private float maxWeight;
    // [SerializeField] private float blendSpeed;

    [SerializeField] private GameObject targetMarker;
    [SerializeField] private Transform targetPos;
    [SerializeField] private Animator targetMarkerAnimator;

    [SerializeField] private Vector3Variable playerAutoAimDirection;

    private GameObject curNearestTarget;
    private Coroutine changeTarget;

    private GameObject NearestTarget()
    {
        GameObject nearest = null;

        var playerPos = PlayerController.Instance.transform.position;
        var minDistance = float.MaxValue;

        // TODO : 레이쏴서 벽 뒤에 있으면 Continue

        foreach (var target in GameManager.Instance.EnemyRuntimeSet.Items)
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

    private void Start()
    {
        StartCoroutine(AimLoop());
    }

    private IEnumerator AimLoop()
    {
        while (true)
        {
            var target = GameManager.Instance.EnemyRuntimeSet.Items.Count > 0 ? NearestTarget() : null;

            if (target == null)
            {
                curNearestTarget = null;
                // targetGroup.m_Targets[1].target = null;
                playerAutoAimDirection.RuntimeValue = Vector3.zero;

                if (targetMarker.activeSelf)
                    targetMarker.SetActive(false);

                // targetGroup.m_Targets[1].weight = .5f;
            }
            else
            {
                var targetChanged = false;
                
                if (curNearestTarget != target)
                {
                    targetChanged = true;
                    curNearestTarget = target;

                    if (changeTarget != null)
                    {
                        // StopCoroutine(changeTarget);
                    }

                    // changeTarget = StartCoroutine(ChangeTarget());

                    targetPos.position = curNearestTarget.transform.position;
                    // targetGroup.m_Targets[1].target = curNearestTarget.transform;
                }

                if (!targetMarker.activeSelf)
                    targetMarker.SetActive(true);
                
                if (targetChanged)
                    targetMarkerAnimator.SetTrigger("ON");
                
                // targetMarker.transform.position = curNearestTarget.transform.position;
                playerAutoAimDirection.RuntimeValue = (curNearestTarget.transform.position - transform.position).normalized;
            }

            targetGroup.m_Targets[1].target =
                Vector3.Distance(PlayerController.Instance.transform.position, targetPos.transform.position) > maxDistance
                    ? null
                    : targetPos;

            yield return new WaitForSeconds(.3f);
        }
    }

    private void Update()
    {
        if (curNearestTarget)
            targetMarker.transform.position = curNearestTarget.transform.position;
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
}