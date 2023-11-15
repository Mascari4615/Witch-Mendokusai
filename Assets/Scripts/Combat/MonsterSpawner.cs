using System;
using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monsterObjectPrefab;
    private Dungeon _curDungeon;
    [SerializeField] private float tick = .2f;
    [SerializeField] private float spawnRange;

    [SerializeField] private DateTimeVarialbe combatStartTime;

    private Coroutine spawnLoop;
    
    public void StartWave(Dungeon curDungeon)
    {
        this._curDungeon = curDungeon;
        
        if (spawnLoop != null)
            StopCoroutine(spawnLoop);

        spawnLoop = StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            foreach (var combatWave in _curDungeon.MonsterWaves)
                TrySpawnMonster(combatWave);
            yield return new WaitForSeconds(tick * 10);
        }
    }

    private void TrySpawnMonster(MonsterWave monsterWave)
    {
        if (DateTime.Now - combatStartTime.RuntimeValue < TimeSpan.FromSeconds(monsterWave.SpawnTime))
            return;
        
        var o = ObjectManager.Instance.PopObject(monsterObjectPrefab);
        
        Vector3 randomOffset = Random.insideUnitCircle * spawnRange;
        randomOffset.z = randomOffset.y;
        randomOffset.y = 0;
        
        o.transform.position = transform.position + randomOffset;
        
        o.GetComponent<EnemyObject>().Init(monsterWave.Monster);
        o.SetActive(true);
    }

    public void StopLoop()
    {
        if (spawnLoop != null)
            StopCoroutine(spawnLoop);

        spawnLoop = null;
    }
}
