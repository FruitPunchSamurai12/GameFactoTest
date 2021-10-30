using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput:MonoBehaviour,IPlayerInput
{
    public bool Move => Input.touchCount>0 || Input.GetMouseButton(0);
    public float TapX =>Input.touchCount>0?Input.GetTouch(0).position.x:0;

}
