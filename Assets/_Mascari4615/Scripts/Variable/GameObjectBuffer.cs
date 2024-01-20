using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(GameObjectBuffer), menuName = "GameSystem/DataBuffer/GameObject")]
	public class GameObjectBuffer : DataBuffer<GameObject>
	{
		public void ClearObjects()
		{
			for (int i = RuntimeItems.Count - 1; i >= 0; i--)
				RuntimeItems[i].SetActive(false);
			ClearBuffer();
		}
	}
}