namespace WitchMendokusai.DomainSDK.Idle
{
    /// <summary>
    /// 전장 하나를 가리키는 손잡이 (changes/idle-dungeon-run v2). 본판과 던전이 각각 하나씩
    ///
    /// ★ 본판은 상태의 배열을 그대로 가리킴 (저장되는 체력). 던전은 판이 가진 자기 배열
    ///   (입장 때 만렙, 본판과 무관. 사용자 2026-09-08). 시뮬은 어느 쪽인지 모른 채 이 손잡이로만 읽고 씀
    /// ★ 상태 배열은 EnsureSeatRoom 과 Load 가 새로 만들 수 있어 붙들지 않고 부를 때마다 다시 잡음
    /// </summary>
    public readonly struct IdleArena
    {
        public IdleArena(IdleBattle battle, double[] seatHealth, double[] seatReviveSeconds, bool dungeon)
        {
            Battle = battle;
            SeatHealth = seatHealth;
            SeatReviveSeconds = seatReviveSeconds;
            Dungeon = dungeon;
        }

        public IdleBattle Battle { get; }

        public double[] SeatHealth { get; }

        public double[] SeatReviveSeconds { get; }

        /// <summary>참이면 던전 판. 적 세기와 처치 규칙이 던전 스펙을 따름</summary>
        public bool Dungeon { get; }
    }
}
