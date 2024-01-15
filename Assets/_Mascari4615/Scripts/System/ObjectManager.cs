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
			private readonly GameObject _prefab;
			private readonly Stack<GameObject> _stack;

			public ObjectPool(GameObject prefab)
			{
				_prefab = prefab;
				_stack = new Stack<GameObject>();
			}

			public void CreateObject(int count)
			{
				for (var i = 0; i < count; i++)
				{
					var g = Instantiate(_prefab);
					g.SetActive(false);
					Push(g);
				}
			}

			public void Push(GameObject targetObject)
			{
				if (_stack.Contains(targetObject))
				{
					// Debug.Log($"{targetObject.name}, 이미 스택에 존재합니다");
					return;
				}

				_stack.Push(targetObject);
			}

			public GameObject Pop()
			{
				if (_stack.Count == 0)
					CreateObject(5);

				var o = _stack.Pop();
				// o.SetActive(true);

				return o;
			}
		}

		[SerializeField] private Transform objectParent;
		private readonly Dictionary<string, ObjectPool> _objectPoolDic = new Dictionary<string, ObjectPool>();

		public void PushObject(GameObject targetObject)
		{
			var objectName = targetObject.name.Contains("(Clone)")
				? targetObject.name.Remove(targetObject.name.IndexOf("(", StringComparison.Ordinal), 7)
				: targetObject.name;

			if (_objectPoolDic.TryGetValue(objectName, out var value))
			{
			}
			else
			{
				_objectPoolDic[objectName] = new ObjectPool(targetObject);
			}

			_objectPoolDic[objectName].Push(targetObject);
		}

		public GameObject PopObject(GameObject targetObject)
		{
			var objectName = targetObject.name.Contains("(Clone)")
				? targetObject.name.Remove(targetObject.name.IndexOf("(", StringComparison.Ordinal), 7)
				: targetObject.name;

			if (_objectPoolDic.TryGetValue(objectName, out var value))
			{
				return value.Pop();
			}
			else
			{
				_objectPoolDic[objectName] = new ObjectPool(targetObject);
				_objectPoolDic[objectName].CreateObject(1);
				return _objectPoolDic[objectName].Pop();
			}
		}
	}
}