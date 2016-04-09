using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
        LeanTween.scale(this.gameObject, this.transform.localScale * 1.1f, 2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setLoopPingPong();
	}
}
