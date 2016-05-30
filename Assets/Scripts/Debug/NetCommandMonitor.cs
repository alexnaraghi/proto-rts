using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

// This is a command logger, for debugging.
public class NetCommandMonitor : MonoBehaviour 
{
    public RtsNetworkPlayer Player;
    public Button ButtonPrefab;

    public Text RemoteOrLocalText;

    public GameObject Content;

    private CommandListElement[] _elements;

    private int _highestElement;

    private int _newestKnownLockstep;
	const int ELEMENT_LENGTH = 10;

    private float _offset = 200;

    private static int _numMonitors;

    void Start () 
	{
        Assert.IsNotNull(Player);
        Assert.IsNotNull(ButtonPrefab);
        Assert.IsNotNull(Content);
        Assert.IsNotNull(RemoteOrLocalText);

        RemoteOrLocalText.text = Player.isLocalPlayer ? "Local " + Player.Id : "Remote" + Player.Id;

        _elements = new CommandListElement[ELEMENT_LENGTH];
        for(int i = 0; i < ELEMENT_LENGTH; i++)
		{
            _elements[i] = new CommandListElement();
            _elements[i].Button = Instantiate(ButtonPrefab);
            _elements[i].Button.transform.SetParent(Content.transform, false);
        }
		
        if(Player != null)
		{
            Player.CommandAddedEvent += OnCommandAdded;
        }
        
        this.transform.localPosition = new Vector3(_offset * _numMonitors, 0, 0);
        _numMonitors++;
    }

    private void OnCommandAdded(NetUnitCommand command)
    {
		var lockStep = command.LockStep;
		
        int difference = lockStep - _newestKnownLockstep;
		if (lockStep < _newestKnownLockstep - ELEMENT_LENGTH)
		{
			// EARLY OUT, NOTHING TO DO HERE IF THE MESSAGE IS TOO OLD FOR THE LIST
			Debug.Log("Really old message came in for lockstep " + lockStep);
			return;
		}
		else if(lockStep > _newestKnownLockstep)
		{
            _newestKnownLockstep = lockStep;
            
            for(int i = 0; i < difference - 1; i++)
            {
                _highestElement = OffsetFromHighIndex(1);
                UnsetElement(_highestElement);
            }
            
            _highestElement = OffsetFromHighIndex(1);
            SetElement(_highestElement, command);
        }
        else
        {
            //Go backwards into the previous results and update an existing entry.
            var index = OffsetFromHighIndex(difference);
            SetElement(index, command);
        }
    }
    
    // Gets the index at difference units away from the highest index.
    private int OffsetFromHighIndex(int difference)
    {
        return Mathf.Abs((_highestElement + difference) % (ELEMENT_LENGTH - 1));
    }
    
    private void UnsetElement(int index)
    {
        _elements[index].HasCommand = false;
        SetButtonText(index, "MISSING");
    }
    private void SetElement(int index, NetUnitCommand command)
    {
        Assert.IsTrue(index < ELEMENT_LENGTH);
        if(index >= ELEMENT_LENGTH)
        {
            Debug.Log("Index = " + index);
        }
        _elements[index].Command = command;
        _elements[index].HasCommand = true;
        SetButtonText(index, command);
    }
	
	private void SetButtonText(int buttonIndex, NetUnitCommand command)
	{
		var text = string.Format("L{0} - {1}", command.LockStep, command.Command.GetType().Name);
        SetButtonText(buttonIndex, text);
    }
	
	private void SetButtonText(int buttonIndex, string newText)
	{
		var text = _elements[buttonIndex].Button.GetComponentInChildren(typeof(Text)) as Text;
		Assert.IsNotNull(text);
		text.text = newText;
	}
    
    void OnDestroy()
    {
        _numMonitors--;
    }
}

public class CommandListElement
{
    public Button Button;

    public bool HasCommand;
    public NetUnitCommand Command;
}