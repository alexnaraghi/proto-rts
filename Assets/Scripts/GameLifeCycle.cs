using UnityEngine;
using System.Collections;

public class GameLifeCycle : MonoBehaviour 
{
	// Use this for initialization
	void Awake () 
    {
	    Injector.AddComponent<GameState>();
	}
}
