using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monsterObjectPrefab;
    [SerializeField] private Enemy targetObject;
    [SerializeField] private float cooltime;
    [SerializeField] private float spawnRange;

    private Coroutine spawnLoop;
    
    public void StartSpawn(Enemy targetObject)
    {
        this.targetObject = targetObject;
        
        if (spawnLoop != null)
            StopCoroutine(spawnLoop);

        spawnLoop = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(cooltime);
        }
    }

    private void SpawnMonster()
    {
        var o = ObjectManager.Instance.PopObject(monsterObjectPrefab);
        o.GetComponent<MonsterObject>().Init(targetObject);

        Vector3 randomOffset = Random.insideUnitCircle * spawnRange;
        randomOffset.z = randomOffset.y;
        randomOffset.y = 0;
        
        o.transform.position = transform.position + randomOffset;
    }
}
