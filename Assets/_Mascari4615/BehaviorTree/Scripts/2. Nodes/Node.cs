using System.Collections;
using System.Collections.Generic;
using Rito.BehaviorTree;
using UnityEngine;

namespace Rito.BehaviorTree
{
	public enum State
	{
		Success,
		Running,
		Failure,

	}

	/// <summary> 행동트리 최상위 인터페이스 </summary>
	public abstract class Node
	{
		public bool Started { get; private set; } = false;
		public State State { get; private set; } = State.Running;

		public State Update()
		{
			if (!Started)
			{
				OnStart();
				Started = true;
			}

			State = OnUpdate();

			if (State != State.Running)
			{
				OnEnd();
				Started = false;
			}

			return State;
		}

		public void Abort()
		{
			Traverse(this, (node) =>
			{
				node.Started = false;
				node.State = State.Running;
				node.OnEnd();
			});
		}

		public abstract void OnEnd();
		public abstract void OnStart();
		public abstract State OnUpdate();

		public static void Traverse(Node node, System.Action<Node> visiter)
		{
			if (node != null)
			{
				visiter.Invoke(node);
				var children = GetChildren(node);
				children.ForEach((n) => Traverse(n, visiter));
			}
		}

		public static List<Node> GetChildren(Node parent)
		{
			List<Node> children = new List<Node>();

			if (parent is DecoraterNode decorator && decorator.child != null)
			{
				children.Add(decorator.child);
			}

			/*if (parent is RootNode rootNode && rootNode.child != null)
			{
				children.Add(rootNode.child);
			}*/

			if (parent is CompositeNode composite)
			{
				return composite.ChildList;
			}

			return children;
		}

	}
}