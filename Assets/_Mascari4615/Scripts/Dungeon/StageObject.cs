using UnityEngine;

namespace Mascari4615
{
	public class StageObject : MonoBehaviour
	{
		public Portal[] Portals => transform.GetComponentsInChildren<Portal>();

		private void OnEnable()
		{
			foreach (Portal portal in Portals)
				portal.Active();
		}
	}
}