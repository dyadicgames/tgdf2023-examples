using System;
using UnityEngine;

namespace Example1
{
    // Simple class representing a star object to be read from JSON.
    [Serializable]
    public struct Star
    {
        public float innerRadius;
        public float outerRadius;
        public int corners;

        public static Star FromJsonString(string jsonString)
        {
            return JsonUtility.FromJson<Star>(jsonString);
        }
    }
}
