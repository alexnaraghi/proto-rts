using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

// This is a command logger, for debugging.
public class NetCommandMonitor : MonoBehaviour 
{
    public Button ButtonPrefab;

    public Text RemoteOrLocalText;

    public GameObject Content;

    private CommandListElement[] _elements;
    private int _highestElement;
    private int _newestKnownLockstep;
	const int ELEMENT_LENGTH = 25;
    private RtsNetworkPlayer Player;

    private int _lastHighlightedIndex;
    private int _lastLockstepColoredIndex;

    private Color _unhighlightedColor = Color.white;
    private Color _highlightedColor = Color.yellow;
    private Color _lockstepColor = Color.cyan;

    void Start () 
	{
        Assert.IsNotNull(ButtonPrefab);
        Assert.IsNotNull(Content);
        Assert.IsNotNull(RemoteOrLocalText);
    }
    
    public void AttachPlayer(RtsNetworkPlayer player)
    {
        Player = player;
        
        //Assert.IsNotNull(Player);

        _elements = new CommandListElement[ELEMENT_LENGTH];
        for(int i = 0; i < ELEMENT_LENGTH; i++)
		{
            _elements[i] = new CommandListElement();
            _elements[i].Button = Instantiate(ButtonPrefab);
            _elements[i].Image = _elements[i].Button.GetComponent<Image>();
            _elements[i].Button.transform.SetParent(Content.transform, false);
        }
		
        if(Player != null)
		{
            RemoteOrLocalText.text = Player.isLocalPlayer ? "Local " + Player.Id : "Remote" + Player.Id;
            Player.CommandAddedEvent += OnCommandAdded;
        }
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
        return Mathf.Abs((_highestElement + difference) % (ELEMENT_LENGTH));
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
        
        
        // This shouldn't really be synced on commands added, it should probably listen for a lockstep
        // update event.
        var realLockstep = Injector.Get<CommandManager>().LockStep;
        for(int i = 0; i < _elements.Length; i++)
        {
            var element = _elements[i];
            if(element.HasCommand && element.Command.LockStep == realLockstep)
            {
                SetLockstepColor(i);
                break;
            }
        }
        
        SetHighlight(index);
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
    
    private void SetHighlight(int buttonIndex)
    {
        if(_elements[_lastHighlightedIndex] != null)
        {
            _elements[_lastHighlightedIndex].Image.color = _unhighlightedColor;
        }

        _elements[buttonIndex].Image.color = _highlightedColor;
        _lastHighlightedIndex = buttonIndex;
    }
    
    private void SetLockstepColor(int buttonIndex)
    {
        if(_elements[_lastLockstepColoredIndex] != null)
        {
            _elements[_lastLockstepColoredIndex].Image.color = _unhighlightedColor;
        }

        _elements[buttonIndex].Image.color = _lockstepColor;
        _lastLockstepColoredIndex = buttonIndex;
    }
    
    void OnDestroy()
    {
        if(Player != null)
		{
            Player.CommandAddedEvent -= OnCommandAdded;
        }
    }
}

public class CommandListElement
{
    public Button Button;
    public Image Image;

    public bool HasCommand;
    public NetUnitCommand Command;
}