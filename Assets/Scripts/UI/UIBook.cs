using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBook : MonoBehaviour
{
    [SerializeField] private GameObject[] chapters;

    public void SelectChapter(int index)
    {
        for (int i = 0; i < chapters.Length; i++)
        {
            chapters[i].SetActive(i == index);
        }
    }
}
