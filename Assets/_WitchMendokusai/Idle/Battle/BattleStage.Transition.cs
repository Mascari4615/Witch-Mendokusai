using UnityEngine;
using UnityEngine.UIElements;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Idle
{
	public sealed partial class BattleStage
	{
		private VisualElement transitionVeil;
		private float transitionSeconds;
		private float transitionElapsed;
		private int transitionPhase;
		private long epochShown;
		private bool epochReady;
		private bool snapCamera;
		private float middleShown;

		public void SetTransitionVeil(VisualElement veil, float seconds)
		{
			transitionVeil = veil;
			transitionSeconds = seconds;
		}

		// 가림 완료 프레임과 재배치 프레임 분리. 큰 delta에서도 노출 재배치 방지
		private bool AdvanceTransition(IdleSnapshot snapshot, float delta)
		{
			if (epochReady == false)
			{
				epochShown = snapshot.BattleEpoch;
				epochReady = true;
				entities.SnapNext();
				return true;
			}

			if (transitionPhase == 0 && epochShown != snapshot.BattleEpoch)
			{
				transitionPhase = 1;
				transitionElapsed = 0f;
			}

			if (transitionPhase == 0) { return true; }
			float half = transitionSeconds * 0.5f;
			if (transitionPhase == 1)
			{
				transitionElapsed += delta;
				float opacity = half > 0f ? Mathf.Clamp01(transitionElapsed / half) : 1f;
				if (transitionVeil != null) { transitionVeil.style.opacity = opacity; }
				if (opacity >= 1f) { transitionPhase = 2; }
				return false;
			}

			if (transitionPhase == 2)
			{
				epochShown = snapshot.BattleEpoch;
				originReady = false;
				snapCamera = true;
				entities.SnapNext();
				fx.Clear();
				transitionPhase = 3;
				transitionElapsed = 0f;
				return true;
			}

			// 가려진 동안 다시 바뀐 전장은 가림을 유지한 채 다음 프레임에 반영
			if (epochShown != snapshot.BattleEpoch)
			{
				if (transitionVeil != null) { transitionVeil.style.opacity = 1f; }
				transitionPhase = 2;
				return false;
			}

			transitionElapsed += delta;
			float remaining = half > 0f ? 1f - Mathf.Clamp01(transitionElapsed / half) : 0f;
			if (transitionVeil != null) { transitionVeil.style.opacity = remaining; }
			if (remaining <= 0f) { transitionPhase = 0; }
			return true;
		}
	}
}
