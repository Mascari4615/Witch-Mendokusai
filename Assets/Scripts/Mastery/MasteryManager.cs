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
    [SerializeField] private MasteryRuntimeSet masterySet;
    [SerializeField] private MasteryRuntimeSet selectMasterySet;
    [SerializeField] private GameObject selectMasteryPanel;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private TextMeshProUGUI stackText;
    private int[] _choices = new int[3];

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


    private void Awake()
    {
             
        selectMasteryPanel.SetActive(false);
        SelectMasteryStack = 0;
    }

    public void Init()
    {
        masterySet.Items.Clear();

        masterySet.Items.AddRange(DataManager.Instance.CurDoll.Masteries);
        masterySet.Items.AddRange(DataManager.Instance.CurStuff(0)!.Masteries);
        masterySet.Items.AddRange(DataManager.Instance.CurStuff(1)!.Masteries);
        masterySet.Items.AddRange(DataManager.Instance.CurStuff(2)!.Masteries);
        
        selectMasteryPanel.SetActive(false);
        SelectMasteryStack = 0;
    }

    public void LevelUp()
    {
        SelectMasteryStack++;

        if (selectMasteryPanel.activeSelf == false)
        {
            ShowMasterys();
            selectMasteryPanel.SetActive(true);
        }
    }

    public void ChooseAbility(int i)
    {
        RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
        // ToolTipManager.Instance.Hide();
        SelectMasteryStack--;
        
        Mastery randomMastery = masterySet.Items[_choices[i]];
        selectMasterySet.Add(randomMastery);
        masterySet.Items.RemoveAt(_choices[i]);

        if (SelectMasteryStack > 0)
        {
            ShowMasterys();
            selectMasteryPanel.SetActive(true);
        }
        else selectMasteryPanel.SetActive(false);
    }

    public void ShowMasterys()
    {
        _choices = new int[] { -1, -1, -1 };
        
        for (int i = 0; i < _choices.Length;)
        {
            int randomIndex = Random.Range(0, masterySet.Items.Count);
            
            if (_choices.Contains(randomIndex))
                continue;
            
            Mastery randomMastery = masterySet.Items[randomIndex];
            buttonImages[i].sprite = randomMastery.Thumbnail;
            texts[i].text = randomMastery.Name;
       
            // toolTipTriggers[i].SetToolTip(randomMasteries[i]);
            
            _choices[i] = randomIndex;
            i++;
        }
    }
}