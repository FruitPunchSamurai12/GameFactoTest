using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviourPunCallbacks
{
    private static Dictionary<PooledMonoBehaviour, Pool> pools = new Dictionary<PooledMonoBehaviour, Pool>();

    private Queue<PooledMonoBehaviour> objects = new Queue<PooledMonoBehaviour>();

    private PooledMonoBehaviour prefab;

    public static void InitializePool(PooledMonoBehaviour prefab)
    {
        var pool = PhotonNetwork.InstantiateRoomObject("Pool", Vector3.zero, Quaternion.identity).GetComponent<Pool>();
        pool.prefab = prefab;

        pools.Add(prefab, pool);
        for (int i = 0; i < prefab.InitialPoolSize; i++)
        {
            //on the client when an object is pulled from the pool it first becomes active before changing position and you can see it in the middle of the level for a brief second. that's why i initialize them far away
            var pooledObjectView = PhotonNetwork.InstantiateRoomObject(prefab.name, new Vector3(-100,-100,-100), Quaternion.identity).GetComponent<PhotonView>();
            pool.photonView.RPC(nameof(RPC_InitializePooledObject), RpcTarget.All, pooledObjectView.ViewID);
        }
    }

    public static Pool GetPool(PooledMonoBehaviour prefab)
    {
        if (pools.ContainsKey(prefab))
            return pools[prefab];

        var pool = PhotonNetwork.Instantiate("Pool", Vector3.zero, Quaternion.identity).GetComponent<Pool>();
        pool.prefab = prefab;

        pools.Add(prefab, pool);
        return pool;
    }

    public T Get<T>() where T:PooledMonoBehaviour
    {
        if(objects.Count == 0)
        {
            GrowPool();
        }

        var pooledObject = objects.Dequeue();
        return pooledObject as T;
    }

    private void GrowPool()
    {
        for (int i = 0; i < prefab.InitialPoolSize; i++)
        {
            var pooledObject = PhotonNetwork.Instantiate(prefab.name,transform.position,Quaternion.identity).GetComponent<PooledMonoBehaviour>();
            //var pooledObject = Instantiate(prefab) as PooledMonoBehaviour;
            //pooledObject.gameObject.name += " " + i;
            //photonView.RPC(nameof(RPC_InitializePooledObject), RpcTarget.All, pooledObjectView.ViewID);
            pooledObject.OnReturnToPool += AddObjectToAvailableQueue;
            pooledObject.transform.SetParent(transform);
            pooledObject.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void RPC_InitializePooledObject(int photonViewID)
    {
        PhotonView view = PhotonView.Find(photonViewID);
        if (view != null)
        {
            view.GetComponent<PooledMonoBehaviour>().OnReturnToPool+=AddObjectToAvailableQueue;
            view.transform.SetParent(transform);
            view.gameObject.SetActive(false);
        }
    }

    private void AddObjectToAvailableQueue(PooledMonoBehaviour pooledObject)
    {
        pooledObject.transform.SetParent(this.transform);
        objects.Enqueue(pooledObject);
    }

    private void OnDestroy()
    {
        if (prefab !=null && pools.ContainsKey(prefab))
            pools.Remove(prefab);
    }
}
