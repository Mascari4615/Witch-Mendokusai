using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class UIDataBuffer<T> : MonoBehaviour where T : Artifact
	{
		public List<UISlot> Slots { get; private set; } = new();
		[SerializeField] private DataBuffer<T> dataBuffer;
		[SerializeField] private bool dontShowEmptySlot = false;
		private bool isInit = false;

		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			if (isInit)
				return;
			
			Slots = GetComponentsInChildren<UISlot>(true).ToList();

			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].SetSlotIndex(i);
				Slots[i].Init();
			}

			isInit = true;
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (i < dataBuffer.RuntimeItems.Count)
				{
					Slots[i].SetArtifact(dataBuffer.RuntimeItems[i] as T);
					Slots[i].gameObject.SetActive(true);
				}
				else
				{
					Slots[i].SetArtifact(null);

					if (dontShowEmptySlot)
						Slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetDataBuffer(DataBuffer<T> newDataBuffer)
		{
			dataBuffer = newDataBuffer;
		}
	}
}