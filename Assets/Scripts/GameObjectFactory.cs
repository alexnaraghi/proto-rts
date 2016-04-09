using UnityEngine;
using System.Collections;

public class GameObjectFactory : MonoBehaviour
{
    public int _idIndex = 0;
    public RtsObject CreateRtsObject(RtsObject prefab)
    {
        var obj = GameObject.Instantiate(prefab);
        obj.Id = _idIndex++;
        return obj;
    }
}
