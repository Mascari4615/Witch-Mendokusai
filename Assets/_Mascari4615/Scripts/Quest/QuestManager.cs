using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class QuestManager : Singleton<QuestManager>
	{
		[SerializeField] private GameObject questObject;

		private void Start()
		{
			QuestDataBuffer questDataBuffer = SOManager.Instance.QuestDataBuffer;
			foreach (Quest quest in questDataBuffer.InitItems)
			{
				QuestObject qo = ObjectManager.Instance.PopObject(questObject).GetComponent<QuestObject>();

				qo.transform.SetParent(transform);
				qo.name = quest.Name;
				qo.Init(quest);
				qo.gameObject.SetActive(true);
			}
		}
	}
}