using System;
using UnityEngine;

namespace Challenges._4._Gizmos.Scripts
{
    public class ClusterBomb : MonoBehaviour
    {
        [SerializeField]
        private ClusterBombData clusterBombData;
        //Edit below

        float a = 0f;
        float h = 1.6f;
        float d = 5f;
        float b = 3f;

        // Not working...

        private void OnDrawGizmos()
        {
            float x = 5f;
            float result = Mathf.Abs(l(x) * f(j(x)));
        }

        private float f(float x)
        {
            return h * Mathf.Pow(x + 1, -a);
        }

        private float l(float x)
        {
            return Mathf.Sin(((b * x) / d) * Mathf.PI);
        }

        private float j(float x)
        {
            float floor = Mathf.Floor((x * b) / d);
            return (floor * d) / b + d / (2 * b);
        }
    }
}
