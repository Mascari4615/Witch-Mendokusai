using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class UIDataBuffer<T> : MonoBehaviour
	{
		public List<UISlot> Slots { get; protected set; } = new();
		[SerializeField] protected Transform slotsParent;
		[SerializeField] protected DataBuffer<T> dataBuffer;
		[SerializeField] protected bool dontShowEmptySlot = false;
		private bool isInit = false;

		private void Awake()
		{
			Init();
		}

		private void OnEnable()
		{
			UpdateUI();
		}

		/// <summary>
		/// UI 초기화, 한 번만 실행됨
		/// </summary>
		/// <returns></returns>
		public virtual bool Init()
		{
			if (isInit)
				return false;

			if (slotsParent == null)
				slotsParent = transform;
			Slots = slotsParent.GetComponentsInChildren<UISlot>(true).ToList();

			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].SetSlotIndex(i);
				Slots[i].Init();
			}

			return isInit = true;
		}

		/// <summary>
		/// UI 갱신
		/// </summary>
		public virtual void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (i < dataBuffer.RuntimeItems.Count)
				{
					Slots[i].SetArtifact(dataBuffer.RuntimeItems[i] as Artifact);
					Slots[i].UpdateUI();
					Slots[i].gameObject.SetActive(true);
				}
				else
				{
					Slots[i].SetArtifact(null);
					Slots[i].UpdateUI();

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