using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
    public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
}
