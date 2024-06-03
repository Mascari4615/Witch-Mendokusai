using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public interface IEffect
	{
		void Apply(EffectInfo effectInfo);
	}

	public class Effect
	{
		public static void ApplyEffects(List<EffectInfoData> effectInfoDatas)
		{
			foreach (EffectInfoData effectInfoData in effectInfoDatas)
			{
				int id = effectInfoData.DataSOID;
				EffectType effectType = effectInfoData.Type;
				DataSO dataSO = null;

				switch (effectInfoData.Type)
				{
					case EffectType.AddCard:
						dataSO = GetCardData(id);
						break;
					case EffectType.AddQuest:
						dataSO = GetQuestSO(id);
						break;
					case EffectType.AddRandomVillageQuest:
						effectType = EffectType.AddQuest;
						dataSO = SOManager.Instance.VQuests.Datas[Random.Range(0, SOManager.Instance.VQuests.Datas.Count)];
						break;
					case EffectType.FloatVariable:
						break;
					case EffectType.IntVariable:
						break;
					case EffectType.Item:
						dataSO = GetItemData(id);
						break;
					case EffectType.SpawnObject:
						break;
					case EffectType.UnitStat:
						dataSO = GetStatData(id);
						break;
					case EffectType.GameStat:
						dataSO = GetGameStatData(id);
						break;
					case EffectType.UnlockQuest:
						dataSO = GetQuestSO(id);
						break;
					default:
						break;
				}

				ApplyEffect(new EffectInfo()
				{
					Type = effectType,
					Data = dataSO,
					ArithmeticOperator = effectInfoData.ArithmeticOperator,
					Value = effectInfoData.Value
				});
			}
		}

		public static void ApplyEffects(List<EffectInfo> effectInfos)
		{
			foreach (EffectInfo effectInfo in effectInfos)
				ApplyEffect(effectInfo);
		}

		public static void ApplyEffect(EffectInfo effectInfo)
		{
			IEffect effect = null;

			switch (effectInfo.Type)
			{
				case EffectType.AddCard:
					effect = new AddCardEffect();
					break;
				case EffectType.AddQuest:
				case EffectType.AddRandomVillageQuest:
					effect = new AddQuestEffect();
					break;
				case EffectType.FloatVariable:
					effect = new FloatVariableEffect();
					break;
				case EffectType.IntVariable:
					effect = new IntVariableEffect();
					break;
				case EffectType.Item:
					effect = new ItemEffect();
					break;
				case EffectType.SpawnObject:
					effect = new SpawnObjectEffect();
					break;
				case EffectType.UnitStat:
					effect = new StatEffect();
					break;
				case EffectType.GameStat:
					effect = new GameStatEffect();
					break;
				case EffectType.UnlockQuest:
					effect = new UnlockQuestEffect();
					break;
			}

			if (effect != null)
				effect.Apply(effectInfo);
		}
	}
}