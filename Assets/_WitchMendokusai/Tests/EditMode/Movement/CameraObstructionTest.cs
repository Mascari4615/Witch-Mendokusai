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
		private Material fadeTestMaterial;

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
			{
				CameraFadeObstacle fade = item.GetComponent<CameraFadeObstacle>();
				if (fade != null)
					typeof(CameraFadeObstacle).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(fade, null);
				Object.DestroyImmediate(item);
			}
			objects.Clear();
			if (fadeTestMaterial != null)
				Object.DestroyImmediate(fadeTestMaterial);
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

		private float Step(float deltaTime, float yaw = 0f)
		{
			CameraState state = CameraState.Default;
			state.ReferenceLookAt = target;
			state.RawPosition = target + Quaternion.Euler(0f, yaw, 0f) * Vector3.back * 6f;
			object[] arguments = { camera, CinemachineCore.Stage.Body, state, deltaTime };
			typeof(CinemachineComponentDeoccluder).GetMethod("PostPipelineStageCallback", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(extension, arguments);
			return Vector3.Distance(target, ((CameraState)arguments[2]).RawPosition);
		}

		[TestCase(30)]
		[TestCase(60)]
		[TestCase(120)]
		public void OrbitPastPillar_SpreadsPullBeforeContact(int frameRate)
		{
			GameObject pillar = Wall(2f);
			pillar.GetComponent<BoxCollider>().size = new Vector3(0.5f, 10f, 0.5f);
			Physics.SyncTransforms();
			float previous = Step(-1f, -50f);
			float maximumPull = 0f;
			for (int index = 1; index <= frameRate * 100 / 30; index++)
			{
				float distance = Step(1f / frameRate, -50f + index * 30f / frameRate);
				maximumPull = Mathf.Max(maximumPull, previous - distance);
				previous = distance;
			}
			TestContext.WriteLine($"Maximum pull per frame: {maximumPull}");
			Assert.That(maximumPull, Is.LessThan(0.75f));
		}

		private CameraFadeObstacle FadePillar(string shaderName = "Universal Render Pipeline/Lit")
		{
			GameObject pillar = Wall(2f);
			pillar.GetComponent<BoxCollider>().size = new Vector3(0.5f, 10f, 0.5f);
			fadeTestMaterial = new Material(Shader.Find(shaderName));
			pillar.AddComponent<MeshRenderer>().sharedMaterial = fadeTestMaterial;
			Physics.SyncTransforms();
			CameraFadeObstacle fade = pillar.AddComponent<CameraFadeObstacle>();
			// 일반 MonoBehaviour 생명주기는 EditMode에서 자동 실행 없음
			typeof(CameraFadeObstacle).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(fade, null);
			return fade;
		}

		[TestCase("Universal Render Pipeline/Lit")]
		[TestCase("Universal Render Pipeline/Unlit")]
		public void FadePillar_RetainsDistanceAndOriginalMaterial(string shaderName)
		{
			CameraFadeObstacle pillar = FadePillar(shaderName);
			Assert.That(Step(-1f), Is.EqualTo(6f).Within(0.001f));
			Assert.That(pillar.Opacity, Is.EqualTo(0.15f).Within(0.001f));
			Material rendered = pillar.GetComponent<MeshRenderer>().sharedMaterial;
			Assert.That(rendered, Is.Not.SameAs(fadeTestMaterial));
			Assert.That(rendered.GetFloat("_Surface"), Is.EqualTo(1f));
			Assert.That(rendered.GetFloat("_ZWrite"), Is.Zero);
			Assert.That(rendered.GetColor("_BaseColor").a, Is.EqualTo(0.15f).Within(0.001f));
			Assert.That(fadeTestMaterial.GetFloat("_Surface"), Is.Zero);
			Assert.That(pillar.GetComponent<Collider>().enabled, Is.True);
		}

		[Test]
		public void DisabledFade_RestoresMaterialAndSafety()
		{
			CameraFadeObstacle pillar = FadePillar();
			Step(-1f);
			pillar.enabled = false;
			typeof(CameraFadeObstacle).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pillar, null);
			Assert.That(pillar.GetComponent<MeshRenderer>().sharedMaterial, Is.SameAs(fadeTestMaterial));
			Assert.That(pillar.Opacity, Is.EqualTo(1f));
			Assert.That(Step(-1f), Is.LessThan(2f));
		}

		[Test]
		public void FadePillar_DoesNotIgnoreWallBehindIt()
		{
			FadePillar();
			Wall(4f);
			Assert.That(Step(-1f), Is.InRange(3f, 4f));
		}

		[Test]
		public void UnsupportedFade_StillPullsCamera()
		{
			GameObject pillar = Wall(2f);
			pillar.AddComponent<MeshRenderer>();
			CameraFadeObstacle fade = pillar.AddComponent<CameraFadeObstacle>();
			Assert.That(fade.CanFade, Is.False);
			Assert.That(Step(-1f), Is.LessThan(2f));
		}

		[Test]
		public void FadeOrbit_DoesNotPullAtAnyPillarAngle()
		{
			FadePillar();
			Step(-1f, -50f);
			for (int index = 0; index <= 200; index++)
				Assert.That(Step(1f / 60f, -50f + index * 0.5f), Is.EqualTo(6f).Within(0.001f));
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
