using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace WitchMendokusai
{
	/// <summary>
	/// GroundSurface 표면 기반 가림 해소. Body 계산 뒤 첫 가림부터 안전 거리 제한.
	/// 장애물 소실 시 거리 복원만 보간. 이동과 카메라 표면 규칙 공유.
	/// </summary>
	[AddComponentMenu("Cinemachine/Extensions/WM Component Deoccluder")]
	[SaveDuringPlay]
	public class CinemachineComponentDeoccluder : CinemachineExtension
	{
		[Tooltip("Broad-phase Physics 검사 레이어 — Everything 그대로 두고 GroundSurface 컴포넌트로 최종 필터")]
		[SerializeField] private LayerMask broadMask = ~0;

		[Tooltip("카메라를 구체로 간주 — 벽에 살짝 띄움. 작을수록 모서리 스침 둔감")]
		[Range(0.01f, 1f)]
		[SerializeField] private float cameraRadius = 0.08f;

		[Tooltip("충돌면과 카메라 구체 사이 여유 거리")]
		[SerializeField] private float collisionPadding = 0.02f;
		[SerializeField, Range(1, 8)] private int overlapIterations = 4;
		[Tooltip("현재 회전 방향의 사전 충돌 검사 시간. 0이면 즉시 안전 제한만 사용")]
		[SerializeField, Range(0f, 0.5f)] private float anticipationTime = 0.25f;
		[SerializeField, Range(2, 16)] private int anticipationSamples = 8;
		[SerializeField, Range(1, 6)] private int anticipationRefinements = 4;
		[SerializeField, Range(0f, 60f)] private float maximumAnticipationAngle = 30f;

		[Tooltip("trigger collider 도 가림으로 인정할지")]
		[SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;


		[Tooltip("가림 풀려 멀어질 때 부드러움 (초) — 보통 damping 보다 더 김. 0.3~0.8 추천")]
		[Range(0f, 2f)]
		[SerializeField] private float smoothingTime = 0.5f;


		[Header("Debug")]
		[Tooltip("Scene view 에 카메라-LookAt 선·hit 지점·구체 표시. Play 중 Scene view 에서 보임. Game view 도 보고 싶으면 Game 탭 상단 Gizmos 토글 ON.")]
		[SerializeField] private bool showDebugGizmos = false;

		[Tooltip("hit 지점 구체 색")]
		[SerializeField] private Color hitColor = new(1f, 0.3f, 0.2f, 0.9f);

		[Tooltip("미가림 카메라 광선 색")]
		[SerializeField] private Color clearColor = new(0.3f, 1f, 0.4f, 0.6f);

		private const int HIT_BUFFER_SIZE = 16;
		private static readonly RaycastHit[] HIT_BUFFER = new RaycastHit[HIT_BUFFER_SIZE];
		private readonly Collider[] overlapBuffer = new Collider[HIT_BUFFER_SIZE];
		private SphereCollider penetrationProbe;

		private readonly Dictionary<CinemachineVirtualCameraBase, VcamState> stateByVcam = new();

		private struct VcamState
		{
			public float currentDistance;
			public float velocity;
			public Vector3 previousDirection;

			// Debug 캐시 — OnDrawGizmos 가 Body callback 안에서 그릴 수 없어 마지막 값 보관
			public Vector3 debugTarget;
			public Vector3 debugDirection;
			public float debugDesiredDistance;
			public float debugOccludedDistance;
			public bool debugWasOccluded;
		}

		protected override void PostPipelineStageCallback(
			CinemachineVirtualCameraBase vcam,
			CinemachineCore.Stage stage,
			ref CameraState state,
			float deltaTime)
		{
			if (stage != CinemachineCore.Stage.Body)
				return;

			if (state.HasLookAt() == false)
				return;

			float nearHeight = state.Lens.NearClipPlane * Mathf.Tan(state.Lens.FieldOfView * Mathf.Deg2Rad * 0.5f);
			float nearWidth = nearHeight * state.Lens.Aspect;
			float probeRadius = Mathf.Max(cameraRadius, new Vector3(nearWidth, nearHeight, state.Lens.NearClipPlane).magnitude);
			Vector3 target = ResolveTarget(state.ReferenceLookAt, probeRadius);
			Vector3 delta = state.RawPosition - target;
			float desiredDistance = delta.magnitude;
			if (desiredDistance < 0.0001f)
				return;
			Vector3 direction = delta / desiredDistance;

			float occludedDistance = SafeDistance(target, direction, probeRadius, desiredDistance);

			if (stateByVcam.TryGetValue(vcam, out VcamState vcamState) == false)
			{
				vcamState = new VcamState { currentDistance = desiredDistance, velocity = 0f, previousDirection = direction };
			}

			// 안전 거리는 첫 프레임부터 적용. 보간은 벽에서 멀어지는 복원에만 사용
			float targetDistance = occludedDistance;
			if (deltaTime > 0f && anticipationTime > 0f && occludedDistance >= desiredDistance)
			{
				Quaternion rotation = Quaternion.FromToRotation(vcamState.previousDirection, direction);
				float angle = Vector3.Angle(vcamState.previousDirection, direction);
				if (angle > 0.001f)
				{
					float prediction = Mathf.Min(anticipationTime / deltaTime, maximumAnticipationAngle / angle);
					float clearFraction = 0f;
					for (int sample = 1; sample <= anticipationSamples; sample++)
					{
						float fraction = (float)sample / anticipationSamples;
						Vector3 predicted = Quaternion.SlerpUnclamped(Quaternion.identity, rotation, prediction * fraction) * direction;
						float clearance = SafeDistance(target, predicted, probeRadius, desiredDistance);
						if (clearance >= desiredDistance)
						{
							clearFraction = fraction;
							continue;
						}
						// 첫 가림 경계 세분화. 표본 번호가 바뀔 때 거리 계단 발생 방지
						for (int refinement = 0; refinement < anticipationRefinements; refinement++)
						{
							float middle = (clearFraction + fraction) * 0.5f;
							predicted = Quaternion.SlerpUnclamped(Quaternion.identity, rotation, prediction * middle) * direction;
							float middleClearance = SafeDistance(target, predicted, probeRadius, desiredDistance);
							if (middleClearance >= desiredDistance)
								clearFraction = middle;
							else
							{
								fraction = middle;
								clearance = middleClearance;
							}
						}
						targetDistance = Mathf.Min(targetDistance, Mathf.Lerp(clearance, desiredDistance, fraction));
						break;
					}
				}
			}
			vcamState.previousDirection = direction;
			bool pullingIn = targetDistance < vcamState.currentDistance;
			float smoothTime = pullingIn ? 0f : smoothingTime;

			if (deltaTime > 0f && smoothTime > 0.0001f)
			{
				vcamState.currentDistance = Mathf.SmoothDamp(
					vcamState.currentDistance,
					targetDistance,
					ref vcamState.velocity,
					smoothTime,
					Mathf.Infinity,
					deltaTime);
			}
			else
			{
				vcamState.currentDistance = targetDistance;
				vcamState.velocity = 0f;
			}

			vcamState.debugTarget = target;
			vcamState.debugDirection = direction;
			vcamState.debugDesiredDistance = desiredDistance;
			vcamState.debugOccludedDistance = occludedDistance;
			vcamState.debugWasOccluded = occludedDistance < desiredDistance;

			stateByVcam[vcam] = vcamState;

			state.RawPosition = target + (direction * vcamState.currentDistance);

			if (showDebugGizmos)
			{
				Color rayColor = vcamState.debugWasOccluded ? hitColor : clearColor;
				// LookAt → 원래 의도 위치 (어두운 색 — "원거리 의도")
				Debug.DrawLine(target, target + (direction * desiredDistance), rayColor * 0.4f);
				// LookAt → 실제 카메라 위치 (밝은 색 — "현 carbon")
				Debug.DrawLine(target, target + (direction * vcamState.currentDistance), rayColor);
				if (vcamState.debugWasOccluded)
				{
					Vector3 hitPoint = target + (direction * occludedDistance);
					Debug.DrawLine(hitPoint + Vector3.up * 0.2f, hitPoint - Vector3.up * 0.2f, hitColor);
					Debug.DrawLine(hitPoint + Vector3.right * 0.2f, hitPoint - Vector3.right * 0.2f, hitColor);
					Debug.DrawLine(hitPoint + Vector3.forward * 0.2f, hitPoint - Vector3.forward * 0.2f, hitColor);
				}
			}
		}

		private float SafeDistance(Vector3 target, Vector3 direction, float radius, float distance)
		{
			int count = Physics.SphereCastNonAlloc(target, radius, direction, HIT_BUFFER, distance, broadMask, triggerInteraction);
			RaycastHit[] hits = HIT_BUFFER;
			// NonAlloc 포화 시 전체 표본 재조회. 가까운 벽 누락 방지
			if (count == hits.Length)
			{
				hits = Physics.SphereCastAll(target, radius, direction, distance, broadMask, triggerInteraction);
				count = hits.Length;
			}
			for (int index = 0; index < count; index++)
			{
				if (hits[index].collider.GetComponentInParent<GroundSurface>() != null)
					distance = Mathf.Min(distance, Mathf.Max(0f, hits[index].distance - collisionPadding));
			}
			return distance;
		}

		private Vector3 ResolveTarget(Vector3 target, float radius)
		{
			if (penetrationProbe == null)
			{
				GameObject probeObject = new("Camera clearance probe") { hideFlags = HideFlags.HideAndDontSave };
				penetrationProbe = probeObject.AddComponent<SphereCollider>();
				penetrationProbe.isTrigger = true;
				penetrationProbe.enabled = false;
			}
			penetrationProbe.radius = radius;
			for (int iteration = 0; iteration < overlapIterations; iteration++)
			{
				int count = Physics.OverlapSphereNonAlloc(target, radius, overlapBuffer, broadMask, triggerInteraction);
				Collider[] overlaps = overlapBuffer;
				if (count == overlaps.Length)
				{
					overlaps = Physics.OverlapSphere(target, radius, broadMask, triggerInteraction);
					count = overlaps.Length;
				}
				bool moved = false;
				for (int index = 0; index < count; index++)
				{
					Collider surface = overlaps[index];
					if (surface.GetComponentInParent<GroundSurface>() == null)
						continue;
					// 비활성 콜라이더의 침투 결과는 false. 계산 동안만 활성화, 물리 tick에는 비활성
					penetrationProbe.enabled = true;
					bool penetrating;
					Vector3 direction;
					float distance;
					try
					{
						penetrating = Physics.ComputePenetration(penetrationProbe, target, Quaternion.identity,
							surface, surface.transform.position, surface.transform.rotation, out direction, out distance);
					}
					finally
					{
						penetrationProbe.enabled = false;
					}
					if (penetrating)
					{
						target += direction * (distance + collisionPadding);
						moved = true;
					}
				}
				if (moved == false)
					break;
			}
			return target;
		}

		private void OnDrawGizmos()
		{
			if (showDebugGizmos == false)
				return;

			foreach (KeyValuePair<CinemachineVirtualCameraBase, VcamState> kv in stateByVcam)
			{
				VcamState s = kv.Value;
				Vector3 currentPos = s.debugTarget + (s.debugDirection * s.currentDistance);
				Vector3 desiredPos = s.debugTarget + (s.debugDirection * s.debugDesiredDistance);

				// 현재 카메라 위치 구체 (radius 시각화)
				Gizmos.color = s.debugWasOccluded ? hitColor : clearColor;
				Gizmos.DrawWireSphere(currentPos, cameraRadius);

				// 원래 의도 위치 구체 (어둡게)
				Gizmos.color = clearColor * 0.4f;
				Gizmos.DrawWireSphere(desiredPos, cameraRadius * 0.6f);

				// hit 지점 구체
				if (s.debugWasOccluded)
				{
					Vector3 hitPos = s.debugTarget + (s.debugDirection * s.debugOccludedDistance);
					Gizmos.color = hitColor;
					Gizmos.DrawWireSphere(hitPos, cameraRadius * 0.8f);
				}
			}
		}

		protected override void OnDestroy()
		{
			if (penetrationProbe != null)
			{
				if (Application.isPlaying)
					Destroy(penetrationProbe.gameObject);
				else
					DestroyImmediate(penetrationProbe.gameObject);
			}
			stateByVcam.Clear();
			base.OnDestroy();
		}
	}
}
