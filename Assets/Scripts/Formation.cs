using UnityEngine;
using System.Collections.Generic;

public enum FormationType
{
    Scattered
}
public class Formation 
{
    public static void Form(Unit[] units, 
        Vector3 destination, 
        FormationType formationType, bool isChaining)
    {
        //Ignore formations if we just have one object.  If that's the case, just move it
        if(units.Length == 1)
        {
            units[0].PushState(new MovingState(destination), isChaining);
        }
        else if(formationType == FormationType.Scattered)
        {
            //What units?
            float radius = units.Length;
            
            //TODO: Make this deterministic.
            foreach(var unit in units)
            {
                float randomRot = UnityEngine.Random.value;
                float randomMag = UnityEngine.Random.value;
                var rotation = 2 * Mathf.PI * randomRot;
                var offset = new Vector3(
                    Mathf.Cos(rotation) / 2f, 0f, 
                    Mathf.Sin(rotation) / 2f) * radius * randomMag;
                
                unit.PushState(new MovingState(destination + offset), isChaining);
            }
        }
        else
        {
            Debug.LogWarning("Formation not yet implemented: " + formationType);
        }
    }
}
