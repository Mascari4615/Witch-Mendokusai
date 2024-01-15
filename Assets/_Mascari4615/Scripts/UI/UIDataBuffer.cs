using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class UIDataBuffer<T> : MonoBehaviour
	{
		[SerializeField] private DataBuffer<T> dataBuffer;
		[HideInInspector] public List<UISlot> slots = new();

		private void Awake()
		{
			slots = GetComponentsInChildren<UISlot>().ToList();

			for (int i = 0; i < slots.Count; i++)
				slots[i].SetSlotIndex(i);
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				if (i < dataBuffer.RuntimeItems.Count)
				{
					slots[i].Init(dataBuffer.RuntimeItems[i] as Artifact);
					// slots[i].gameObject.SetActive(true);
				}
				else
				{
					slots[i].Init(null);
					// slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetDataBuffer(DataBuffer<T> newDataBuffer)
		{
			dataBuffer = newDataBuffer;
		}
	}
}