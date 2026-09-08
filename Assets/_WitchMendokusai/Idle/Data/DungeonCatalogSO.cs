using System;
using System.Collections.Generic;
using UnityEngine;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Idle
{
	/// <summary>던전 4종의 규칙 묶음. TuningSO 가 참조하고 ToTuning 이 코어 배열로 넘김</summary>
	[CreateAssetMenu(fileName = "IdleDungeonCatalog", menuName = "WM/Idle/Dungeon Catalog")]
	public sealed class DungeonCatalogSO : ScriptableObject
	{
		[SerializeField] private List<DungeonSO> dungeons = new List<DungeonSO>();

		public int Count => dungeons.Count;

		/// <summary>그 던전의 SO. 무대가 그림을 고를 때. 없으면 null</summary>
		public DungeonSO DungeonOf(IdleDungeonKind kind)
		{
			for (int index = 0; index < dungeons.Count; index++)
			{
				if (dungeons[index] != null && dungeons[index].Kind == kind)
				{
					return dungeons[index];
				}
			}

			return null;
		}

		public IdleDungeonSpec[] ToDomain()
		{
			IdleDungeonSpec[] specs = new IdleDungeonSpec[dungeons.Count];
			for (int index = 0; index < dungeons.Count; index++)
			{
				DungeonSO dungeon = dungeons[index];
				if (dungeon == null)
				{
					throw new InvalidOperationException("던전 카탈로그 " + index + "번 항목이 비었다.");
				}

				specs[index] = dungeon.ToDomain();
			}

			return specs;
		}

		public bool TryValidate(out string error)
		{
			if (dungeons.Count != IdleDungeons.COUNT)
			{
				error = "dungeons must have " + IdleDungeons.COUNT + " entries";
				return false;
			}

			for (int index = 0; index < dungeons.Count; index++)
			{
				DungeonSO dungeon = dungeons[index];
				if (dungeon == null)
				{
					error = "dungeons contains an empty entry at " + index;
					return false;
				}

				if ((int)dungeon.Kind != index)
				{
					error = "dungeon kind must match its catalog index at " + index;
					return false;
				}
			}

			error = string.Empty;
			return true;
		}
	}
}
