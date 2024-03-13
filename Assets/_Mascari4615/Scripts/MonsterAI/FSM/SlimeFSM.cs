using UnityEngine;

namespace Mascari4615
{
	public class SlimeFSM : StateMachine<TempState>
	{
		private BT_Idle idle;

		private void Awake()
		{
			UnitObject unitObject = GetComponent<UnitObject>();

			idle = new(unitObject);

			SetStateEvent(TempState.Idle, StateEvent.Update, () =>
			{
				idle.Update();
			});
		}

		protected override void Init()
		{
			idle.Init();
		}
	}
}