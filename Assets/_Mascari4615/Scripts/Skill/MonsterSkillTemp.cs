using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterSkillTemp : MonoBehaviour
{
    [ContextMenu(nameof(Test))]
    void Test()
    {
        Addressables.InstantiateAsync("Assets/Prefab/Bullet.prefab");
    }
}
