using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryRuntimeSet masteryRuntimeSet;
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    // [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int selectMasteryStack = 0;
    private readonly Mastery[] randomMasteries = new Mastery[3];

    private void Awake()
    {
        selectMasteryPanel.SetActive(false);
    }

    private void Initialize()
    {
        // TODO : 가지고 있는 스킬의 마스터리에 대해서만 작동하도록
        List<Mastery> temp = DataManager.Instance.MasteryBuffer.items.ToList();
        for (int i = 0; i < 3; i++)
        {
            int random = Random.Range(0, temp.Count);
            randomMasteries[i] = temp[random];
            buttonImages[i].sprite = randomMasteries[i].Thumbnail;
            //  toolTipTriggers[i].SetToolTip(randomMasteries[i]);
            temp.RemoveAt(random);
        }
    }

    public void SetSelectMasteryPanelOff() => selectMasteryPanel.SetActive(false);

    public void LevelUp()
    {
        selectMasteryStack++;

        if (selectMasteryPanel.activeSelf == false)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
    }

    public void ChooseAbility(int i)
    {
        RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
        // ToolTipManager.Instance.Hide();
        selectMasteryStack--;

        masteryRuntimeSet.Add(randomMasteries[i]);

        if (selectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}