using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rito.BehaviorTree.NodeHelper;
using UnityEngine.AI;
using Node = Rito.BehaviorTree.Node;

namespace Mascari4615
{
	public class Wisp : MonoBehaviour
	{
		[field: Header("_" + nameof(MonsterTemp))]
		[SerializeField] private float sight = 5f;
		[SerializeField] private float attackRange = 5f;

		private Rigidbody rb;
		private MonsterObject monsterObject;
		private SpriteRenderer spriteRenderer;
		private NavMeshAgent agent;

		[SerializeField] private float randomMoveDistance;

		private Vector3 pivot;
		private Vector3 moveDest = Vector3.zero;

		public float stoppingDistance = 0.1f;
		public bool updateRotation = false;
		public float acceleration = 40.0f;
		public float tolerance = 1.0f;
		[SerializeField] private GameObject spawnObject;

		private Node _rootNode;
		[SerializeField] private float tick = .1f;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			monsterObject = GetComponent<MonsterObject>();
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			agent = GetComponent<NavMeshAgent>();

			MakeNode();
		}

		private void Init()
		{
			pivot = transform.position;

			agent.stoppingDistance = stoppingDistance;
			agent.speed = monsterObject.UnitData.MoveSpeed;
			// agent.destination = moveDest;
			agent.updateRotation = updateRotation;
			agent.acceleration = acceleration;
		}

		private void OnEnable()
		{
			Init();
			StartCoroutine(Loop());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private IEnumerator Loop()
		{
			WaitForSeconds waitForTick = new WaitForSeconds(tick);
			while (true)
			{
				_rootNode.Update();
				yield return waitForTick;
			}
		}

		// ParallelNode
		// ó�� ����� �� (IsRunning�� �ƴ� ���¿��� ����� ��), Child List �ʱ�ȭ
		// �ϳ��� Fail�ϰų�, ��� Child�� Success�� �� �� ���� Child List ���� (Running�� �� ����)

		// �������̸� IsRunning State�� ����ϰ�,
		// �ٽ� Root���� ������ �� Ȯ��

		/*private void Update()
		{
		}*/

		/// <summary> BT ��� ���� </summary>
		private void MakeNode()
		{
			_rootNode =
				// True ã�� �� ����
				Selector
				(
					Sequence
					(
						Condition(IsPlayerInAttackRange),

						Condition(IsSkill0Ready),
						Action(UseSkill0)
					),

					// ��ó�� ������ Player���ؼ� Move
					Sequence
					(
						Condition(IsPlayerNear),

						Action(SetDestinationPlayer),
						Action(MoveToDestination),
						Action(UpdateSpriteFlip)
					),

					// ������ RandomMove
					Sequence
					(
						Wait(3),
						Action(SetDestinationRandom),
						Action(MoveToDestination)
					)
				);
		}

		private void SetDestinationPlayer()
		{
			moveDest = PlayerController.Instance.transform.position;
		}

		private void SetDestinationRandom()
		{
			Vector2 random = UnityEngine.Random.insideUnitCircle * randomMoveDistance;
			Vector3 formattedRandom = new(random.x, 0, random.y);
			moveDest = pivot + formattedRandom;
		}

		private void MoveToDestination()
		{
			pivot = transform.position;

			// Vector3 dir = PlayerController.Instance.transform.position - transform.position;
			// dir.Normalize();
			// rb.velocity = dir * enemyObject.UnitData.Speed;

			Vector3 dir = (moveDest - transform.position).normalized;
			agent.destination = transform.position + dir;

			if (agent.pathPending)
			{
				// return State.Running;
			}

			if (agent.remainingDistance < tolerance)
			{
				// return State.Success;
			}

			if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
			{
				// return State.Failure;
			}
		}

		protected bool IsPlayerNear()
		{
			float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
			bool isPlayerNear = distance < sight;

			return isPlayerNear;
		}

		protected bool IsPlayerInAttackRange()
		{
			float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
			bool isPlayerNear = distance < attackRange;

			return isPlayerNear;
		}

		protected bool IsSkill0Ready()
		{
			return monsterObject.UnitSkillHandler.IsReady(0);
		}

		private void UpdateSpriteFlip()
		{
			spriteRenderer.flipX = IsPlayerOnLeft();
		}

		protected bool IsPlayerOnLeft()
		{
			// return PlayerController.Instance.transform.position.x < transform.position.x;

			// HACK
			return Camera.main.WorldToViewportPoint(transform.position).x > .5f;
		}


		protected void SpawnObject()
		{
			ObjectManager.Instance?.PopObject(spawnObject).SetActive(true);
		}

		protected void UseSkill0()
		{
			monsterObject.UseSkill(0);
		}
	}
}