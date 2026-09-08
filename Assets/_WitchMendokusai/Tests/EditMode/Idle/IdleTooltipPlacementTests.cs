using NUnit.Framework;
using UnityEngine;
using WitchMendokusai.Idle.UI;

namespace WitchMendokusai.Tests
{
	/// <summary>
	/// 툴팁 자리. 손가락은 위, 위가 없으면 옆, 아래는 마지막 (손바닥에 가림). 마우스는 커서 옆
	/// 사용자 2026-09-08: 화면 위쪽 칩과 편성 칸에서 손가락 아래로 나오면 안 됨
	/// </summary>
	public sealed class IdleTooltipPlacementTests
	{
		private static readonly Vector2 ROOT = new Vector2(1920f, 1080f);
		private static readonly Vector2 TIP = new Vector2(300f, 120f);
		private const float FINGER = 40f;

		private static PointerTooltipController.Layout Layout() => new PointerTooltipController.Layout
		{
			MouseGap = 18f,
			TouchGap = 72f,
			EdgeMargin = 12f,
		};

		private static bool CoversFinger(Vector2 placed, Vector2 finger)
		{
			return placed.x <= finger.x + FINGER && placed.x + TIP.x >= finger.x - FINGER
				&& placed.y <= finger.y + FINGER && placed.y + TIP.y >= finger.y - FINGER;
		}

		[Test]
		public void Touch_InTheMiddle_GoesAboveTheFinger()
		{
			Vector2 finger = new Vector2(960f, 600f);
			Vector2 placed = PointerTooltipController.Place(finger, TIP, ROOT, true, Layout());

			Assert.Less(placed.y + TIP.y, finger.y, "위쪽이어야 한다");
			Assert.IsFalse(CoversFinger(placed, finger));
		}

		[Test]
		public void Touch_AtTheTopEdge_GoesBeside_NotBelow()
		{
			Vector2 finger = new Vector2(960f, 40f);
			Vector2 placed = PointerTooltipController.Place(finger, TIP, ROOT, true, Layout());

			Assert.IsFalse(CoversFinger(placed, finger), "손가락 위에 겹침");
			Assert.Less(placed.y, finger.y + 72f, "아래로 보내면 손바닥에 가림. 옆이어야 한다");
			Assert.IsTrue(placed.x + TIP.x < finger.x || placed.x > finger.x, "옆에 있어야 한다");
		}

		[Test]
		public void Touch_AtTheTopRightCorner_GoesLeft()
		{
			Vector2 finger = new Vector2(1880f, 40f);
			Vector2 placed = PointerTooltipController.Place(finger, TIP, ROOT, true, Layout());

			Assert.Less(placed.x + TIP.x, finger.x, "왼쪽");
			Assert.IsFalse(CoversFinger(placed, finger));
		}

		[Test]
		public void Touch_AtTheTopLeftCorner_GoesRight()
		{
			Vector2 finger = new Vector2(40f, 40f);
			Vector2 placed = PointerTooltipController.Place(finger, TIP, ROOT, true, Layout());

			Assert.Greater(placed.x, finger.x, "오른쪽");
			Assert.IsFalse(CoversFinger(placed, finger));
		}

		[Test]
		public void Touch_OnATinyScreen_FallsBelow_ButStaysInside()
		{
			Vector2 root = new Vector2(360f, 640f);
			Vector2 finger = new Vector2(180f, 40f);
			Vector2 placed = PointerTooltipController.Place(finger, TIP, root, true, Layout());

			Assert.GreaterOrEqual(placed.x, 12f);
			Assert.LessOrEqual(placed.x + TIP.x, root.x - 12f);
			Assert.Greater(placed.y, finger.y, "옆도 안 되면 아래");
		}

		[Test]
		public void Mouse_SitsBesideTheCursor_AndFlipsAtTheEdge()
		{
			Vector2 middle = PointerTooltipController.Place(new Vector2(500f, 500f), TIP, ROOT, false, Layout());
			Assert.AreEqual(518f, middle.x, 0.01f);
			Assert.AreEqual(518f, middle.y, 0.01f);

			Vector2 corner = PointerTooltipController.Place(new Vector2(1900f, 1060f), TIP, ROOT, false, Layout());
			Assert.Less(corner.x + TIP.x, 1900f);
			Assert.Less(corner.y + TIP.y, 1060f);
		}
	}
}
