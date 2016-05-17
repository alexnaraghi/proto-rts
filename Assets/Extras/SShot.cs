using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SShot : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("Screen shot!");
            Application.CaptureScreenshot("Screenshot.png", 2);
        }
    }
}