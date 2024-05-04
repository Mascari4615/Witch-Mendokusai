using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum ObjectType
	{
		SpawnCircle,
		Monster,
		Drop,
		Skill,
		Interactive,
	}

	public class ObjectBufferManager : Singleton<ObjectBufferManager>
	{
		private readonly Dictionary<ObjectType, List<GameObject>> bufferDic = new()
		{
			{ ObjectType.SpawnCircle, new List<GameObject>() },
			{ ObjectType.Monster, new List<GameObject>() },
			{ ObjectType.Drop, new List<GameObject>() },
			{ ObjectType.Skill, new List<GameObject>() },
			{ ObjectType.Interactive, new List<GameObject>() },
		};

		public void AddObject(ObjectType type, GameObject obj)
		{
			bufferDic[type].Add(obj);
		}

		public void RemoveObject(ObjectType type, GameObject obj)
		{
			bufferDic[type].Remove(obj);
		}

		public void ClearObjects(ObjectType type)
		{
			foreach (GameObject obj in bufferDic[type])
				obj.SetActive(false);

			bufferDic[type].Clear();
		}

		public GameObject GetNearestObject(ObjectType type, Vector3 position, float maxDistance)
		{
			GameObject nearest = null;
			float nearestDistance = float.MaxValue;

			foreach (GameObject obj in bufferDic[type])
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

		public List<GameObject> GetObjectsWithDistance(ObjectType type, Vector3 position, float maxDistance)
		{
			List<GameObject> targetObjects = new();

			foreach (GameObject obj in bufferDic[type])
			{
				float distance = Vector3.Distance(obj.transform.position, position);
				if (distance < maxDistance)
					targetObjects.Add(obj);
			}

			return targetObjects;
		}
	}
}