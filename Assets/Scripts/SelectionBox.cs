using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SelectionBox : MonoBehaviour
{
    public RectTransform RectTransform;
    public Image Image;

    private bool _isSelecting;
    private Vector2 _selectionStart;
    private Vector2 _selectionEnd;
    
    private List<RtsObject> _selectionCache = new List<RtsObject>(200);
    private List<Unit> _unitCache = new List<Unit>(200);

    public static void ForeachSelectedObject(HashSet<int> selectedIds, Action<RtsObject> action)
    {
        foreach (var id in selectedIds)
        {
            var rtsObj = Injector.Get<GameState>().RtsObjects[id];
            if (rtsObj != null)
            {
                action(rtsObj);
            }
            else
            {
                Debug.LogWarning("Using missing rts object, id " + id);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Destination chosen
        if (IsActionTriggered())
        {
            var cursorPosition = GetActionPosition();
            var objUnderCursor = GetObjectUnder(cursorPosition);
            
            var selectedIds = Injector.Get<GameState>().SelectedObjectIds;
            var team = Injector.Get<GameState>().LocalTeam;
            
            _unitCache.Clear();
            ForeachSelectedObject(selectedIds, rtsObj =>
                {
                    var unit = rtsObj.GetComponent<Unit>();
                    if(unit)
                    {
                        _unitCache.Add(unit);
                    }
                    else
                    {
                        Debug.LogWarning("not unit");
                    }
                });
            var units = _unitCache.ToArray();

            if (objUnderCursor != null && objUnderCursor.IsTargetable)
            {
                Debug.LogFormat("Targeted [{0} ({1},{2})]", objUnderCursor.ToString(), objUnderCursor.transform.position.x, objUnderCursor.transform.position.z);
                
                var command = new TargetRtsObjectCommand(team, units, objUnderCursor, isChaining: false);
                Injector.Get<CommandManager>().QueueCommand(command);
            }
            else
            {
                var destination = ConvertToWorld(cursorPosition);
                var command = new TargetPositionCommand(team, units, destination, isChaining: false);
                Injector.Get<CommandManager>().QueueCommand(command);
            }
        }

        //Selection finished
        if (_isSelecting && IsSelectionFinished())
        {
            _isSelecting = false;
            _selectionEnd = GetSelectionPosition();

            _selectionCache.Clear();
            SelectObjectUnder(_selectionEnd);
            SelectObjects(_selectionStart, _selectionEnd);
            
            if(_selectionCache.Count > 0)
            {
                //Queue up the command
                var selectionArray = _selectionCache.ToArray();
                var localTeam = Injector.Get<GameState>().LocalTeam;
                var command = new SelectCommand(localTeam, selectionArray);
                Injector.Get<CommandManager>().QueueCommand(command);
            }

            if (Image != null)
            {
                Image.enabled = false;
            }
        }
        //Selection started
        else if (IsSelectionStarted())
        {
            _isSelecting = true;
            _selectionStart = GetSelectionPosition();

            var localTeam = Injector.Get<GameState>().LocalTeam;
            Injector.Get<CommandManager>().QueueCommand(new UnselectAllCommand(localTeam));

            if (Image != null)
            {
                Image.enabled = true;
            }

            /*
            Debug.Log("Position: " + _selectionStart.x + " " + _selectionStart.y 
                + Camera.main.ScreenToWorldPoint(new Vector3(_selectionStart.x, _selectionStart.y, 10f)).ToString());
            */
        }

        //Adjust rectangle display
        if (RectTransform != null)
        {
            if (_isSelecting)
            {
                var startPosition = _selectionStart;
                var endPosition = GetSelectionPosition();
                var diff = startPosition + (endPosition - startPosition) / 2;
                var height = Mathf.Abs(endPosition.y - startPosition.y);
                var width = Mathf.Abs(endPosition.x - startPosition.x);

                RectTransform.position = diff;
                RectTransform.sizeDelta = new Vector2(width, height);
            }
        }
    }

    private Vector3 ConvertToWorld(Vector2 screenPosition)
    {
        var position = new Vector3(screenPosition.x, screenPosition.y, 10.1f);
        return Camera.main.ScreenToWorldPoint(position);
    }

    private void SelectObjects(Vector2 startPosition, Vector2 endPosition)
    {
        var gameState = Injector.Get<GameState>();

        float left = Mathf.Min(startPosition.x, endPosition.x);
        float top = Mathf.Max(startPosition.y, endPosition.y);
        float bottom = Mathf.Min(startPosition.y, endPosition.y);
        float right = Mathf.Max(startPosition.x, endPosition.x);
        var rect = new Rect(left, bottom, right - left, top - bottom);
        
        _selectionCache.Clear();
        foreach (var rtsObj in gameState.RtsObjects.Values)
        {
            var objPos = Camera.main.WorldToScreenPoint(rtsObj.transform.position);

            if (rect.Contains(objPos))
            {
                _selectionCache.Add(rtsObj);
            }
        }
    }

    private void SelectObjectUnder(Vector2 cursorPosition)
    {
        var obj = GetObjectUnder(cursorPosition);
        if (obj != null && obj.IsSelectable)
        {
            if(!_selectionCache.Contains(obj))
            {
                _selectionCache.Add(obj);
            }
        }
    }

    private RtsObject GetObjectUnder(Vector2 cursorPosition)
    {
        RtsObject obj = null;
        var ray = Camera.main.ScreenPointToRay(cursorPosition);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction * 100, out hit))
        {
            obj = hit.collider.gameObject.GetComponent<RtsObject>();
        }
        return obj;
    }

    private bool IsSelectionStarted()
    {
        return Input.GetMouseButtonDown(0);
    }

    private bool IsSelectionFinished()
    {
        return Input.GetMouseButtonUp(0) || IsActionTriggered();
    }

    private Vector2 GetSelectionPosition()
    {
        return Input.mousePosition;
    }

    private bool IsActionTriggered()
    {
        return Input.GetMouseButtonDown(1);
    }

    private Vector2 GetActionPosition()
    {
        return Input.mousePosition;
    }
}
