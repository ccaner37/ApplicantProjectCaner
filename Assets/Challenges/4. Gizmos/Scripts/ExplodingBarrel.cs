using System;
using UnityEditor;
using UnityEngine;

namespace Challenges._4._Gizmos.Scripts
{
    public class ExplodingBarrel : MonoBehaviour
    {
        [SerializeField]
        private ExplodingBarrelData explodingBarrelData;
        //Edit below

        [SerializeField]
        private GizmoData _gizmoData;

        private GUIStyle _textStyle;

        private void OnDrawGizmos()
        {
            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = _gizmoData.TextSize,
                fontStyle = _gizmoData.TextFontStyle
            };

            Gizmos.color = _gizmoData.GizmoColor;
            Gizmos.DrawWireSphere(transform.position, explodingBarrelData.ExplosionRadius);

            Handles.Label(transform.position + Vector3.up, explodingBarrelData.Damage.ToString(), _textStyle);
            Handles.Label(transform.position, explodingBarrelData.DamageType.ToString(), _textStyle);
        }
    }
}
