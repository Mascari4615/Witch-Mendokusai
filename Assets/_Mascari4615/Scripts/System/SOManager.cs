using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(SOManager), menuName = "SOManager")]
	public class SOManager : ScriptableObject
	{
		[NonSerialized] private static SOManager instance;
		public static SOManager Instance
		{
			get
			{
				if (instance == null)
					instance = Resources.Load(typeof(SOManager).Name) as SOManager;

				return instance;
			}
			private set => instance = value;
		}

		[field: Header("_" + nameof(SOManager))]
		public Dictionary<Type, Dictionary<int, DataSO>> DataSOs { get; private set; } = new();

		[field: Space(10), Header("PlayerData")]
		[field: SerializeField] public Statistics Statistics { get; private set; }
		[field: SerializeField] public FloatVariable InvincibleTime { get; private set; }
		[field: SerializeField] public FloatVariable JoystickX { get; private set; }
		[field: SerializeField] public FloatVariable JoystickY { get; private set; }
		[field: SerializeField] public FloatVariable DashDuration { get; private set; }
		[field: SerializeField] public FloatVariable DashSpeed { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerMoveDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerLookDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerAimDirection { get; private set; }
		[field: SerializeField] public Vector3Variable PlayerAutoAimDirection { get; private set; }
		[field: SerializeField] public BoolVariable IsChatting { get; private set; }
		[field: SerializeField] public BoolVariable IsDashing { get; private set; }
		[field: SerializeField] public BoolVariable IsGround { get; private set; }
		[field: SerializeField] public BoolVariable IsCooling { get; private set; }
		[field: SerializeField] public BoolVariable IsPaused { get; private set; }
		[field: SerializeField] public BoolVariable IsDied { get; private set; }
		[field: SerializeField] public BoolVariable IsMouseOnUI { get; private set; }
		[field: SerializeField] public MonsterObjectVariable LastHitMonsterObject { get; private set; }
		[field: SerializeField] public IntVariable Nyang { get; private set; }
		[field: SerializeField] public IntVariable VQExp { get; private set; }
		[field: SerializeField] public ItemVariable LastEquipedItem { get; private set; }

		[field: Space(10), Header("Buffer")]
		[field: SerializeField] public QuestDataBuffer QuestDataBuffer { get; private set; }
		[field: SerializeField] public QuestBuffer QuestBuffer { get; private set; }
		[field: SerializeField] public DollBuffer DollBuffer { get; private set; }
		[field: SerializeField] public ItemDataBuffer ItemDataBuffer { get; private set; }
		[field: SerializeField] public Inventory ItemInventory { get; private set; }
		[field: SerializeField] public CardBuffer SelectedCardBuffer { get; private set; }
		[field: SerializeField] public QuestData VQuestLoadQuest { get; private set; }

		[field: Space(10), Header(nameof(GameEvent))]
		[field: SerializeField] public GameEvent OnPlayerHit { get; private set; }
		[field: SerializeField] public GameEvent OnPlayerDied { get; private set; }
		[field: SerializeField] public GameEvent OnDungeonStart { get; private set; }
		[field: SerializeField] public GameEvent OnTick { get; private set; }
		[field: SerializeField] public GameEvent OnLevelUp { get; private set; }
	}
}