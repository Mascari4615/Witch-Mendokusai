namespace Mascari4615
{
	public class DungeonRecorder
	{
		private DungeonRecord startRecord;

		public DungeonRecorder()
		{
			startRecord = new DungeonRecord();
			SetRecord(ref startRecord);
		}

		private void SetRecord(ref DungeonRecord record)
		{
			record.PlayTime = DungeonManager.Instance.DungeonCurTime;
			record.KillCount = SOManager.Instance.Statistics[StatisticsType.MONSTER_KILLED];
			record.BossKillCount = SOManager.Instance.Statistics[StatisticsType.BOSS_MONSTER_KILLED];
			record.Nyang = SOManager.Instance.Nyang.RuntimeValue;
		}

		public DungeonRecord GetResultRecord()
		{
			DungeonRecord endRecord = new();
			SetRecord(ref endRecord);

			DungeonRecord result = new()
			{
				PlayTime = endRecord.PlayTime - startRecord.PlayTime,
				KillCount = endRecord.KillCount - startRecord.KillCount,
				BossKillCount = endRecord.BossKillCount - startRecord.BossKillCount,
				Nyang = endRecord.Nyang - startRecord.Nyang
			};
			return result;
		}
	}
}