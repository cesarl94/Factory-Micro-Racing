using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorNode : MonoBehaviour
{
    public Vector2 anchorPoint;

    void Update()
    {
        transform.localPosition = new Vector3(-Screen.width / 2 + Screen.width * anchorPoint.x, Screen.height / 2 - Screen.height * anchorPoint.y, 0);
    }
}
