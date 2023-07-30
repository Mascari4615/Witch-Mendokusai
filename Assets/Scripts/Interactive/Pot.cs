using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pot : InteractiveObject
{
    public UnityEvent Response;

    public override void Interact()
    {
        Response.Invoke();
    }
}
