using UnityEngine;
using System.Collections.Generic;

public class DebugManager : MonoBehaviour 
{
    private GameObject _netMonitorPrefab;
    private GameObject _netMonitorCanvasPrefab;

    private GameObject _netMonitorCanvas;

    private bool _isNetCommandManagerToggled;

    private List<NetCommandMonitor> _monitors = new List<NetCommandMonitor>();
	private List<RtsNetworkPlayer> _netPlayers = new List<RtsNetworkPlayer>();
	

    // Use this for initialization
    void Start () 
	{
        _netMonitorPrefab = Resources.Load("Prefabs/UI/DebugScroll/DebugScrollView") as GameObject;
        _netMonitorCanvasPrefab = Resources.Load("Prefabs/UI/DebugScroll/DebugCanvas") as GameObject;

		if(_netMonitorCanvasPrefab != null)
		{
        	_netMonitorCanvas = Instantiate(_netMonitorCanvasPrefab);
            _netMonitorCanvas.name = "DebugCanvas";
            _netMonitorCanvas.SetActive(false);
        }
		else
		{
            Debug.LogWarning("Net command monitor canvas prefab not found.");
		}
    }
	
	// Update is called once per frame
	void Update () 
	{
        // Could make this an event too.
        var allPlayers = Injector.Get<CommandManager>().GetAllPlayers();
		for(int i = 0; i < allPlayers.Count; i++)
		{
			RtsNetworkPlayer netPlayer = allPlayers[i] as RtsNetworkPlayer;
			if(netPlayer != null && !_netPlayers.Contains(netPlayer))
			{
				_netPlayers.Add(netPlayer);
				AddMonitor(netPlayer);
			}
		}
			
		if(Input.GetKeyDown(KeyCode.Minus))
		{
            toggleNetCommandManager();
        }
	}
	
	private void toggleNetCommandManager()
	{
		if(!_isNetCommandManagerToggled)
		{
            // EARLY OUT if our prefab isn't found.
            if(_netMonitorPrefab == null)
			{
				Debug.LogWarning("Net command monitor prefab not found.");
				return;
			}

            _netMonitorCanvas.SetActive(true);
		}
		else
		{
            _netMonitorCanvas.SetActive(false);
		}
		_isNetCommandManagerToggled = !_isNetCommandManagerToggled;
    }
	
	private void AddMonitor(RtsNetworkPlayer associatedPlayer)
	{
		var newMonitor = Instantiate(_netMonitorPrefab);
		newMonitor.transform.SetParent(_netMonitorCanvas.transform, false);

        const float offset = 200f;
        newMonitor.transform.localPosition = new Vector3(offset * _monitors.Count, 0, 0);

        var component = newMonitor.GetComponent<NetCommandMonitor>();
		if(component != null)
		{
			_monitors.Add(component);
			component.AttachPlayer(associatedPlayer);
        }
	}
	
}
