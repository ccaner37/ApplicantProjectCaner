using System;
using UnityEditor;
using UnityEngine;

namespace Challenges._4._Gizmos.Scripts
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField]
        private Transform point1;
        [SerializeField]
        private Transform handle1;
        [SerializeField]
        private Transform point2;
        [SerializeField]
        private Transform handle2;
        //Edit below

        [SerializeField]
        private GizmoData _gizmoData;

        [SerializeField]
        private Texture2D _texture;

        private void OnDrawGizmos()
        {
            Handles.DrawBezier(point1.position, point2.position, handle1.position, handle2.position, _gizmoData.GizmoColor, _texture,_gizmoData.LineWidth);
        }

        // -- Mathematical Method (Alternative)
        /*
            private Vector3 gizmosPosition;
            for (float t = 0; t <= 1; t += 0.05f)
            {
                gizmosPosition = Mathf.Pow(1 - t, 3) * point1.position + 3 * Mathf.Pow(1 - t, 2) * t * handle1.position + 3 * (1 - t) * Mathf.Pow(t, 2) * handle2.position + Mathf.Pow(t, 3) * point2.position;
                Gizmos.DrawSphere(gizmosPosition, 0.25f);
            }

            Gizmos.DrawLine(new Vector3(point1.position.x, point1.position.y, point1.position.z), new Vector3(handle1.position.x, handle1.position.y, handle1.position.z));
            Gizmos.DrawLine(new Vector3(handle2.position.x, handle2.position.y, handle2.position.z), new Vector3(point2.position.x, point2.position.y, point2.position.z));

         */
    }
}
