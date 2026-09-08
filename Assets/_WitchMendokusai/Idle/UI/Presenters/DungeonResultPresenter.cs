using UnityEngine.UIElements;
using WitchMendokusai.DomainSDK.Idle;
using BigNumberText = WitchMendokusai.Numerics.BigNumberText;

namespace WitchMendokusai.Idle.UI
{
	/// <summary>던전 한 판이 끝났을 때 결과 팝업 (changes/idle-dungeon-run). 돌아온 보상 팝업과 같은 꼴</summary>
	public static class DungeonResultPresenter
	{
		public static void Bind(VisualElement popup, IdleDungeonResult result, UIContentSO content)
		{
			popup.style.display = DisplayStyle.Flex;
			popup.RegisterCallback<PointerDownEvent>(moment => moment.StopPropagation());
			popup.RequireQ<Label>("dungeon-result-title").text = content.DungeonName(result.Kind);
			popup.RequireQ<Label>("dungeon-result-status").text = content.DungeonResultStatusText(result.Cleared);
			popup.RequireQ<Label>("dungeon-result-span").text = content.DescribeSpan(result.SecondsSpent);
			popup.RequireQ<Label>("kills-value").text = content.GainText(BigNumberText.Format(result.Kills));
			popup.RequireQ<Label>("gold-value").text = content.GainText(BigNumberText.Format(result.Gold));
			popup.RequireQ<Label>("shards-value").text = content.GainText(BigNumberText.Format(result.Shards));
			popup.RequireQ<Label>("gear-value").text = content.GainText(BigNumberText.Format(result.Gear));
			popup.RequireQ<Button>("dungeon-result-close").clicked += () => popup.style.display = DisplayStyle.None;
		}
	}
}
