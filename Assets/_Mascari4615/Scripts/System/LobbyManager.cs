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
			PlayFabManager.Instance.Login();
		}

		public void ForDebug()
		{
			SceneManager.LoadScene(1);
		}
	}
}