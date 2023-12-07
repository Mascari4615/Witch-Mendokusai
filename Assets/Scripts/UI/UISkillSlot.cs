using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UISkillSlot : MonoBehaviour
	{
		[SerializeField] private Image skillIconImage;
		[SerializeField] private Image coolTimeImage;

		public void UpdateUI(Skill skill, SkillCoolTime skillCoolTime)
		{
			skillIconImage.sprite = skill.Thumbnail;
			coolTimeImage.fillAmount = skillCoolTime.CurCooltime / skillCoolTime.Cooltime;
		}
	}
}