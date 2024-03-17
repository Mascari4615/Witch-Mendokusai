using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public class GameManager : Singleton<GameManager>
	{
		protected override void Awake()
		{
			base.Awake();

			SceneManager.sceneLoaded += OnSceneLoaded;
			ClearDungeonObjects();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			ClearDungeonObjects();
		}

		public void ClearDungeonObjects()
		{
			SOManager.Instance.SpawnCircleBuffer.ClearObjects();
			SOManager.Instance.MonsterObjectBuffer.ClearObjects();
			SOManager.Instance.DropsBuffer.ClearObjects();
			SOManager.Instance.SkillObjectBuffer.ClearObjects();
		}
	}
}