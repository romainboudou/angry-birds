using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<GameObject, Queue<GameObject>> pool = new();

    void Awake()
    {
        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        if (pool[prefab].Count > 0)
        {
            GameObject obj = pool[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return Instantiate(prefab, position, rotation);
        }
    }

    public void Despawn(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
    }
}
