using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayerObject : UnitObject, IHitable
{
    [SerializeField] private IntVariable curHp;
    
    protected override void OnReceiveAttack(int damage)
    {
        base.OnReceiveAttack(damage);
        
        curHp.RuntimeValue = CurHp;
        hpBar.localScale = new Vector3((float)CurHp / unitData.MaxHp, 1, 1);

        if (!IsAlive)
        {
            return;
        }


        // Rigidbody2D.velocity = Vector3.zero;

        /*
        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>()
            .SetText(damage.ToString(), hitType);
        ObjectManager.Instance.PopObject("Effect_Hit",
            transform.position + (Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f));*/

        /*
        if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[36]))
        {
            if ((float)hp / MaxHp <= 0.1f * DataManager.Instance.wgItemInven.itemCountDic[36])
            {
                ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up)
                    .GetComponent<AnimatedText>().SetText("처형", Color.red);
                RuntimeManager.PlayOneShot(hurtSFX, transform.position);
                StopAllCoroutines();
                Collapse();
                return;
            }
        }
        */

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());

        RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);

        switch (CurHp)
        {
            case > 0:
                // Animator.SetTrigger("AHYA");
                break;
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material.SetFloat("_Emission", 1);
        yield return new WaitForSeconds(.1f);
        spriteRenderer.material.SetFloat("_Emission", 0);
        flashRoutine = null;
    }

    private Coroutine flashRoutine;
    [SerializeField] private Transform hpBar;
}