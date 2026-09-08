using UnityEngine;
using UnityEngine.Rendering;

namespace WitchMendokusai
{
	/// <summary>카메라 거리 대신 투명도로 가림 해소. 동일 물체의 URP Lit/Unlit 렌더러만 지원.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MeshRenderer))]
	public sealed class CameraFadeObstacle : MonoBehaviour
	{
		[SerializeField, Range(0f, 1f)] private float obscuredOpacity = 0.15f;
		[SerializeField, Min(0f)] private float holdTime = 0.1f;
		[SerializeField, Min(0.01f)] private float restoreTime = 0.25f;
		private MeshRenderer targetRenderer;
		private Material[] originalMaterials;
		private Material[] fadedMaterials;
		private float lastRequested;
		private float opacity = 1f;
		private bool supported;
		private bool fading;
		public bool CanFade => isActiveAndEnabled && supported && targetRenderer.enabled && targetRenderer.HasPropertyBlock() == false;
		public float Opacity => opacity;

		private void OnEnable()
		{
			targetRenderer = GetComponent<MeshRenderer>();
			originalMaterials = targetRenderer.sharedMaterials;
			supported = originalMaterials.Length > 0;
			foreach (Material material in originalMaterials)
				supported &= material != null && (material.shader.name == "Universal Render Pipeline/Lit" || material.shader.name == "Universal Render Pipeline/Unlit");
		}

		public void RequestFade()
		{
			if (CanFade == false)
				return;
			if (fadedMaterials == null)
			{
				fadedMaterials = new Material[originalMaterials.Length];
				for (int index = 0; index < fadedMaterials.Length; index++)
				{
					Material material = new(originalMaterials[index]) { hideFlags = HideFlags.HideAndDontSave };
					material.SetFloat("_Surface", 1f);
					material.SetFloat("_Blend", 0f);
					material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
					material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
					material.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
					material.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
					material.SetFloat("_ZWrite", 0f);
					material.SetFloat("_AlphaClip", 0f);
					material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					material.DisableKeyword("_ALPHATEST_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.DisableKeyword("_ALPHAMODULATE_ON");
					material.SetOverrideTag("RenderType", "Transparent");
					material.SetShaderPassEnabled("ShadowCaster", false);
					material.SetShaderPassEnabled("DepthOnly", false);
					material.renderQueue = (int)RenderQueue.Transparent;
					fadedMaterials[index] = material;
				}
			}
			if (fading == false)
				targetRenderer.sharedMaterials = fadedMaterials;
			fading = true;
			lastRequested = Time.unscaledTime;
			// 거리 유지 첫 프레임부터 피사체 노출. 복원만 보간
			SetOpacity(obscuredOpacity);
		}

		private void Update()
		{
			if (fading == false || Time.unscaledTime - lastRequested <= holdTime)
				return;
			SetOpacity(Mathf.MoveTowards(opacity, 1f, Time.unscaledDeltaTime / restoreTime));
			if (opacity >= 1f)
				Restore();
		}

		private void SetOpacity(float value)
		{
			opacity = value;
			for (int index = 0; index < fadedMaterials.Length; index++)
			{
				Color color = originalMaterials[index].GetColor("_BaseColor");
				color.a *= opacity;
				fadedMaterials[index].SetColor("_BaseColor", color);
			}
		}

		private void Restore()
		{
			if (fading && targetRenderer != null)
				targetRenderer.sharedMaterials = originalMaterials;
			fading = false;
			opacity = 1f;
		}

		private void OnDisable()
		{
			Restore();
			if (fadedMaterials == null)
				return;
			foreach (Material material in fadedMaterials)
			{
				if (Application.isPlaying)
					Destroy(material);
				else
					DestroyImmediate(material);
			}
			fadedMaterials = null;
		}
	}
}
