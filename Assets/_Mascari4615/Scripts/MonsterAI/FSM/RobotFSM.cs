using UnityEngine;

namespace Mascari4615
{
	public class RobotFSM : StateMachine<TempState>
	{
		[SerializeField] private float attackRange = 10f;

		private BT_Idle idle;
		private BT_MoveToPlayer attack;

		private void Awake()
		{
			UnitObject unitObject = GetComponent<UnitObject>();

			idle = new(unitObject);
			attack = new(unitObject);

			SetStateEvent(TempState.Idle, StateEvent.Update, () =>
			{
				CanSeePlayer();
				idle.Update();
			});

			SetStateEvent(TempState.Attack, StateEvent.Update, () =>
			{
				// CanSeePlayer();
				attack.Update();
			});
		}

		protected override void Init()
		{
			idle.Init();
		}

		private void CanSeePlayer()
		{
			if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
			{
				if (currentState != TempState.Attack)
					ChangeState(TempState.Attack);
			}
			else
			{
				if (currentState != TempState.Idle)
					ChangeState(TempState.Idle);
			}
		}
	}
}