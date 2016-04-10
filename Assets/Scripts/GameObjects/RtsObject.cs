using UnityEngine;

public class RtsObject : MonoBehaviour
{
    public int Id;
    public SelectableObject MySelectableObject;
    
    public bool IsAlive;
    
    //Can be attacked and killed.
    public bool IsAttackable;
    
    //Can be targetted and moved to
    public bool IsTargetable;
    
    public int Team;
    
    protected virtual void Awake()
    {
        MySelectableObject = GetComponent<SelectableObject>();
    }
    
    protected virtual void Start()
    {
        IsAlive = true;
    }
    
    public bool IsSelectable
    {
        get
        {
            return MySelectableObject != null;
        }
    }
    
    public virtual void OnTargeted(Unit attacker, bool isChaining)
    {
        //Do nothing
    }
    
    public bool Equals (RtsObject obj)
    {
        if (obj == null || Id != obj.Id)
        {
            return false;
        }
        
        return true;
    }
}
