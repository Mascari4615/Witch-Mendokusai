using UnityEngine;

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

    [SerializeField] private GameObjectRuntimeSet enemyRuntimeSet;
    public GameObjectRuntimeSet EnemyRuntimeSet => enemyRuntimeSet;

    protected override void Awake()
    {
        base.Awake();
        enemyRuntimeSet.Items.Clear();
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
}