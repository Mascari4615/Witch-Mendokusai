using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEquipMsgElement : MonoBehaviour
{
    [SerializeField] private MMF_Player _mmfPlayer;
    [SerializeField] private Slot _slot;

    public void SetAndPop(ItemData item)
    {
        _slot.UpdateUI(item);
        _mmfPlayer.PlayFeedbacks();
    }
}
