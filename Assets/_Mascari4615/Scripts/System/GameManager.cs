using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class GameManager : Singleton<GameManager>
	{
		public List<GameObject> SpawnCircleObjects { get; private set; } = new();
		public List<GameObject> MonsterObjects { get; private set; } = new();
		public List<GameObject> DropObjects { get; private set; } = new();
		public List<GameObject> SkillObjects { get; private set; } = new();
		public List<GameObject> InteractiveObjects { get; private set; } = new();

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
			ReturnObjects(SpawnCircleObjects);
			ReturnObjects(MonsterObjects);
			ReturnObjects(DropObjects);
			ReturnObjects(SkillObjects);

			static void ReturnObjects(List<GameObject> objects)
			{
				for (int i = objects.Count - 1; i >= 0; i--)
					objects[i].SetActive(false);
				objects.Clear();
			}
		}

	}
}