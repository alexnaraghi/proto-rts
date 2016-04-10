using UnityEngine;

public class DependencyResolver : MonoBehaviour 
{
	void Awake () 
    {
        //Initialize all platform dependency junk
        
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
        Injector.AddComponent<RtsInput, KeyboardAndMouseInput>();
#endif
    }
}
