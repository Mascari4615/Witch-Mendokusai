using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public class LobbyManager : MonoBehaviour
	{
		[SerializeField] private GameObject settingPanel;

		private void Start()
		{
			Invoke(nameof(TryLogin), 1f);
		}

		private void TryLogin()
		{
			DataManager.Instance.PlayFabManager.Login();
		}

		public void StartGame()
		{
			Debug.Log(nameof(StartGame));
			UISceneLoading.LoadScene("World");
		}

		public void ExitGame()
		{
			Debug.Log(nameof(ExitGame));

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				settingPanel.SetActive(false);
		}
	}
}