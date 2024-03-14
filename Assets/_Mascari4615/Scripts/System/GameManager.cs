using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public enum PlayerState
	{
		Peaceful,
		Interact,
		Dungeon
	}

	public class GameManager : Singleton<GameManager>
	{
		[SerializeField] private UIManager uiManager;
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
			SetPlayerState(PlayerState.Peaceful);

			// if (Application.isMobilePlatform)
			// Application.targetFrameRate = 30;
		}

		public void SetPlayerState(int newPlayerState)
		{
			SetPlayerState((PlayerState)newPlayerState);
		}

		public void SetPlayerState(PlayerState newPlayerState)
		{
			CurPlayerState = newPlayerState;
			CameraManager.Instance.SetCamera((int)CurPlayerState);
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