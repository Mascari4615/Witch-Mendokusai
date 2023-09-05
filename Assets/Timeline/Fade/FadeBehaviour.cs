using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class FadeBehaviour : PlayableBehaviour
{
    // public float alpha;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
#if UNITY_EDITOR
        CanvasGroup canvasGroup = Object.FindObjectOfType<UIManager>().CutSceneModule.FadeCanvasGroup;
#else
        CanvasGroup canvasGroup = UIManager.Instance.CutSceneModule.FadeCanvasGroup;
#endif
        // canvasGroup.alpha = alpha;
    }
}