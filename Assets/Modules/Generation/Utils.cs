using System.Collections.Generic;
using UnityEngine;

namespace Generation.Noise
{
    public static class Utils
    {
        public static bool IsTesting()
        {
            #if UNITY_EDITOR
            return Application.isEditor && !Application.isPlaying;
            #else
            return false;
            #endif
        }
        
        public static int AddVertex(this Dictionary<Vector3, int> dict, Vector3 v)
        {
            if (dict.TryGetValue(v, out int i))
                return i;

            dict.Add(v, dict.Count);
            return dict.Count - 1;
        }
        
        public static Vector3[] ToVertexArray(this Dictionary<Vector3, int> dict)
        {
            Vector3[] arr                                         = new Vector3[dict.Count];
            foreach ((Vector3 key, int value) in dict) arr[value] = key;

            return arr;
        }

    }
}