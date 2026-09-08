using UnityEngine.UIElements;

namespace WitchMendokusai
{
	public sealed class TraversalDevMode : IDevMode
	{
		public string Id => "traversal";
		public string DisplayName => "이동 시험";
		public VisualElement Root { get; } = new VisualElement();

		public TraversalDevMode(System.Action enter, System.Action explore)
		{
			Root.Add(new Label("기존 Stage로 진입하는 지상, 등반, 활공 시험 코스"));
			Root.Add(new Button(() => enter()) { text = "시험 코스 입장" });
			Root.Add(new Button(() => explore()) { text = "3분 탐험 입장" });
		}

		public void OnActivate() { }
		public void OnDeactivate() { }
	}
}
