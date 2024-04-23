using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Mascari4615
{
	public class SubtitleBehaviour : PlayableBehaviour
	{
		public string subtitleText;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			// TextMeshProUGUI text = playerData as TextMeshProUGUI;
#if UNITY_EDITOR
			TextMeshProUGUI text = Object.FindObjectOfType<UIManager>().CutSceneModule.Subtitle;
#else
        TextMeshProUGUI text = UIManager.Instance.CutSceneModule.Subtitle;
#endif

			text.text = subtitleText;
			text.color = new Color(1, 1, 1, info.weight);
		}
	}
}