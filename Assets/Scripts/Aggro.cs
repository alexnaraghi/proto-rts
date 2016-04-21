using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class Aggro : MonoBehaviour 
{
	public LinkedList<Aggro> AggroInRange;

    public RtsObject AttachedParent;

	public RtsObject Target
	{
		get
		{
            var myTeam = AttachedParent.Team;
			
			if(myTeam == 0)
			{
                return null;
            }

            RtsObject target = null;			
            foreach(var item in AggroInRange)
			{
				if(item.AttachedParent.Team != myTeam)
				{
                    target = item.AttachedParent;
                    break;
                }
			}

            return target;
        }
	}
	
	void Update()
	{
        // Hmmmmmmm is there a better way to clean the aggro list out or is this
        // good enough.
        var node = AggroInRange.First;
        while(node != null)
        {
            var next = node.Next;
            if(node.Value == null 
                || node.Value.AttachedParent == null 
                || !node.Value.AttachedParent.IsAlive)
            {
                AggroInRange.Remove(node);
            }
            node = next;
        }
	}

    void Awake()
	{
        AggroInRange = new LinkedList<Aggro>();
        Assert.IsTrue(AttachedParent != null);
    }
	
	void OnTriggerEnter(Collider other)
    {
        var otherAggro = other.gameObject.GetComponent<Aggro>();

        if (otherAggro != null)
        {
            //Warning: very ineficient
            AggroInRange.AddLast(otherAggro);
        }
    }

    void OnTriggerExit(Collider other)
    {
		var otherAggro = other.gameObject.GetComponent<Aggro>();

        if (otherAggro != null)
        {
            //Warning: very ineficient            
            AggroInRange.Remove(otherAggro);
        }
    }
}
