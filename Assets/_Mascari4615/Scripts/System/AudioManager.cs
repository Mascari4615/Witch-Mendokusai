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

		private readonly Bus[] _buses = new Bus[3];
		private EventInstance _sfxVolumeTestEvent;
		private EventInstance _bgmEvent;
		private PLAYBACK_STATE _pbState;
		private readonly string[] _bgmTitles = { "Happy" };
		private int _i = 0;

		protected override void Awake()
		{
			base.Awake();

			_buses[0] = RuntimeManager.GetBus("bus:/");
			_buses[1] = RuntimeManager.GetBus("bus:/BGM");
			_buses[2] = RuntimeManager.GetBus("bus:/SFX");
			_sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXTest");
		}

		private void Start()
		{
			UpdateVolume();
			PlayMusic(_bgmTitles[0]);
		}

		private void UpdateVolume()
		{
			_buses[0].setVolume(GetVolume(BusType.Master));
			_buses[1].setVolume(GetVolume(BusType.BGM));
			_buses[2].setVolume(GetVolume(BusType.SFX));
		}

		private void Update()
		{
			// TODO : else if (DataManager.Instance.CurGameData.muteOnOutfocus)
			{
				// TODO : 	master.setVolume(0);
			}

			return;

			// if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				_bgmEvent.getPlaybackState(out _pbState);

				if (_pbState != PLAYBACK_STATE.STOPPED)
					return;

				_i = (_i + 1) % _bgmTitles.Length;

				_bgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{_bgmTitles[_i]}");
				_bgmEvent.start();
			}
		}

		public void StopMusic() => _bgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		public void PlayMusic(string musicName)
		{
			_bgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_bgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{musicName}");
			_bgmEvent.start();
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
			_buses[(int)busType].setVolume(volume);

			if (busType == BusType.SFX)
			{
				_sfxVolumeTestEvent.getPlaybackState(out var playbackState);
				if (playbackState != PLAYBACK_STATE.PLAYING)
					_sfxVolumeTestEvent.start();
			}
		}
	}
}