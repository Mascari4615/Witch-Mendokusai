using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Doll), menuName = "Variable/" + nameof(Unit) +"/"+ nameof(Doll))]
	public class Doll : Unit, ISavable<DollData>
	{
		public const int DUMMY_ID = 4444;

		[field: Header("_" + nameof(Doll))]
		[field: SerializeField] public List<EquipmentData> DefaultEquipments { get; private set; }

		[field: NonSerialized] public int Level { get; private set; } = 0;
		[field: NonSerialized] public int Exp { get; private set; } = 0;
		[field: NonSerialized] public List<Guid?> EquipmentGuids { get; private set; } = new() { null, null, null };

		public void Load(DollData dollData)
		{
			Level = dollData.Level;
			Exp = dollData.Exp;
			EquipmentGuids = dollData.EquipmentGuids.ToList();

			if (EquipmentGuids.Count < 3)
				EquipmentGuids.AddRange(new Guid?[3 - EquipmentGuids.Count]);
		}

		public DollData Save()
		{
			return new DollData
			{
				DollID = ID,
				Level = Level,
				Exp = Exp,
				EquipmentGuids = EquipmentGuids.ToList()
			};
		}
	}
}