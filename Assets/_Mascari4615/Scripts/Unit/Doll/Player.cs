using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class Player : Singleton<Player>
	{
		public PlayerObject Object { get; private set; }
		[field: SerializeField] public GameObject ExpCollider { get; private set; }

		private PlayerInteraction interaction;
		private PlayerAim aim;

		public Vector3 AimDirection { get; private set; }
		public Vector3 AutoAimPos { get; private set; }
		public bool IsAutoAim { get; private set; }
		public Transform NearestTarget { get; private set; }

		public Stat Stat => Object.Stat;

		protected override void Awake()
		{
			base.Awake();
			interaction = new(transform);
			aim = new(transform);
			Object = GetComponent<PlayerObject>();
		}

		private void Start()
		{
			Object.Init(GetDoll(DataManager.Instance.CurDollID));
		}

		private void Update()
		{
			AutoAimPos = aim.CalcAutoAim();
			AimDirection = aim.CalcMouseAimDriection();

			if (IsAutoAim && AutoAimPos != Vector3.zero)
			{
				Vector3 autoAimDirection = (AutoAimPos - transform.position).normalized;
				AimDirection = autoAimDirection;
			}

			NearestTarget = aim.GetNearestTarget()?.transform;
		}

		public void TryInteract()
		{
			if (UIManager.Instance.CurOverlay != MPanelType.None)
				return;

			interaction.TryInteraction();
		}

		public void TryUseSkill(int skillIndex)
		{
			if (GameManager.Instance.IsCooling)
				return;
			Object.UseSkill(skillIndex);
		}

		public void SetAutoAim(bool isAutoAim)
		{
			IsAutoAim = isAutoAim;
		}
	}
}