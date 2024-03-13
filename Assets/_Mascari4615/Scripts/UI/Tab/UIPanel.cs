using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class UIPanel : MonoBehaviour
	{
		public abstract void Init();
		public abstract void UpdateUI();
	}
}