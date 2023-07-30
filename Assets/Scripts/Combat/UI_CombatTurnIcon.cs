using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CombatTurnIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void SetIconSprite(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
}
