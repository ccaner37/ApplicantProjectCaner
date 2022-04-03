using System;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges._4._Gizmos.Scripts
{
    public class Node : MonoBehaviour
    {
        [SerializeField]
        private List<Node> childrenNodes;
        //Edit below

        [SerializeField]
        private Mesh _arrowMesh;

        [SerializeField]
        private float _arrowDistance = 0.25f;

        [SerializeField]
        private GizmoData _gizmoData;

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoData.GizmoColor;

            for (int i = 0; i < childrenNodes.Count; i++)
            {
                Vector3 childrenPos = childrenNodes[i].transform.position;
                Vector3 pos = Vector3.Lerp(transform.position, childrenPos, _arrowDistance);
                Vector3 relativePos = childrenPos - transform.position;

                Gizmos.DrawLine(transform.position, childrenPos);
                Gizmos.DrawMesh(_arrowMesh, pos, Quaternion.LookRotation(-relativePos, Vector3.up));
            }
        }
    }
}
