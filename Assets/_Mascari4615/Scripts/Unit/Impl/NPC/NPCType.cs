namespace Mascari4615
{
	// [Flags]
	public enum NPCType
	{
		None = 0,
		Shop = 1 << 0,
		DungeonEntrance = 1 << 1,
		Pot = 1 << 2,
		Anvil = 1 << 3,
		Quest = 1 << 4,
		Furnace = 1 << 5,
	}
}