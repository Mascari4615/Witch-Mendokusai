using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public class LobbyManager : MonoBehaviour
	{
		private void Start()
		{
			Invoke(nameof(TryLogin), 1f);
		}

		private void TryLogin()
		{
			DataManager.Instance.PlayFabManager.Login();
		}

		public void ForDebug()
		{
			UISceneLoading.LoadScene("World");
		}
	}
}