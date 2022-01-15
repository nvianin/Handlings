using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils
{
    public static float Map(float val, float min, float max, float min1, float max1)
    {
        return min1 + (max1 - min1) * (val - min) / (max - min);
    }
}