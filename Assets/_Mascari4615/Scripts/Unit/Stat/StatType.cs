namespace Mascari4615
{
	public enum StatType
	{
		// 체력
		HP_CUR = 0,
		HP_MAX = 1,

		// 경험치, 레벨
		EXP_CUR = 100,
		EXP_MAX = 101,
		LEVEL_CUR = 102,

		// 마나
		MANA_CUR = 200,
		MANA_MAX = 201,

		// 이동
		MOVEMENT_SPEED = 300,
		MOVEMENT_SPEED_BONUS = 301,

		// 스킬
		COOLTIME_BONUS = 400,
		ATTACK_SPEED_BONUS = 401,

		// 데미지
		DAMAGE_BONUS = 500,

		// 키워드
		EXP_COLLIDER_SCALE = 10000,
		SATELLITE_COUNT = 10001,
	}
}