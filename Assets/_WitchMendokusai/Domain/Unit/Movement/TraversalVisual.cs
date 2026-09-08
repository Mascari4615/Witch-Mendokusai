using UnityEngine;

namespace WitchMendokusai
{
	public sealed class TraversalVisual : MonoBehaviour
	{
		[SerializeField] private GameObject glider;
		private UnitMovement movement;

		private void Awake() => movement = GetComponent<UnitMovement>();

		private void LateUpdate()
		{
			bool active = movement.Traversal.Mode == TraversalMode.Glide;
			glider.SetActive(active);
			Vector3 direction = Vector3.ProjectOnPlane(movement.Velocity, Vector3.up);
			if (active && direction.sqrMagnitude > 0f)
				glider.transform.rotation = Quaternion.LookRotation(direction);
		}

		private void OnDisable() => glider.SetActive(false);
	}
}
