using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GizmoDataInstaller", menuName = "Installers/GizmoDataInstaller")]
public class GizmoDataInstaller : ScriptableObjectInstaller<GizmoDataInstaller>
{
    public GizmoData GizmoDataObject;

    public override void InstallBindings()
    {
        Container.BindInstances(GizmoDataObject);
    }
}