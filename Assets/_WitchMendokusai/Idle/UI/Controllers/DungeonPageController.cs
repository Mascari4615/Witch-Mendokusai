using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using WitchMendokusai.DomainSDK.Idle;
using BigNumberText = WitchMendokusai.Numerics.BigNumberText;

namespace WitchMendokusai.Idle.UI
{
	/// <summary>
	/// 던전 탭 (layout.md 표 3, changes/idle-dungeon-run v2). 던전 4 묶음. 묶음마다 난이도 탭 3 과 스테이지 칸 5, 고른 칸의 보상, 입장과 소탕
	///
	/// ★ 규칙은 한 줄도 없음. 칸의 열림, 깸, 보상은 전부 사진 (DungeonCells) 에서 읽고, 누르면 세션에 보냄
	/// ★ 입장은 판을 연다. 판 도는 동안은 전부 잠김. 소탕은 깬 칸만
	/// </summary>
	public sealed class DungeonPageController
	{
		private sealed class Block
		{
			public IdleDungeonKind Kind;
			public Label Name;
			public Label Ticket;
			public Label Reward;
			public Label Refill;
			public Button Enter;
			public Button Sweep;
			public readonly List<Button> Difficulties = new List<Button>();
			public readonly List<Button> Stages = new List<Button>();
			public int Difficulty;
			public int Stage;
			public bool Picked;
		}

		private readonly IdleSession session;
		private readonly UIContentSO content;
		private readonly Action writeDown;
		private readonly Action requestRender;
		private readonly Action<string, float> showFeedback;
		private readonly float feedbackSeconds;
		private readonly List<Block> blocks = new List<Block>();

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

				Block block = new Block
				{
					Kind = kind,
					Name = made.RequireQ<Label>("dungeon-name"),
					Ticket = made.RequireQ<Label>("dungeon-ticket"),
					Reward = made.RequireQ<Label>("dungeon-reward"),
					Refill = made.RequireQ<Label>("dungeon-refill"),
					Enter = made.RequireQ<Button>("dungeon-enter"),
					Sweep = made.RequireQ<Button>("dungeon-sweep"),
				};

				VisualElement difficulties = made.RequireQ<VisualElement>("dungeon-difficulties");
				for (int difficulty = 0; difficulty < IdleDungeons.DIFFICULTY_COUNT; difficulty++)
				{
					int picked = difficulty;
					Button tab = new Button(() => PickDifficulty(block, picked));
					tab.AddToClassList("idle-dungeon-tab");
					difficulties.Add(tab);
					block.Difficulties.Add(tab);
				}

				VisualElement stages = made.RequireQ<VisualElement>("dungeon-stages");
				for (int stage = 0; stage < IdleDungeons.STAGE_COUNT; stage++)
				{
					int picked = stage;
					Button cell = new Button(() => PickStage(block, picked));
					cell.AddToClassList("idle-dungeon-tab");
					stages.Add(cell);
					block.Stages.Add(cell);
				}

