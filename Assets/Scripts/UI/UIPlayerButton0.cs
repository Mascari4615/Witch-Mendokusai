using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerButton0 : MonoBehaviour
{
    private static readonly int State = Animator.StringToHash("STATE");
    private static readonly int Update = Animator.StringToHash("UPDATE");
    
    [SerializeField] private Animator animator;
    [SerializeField] private BoolVariable canInteract;
    [SerializeField] private BoolVariable isPlayerButton0Down;
    
    public void UpdateIcon()
    {
        animator.SetInteger(State, canInteract.RuntimeValue ? 1 : 0);
        animator.SetTrigger(Update);
    }

    public void Down()
    {
        isPlayerButton0Down.RuntimeValue = true;
    }

    public void Up()
    {
        isPlayerButton0Down.RuntimeValue = false;
    }
}
