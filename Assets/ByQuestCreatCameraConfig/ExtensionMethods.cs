using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 ToFloatArray(this float[] arr)
    {
        if (arr == null || arr.Length < 3)
        {
            return Vector3.zero;
        }

        return new Vector3(arr[0], arr[1], arr[2]);
    }

    public static float[] ToVector3(this Vector3 vct)
    {
        return new[] { vct.x, vct.y, vct.z };
    }
}