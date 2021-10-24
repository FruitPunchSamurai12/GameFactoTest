using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEngine : MonoBehaviour
{
    [SerializeField]
    float blowWindThreshold = 5f;
    [SerializeField]
    float blowWindDuration = 3f;
    [SerializeField]
    float windForce = 25f;

    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>blowWindThreshold)
        {
            foreach (var pc in GameManager.Instance.GetPlayers())
            {
                if (pc!=null && !pc.InCover)
                    pc.GetBlownAway((pc.transform.position - transform.position).normalized * windForce);
            }
            if (timer > blowWindThreshold + blowWindDuration)
                timer = 0;
        }
    }
}
