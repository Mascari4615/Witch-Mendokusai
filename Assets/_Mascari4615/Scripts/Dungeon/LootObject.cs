using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class LootObject : MonoBehaviour
	{
		[SerializeField] private float moveSpeed;
		[SerializeField] private bool useBezierCurve;

		private Coroutine _moveLoop;

		public abstract void Effect();

		private void OnEnable()
		{
			StopAllCoroutines();
			_moveLoop = null;
			SOManager.Instance.DropsBuffer.AddItem(gameObject);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (_moveLoop != null)
				return;

			if (other.CompareTag("PlayerExpCollider"))
			{
				_moveLoop = StartCoroutine(MoveLoop());
			}
		}

		private IEnumerator MoveLoop()
		{
			float t = 0;
			while (true)
			{
				t += Time.deltaTime * moveSpeed;
				transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position, t);

				if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < .3f)
				{
					Effect();
					gameObject.SetActive(false);

					_moveLoop = null;
					break;
				}

				yield return null;
			}
		}

		private void OnDisable()
		{
			SOManager.Instance.DropsBuffer.RemoveItem(gameObject);
		}
	}
}