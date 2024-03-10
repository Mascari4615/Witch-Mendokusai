using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public enum ContentType
	{
		Home,
		Combat,
	}

	public enum PlayerState
	{
		Peaceful,
		Interact,
		Combat
	}

	public class GameManager : Singleton<GameManager>
	{
		[SerializeField] private CameraManager cameraManager;
		[SerializeField] private UIManager uiManager;
		public ContentType CurContent { get; private set; } = ContentType.Home;
		public PlayerState CurPlayerState { get; private set; } = PlayerState.Peaceful;


		[SerializeField] private GameObjectBuffer spawnCircleObjectBuffer;
		public GameObjectBuffer SpawnCircleObjectBuffer => spawnCircleObjectBuffer;
		[SerializeField] private GameObjectBuffer monsterObjectBuffer;
		public GameObjectBuffer MonsterObjectBuffer => monsterObjectBuffer;
		[SerializeField] private GameObjectBuffer skillObjectBuffer;
		public GameObjectBuffer SkillObjectBuffer => skillObjectBuffer;

		protected override void Awake()
		{
			base.Awake();
			SceneManager.sceneLoaded += OnSceneLoaded;
			ClearDungeonObjects();
		}
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			spawnCircleObjectBuffer.ClearBuffer();
			monsterObjectBuffer.ClearBuffer();
			SOManager.Instance.DropsBuffer.ClearBuffer();
			skillObjectBuffer.ClearBuffer();
		}

		private void Start()
		{
			SetContent(ContentType.Home);
			SetPlayerState(PlayerState.Peaceful);

			// if (Application.isMobilePlatform)
			// Application.targetFrameRate = 30;
		}

		public void SetContent(int newContent)
		{
			SetContent((ContentType)newContent);
		}

		public void SetContent(ContentType newContent)
		{
			CurContent = newContent;
			// cameraManager.SetCamera((int)CurContent);
			// clickerManager.TryOpenClicker(newContent);
		}

		public void SetPlayerState(int newPlayerState)
		{
			SetPlayerState((PlayerState)newPlayerState);
		}

		public void SetPlayerState(PlayerState newPlayerState)
		{
			CurPlayerState = newPlayerState;
			cameraManager.SetCamera((int)CurPlayerState);
			uiManager.OpenCanvas(CurPlayerState);
		}

		public void ClearDungeonObjects()
		{
			spawnCircleObjectBuffer.ClearObjects();
			monsterObjectBuffer.ClearObjects();
			SOManager.Instance.DropsBuffer.ClearObjects();
			skillObjectBuffer.ClearObjects();
		}
	}
}