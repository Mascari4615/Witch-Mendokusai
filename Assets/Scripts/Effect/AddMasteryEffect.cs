using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddMasteryEffect), menuName = "Effect/AddMasteryEffect")]
public class AddMasteryEffect : Effect
{
    [SerializeField] private MasteryRuntimeSet masteryDeck;
    [SerializeField] private Mastery targetMastery;
    
    public override void OnEquip()
    {
        masteryDeck.Add(targetMastery);
    }

    public override void OnRemove()
    {
    }
}