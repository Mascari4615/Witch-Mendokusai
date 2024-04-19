using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class ObjectManager : Singleton<ObjectManager>
	{
		private class ObjectPool
		{
			private readonly GameObject prefab;
			private readonly Stack<GameObject> stack;

			public ObjectPool(GameObject prefab)
			{
				this.prefab = prefab;
				stack = new();
			}

			public void CreateObject(int count)
			{
				for (var i = 0; i < count; i++)
				{
					GameObject g = Instantiate(prefab);
					g.SetActive(false);
					Push(g);
				}
			}

			public void Push(GameObject targetObject)
			{
				if (stack.Contains(targetObject))
				{
					// Debug.Log($"{targetObject.name}, 이미 스택에 존재합니다");
					return;
				}

				stack.Push(targetObject);
			}

			public GameObject Pop()
			{
				if (stack.Count == 0)
					CreateObject(5);

				GameObject o = stack.Pop();
				// o.SetActive(true);

				return o;
			}
		}

		[SerializeField] private Transform objectParent;
		private readonly Dictionary<string, ObjectPool> objectPoolDic = new();

		public void PushObject(GameObject targetObject)
		{
			// Debug.Log($"PushObject: {targetObject.name}");
			string objectName = targetObject.name.Contains("(Clone)")
				? targetObject.name.Remove(targetObject.name.IndexOf("(", StringComparison.Ordinal), 7)
				: targetObject.name;

			if (objectPoolDic.ContainsKey(objectName) == false)
				objectPoolDic[objectName] = new ObjectPool(targetObject);

			objectPoolDic[objectName].Push(targetObject);
		}

		public GameObject PopObject(GameObject targetObject)
		{
			// Debug.Log($"PopObject: {targetObject.name}");
			string objectName = targetObject.name.Contains("(Clone)")
				? targetObject.name.Remove(targetObject.name.IndexOf("(", StringComparison.Ordinal), 7)
				: targetObject.name;

			if (objectPoolDic.TryGetValue(objectName, out ObjectPool pool))
			{
				return pool.Pop();
			}
			else
			{
				objectPoolDic[objectName] = new ObjectPool(targetObject);
				objectPoolDic[objectName].CreateObject(1);
				return objectPoolDic[objectName].Pop();
			}
		}
	}
}