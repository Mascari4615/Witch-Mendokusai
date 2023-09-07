using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIDamage : MonoBehaviour
{
    // public Stack<GameObject> mobHpBars;
    [SerializeField] private Transform damageTextsRoot;
    private Stack<(Animator animator, TextMeshProUGUI tmp)> _damageUIs;

    private void Start()
    {
        // mobHpBars = new Stack<GameObject>();
        _damageUIs = new Stack<(Animator, TextMeshProUGUI)>();

        /*for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            GameObject mobHpBar = transform.GetChild(0).GetChild(i).gameObject;
            mobHpBar.SetActive(false);
            mobHpBars.Push(mobHpBar);
        }*/
        for (int i = 0; i < damageTextsRoot.childCount; i++)
        {
            Animator damageUIAnimator = damageTextsRoot.GetChild(i).GetComponent<Animator>();
            damageUIAnimator.keepAnimatorStateOnDisable = true;
            damageUIAnimator.gameObject.SetActive(false);
            _damageUIs.Push((damageUIAnimator, damageUIAnimator.transform.GetChild(0).GetComponent<TextMeshProUGUI>()));
        }
    }

    public IEnumerator DamageTextUI(Vector3 pos, int damage)
    {
        float time = 0;
        Vector3 lastPosition = pos + (Random.insideUnitSphere * .3f);
        var damageUI = _damageUIs.Pop();
        damageUI.animator.SetTrigger("POP");
        damageUI.animator.gameObject.SetActive(true);
        damageUI.tmp.text = damage.ToString();
        while (true)
        {
            damageUI.animator.transform.position = Camera.main.WorldToScreenPoint(lastPosition);
            yield return null;
            time += Time.deltaTime;
            
            // damageText.gameObject.SetActive(PlayerBody.Local.IsTargetInSight(lastPosition));

            if (time >= 1)
                break;
        }
        damageUI.animator.gameObject.SetActive(false);
        _damageUIs.Push(damageUI);
    }
}
