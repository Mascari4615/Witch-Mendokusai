
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Mascari4615
{
	// https://mentum.tistory.com/657
	[InitializeOnLoad]
	public class EditorStartInit
	{
		static EditorStartInit()
		{
			string pathOfFirstScene = EditorBuildSettings.scenes[0].path;
			SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
			EditorSceneManager.playModeStartScene = sceneAsset;
		}
	}
}