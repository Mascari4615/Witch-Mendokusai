using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public abstract class UIDataBuffer<T> : MonoBehaviour
	{
		[field: SerializeField] public DataBuffer<T> DataBuffer { get; private set; }
		public List<UISlot> Slots { get; protected set; } = new();
		public int CurSlotIndex { get; protected set; } = 0;
		public UISlot CurSlot => Slots[CurSlotIndex];
		[SerializeField] protected Transform slotsParent;
		[SerializeField] protected bool dontShowEmptySlot = false;
		[SerializeField] protected ToolTip clickToolTip;
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
				Slots[i].SetSelectAction((slot) => { SelectSlot(slot.Index); });
				Slots[i].Init();

				if (clickToolTip != null)
					Slots[i].ToolTipTrigger.SetClickToolTip(clickToolTip);
			}

			SelectSlot(0);

			return isInit = true;
		}

		/// <summary>
		/// UI 갱신
		/// </summary>
		public virtual void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				if (i < DataBuffer.RuntimeItems.Count)
				{
					Slots[i].SetSlot(DataBuffer.RuntimeItems[i] as DataSO);
					Slots[i].gameObject.SetActive(true);
				}
				else
				{
					Slots[i].SetSlot(null);

					if (dontShowEmptySlot)
						Slots[i].gameObject.SetActive(false);
				}
			}

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Data);
		}

		public void SetDataBuffer(DataBuffer<T> newDataBuffer)
		{
			DataBuffer = newDataBuffer;
		}

		public void SelectSlot(int index)
		{
			CurSlotIndex = index;
			for (int i = 0; i < Slots.Count; i++)
				Slots[i].SetSelected(i == index);
			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Data);
		}
	}
}