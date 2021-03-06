﻿using UnityEngine;
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
        //The range that we add for each new unit in the formation.
        const float RADIUS_MAGNITUDE_SCALAR = 0.4f;
        
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
                float randomRot = RtsRandom.Value;
                float randomMag = RtsRandom.Value;
                var rotation = 2 * Mathf.PI * randomRot;

                //These formations are getting too damn big.  Temporarily keep them in line.
                var clampedMagnitude = Mathf.Clamp(radius * RADIUS_MAGNITUDE_SCALAR, 1f, 40f);

                var offset = new Vector3(
                    Mathf.Cos(rotation) / 2f, 
                    0f, 
                    Mathf.Sin(rotation) / 2f) * randomMag * clampedMagnitude;
                
                unit.PushState(new MovingState(destination + offset), isChaining);
            }
        }
        else
        {
            Debug.LogWarning("Formation not yet implemented: " + formationType);
        }
    }
}
