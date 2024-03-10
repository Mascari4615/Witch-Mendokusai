using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UISkillSlot : UISlot
	{
		[SerializeField] private Image coolTimeImage;

		public void UpdateCooltime(Skill skill, SkillCoolTime skillCoolTime)
		{
			coolTimeImage.fillAmount = skillCoolTime.CurCooltime / skillCoolTime.Cooltime;
		}
	}
}