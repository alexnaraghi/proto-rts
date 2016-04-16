using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public RectTransform RectTransform;
    public Image Image;

    private bool _isSelecting;
    private Vector2 _selectionStart;
    private Vector2 _selectionEnd;

    public List<string> DebugSelectionLog = new List<string>();
    
    private List<RtsObject> _selectionCache = new List<RtsObject>(200);
    private List<Unit> _unitCache = new List<Unit>(200);

    private RtsInput _input;

    void Start()
    {
        // TODO: Make dependency injection support compile time or runtime resolution of interfaces/
        // abstract classes
        _input = Injector.Get<RtsInput>();
    }

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
        if (_input.IsActionTriggered())
        {
            var cursorPosition = _input.GetActionPosition();

            // don't select anything outsize the map bounds.
            var bounds = Injector.Get<GameState>().MapBounds;
            var worldPosition = ConvertToWorld(cursorPosition);
            
            bool isInBounds = Mathf.Abs(worldPosition.x) < bounds.x
                                && Mathf.Abs(worldPosition.z) < bounds.z;

            if (isInBounds)
            {
                var objUnderCursor = GetObjectUnder(cursorPosition);

                var selectedIds = Injector.Get<GameState>().SelectedObjectIds;
                var team = Injector.Get<GameState>().LocalTeam;

                _unitCache.Clear();
                ForeachSelectedObject(selectedIds, rtsObj =>
                    {
                        var unit = rtsObj.GetComponent<Unit>();
                        if (unit)
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
                    DebugSelectionLog.Add(string.Format("Targeted [{0} ({1},{2})]",
                        objUnderCursor.ToString(),
                        objUnderCursor.transform.position.x,
                        objUnderCursor.transform.position.z));

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
        }

        //Selection finished
        if (_isSelecting && _input.IsSelectionFinished())
        {
            _isSelecting = false;
            _selectionEnd = _input.GetSelectionPosition();

            _selectionCache.Clear();
            SelectObjectUnder(_selectionEnd);
            SelectObjects(_selectionStart, _selectionEnd);

            if (_selectionCache.Count > 0)
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
        else if (_input.IsSelectionStarted())
        {
            _isSelecting = true;
            _selectionStart = _input.GetSelectionPosition();

            var localTeam = Injector.Get<GameState>().LocalTeam;
            Injector.Get<CommandManager>().QueueCommand(new UnselectAllCommand(localTeam));

            if (Image != null)
            {
                Image.enabled = true;
            }

            /*
            Debug.Log("Position: " + _selectionStart.x + " " + _selectionStart.y 
                + Camera.main.ScreenToWorldPoint(new Vector3(_selectionStart.x,
                                                             _selectionStart.y, 
                                                             10f)).ToString());
            */
        }

        //Camera zooming
        {
            const float ZOOM_SPEED_SCALAR = 15f;

            //The orthographic size limits
            const float MIN_ZOOM_SIZE = 4f;
            const float MAX_ZOOM_SIZE = 50f;

            var distance = _input.GetSpeedZ() * ZOOM_SPEED_SCALAR;
            var currentSize = Camera.main.orthographicSize;
            
            Camera.main.orthographicSize = Mathf.Clamp(currentSize - distance, 
                                                       MIN_ZOOM_SIZE, 
                                                       MAX_ZOOM_SIZE);
        }

        //Camera panning
        if (_input.IsPanning())
        {
            const float MOVE_SPEED_SCALAR = 0.05f;

            //Get the bounds of the unit movement.
            var bounds = Injector.Get<GameState>().MapBounds;

            float moveSpeed = Camera.main.orthographicSize * MOVE_SPEED_SCALAR;

            var position0 = Camera.main.transform.position;
            var x = Mathf.Clamp(position0.x - _input.GetSpeedX() * moveSpeed, -bounds.x, bounds.x);
            var z = Mathf.Clamp(position0.z - _input.GetSpeedY() * moveSpeed, -bounds.z, bounds.z);
            
            Camera.main.transform.position = new Vector3(x, position0.y, z);
        }

        //Adjust rectangle display
        if (RectTransform != null)
        {
            if (_isSelecting)
            {
                var startPosition = _selectionStart;
                var endPosition = _input.GetSelectionPosition();
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
}
