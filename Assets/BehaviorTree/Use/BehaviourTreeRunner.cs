using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615.BehaviourTree
{
	public class BehaviourTreeRunner : MonoBehaviour
	{
		private INode rootNode;

		private void Awake()
		{
			// Init RootNode
		}

		private void Update()
		{
			rootNode.Run();
		}
	}
}