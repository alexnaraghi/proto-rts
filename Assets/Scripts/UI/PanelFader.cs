using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
public class PanelFader : MonoBehaviour 
{
	public CanvasGroup Panel;

    void Awake()
	{
        Assert.IsNotNull(Panel);
    }
	
	public void Show()
	{
        Panel.alpha = 1f;		
	}
	
	public void Hide()
	{
        Panel.alpha = 0f;
    }
	
	public void FadeIn(float seconds, float delay)
	{
        Panel.alpha = 0f;
        if(delay > 0f)
		{
            LeanTween.delayedCall(gameObject, delay, () => fadeInInternal(seconds));
        }
		else
		{
            fadeInInternal(seconds);
        }
	}
	
	public void FadeOut(float seconds, float delay)
	{
        Panel.alpha = 1f;
        if(delay > 0f)
		{
            LeanTween.delayedCall(gameObject, delay, () => fadeOutInternal(seconds));
        }
		else
		{
            fadeOutInternal(seconds);
        }
	}

    private void fadeInInternal(float seconds)
    {
		LeanTween.value(Panel.gameObject, (f) => Panel.alpha = f, 0f, 1f, seconds)
			.setEase(LeanTweenType.easeInQuad);
    }

    private void fadeOutInternal(float seconds)
	{
		LeanTween.value(Panel.gameObject, (f) => Panel.alpha = f, 1f, 0f, seconds)
			.setEase(LeanTweenType.easeOutCirc);
	}
}
