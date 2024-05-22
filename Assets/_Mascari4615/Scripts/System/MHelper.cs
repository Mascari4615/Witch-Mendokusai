using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mascari4615
{
	public static class MHelper
	{
		public static bool IsPlaying => (SceneManager.GetActiveScene().isLoaded == false || Application.isPlaying == false) == false;

		#region GetNearest
		public static T GetNearest<T>(List<T> list, Vector3 targetPosition, float maxDistance) where T : MonoBehaviour
		{
			return GetNearest(list, element => element.transform.position, targetPosition, maxDistance);
		}

		public static Transform GetNearest(List<Transform> list, Vector3 position, float maxDistance)
		{
			return GetNearest(list, transform => transform.position, position, maxDistance);
		}

		public static GameObject GetNearest(List<GameObject> list, Vector3 position, float maxDistance)
		{
			return GetNearest(list, obj => obj.transform.position, position, maxDistance);
		}

		public static T GetNearest<T>(List<T> list, Func<T, Vector3> getPositionByElement, Vector3 targetPosition, float maxDistance)
		{
			T nearest = default;
			float nearestDistance = float.MaxValue;

			foreach (T element in list)
			{
				Vector3 elementPosition = getPositionByElement(element);
				float distance = Vector3.Distance(elementPosition, targetPosition);

				if (distance < nearestDistance)
				{
					nearest = element;
					nearestDistance = distance;
				}
			}

			return nearestDistance <= maxDistance ? nearest : default;
		}
	}
	#endregion
}