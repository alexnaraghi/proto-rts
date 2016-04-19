using UnityEngine;

public class KeyboardAndMouseInput : RtsInput
{
    public override bool IsQuitting()
    {
        return Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl);
    }
    public override bool IsSelectionStarted()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override bool IsSelectionFinished()
    {
        return Input.GetMouseButtonUp(0) || IsActionTriggered();
    }

    public override Vector2 GetSelectionPosition()
    {
        return Input.mousePosition;
    }

    public override bool IsActionTriggered()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override Vector2 GetActionPosition()
    {
        return Input.mousePosition;
    }
    
    public override bool IsPanning()
    {
        return Input.GetMouseButton(2);
    }
    
    public override float GetSpeedX()
    {
        return Input.GetAxis("Mouse X");
    }
    
    public override float GetSpeedY()
    {
        return Input.GetAxis("Mouse Y");
    }
    
    public override float GetSpeedZ()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
}
