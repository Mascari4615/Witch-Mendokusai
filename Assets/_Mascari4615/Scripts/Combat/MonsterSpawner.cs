using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mascari4615
{
	public class MonsterSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject monsterObjectPrefab;
		[SerializeField] private GameObject spawnCirclePrefab;
		private Dungeon _curDungeon;
		[SerializeField] private float tick = .2f;
		[SerializeField] private float spawnDelay = .2f;
		[SerializeField] private float spawnRange;

		[SerializeField] private DateTimeVarialbe combatStartTime;

		private Coroutine spawnLoop;

		public void StartWave(Dungeon curDungeon)
		{
			Debug.Log(nameof(StartWave));
			this._curDungeon = curDungeon;

			if (spawnLoop != null)
				StopCoroutine(spawnLoop);

			spawnLoop = StartCoroutine(WaveLoop());
		}

		private IEnumerator WaveLoop()
		{
			Debug.Log(nameof(WaveLoop));
			while (true)
			{
				foreach (var combatWave in _curDungeon.MonsterWaves)
					TrySpawnMonster(combatWave);
				yield return new WaitForSeconds(tick * 10);
			}
		}

		private void TrySpawnMonster(MonsterWave monsterWave)
		{
			// Debug.Log(nameof(TrySpawnMonster));
			if (DateTime.Now - combatStartTime.RuntimeValue < TimeSpan.FromSeconds(monsterWave.SpawnTime))
				return;

			StartCoroutine(SpawnMonster(monsterWave.Monster, spawnDelay));
		}

		private IEnumerator SpawnMonster(Enemy monster, float spawnDelay)
		{
			Vector3 randomOffset = Random.insideUnitCircle * spawnRange;
			randomOffset.z = randomOffset.y;
			randomOffset.y = 0;

			Vector3 spawnPos = transform.position + randomOffset;

			GameObject spawnCircle = ObjectManager.Instance.PopObject(spawnCirclePrefab);
			spawnCircle.transform.position = spawnPos;
			spawnCircle.SetActive(true);
			SOManager.Instance.SpawnCircleBuffer.AddItem(spawnCircle);

			yield return new WaitForSeconds(spawnDelay);

			GameObject monsterObject = ObjectManager.Instance.PopObject(monsterObjectPrefab);
			monsterObject.transform.position = spawnPos;
			monsterObject.GetComponent<MonsterObject>().Init(monster);
			monsterObject.SetActive(true);
		}

		public void StopLoop()
		{
			Debug.Log(nameof(StopLoop));

			if (spawnLoop != null)
				StopCoroutine(spawnLoop);
			spawnLoop = null;

			StopAllCoroutines();
		}
	}
}