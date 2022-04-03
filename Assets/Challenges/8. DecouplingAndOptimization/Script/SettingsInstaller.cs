using Challenges._8._DecouplingAndOptimization.Script;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
{
    public GameTimeMaterialTintSettings Settings;

    public override void InstallBindings()
    {
        Container.BindInstances(Settings);
    }
}