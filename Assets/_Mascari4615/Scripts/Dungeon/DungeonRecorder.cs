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
			record.KillCount = SOManager.Instance.GameStat[GameStatType.MONSTER_KILL];
			record.BossKillCount = SOManager.Instance.GameStat[GameStatType.BOSS_KILL];
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