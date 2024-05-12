using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class GameManager : Singleton<GameManager>
	{
		public bool IsChatting { get; set; }
		public bool IsDashing { get; set; }
		public bool IsCooling { get; set; }
		public bool IsDied { get; set; }
		public bool IsMouseOnUI { get; set; }

		// 게임 상태 초기화
		public void Init()
		{
			ObjectBufferManager.Instance.ClearObjects(ObjectType.Drop);
			ObjectBufferManager.Instance.ClearObjects(ObjectType.Monster);
			ObjectBufferManager.Instance.ClearObjects(ObjectType.Skill);
			ObjectBufferManager.Instance.ClearObjects(ObjectType.SpawnCircle);

			Player.Instance.Object.Init(GetDoll(DataManager.Instance.CurDollID));

			List<EquipmentData> equipments = DataManager.Instance.GetEquipmentDatas(DataManager.Instance.CurDollID);
			foreach (EquipmentData equipment in equipments)
			{
				if (equipment == null)
					continue;

				Effect.ApplyEffects(equipment.Effects);

				if (equipment.Object != null)
				{
					GameObject g = ObjectPoolManager.Instance.Spawn(equipment.Object);

					if (g.TryGetComponent(out SkillObject skillObject))
						skillObject.InitContext(Player.Instance.Object);

					g.SetActive(true);
				}
			}
		}
	}
}