using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour 
{
    public GameObject HighlightObject;
    
    Color _originalColor;
    
    void Start()
    {
        if(HighlightObject != null)
        {
            HighlightObject.SetActive(false);
        }
    }
	public void ToggleSelection(bool isOn)
    {
        if(HighlightObject != null)
        {
            HighlightObject.SetActive(isOn);
        }
    }
}
