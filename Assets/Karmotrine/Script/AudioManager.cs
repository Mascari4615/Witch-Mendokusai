using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	private static AudioManager instance;
	public static AudioManager Instance
	{
		get => instance
			? instance
			: FindObjectOfType<AudioManager>() ?? Instantiate(Resources.Load<AudioManager>("Audio_Manager"));
		private set => instance = value;
	}

	private Bus bgm, sfx, master;
	private EventInstance sfxVolumeTestEvent;
	private EventInstance BgmEvent;
	private PLAYBACK_STATE pbState;
	private string[] bgmTitles = { "yeppSun - ���ſ�", "yeppSun - ������ ��", "yeppSun - �峭�� ���" };
	private string[] bossBgmTitles = { "Badassgatsby - �ν����", "Badassgatsby  - �α׶� ��������", "�߸��� - Wakgood FC" };
	private int i = 0;

	private void Awake()
	{
		if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		bgm = RuntimeManager.GetBus("bus:/BGM");
		sfx = RuntimeManager.GetBus("bus:/SFX");
		master = RuntimeManager.GetBus("bus:/");
		sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXVolumeTest");
	}

	private void Start()
	{
		DataManager.Instance.OnCurGameDataLoad += InitVolume;
	}

	private void InitVolume()
	{
		// TODO : master.setVolume(DataManager.Instance.CurGameData.Volume[0]);
		// TODO : sfx.setVolume(DataManager.Instance.CurGameData.Volume[1]);
		// TODO : bgm.setVolume(DataManager.Instance.CurGameData.Volume[2]);
	}

	private void Update()
	{
		if (DataManager.Instance?.CurGameData != null)
		{
			if (Application.isFocused)
			{
				InitVolume();
			}
			// TODO : else if (DataManager.Instance.CurGameData.muteOnOutfocus)
			{
				// TODO : 	master.setVolume(0);
			}
		}

		//PlayMusic("Main_BGM");
		// BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/Main_BGM");

		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			BgmEvent.getPlaybackState(out pbState);
			if (pbState == PLAYBACK_STATE.STOPPED)
			{
				if (i >= bgmTitles.Length) i = 0;
				//if (UIManager.Instance != null)
				//	UIManager.Instance.SetMusicName(bgmTitles[i]);
				BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{bgmTitles[i++]}");
				if (i >= bgmTitles.Length) i = 0;
				BgmEvent.start();
			}
		}
		else if (SceneManager.GetActiveScene().buildIndex != 0)
		{
			BgmEvent.getPlaybackState(out pbState);
			if (pbState == PLAYBACK_STATE.STOPPED)
			{
				// UIManager.Instance.SetMusicName(bossBgmTitles[StageManager.Instance.currentStageID]);
				BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{bossBgmTitles[0]}");
				//StageManager.Instance.currentStageID
				BgmEvent.start();
			}
		}
	}

	public void StopMusic() => BgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

	public void PlayMusic(string musicName)
	{
		BgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		// if (UIManager.Instance != null) UIManager.Instance.SetMusicName(musicName);
		BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{musicName}");
		BgmEvent.start();
	}

	// TODO : public void MasterVolumeLevel(float newVolume) =>
	// TODO : 	master.setVolume(DataManager.Instance.CurGameData.Volume[0] = newVolume);

	public void SfxVolumeLevel(float newVolume)
	{
		// TODO : sfx.setVolume(DataManager.Instance.CurGameData.Volume[1] = newVolume);

		sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
		if (playbackState != PLAYBACK_STATE.PLAYING)
		{
			sfxVolumeTestEvent.start();
		}
	}

	// TODO : public void BgmVolumeLevel(float newVolume) =>
	// TODO : 	bgm.setVolume(DataManager.Instance.CurGameData.Volume[2] = newVolume);
}