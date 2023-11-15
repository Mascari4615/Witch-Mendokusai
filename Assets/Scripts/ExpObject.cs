using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpObject : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private IntVariable curExp;
    [SerializeField] private int amount;

    private Coroutine _moveLoop;

    private void OnEnable()
    {
        StopAllCoroutines();
        _moveLoop = null;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_moveLoop != null)
            return;
        
        if (other.CompareTag("PlayerExpCollider"))
        {
            _moveLoop = StartCoroutine(MoveLoop());
        }
    }

    private IEnumerator MoveLoop()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position, t);

            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < .3f)
            {
                curExp.RuntimeValue += amount;
                gameObject.SetActive(false);
                
                _moveLoop = null;
                break;
            }
            
            yield return null;
        }
    }
}