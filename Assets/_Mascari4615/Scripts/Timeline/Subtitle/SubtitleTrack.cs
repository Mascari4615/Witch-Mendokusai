using UnityEngine;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.Playables;

namespace Mascari4615
{
	[TrackBindingType(typeof(TextMeshProUGUI))]
	[TrackClipType(typeof(SubtitleClip))]
	public class SubtitleTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<SubtitleTackMixer>.Create(graph, inputCount);
		}
	}
}