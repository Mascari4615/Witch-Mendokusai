using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public enum ObjectBufferType
	{
		SpawnCircle,
		Monster,
		Drop,
		Skill,
		Interactive,
	}

	public class GameManager : Singleton<GameManager>
	{
		public Dictionary<ObjectBufferType, List<GameObject>> ObjectBuffers { get; private set; } = new();

		protected override void Awake()
		{
			base.Awake();

			SceneManager.sceneLoaded += OnSceneLoaded;
			ClearDungeonObjects();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			ClearDungeonObjects();
			TimeManager.Instance.AddCallback(DataManager.Instance.WorkManager.TickEachWorks);
			PlayerController.Instance.PlayerObject.Init(GetDoll(DataManager.Instance.CurDollID));
		}

		public void ClearDungeonObjects()
		{
			foreach (var objectBuffer in ObjectBuffers)
			{
				if (objectBuffer.Key == ObjectBufferType.Interactive)
					continue;

				ReturnObjects(objectBuffer.Value);
			}

			static void ReturnObjects(List<GameObject> objects)
			{
				for (int i = objects.Count - 1; i >= 0; i--)
					objects[i].SetActive(false);
				objects.Clear();
			}
		}

		public void AddObject(ObjectBufferType type, GameObject obj)
		{
			if (!ObjectBuffers.ContainsKey(type))
				ObjectBuffers.Add(type, new List<GameObject>());

			ObjectBuffers[type].Add(obj);
		}

		public void RemoveObject(ObjectBufferType type, GameObject obj)
		{
			if (!ObjectBuffers.ContainsKey(type))
				return;

			ObjectBuffers[type].Remove(obj);
		}

		public GameObject GetNearestObject(ObjectBufferType type, Vector3 position, float maxDistance)
		{
			if (!ObjectBuffers.ContainsKey(type))
				return null;

			GameObject nearest = null;
			float nearestDistance = float.MaxValue;

			foreach (GameObject obj in ObjectBuffers[type])
			{
				float distance = Vector3.Distance(obj.transform.position, position);
				if (distance < nearestDistance)
				{
					nearest = obj;
					nearestDistance = distance;
				}
			}

			return nearestDistance <= maxDistance ? nearest : null;
		}

		public GameObject GetNearestObjectRaycast(ObjectBufferType type, Vector3 position, float maxDistance)
		{
			if (!ObjectBuffers.ContainsKey(type))
				return null;

			GameObject nearest = null;
			float nearestDistance = float.MaxValue;

			foreach (GameObject obj in ObjectBuffers[type])
			{
				float distance = Vector3.Distance(obj.transform.position, position);
				if (distance < nearestDistance)
				{
					nearest = obj;
					nearestDistance = distance;
				}
			}

			if (nearestDistance > maxDistance)
				return null;

			if (Physics.Raycast(position, nearest.transform.position - position, out RaycastHit hit, maxDistance))
			{
				if (hit.collider.gameObject == nearest)
					return nearest;
			}

			return null;
		}
	}
}