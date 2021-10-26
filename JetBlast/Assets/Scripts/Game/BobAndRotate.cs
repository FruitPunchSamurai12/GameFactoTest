using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    [SerializeField]
    float verticalBobFrequency = 1f;
    [SerializeField]
    float bobbingAmount = 1f;
    [SerializeField]
    float rotatingSpeed = 360f;


    Vector3 m_StartPosition;

    void Start()
    {
        m_StartPosition = transform.position;
    }

    void Update()
    {
        float bobbingAnimationPhase = ((Mathf.Sin(Time.time * verticalBobFrequency) * 0.5f) + 0.5f) * bobbingAmount;
        transform.position = m_StartPosition + Vector3.up * bobbingAnimationPhase;

        transform.Rotate(Vector3.up, rotatingSpeed * Time.deltaTime, Space.Self);
    }
}
