using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    public bool Move => Input.GetMouseButton(0);
    public float TapX => Input.mousePosition.x; 
}
