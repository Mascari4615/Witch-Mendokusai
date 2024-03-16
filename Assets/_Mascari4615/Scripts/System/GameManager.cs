using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public enum GameContent
	{
		None,
		Dungeon,
	}

	public class GameManager : Singleton<GameManager>
	{
		public GameContent LastContent { get; private set; } = GameContent.None;
		public GameContent CurContent { get; private set; } = GameContent.None;

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

		private void Start()
		{
			SetContent(GameContent.None);
		}

		public void SetContent(GameContent newContent)
		{
			CurContent = newContent;
			Debug.Log($"SetContent: {LastContent} -> {CurContent}");
			CameraManager.Instance.SetCamera(CurContent);
			UIManager.Instance.SetContentUI(CurContent);
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