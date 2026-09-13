using UnityEngine;

public static class Vector3Extensions
{
    public static bool IsValid(this Vector3 vector)
    {
        return float.IsNaN(vector.sqrMagnitude) == false && 
            float.IsInfinity(vector.sqrMagnitude) == false;
    }
}
