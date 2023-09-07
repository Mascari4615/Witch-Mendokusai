using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MasteryManager : MonoBehaviour
{
    [SerializeField] private MasteryRuntimeSet masteryRuntimeSet;
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;

    [SerializeField] private TextMeshProUGUI stackText;

    // [SerializeField] private ToolTipTrigger[] toolTipTriggers;
    private int _selectMasteryStack;

    private int SelectMasteryStack
    {
        get => _selectMasteryStack;
        set
        {
            _selectMasteryStack = value;
            stackText.text = _selectMasteryStack > 1 ? $"x{_selectMasteryStack}" : string.Empty;
        }
    }

    private readonly Mastery[] randomMasteries = new Mastery[3];

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        selectMasteryPanel.SetActive(false);
        SelectMasteryStack = 0;
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

    public void LevelUp()
    {
        SelectMasteryStack++;

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
        SelectMasteryStack--;

        masteryRuntimeSet.Add(randomMasteries[i]);

        if (SelectMasteryStack > 0)
        {
            Initialize();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }
}