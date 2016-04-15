using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class Aggro : MonoBehaviour 
{
	public List<Aggro> AggroInRange;

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
            for(int i = 0; i < AggroInRange.Count; i++)
			{
				if(AggroInRange[i].AttachedParent.Team != myTeam)
				{
                    target = AggroInRange[i].AttachedParent;
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
		for(int i = 0; i < AggroInRange.Count; i++)
		{
			if(AggroInRange[i] == null 
				|| AggroInRange[i].AttachedParent == null 
				|| !AggroInRange[i].AttachedParent.IsAlive)
			{
                AggroInRange.RemoveAt(i);
                i--;
            }
		}
	}

    void Awake()
	{
        AggroInRange = new List<Aggro>();
        Assert.IsTrue(AttachedParent != null);
    }
	
	void OnTriggerEnter(Collider other)
    {
        var otherAggro = other.gameObject.GetComponent<Aggro>();

        if (otherAggro != null)
        {
            AggroInRange.Add(otherAggro);
        }
    }

    void OnTriggerExit(Collider other)
    {
		var otherAggro = other.gameObject.GetComponent<Aggro>();

        if (otherAggro != null)
        {
            AggroInRange.Remove(otherAggro);
        }
    }
}
