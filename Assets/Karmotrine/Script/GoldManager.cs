using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karmotrine
{
    public class GoldManager : MonoBehaviour
    {
        public static GoldManager Instance => instance;
        private static GoldManager instance;
        
        public IntVariable gold;
        public IntVariable goldPerClick;
        public IntVariable goldPerSec;

        private void Awake()
        {
            instance = this;
            StartCoroutine(nameof(GoldLoop));
        }

        private IEnumerator GoldLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                gold.RuntimeValue += goldPerSec.RuntimeValue;
                // Debug.Log(goldPerSec.RuntimeValue.ToString());
            }
        }

        public void SetGold(int amount) => gold.RuntimeValue = amount;
        public void AddGold(int amount) => gold.RuntimeValue += amount;
        public void SubGold(int amount) => gold.RuntimeValue -= amount;
        
        public void SetGoldPerClick(int amount) => goldPerClick.RuntimeValue = amount;
        public void AddGoldPerClick(int amount) => goldPerClick.RuntimeValue += amount;
        public void SubGoldPerClick(int amount) => goldPerClick.RuntimeValue -= amount;
        
        public void SetGoldPerSec(int amount) => goldPerSec.RuntimeValue = amount;
        public void AddGoldPerSec(int amount) => goldPerSec.RuntimeValue += amount;
        public void SubGoldPerSec(int amount) => goldPerSec.RuntimeValue -= amount;
        
        
    }
}
