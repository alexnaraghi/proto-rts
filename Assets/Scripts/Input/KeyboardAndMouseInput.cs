using UnityEngine;

public class KeyboardAndMouseInput : RtsInput
{
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
}