				block.Enter.clicked += () => Enter(block);
				block.Sweep.clicked += () => Sweep(block);
				blocks.Add(block);
			}
		}

		public void Render(IdleSnapshot snapshot)
		{
			string span = content.DescribeSpan(snapshot.TicketRefillSeconds);
			bool idle = snapshot.DungeonRun.Active == false;

			for (int index = 0; index < blocks.Count; index++)
			{
				Block block = blocks[index];
				IdleDungeonKind kind = block.Kind;
				long left = index < snapshot.Tickets.Length ? snapshot.Tickets[index] : 0L;
				bool open = snapshot.DungeonCells[IdleDungeons.CellIndexOf(kind, 0, 0)].Open;

				if (block.Picked == false)
				{
					PickDefault(block, snapshot);
				}

				block.Name.text = content.DungeonName(kind);
				block.Ticket.text = content.DungeonTicketText(left, snapshot.TicketsPerDay);
				block.Refill.text = left < snapshot.TicketsPerDay ? content.DungeonRefillText(span) : string.Empty;
				block.Enter.text = content.DungeonEnterText;
				block.Sweep.text = content.DungeonSweepText(left);

				for (int difficulty = 0; difficulty < block.Difficulties.Count; difficulty++)
				{
					Button tab = block.Difficulties[difficulty];
					IdleDungeonCellView first = snapshot.DungeonCells[IdleDungeons.CellIndexOf(kind, difficulty, 0)];
					tab.text = content.DungeonDifficultyName(difficulty);
					tab.EnableInClassList("idle-dungeon-tab--on", difficulty == block.Difficulty);
					tab.EnableInClassList("idle-dungeon-tab--locked", first.Unlocked == false);
					tab.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
				}

				for (int stage = 0; stage < block.Stages.Count; stage++)
				{
					Button cell = block.Stages[stage];
					IdleDungeonCellView view = snapshot.DungeonCells[IdleDungeons.CellIndexOf(kind, block.Difficulty, stage)];
					cell.text = content.DungeonStageText(stage);
					cell.EnableInClassList("idle-dungeon-tab--on", stage == block.Stage);
					cell.EnableInClassList("idle-dungeon-tab--cleared", view.Cleared);
					cell.EnableInClassList("idle-dungeon-tab--locked", view.Unlocked == false);
					cell.style.display = open && view.Open ? DisplayStyle.Flex : DisplayStyle.None;
				}

				IdleDungeonCellView chosen = snapshot.DungeonCells[IdleDungeons.CellIndexOf(kind, block.Difficulty, block.Stage)];
				block.Reward.text = open == false ? content.DungeonClosedText
					: chosen.Unlocked == false ? content.DungeonLockedText
					: RewardText(chosen);
				block.Enter.SetEnabled(chosen.Unlocked && left > 0L && idle);
				block.Sweep.SetEnabled(chosen.Cleared && left > 0L && idle);
			}
		}

		/// <summary>처음 그릴 때 고르는 칸. 열린 것 중 제일 뒤 (안 깬 첫 칸). 다 깼으면 마지막</summary>
		private static void PickDefault(Block block, IdleSnapshot snapshot)
		{
			block.Picked = true;
			block.Difficulty = 0;
			block.Stage = 0;
			for (int difficulty = 0; difficulty < IdleDungeons.DIFFICULTY_COUNT; difficulty++)
			{
				for (int stage = 0; stage < IdleDungeons.STAGE_COUNT; stage++)
				{
					IdleDungeonCellView view = snapshot.DungeonCells[IdleDungeons.CellIndexOf(block.Kind, difficulty, stage)];
					if (view.Unlocked == false)
					{
						return;
					}

					block.Difficulty = difficulty;
					block.Stage = stage;
				}
			}
		}

		/// <summary>고른 칸을 끝까지 깨면 주는 것 (소탕 한 판 몫). 수치는 사진이 실어 온다</summary>
		private string RewardText(IdleDungeonCellView cell)
		{
			string level = content.DungeonLevelText(cell.Level);
			switch (cell.Kind)
			{
				case IdleDungeonKind.Gold:
					return content.DungeonGoldRewardText(BigNumberText.Format(cell.SweepGold)) + "  " + level;
				case IdleDungeonKind.Boss:
					return content.DungeonBossRewardText(cell.Shards, cell.GearCount, cell.GearTier) + "  " + level;
				case IdleDungeonKind.Gear:
					return content.DungeonGearRewardText(cell.GearCount * (cell.Waves > 0 ? cell.Waves : 1), cell.GearTier) + "  " + level;
				default:
					return content.DungeonClosedText;
			}
		}

		private void PickDifficulty(Block block, int difficulty)
		{
			block.Difficulty = difficulty;
			block.Stage = 0;
			requestRender();
		}

		private void PickStage(Block block, int stage)
		{
			block.Stage = stage;
			requestRender();
		}

		/// <summary>판 열기. 보상은 안에서 싸워 얻고 결과는 판이 끝날 때 팝업</summary>
		private void Enter(Block block)
		{
			if (session.TryEnterDungeon(block.Kind, block.Difficulty, block.Stage))
			{
				writeDown();
				requestRender();
			}
		}

		private void Sweep(Block block)
		{
			if (session.TrySweepDungeon(block.Kind, block.Difficulty, block.Stage, out IdleDungeonReward reward))
			{
				Say(block, reward);
			}
		}

		private void Say(Block block, IdleDungeonReward reward)
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

			showFeedback(content.DungeonFeedbackText(content.DungeonCellText(reward.Kind, block.Difficulty, block.Stage), reward.Runs, got), feedbackSeconds);
			writeDown();
			requestRender();
		}
	}
}
