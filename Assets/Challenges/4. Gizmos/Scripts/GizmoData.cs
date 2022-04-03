using UnityEngine;

[CreateAssetMenu(fileName = "GizmoData", menuName = "GizmoChallenge/GizmoData")]
public class GizmoData : ScriptableObject
{
    [SerializeField]
    private Color color;

    [SerializeField]
    private float lineWidth;

    [SerializeField]
    private int textSize;

    [SerializeField]
    private FontStyle fontStyle;

    public Color GizmoColor => color;

    public float LineWidth => lineWidth;

    public int TextSize => textSize;

    public FontStyle TextFontStyle => fontStyle;
}
