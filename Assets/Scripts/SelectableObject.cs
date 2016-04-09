using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour 
{
    public MeshRenderer HighlightObject;
    
    Color _originalColor;
    
    void Start()
    {
        if(HighlightObject != null)
        {
            HighlightObject.enabled = false;
        }
    }
	public void ToggleSelection(bool isOn)
    {
        if(HighlightObject != null)
        {
            if(isOn)
            {
                HighlightObject.enabled = true;
            }
            else
            {
                HighlightObject.enabled = false;                
            }
        }
    }
}
