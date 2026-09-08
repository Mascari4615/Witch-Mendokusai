using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using WitchMendokusai.DomainSDK.Idle;
using BigNumberText = WitchMendokusai.Numerics.BigNumberText;

namespace WitchMendokusai.Idle.UI
{
	/// <summary>
	/// 던전 탭 (layout.md 표 3). 4종 줄. 줄마다 보상, 입장권 n/n, 입장, 소탕
	///
	/// ★ 규칙은 한 줄도 없음. 보상 수치와 여닫힘은 전부 사진에서 읽고, 누르면 세션에 보냄
	/// ★ 입장은 판을 연다 (changes/idle-dungeon-run). 판 도는 동안은 둘 다 잠김. 소탕은 한 번 깬 던전만
	/// </summary>
	public sealed class DungeonPageController
	{
		private sealed class Row
		{
			public Label Name;
			public Label Ticket;
			public Label Reward;
			public Label Refill;
			public Button Enter;
			public Button Sweep;
		}

		private readonly IdleSession session;
		private readonly UIContentSO content;
		private readonly Action writeDown;
		private readonly Action requestRender;
		private readonly Action<string, float> showFeedback;
		private readonly float feedbackSeconds;
		private readonly List<Row> rows = new List<Row>();

		public DungeonPageController(VisualElement page, IdleSession session, UIContentSO content,
			VisualTreeAsset rowAsset, Action writeDown, Action requestRender,
			Action<string, float> showFeedback, float feedbackSeconds)
		{
			this.session = session;
			this.content = content;
			this.writeDown = writeDown;
			this.requestRender = requestRender;
			this.showFeedback = showFeedback;
			this.feedbackSeconds = feedbackSeconds;

			VisualElement host = page.RequireQ<VisualElement>("dungeon-rows");
			for (int index = 0; index < IdleDungeons.COUNT; index++)
			{
				IdleDungeonKind kind = (IdleDungeonKind)index;
				TemplateContainer tree = rowAsset.Instantiate();
				VisualElement made = tree.RequireQ<VisualElement>("dungeon-row");
				made.RemoveFromHierarchy();
				host.Add(made);

				Row row = new Row
				{
					Name = made.RequireQ<Label>("dungeon-name"),
					Ticket = made.RequireQ<Label>("dungeon-ticket"),
					Reward = made.RequireQ<Label>("dungeon-reward"),
					Refill = made.RequireQ<Label>("dungeon-refill"),
					Enter = made.RequireQ<Button>("dungeon-enter"),
					Sweep = made.RequireQ<Button>("dungeon-sweep"),
				};
				row.Enter.clicked += () => Enter(kind);
				row.Sweep.clicked += () => Sweep(kind);
				rows.Add(row);
			}
		}

		public void Render(IdleSnapshot snapshot)
		{
			string span = content.DescribeSpan(snapshot.TicketRefillSeconds);

			for (int index = 0; index < rows.Count; index++)
			{
				IdleDungeonKind kind = (IdleDungeonKind)index;
				Row row = rows[index];
				long left = index < snapshot.Tickets.Length ? snapshot.Tickets[index] : 0L;
				IdleDungeonSpecView spec = snapshot.DungeonSpecs[index];
				bool idle = snapshot.DungeonRun.Active == false;

				row.Name.text = content.DungeonName(kind);
				row.Ticket.text = content.DungeonTicketText(left, snapshot.TicketsPerDay);
				row.Reward.text = spec.Open ? RewardText(spec, snapshot.DungeonGearTier) : content.DungeonClosedText;
				row.Refill.text = left < snapshot.TicketsPerDay ? content.DungeonRefillText(span) : string.Empty;
				row.Enter.text = content.DungeonEnterText;
				row.Sweep.text = content.DungeonSweepText(left);
				row.Enter.SetEnabled(spec.Open && left > 0L && idle);
				row.Sweep.SetEnabled(spec.Open && left > 0L && idle && spec.Cleared);
			}
		}

		/// <summary>그 던전을 끝까지 깨면 주는 것 (소탕 한 판 몫). 수치는 사진이 실어 온다</summary>
		private string RewardText(IdleDungeonSpecView spec, int tier)
		{
			switch (spec.Kind)
			{
				case IdleDungeonKind.Gold:
					return content.DungeonGoldRewardText(BigNumberText.Format(spec.SweepGold));
				case IdleDungeonKind.Boss:
					return content.DungeonBossRewardText(spec.Shards, spec.GearCount, tier);
				case IdleDungeonKind.Gear:
					return content.DungeonGearRewardText(spec.GearCount * (spec.Waves > 0 ? spec.Waves : 1), tier);
				default:
					return content.DungeonClosedText;
			}
		}

		/// <summary>판 열기. 보상은 안에서 싸워 얻고 결과는 판이 끝날 때 팝업</summary>
		private void Enter(IdleDungeonKind kind)
		{
			if (session.TryEnterDungeon(kind))
			{
				writeDown();
				requestRender();
			}
		}

		private void Sweep(IdleDungeonKind kind)
		{
			if (session.TrySweepDungeon(kind, out IdleDungeonReward reward))
			{
				Say(reward);
			}
		}

		private void Say(IdleDungeonReward reward)
		{
			string got;
			switch (reward.Kind)
			{
				case IdleDungeonKind.Gold:
					got = content.DungeonGoldRewardText(BigNumberText.Format(reward.Gold));
					break;
				case IdleDungeonKind.Boss:
					got = content.DungeonBossRewardText(reward.Shards, reward.Gear, 0);
					break;
				default:
					got = content.DungeonGearRewardText(reward.Gear, 0);
					break;
			}

			showFeedback(content.DungeonFeedbackText(content.DungeonName(reward.Kind), reward.Runs, got), feedbackSeconds);
			writeDown();
			requestRender();
		}
	}
}
