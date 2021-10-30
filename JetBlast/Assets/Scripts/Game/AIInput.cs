using System;
using UnityEngine;

[SelectionBase]
public class AIInput : MonoBehaviour, IPlayerInput
{
    bool move;
    float tapX;
    public bool Move => move;

    public float TapX => tapX;

    PlayerController pc;
    float leftRightInputOffset;
    public Vector3 targetCoverPosition;
    bool lastStretch = false;
    Camera cam;
    Vector3 endPosition;
    private void Awake()
    {
        cam = Camera.main;
        pc = GetComponent<PlayerController>();
        pc.onCover += HandleOnCover;
        pc.onStunEnd += HandleWindReset;
        leftRightInputOffset = pc.LeftRightInputOffset;
    }

    private void Start()
    {
        var spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.onStopSpawningCovers += HandleCoversStopSpawning;
        JetEngine jetEngine = FindObjectOfType<JetEngine>();
        jetEngine.onWindReset += HandleWindReset;
        jetEngine.onWindStart += HandleWindReset;
        endPosition = FindObjectOfType<WinZone>().transform.position;
    }


    void HandleCoversStopSpawning()
    {
        targetCoverPosition = endPosition;
        lastStretch = true;
    }

    void HandleOnCover(bool onCover)
    {
        if (onCover)
        {
            if (lastStretch)
                move = true;
            else if (targetCoverPosition.z - transform.position.z > 3f)
                move = true;
            else
                move = false;
        }
        else
            move = true;
    }

    void HandleWindReset()
    {
        move = true;
    }

    void Update()
    {
        if (targetCoverPosition.x < transform.position.x-transform.localScale.x/2)
            tapX = cam.WorldToScreenPoint(transform.position).x - leftRightInputOffset-1;
        else if (targetCoverPosition.x > transform.position.x+ transform.localScale.x / 2)
            tapX = cam.WorldToScreenPoint(transform.position).x + leftRightInputOffset+1;
        else
            tapX = cam.WorldToScreenPoint(transform.position).x;
    }

    public void SetCoverTarget(Vector3 coverPos)
    {
        targetCoverPosition = coverPos;
    }
}