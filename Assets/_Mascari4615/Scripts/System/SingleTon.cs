using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mascari4615
{
	[DefaultExecutionOrder(-100)]
	public abstract class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T instance;
		public static T Instance
		{
			get => instance
				? instance
				: FindObjectOfType<T>() ??
				  File.Exists($"Assets/Resources/{nameof(T)}") ? Instantiate(Resources.Load<T>(nameof(T))) : null;
			private set => instance = value;
		}

		[SerializeField] private bool dontDestroyOnLoad = false;

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this as T;

				if (dontDestroyOnLoad)
					DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}