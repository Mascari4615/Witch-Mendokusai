using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public interface IEffect
	{
		void Apply(EffectInfo effectInfo);
	}

	public class Effect
	{
		public static IEffect GetEffect(EffectType effectType)
		{
			return effectType switch
			{
				EffectType.AddCard => new AddCardEffect(),
				EffectType.AddQuest => new AddQuestEffect(),
				EffectType.AddRandomQuest => new AddRandomQuestEffect(),
				EffectType.FloatVariable => new FloatVariableEffect(),
				EffectType.IntVariable => new IntVariableEffect(),
				EffectType.Item => new ItemEffect(),
				EffectType.SpawnObject => new SpawnObjectEffect(),
				EffectType.Stat => new StatEffect(),
				EffectType.Statistics => new StatisticsEffect(),
				_ => null,
			};
		}

		public static void ApplyEffects(List<EffectInfo> effectInfos)
		{
			foreach (EffectInfo effectInfo in effectInfos)
				ApplyEffect(effectInfo);
		}

		public static void ApplyEffect(EffectInfo effectInfo)
		{
			GetEffect(effectInfo.Type).Apply(effectInfo);
		}
	}
}