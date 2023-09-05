using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_JRPGTurnIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void SetIconSprite(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
}
