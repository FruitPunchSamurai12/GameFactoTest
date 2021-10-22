using UnityEngine;

[CreateAssetMenu(menuName ="Singletons/MasterManager")]
public class MasterManager:SingletonScriptableObject<MasterManager>
{
    [SerializeField]
    GameSettings gameSettings;
    public static GameSettings GameSettings => Instance.gameSettings;
}
