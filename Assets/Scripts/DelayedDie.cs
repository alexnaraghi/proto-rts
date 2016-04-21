using UnityEngine;
using System.Collections;

public class DelayedDie : MonoBehaviour 
{
	public float LifeSeconds = 5f;

    private float _timeSeconds;
    // Use this for initialization
    void Update()
	{
		if(_timeSeconds > LifeSeconds)
		{
            Destroy(this);
            return;
        }
        _timeSeconds += Time.deltaTime;
    }
}
