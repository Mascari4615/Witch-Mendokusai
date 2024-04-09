using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public class AudioManager : Singleton<AudioManager>
	{
		public enum BusType
		{
			Master,
			BGM,
			SFX
		}

		private readonly Bus[] buses = new Bus[3];
		private EventInstance sfxVolumeTestEvent;
		private EventInstance bgmEvent;
		private PLAYBACK_STATE pbState;
		private readonly string[] bgmTitles = { "PROTODOME" };
		private int bgmIndex = 0;

		protected override void Awake()
		{
			base.Awake();

			buses[0] = RuntimeManager.GetBus("bus:/");
			buses[1] = RuntimeManager.GetBus("bus:/BGM");
			buses[2] = RuntimeManager.GetBus("bus:/SFX");
			sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXTest");
		}

		private void Start()
		{
			UpdateVolume();
			PlayMusic(bgmTitles[0]);
		}

		private void UpdateVolume()
		{
			buses[0].setVolume(GetVolume(BusType.Master));
			buses[1].setVolume(GetVolume(BusType.BGM));
			buses[2].setVolume(GetVolume(BusType.SFX));
		}

		private void Update()
		{
			// TODO : else if (DataManager.Instance.CurGameData.muteOnOutfocus)
			{
				// TODO : master.setVolume(0);
			}

			// if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				bgmEvent.getPlaybackState(out pbState);

				if (pbState != PLAYBACK_STATE.STOPPED)
					return;

				Debug.Log("BGM End");
				PlayMusic(bgmTitles[bgmIndex = (bgmIndex + 1) % bgmTitles.Length]);
			}
		}

		public void StopMusic() => bgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		public void PlayMusic(string musicName)
		{
			bgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			bgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{musicName}");
			bgmEvent.start();
		}

		public float GetVolume(BusType busType)
		{
			string key = $"Volume{(int)busType}";

			if (!PlayerPrefs.HasKey(key))
				PlayerPrefs.SetFloat(key, 1);

			float volume = PlayerPrefs.GetFloat(key);
			return volume;
		}

		public void SetVolume(BusType busType, float volume)
		{
			var key = $"Volume{(int)busType}";
			PlayerPrefs.SetFloat(key, volume);
			buses[(int)busType].setVolume(volume);

			if (busType == BusType.SFX)
			{
				sfxVolumeTestEvent.getPlaybackState(out var playbackState);
				if (playbackState != PLAYBACK_STATE.PLAYING)
					sfxVolumeTestEvent.start();
			}
		}
	}
}