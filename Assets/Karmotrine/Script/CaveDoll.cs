using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveDoll : MonoBehaviour
{
    enum State
    {
        Idle,
        Move,
        Mining
    }

    private Vector3 _touchPos;

    [SerializeField] private Rigidbody2D manager;
    [SerializeField] private float moveSpeed = 1;

    private bool _isTargetingUnit = false;
    private StoneObject _targetStoneObject;

    private State curState = State.Idle;
    private Coroutine miningLoop;

    private void Update()
    {
        UpdateState();

        switch (curState)
        {
            case State.Move:
                Move();
                break;
            case State.Mining:
                Mining();
                break;
            case State.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateState()
    {
        CheckInput();
        if (curState == State.Move)
            CheckDistance();

        if (curState != State.Mining)
            if (miningLoop != null)
            {
                StopCoroutine(miningLoop);
                miningLoop = null;
            }

        void CheckInput()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount == 0)
                    return;

                var touch = Input.GetTouch(0);
                if (touch.phase is not (TouchPhase.Stationary or TouchPhase.Began))
                    return;

                _touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                _touchPos.z = 0;
                SetState(State.Move);
                _isTargetingUnit = false;
            }
            else
            {
                if (!Input.GetMouseButtonDown(0))
                    return;

                _touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _touchPos.z = 0;
                SetState(State.Move);
                _isTargetingUnit = false;

                var ray = new Ray2D(_touchPos, Vector2.zero);
                const float distance = Mathf.Infinity;
                var hit = Physics2D.Raycast(ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer("Unit"));

                if (!hit)
                    return;

                if (!hit.transform.parent.TryGetComponent(out _targetStoneObject))
                    return;

                _touchPos = hit.transform.position;
                _touchPos.z = 0;
                SetState(State.Move);
                _isTargetingUnit = true;
            }
        }

        void CheckDistance()
        {
            if (_isTargetingUnit)
            {
                if (Vector3.Distance(_touchPos, manager.transform.position) <= 1)
                {
                    manager.velocity = Vector2.zero;
                    SetState(State.Mining);
                }
            }
            else
            {
                if (Vector3.Distance(_touchPos, manager.transform.position) <= .05f)
                {
                    manager.transform.position = _touchPos;
                    manager.velocity = Vector2.zero;
                    SetState(State.Idle);
                }
            }
        }
    }

    private void SetState(State newState)
    {
        curState = newState;
    }

    private void Move()
    {
        manager.velocity = (_touchPos - manager.transform.position).normalized * moveSpeed;
    }

    private void Mining()
    {
        // _targetStoneObject.

        if (miningLoop == null)
            miningLoop = StartCoroutine(MiningLoop());
    }

    private IEnumerator MiningLoop()
    {
        while (true)
        {
            _targetStoneObject.ReceiveAttack(1);
            yield return new WaitForSeconds(.5f);
        }
    }
}