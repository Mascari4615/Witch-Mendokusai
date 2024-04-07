using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public static class MHelper
	{
		public static bool IsPlaying => (SceneManager.GetActiveScene().isLoaded == false || Application.isPlaying == false) == false;
	}
}