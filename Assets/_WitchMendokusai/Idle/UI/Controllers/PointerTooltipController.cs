using System;
using UnityEngine;
using UnityEngine.UIElements;
using UIPointerType = UnityEngine.UIElements.PointerType;

namespace WitchMendokusai.Idle.UI
{
	public sealed class PointerTooltipController
	{
		/// <summary>툴팁 배치 (px). 마우스는 옆에 바짝, 손가락은 위로 멀리 (손가락에 안 가리게)</summary>
		public sealed class Layout
		{
			public long TouchDisplayMilliseconds { get; set; }
			public float MouseGap { get; set; }
			public float TouchGap { get; set; }
			public float EdgeMargin { get; set; }
			/// <summary>아직 안 잰 판과 툴팁의 대체 크기. 첫 표시 프레임에만 쓰임</summary>
			public Vector2 RootFallbackSize { get; set; }
			public Vector2 TipFallbackSize { get; set; }
		}

		private readonly Label tooltip;
		private readonly Layout layout;
		private int version;
		private int touchPointer = -1;

		public PointerTooltipController(Label tooltip, Layout layout)
		{
			this.tooltip = tooltip;
			this.layout = layout;
		}

		public void Bind(VisualElement target, Func<string> text)
		{
			target.RegisterCallback<PointerDownEvent>(moment =>
			{
				if (moment.pointerType == UIPointerType.mouse)
				{
					return;
				}

				touchPointer = moment.pointerId;
				Show(text());
				Move(moment.position, true);
				int scheduledVersion = ++version;
				tooltip.schedule.Execute(() =>
				{
					if (scheduledVersion == version)
					{
						Hide();
					}
				}).StartingIn(layout.TouchDisplayMilliseconds);
			});

			target.RegisterCallback<PointerEnterEvent>(moment =>
			{
				if (moment.pointerType == UIPointerType.mouse)
				{
					Show(text());
					Move(moment.position, false);
				}
			});

			target.RegisterCallback<PointerMoveEvent>(moment =>
			{
				if (moment.pointerType == UIPointerType.mouse || moment.pointerId == touchPointer)
				{
					Move(moment.position, moment.pointerType != UIPointerType.mouse);
				}
			});

			target.RegisterCallback<PointerLeaveEvent>(moment =>
			{
				if (moment.pointerType == UIPointerType.mouse)
				{
					Hide();
				}
			});
		}

		private void Show(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				tooltip.style.display = DisplayStyle.None;
				return;
			}

			tooltip.text = text;
			tooltip.style.display = DisplayStyle.Flex;
			tooltip.BringToFront();
		}

		private void Hide()
		{
			version++;
			touchPointer = -1;
			tooltip.style.display = DisplayStyle.None;
		}

		private void Move(Vector2 at, bool touch)
		{
			VisualElement owner = tooltip.parent;
			Vector2 local = owner != null ? owner.WorldToLocal(at) : at;
			float rootWidth = owner != null ? owner.resolvedStyle.width : layout.RootFallbackSize.x;
			float rootHeight = owner != null ? owner.resolvedStyle.height : layout.RootFallbackSize.y;
			float tipWidth = tooltip.resolvedStyle.width > 0f ? tooltip.resolvedStyle.width : layout.TipFallbackSize.x;
			float tipHeight = tooltip.resolvedStyle.height > 0f ? tooltip.resolvedStyle.height : layout.TipFallbackSize.y;
			Vector2 placed = Place(local, new Vector2(tipWidth, tipHeight), new Vector2(rootWidth, rootHeight), touch, layout);
			tooltip.style.left = placed.x;
			tooltip.style.top = placed.y;
		}

		/// <summary>
		/// 툴팁 자리 셈. 순수 함수라 시험이 잼
		///
		/// 마우스: 커서 오른쪽 아래, 판을 넘으면 왼쪽 또는 위로
		/// 손가락: 위 우선 (손이 안 가림). 위에 자리가 없으면 아래가 아니라 옆 (손가락 높이, 오른손 기준 왼쪽 먼저).
		///   아래로 보내면 손바닥에 가림 (사용자 2026-09-08 지적: 화면 위쪽 칩과 편성 칸). 옆도 없을 때만 아래
		/// </summary>
		public static Vector2 Place(Vector2 local, Vector2 tip, Vector2 root, bool touch, Layout layout)
		{
			float gap = touch ? layout.TouchGap : layout.MouseGap;
			float edge = layout.EdgeMargin;
			float x;
			float y;

			if (touch == false)
			{
				x = local.x + gap;
				y = local.y + gap;
				if (x + tip.x > root.x)
				{
					x = local.x - tip.x - gap;
				}
				if (y + tip.y > root.y)
				{
					y = local.y - tip.y - gap;
				}
			}
			else if (local.y >= tip.y + gap + edge)
			{
				x = local.x - tip.x * 0.5f;
				y = local.y - tip.y - gap;
			}
			else if (local.x - gap - tip.x >= edge)
			{
				x = local.x - gap - tip.x;
				y = local.y - tip.y * 0.5f;
			}
			else if (local.x + gap + tip.x <= root.x - edge)
			{
				x = local.x + gap;
				y = local.y - tip.y * 0.5f;
			}
			else
			{
				x = local.x - tip.x * 0.5f;
				y = local.y + gap;
			}

			return new Vector2(
				Mathf.Clamp(x, edge, Mathf.Max(edge, root.x - tip.x - edge)),
				Mathf.Clamp(y, edge, Mathf.Max(edge, root.y - tip.y - edge)));
		}
	}
}
