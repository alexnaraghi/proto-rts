using UnityEngine;

public class GameObjectFactory : MonoBehaviour
{
    public int _idIndex = 0;
    public T CreateRtsObject<T>(T prefab) where T : RtsObject
    {
        var obj = GameObject.Instantiate(prefab);
        obj.Id = _idIndex++;
        Injector.Get<GameState>().RtsObjects.Add(obj.Id, obj);
        return obj;
    }
}
