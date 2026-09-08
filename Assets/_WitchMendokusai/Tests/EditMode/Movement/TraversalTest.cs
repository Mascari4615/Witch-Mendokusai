using NUnit.Framework;
using UnityEngine;

namespace WitchMendokusai.Tests
{
	public sealed class TraversalTest
	{
		private MotorTestHarness harness;
		private TraversalController traversal;

		private void Create(Vector3 position)
		{
			harness = new MotorTestHarness(position);
			harness.Capsule.height = 1f;
			harness.Capsule.radius = 0.2f;
			harness.Capsule.center = Vector3.up * 0.5f;
			TraversalTuning tuning = JsonUtility.FromJson<TraversalTuning>("{\"<Enabled>k__BackingField\":true}");
			traversal = new TraversalController(harness.Capsule, tuning);
			harness.Motor.OverrideVelocity = traversal.Apply;
			harness.AddContributor(new ConstantHorizontalContributor(harness));
			harness.AddContributor(new GravityContributor());
		}

		[TearDown]
		public void TearDown() => harness?.Dispose();

		private GameObject Wall(bool climbable = true)
		{
			harness.AddGround(new Vector3(0f, -0.5f, 0f), new Vector3(20f, 1f, 20f));
			GameObject wall = harness.AddGround(new Vector3(0f, 3f, 2f), new Vector3(8f, 6f, 2f));
			if (climbable)
				wall.AddComponent<ClimbableSurface>();
			harness.SetHorizontalIntent(Vector3.forward, 3f);
			traversal.LocalInput = Vector2.up;
			return wall;
		}

		[Test]
		public void MarkedWall_ClimbsAndMantlesOntoTop()
		{
			Create(Vector3.zero);
			Wall();
			bool climbed = false;
			bool mantled = false;
			for (int i = 0; i < 240; i++)
			{
				harness.Step();
				climbed |= traversal.Mode == TraversalMode.Climb;
				mantled |= traversal.Mode == TraversalMode.Mantle;
				if (mantled && traversal.Mode == TraversalMode.None)
				{
					harness.SetHorizontalIntent(Vector3.zero, 0f);
					traversal.LocalInput = Vector2.zero;
				}
			}
			Assert.That(climbed, Is.True);
			Assert.That(mantled, Is.True);
			Assert.That(harness.Position.y, Is.EqualTo(6f).Within(0.08f));
			Assert.That(harness.IsGrounded, Is.True);
			Assert.That(harness.IsOverlappingGeometry(), Is.False);
		}

		[Test]
		public void UnmarkedWall_DoesNotClimb()
		{
			Create(Vector3.zero);
			Wall(false);
			harness.StepMany(120);
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
			Assert.That(harness.Position.y, Is.LessThan(0.1f));
		}

		[Test]
		public void Ceiling_BlocksMantleAndFullBodyAscent()
		{
			Create(Vector3.zero);
			Wall();
			harness.AddGround(new Vector3(0f, 6.4f, 1f), new Vector3(6f, 0.4f, 5f));
			harness.StepMany(240);
			Assert.That(harness.Position.y, Is.LessThan(5.25f));
			Assert.That(traversal.Mode, Is.Not.EqualTo(TraversalMode.Mantle));
		}

		[Test]
		public void Release_PreventsImmediateRegrab()
		{
			Create(Vector3.zero);
			Wall();
			harness.StepMany(60);
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.Climb));
			traversal.Cancel();
			harness.Step();
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
		}

		[Test]
		public void RemovedWall_Detaches()
		{
			Create(Vector3.zero);
			GameObject wall = Wall();
			harness.StepMany(60);
			wall.SetActive(false);
			harness.Step();
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
		}

		[Test]
		public void Glide_ControlsSinkAndCanFold()
		{
			Create(Vector3.up * 20f);
			harness.Step();
			Assert.That(traversal.HandleJump(harness.Context), Is.True);
			harness.SetHorizontalIntent(Vector3.forward, 0f);
			harness.StepMany(100);
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.Glide));
			Assert.That(harness.Context.Velocity.y, Is.EqualTo(-1.5f).Within(0.01f));
			Assert.That(harness.Position.z, Is.GreaterThan(8f));
			Assert.That(traversal.Stamina, Is.EqualTo(84f).Within(0.05f));
			traversal.HandleJump(harness.Context);
			harness.Step();
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
			Assert.That(harness.Context.Velocity.y, Is.LessThan(-1.5f));
		}

		[Test]
		public void Exhaustion_DropsAndCannotReopenInAir()
		{
			Create(Vector3.up * 100f);
			harness.Step();
			traversal.HandleJump(harness.Context);
			harness.StepMany(640);
			Assert.That(traversal.Stamina, Is.Zero);
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
			Assert.That(traversal.HandleJump(harness.Context), Is.False);
		}

		[Test]
		public void InputBlock_CancelsGlideAndDoesNotResume()
		{
			Create(Vector3.up * 20f);
			harness.Step();
			traversal.HandleJump(harness.Context);
			harness.Step();
			harness.Context.BlockedByExternal = true;
			harness.Step();
			harness.Context.BlockedByExternal = false;
			harness.Step();
			Assert.That(traversal.Mode, Is.EqualTo(TraversalMode.None));
		}

		[Test]
		public void Teleport_ClearsVelocityAndGroundPolicy()
		{
			Create(Vector3.zero);
			harness.Context.Velocity = Vector3.up * 10f;
			harness.Context.SuppressGrounding = true;
			harness.Motor.Teleport(Vector3.one * 5f);
			Assert.That(harness.Context.Position, Is.EqualTo(Vector3.one * 5f));
			Assert.That(harness.Context.Velocity, Is.EqualTo(Vector3.zero));
			Assert.That(harness.Context.SuppressGrounding, Is.False);
		}
	}
}
