using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    IObjectPool<ObjectPool> PoolManager;
    public GameObject ObjectPrefab;
    void Update()
    {
        PoolManager = new ObjectPool<ObjectPool>(CreateObject, OnGetObj, OnReleaseObj, OnDestroyObj, true, 15, 100);
    }
        public void SetManager(IObjectPool<ObjectPool> poolManager)
    {
        PoolManager = poolManager;
    }

    private ObjectPool CreateObject()
    {
        ObjectPool Object = Instantiate(ObjectPrefab).GetComponent<ObjectPool>();
        Object.SetManager(PoolManager);
        return Object;
    }

    void OnGetObj(ObjectPool bomb)
    {
        bomb.gameObject.SetActive(true);
    }

    void OnReleaseObj(ObjectPool bomb)
    {
        bomb.gameObject.SetActive(false);
    }

    void OnDestroyObj(ObjectPool bomb)
    {
        Destroy(bomb.gameObject);
    }
}
