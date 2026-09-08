using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

namespace WitchMendokusai.Tests
{
	public sealed class CameraObstructionTest
	{
		private readonly List<GameObject> objects = new();
		private readonly Vector3 target = new(0f, 1000f, 0f);
		private CinemachineComponentDeoccluder extension;
		private CinemachineCamera camera;

		[SetUp]
		public void SetUp()
		{
			GameObject owner = new("Camera obstruction test");
			objects.Add(owner);
			camera = owner.AddComponent<CinemachineCamera>();
			extension = owner.AddComponent<CinemachineComponentDeoccluder>();
		}

		[TearDown]
		public void TearDown()
		{
			foreach (GameObject item in objects)
				Object.DestroyImmediate(item);
			objects.Clear();
		}

		private GameObject Wall(float distance, bool marked = true)
		{
			GameObject wall = new("Wall");
			objects.Add(wall);
			wall.transform.position = target + Vector3.back * distance;
			wall.AddComponent<BoxCollider>().size = new Vector3(10f, 10f, 0.1f);
			if (marked)
				wall.AddComponent<GroundSurface>();
			Physics.SyncTransforms();
			return wall;
		}

		private float Step(float deltaTime)
		{
			CameraState state = CameraState.Default;
			state.ReferenceLookAt = target;
			state.RawPosition = target + Vector3.back * 6f;
			object[] arguments = { camera, CinemachineCore.Stage.Body, state, deltaTime };
			typeof(CinemachineComponentDeoccluder).GetMethod("PostPipelineStageCallback", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(extension, arguments);
			return Vector3.Distance(target, ((CameraState)arguments[2]).RawPosition);
		}

		[Test]
		public void NewWall_ImmediatelyLimitsDistance()
		{
			Step(0.016f);
			Wall(2f);
			Assert.That(Step(0.016f), Is.LessThan(1.96f));
		}

		[Test]
		public void NearWall_SafetyWinsOverMinimumDistance()
		{
			Wall(0.3f);
			Assert.That(Step(-1f), Is.LessThan(0.25f));
		}

		[Test]
		public void RemovedWall_ReturnsGradually()
		{
			GameObject wall = Wall(2f);
			float blocked = Step(-1f);
			wall.SetActive(false);
			Physics.SyncTransforms();
			float released = Step(0.016f);
			Assert.That(released, Is.GreaterThanOrEqualTo(blocked));
			Assert.That(released, Is.LessThan(6f));
		}

		[Test]
		public void UnmarkedCollider_IsNotCameraGeometry()
		{
			Wall(2f, false);
			Assert.That(Step(-1f), Is.EqualTo(6f).Within(0.001f));
		}

		[Test]
		public void LookTargetInsideClimbWall_RetainsCameraOnActorSide()
		{
			GameObject wall = Wall(-0.96f);
			wall.GetComponent<BoxCollider>().size = new Vector3(10f, 10f, 2f);
			Physics.SyncTransforms();
			Assert.That(Step(-1f), Is.GreaterThan(5f));
		}

		[Test]
		public void DenseCast_DoesNotLoseWallBehindUnmarkedColliders()
		{
			for (int index = 0; index < 24; index++)
				Wall(0.3f + index * 0.05f, false);
			Wall(2f);
			Assert.That(Step(-1f), Is.LessThan(1.96f));
		}
	}
}
