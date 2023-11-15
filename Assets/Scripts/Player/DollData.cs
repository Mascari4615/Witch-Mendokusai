using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(DollData), menuName = "Variable/Doll")]
public class DollData : Unit
{
    public Mastery[] Masteries => masteries;
    [SerializeField] private Mastery[] masteries;
}