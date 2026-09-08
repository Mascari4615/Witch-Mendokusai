using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace WitchMendokusai.Tests
{
	public sealed class CameraOrbitRigTest
	{
		[TestCase(0f, 100f, 142f)]
		[TestCase(500f, 100f, -300f)]
		public void OrbitRotation_DoesNotMoveTrackingReferences(float x, float y, float z)
		{
			GameObject root = new("Orbit rig test");
			root.SetActive(false);
			try
			{
				CameraManager manager = root.AddComponent<CameraManager>();
				GameObject pivot = new("Cameras");
				pivot.transform.SetParent(root.transform);
				GameObject tracking = new("Tracking reference");
				tracking.transform.SetParent(root.transform);
				tracking.transform.position = new Vector3(x, y, z);
				typeof(CameraManager).GetField("pitchPivot", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(manager, pivot.transform);
				Vector3 initial = tracking.transform.position;
				manager.SetOrbitAngles(new Vector2(1f, 20f));
				Assert.That(Vector3.Distance(initial, tracking.transform.position), Is.LessThan(0.0001f), "Orbit must not rotate tracking references around world origin");
				Assert.That(Quaternion.Angle(pivot.transform.rotation, Quaternion.Euler(20f, 1f, 0f)), Is.LessThan(0.01f));
			}
			finally
			{
				Object.DestroyImmediate(root);
			}
		}
	}
}
