using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public abstract class BTRunner
	{
		private readonly Node rootNode;
		protected UnitObject unitObject;

		public BTRunner(UnitObject unitObject)
		{
			rootNode = MakeNode();
			this.unitObject = unitObject;
		}

		protected abstract Node MakeNode();
		
		public void Update()
		{
			if (rootNode == null)
				return;

			rootNode.Update();
		}
	}
}