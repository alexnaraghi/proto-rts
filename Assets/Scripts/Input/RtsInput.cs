using UnityEngine;
public abstract class RtsInput : MonoBehaviour
{
    public abstract bool IsSelectionStarted();
    public abstract bool IsSelectionFinished();
    public abstract Vector2 GetSelectionPosition();
    public abstract bool IsActionTriggered();
    public abstract Vector2 GetActionPosition();
}
