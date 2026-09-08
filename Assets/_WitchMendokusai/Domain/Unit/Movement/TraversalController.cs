using UnityEngine;

namespace WitchMendokusai
{
	public enum TraversalMode
	{
		None = 0,
		Climb = 1,
		Mantle = 2,
		Glide = 3,
	}

	// 특수 이동의 속도 소유자. 활성 tick은 기본 입력, 중력, 점프 contributor 대신 실행
	public sealed class TraversalController
	{
		private readonly TraversalTuning tuning;
		private readonly TraversalProbe probe;
		private float regrabRemaining;
		private float recoveryRemaining;
		private bool exhausted;
		private bool jumpRequested;
		private Vector3 wallNormal;
		private Collider wall;
		private Vector3 glideDirection;
		private Vector3 mantleTarget;
		private float mantleRemaining;

		public TraversalMode Mode { get; private set; }
		public float Stamina { get; private set; }
		public float StaminaFraction => Stamina / tuning.MaxStamina;
		public Vector2 LocalInput { get; set; }
		public Vector3 Facing { get; set; } = Vector3.forward;
		public bool Unavailable { get; set; }

		public TraversalController(CapsuleCollider capsule, TraversalTuning tuning)
		{
			this.tuning = tuning;
			probe = new TraversalProbe(capsule, tuning);
			Reset();
		}

		public void Reset()
		{
			Cancel();
			Stamina = tuning.MaxStamina;
			exhausted = false;
			recoveryRemaining = 0f;
		}

		public void Cancel()
		{
			Mode = TraversalMode.None;
			wall = null;
			jumpRequested = false;
			regrabRemaining = tuning.RegrabDelay;
		}

		public bool HandleJump(MotorContext context)
		{
			if (tuning.Enabled == false || Unavailable || context.BlockedByExternal)
				return false;
			if (Mode != TraversalMode.None ||
				(context.GroundState == MotorGroundState.Airborne && exhausted == false && probe.GlideClearance(context.Position)))
			{
				jumpRequested = true;
				return true;
			}
			return false;
		}

		public bool Apply(MotorContext context, float deltaTime)
		{
			context.SuppressGrounding = false;
			context.FullBodySweep = false;
			regrabRemaining = Mathf.Max(0f, regrabRemaining - deltaTime);
			recoveryRemaining = Mathf.Max(0f, recoveryRemaining - deltaTime);
			bool grounded = context.GroundState == MotorGroundState.Grounded;
			if (tuning.Enabled == false || Unavailable || context.BlockedByExternal)
			{
				Cancel();
				return false;
			}
			if (grounded && Mode == TraversalMode.Glide)
				Cancel();
			if (grounded && Mode == TraversalMode.None && recoveryRemaining <= 0f)
			{
				Stamina = Mathf.Min(tuning.MaxStamina, Stamina + tuning.RecoveryPerSecond * deltaTime);
				if (Stamina >= Mathf.Min(tuning.RestartStamina, tuning.MaxStamina))
					exhausted = false;
			}

			if (jumpRequested)
			{
				jumpRequested = false;
				if (Mode == TraversalMode.Climb)
				{
					Vector3 launchNormal = wallNormal;
					Cancel();
					if (Spend(tuning.ClimbJumpCost))
					{
						context.Velocity = launchNormal * tuning.ClimbJumpOutSpeed + Vector3.up * tuning.ClimbJumpUpSpeed;
						context.GroundState = MotorGroundState.Airborne;
						return true;
					}
				}
				else if (Mode == TraversalMode.Glide)
				{
					Cancel();
					return false;
				}
				else if (Mode == TraversalMode.None && grounded == false && exhausted == false && probe.GlideClearance(context.Position))
				{
					Mode = TraversalMode.Glide;
					glideDirection = Vector3.ProjectOnPlane(Facing, Vector3.up).normalized;
					if (glideDirection.sqrMagnitude == 0f)
						glideDirection = Vector3.forward;
					context.Velocity.y = Mathf.Min(0f, context.Velocity.y);
				}
			}

			if (Mode == TraversalMode.None && exhausted == false && regrabRemaining <= 0f && LocalInput.y > 0f &&
				probe.Wall(context.Position, context.MoveDirection, out RaycastHit entry))
			{
				Mode = TraversalMode.Climb;
				wallNormal = entry.normal;
				wall = entry.collider;
			}

			if (Mode == TraversalMode.Climb)
			{
				if (wall == null || wall.enabled == false || wall.gameObject.activeInHierarchy == false || Spend(tuning.ClimbCostPerSecond * deltaTime) == false)
				{
					Cancel();
					return false;
				}
				if (LocalInput.y > 0f && probe.Mantle(context.Position, wallNormal, out mantleTarget))
				{
					Mode = TraversalMode.Mantle;
					mantleRemaining = tuning.MantleTimeout;
				}
				else if (probe.Wall(context.Position, -wallNormal, out RaycastHit contact))
				{
					wallNormal = contact.normal;
					wall = contact.collider;
					Vector3 right = Vector3.Cross(wallNormal, Vector3.up).normalized;
					context.Velocity = (right * LocalInput.x + Vector3.up * LocalInput.y) * tuning.ClimbSpeed;
					context.SuppressGrounding = true;
					context.FullBodySweep = true;
					return true;
				}
				else
				{
					Cancel();
					return false;
				}
			}
			if (Mode == TraversalMode.Mantle)
			{
				mantleRemaining -= deltaTime;
				if (mantleRemaining <= 0f || probe.HasRoom(mantleTarget) == false)
				{
					Cancel();
					return false;
				}
				Vector3 target = context.Position.y < mantleTarget.y - tuning.Clearance * 0.5f
					? new Vector3(context.Position.x, mantleTarget.y, context.Position.z) : mantleTarget;
				Vector3 offset = target - context.Position;
				context.Velocity = Vector3.ClampMagnitude(offset / deltaTime, tuning.MantleSpeed);
				context.SuppressGrounding = true;
				context.FullBodySweep = true;
				if (Vector3.Distance(context.Position, mantleTarget) <= tuning.Clearance)
				{
					Cancel();
					context.Velocity = Vector3.zero;
					context.SuppressGrounding = false;
				}
				return true;
			}
			if (Mode == TraversalMode.Glide)
			{
				if (Spend(tuning.GlideCostPerSecond * deltaTime) == false)
				{
					Cancel();
					return false;
				}
				if (context.MoveDirection.sqrMagnitude > 0f)
					glideDirection = Vector3.RotateTowards(glideDirection, context.MoveDirection.normalized, tuning.GlideTurnDegrees * Mathf.Deg2Rad * deltaTime, 0f);
				Vector3 target = glideDirection * tuning.GlideSpeed + Vector3.down * tuning.GlideSinkSpeed;
				context.Velocity = Vector3.MoveTowards(context.Velocity, target, tuning.GlideAcceleration * deltaTime);
				return true;
			}
			return false;
		}

		private bool Spend(float amount)
		{
			recoveryRemaining = tuning.RecoveryDelay;
			Stamina = Mathf.Max(0f, Stamina - amount);
			if (Stamina <= 0f)
				exhausted = true;
			return exhausted == false;
		}
	}
}
