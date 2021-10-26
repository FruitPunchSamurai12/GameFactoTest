using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PooledMonoBehaviour : MonoBehaviourPunCallbacks
{
    [SerializeField] int initialPoolSize = 50;

    public event Action<PooledMonoBehaviour> OnReturnToPool;

    public int InitialPoolSize { get { return initialPoolSize; } }


    public void InitializePool()
    {
        Pool.InitializePool(this);
    }

    public T Get<T>(bool enable = true) where T:PooledMonoBehaviour
    {
        var pool = Pool.GetPool(this);
        var pooledObject = pool.Get<T>();

        if (enable)
        {
            PhotonView pv = pooledObject.GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC(nameof(RPC_ToggleObject), RpcTarget.All, pv.ViewID,true);
            }
        }

        return pooledObject;
    }

    [PunRPC]
    public void RPC_ToggleObject(int viewID, bool activate)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
            pv.gameObject.SetActive(activate);
    }

    public T Get<T>(Vector3 position,Quaternion rotation) where T : PooledMonoBehaviour
    {
        var pooledObject = Get<T>();
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        return pooledObject;
    }

    public T Get<T>(Vector2 position, Quaternion rotation) where T : PooledMonoBehaviour
    {
        return Get<T>(new Vector3(position.x,position.y,0), rotation);
    }


    public override void OnDisable()
    {
        base.OnDisable();
        OnReturnToPool?.Invoke(this);
    }

    protected void ReturnToPool(float delay = 0)
    {
        StartCoroutine(ReturnToPoolAfterSeconds(delay));
    }

    private IEnumerator ReturnToPoolAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC(nameof(RPC_ToggleObject), RpcTarget.All, photonView.ViewID, false);
        
    }
}