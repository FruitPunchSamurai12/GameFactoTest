using UnityEngine;

[CreateAssetMenu(menuName ="Manager/GameSettings")]
public class GameSettings:ScriptableObject
{
    [SerializeField]
    string gameVersion = "0.0.0";
    public string GameVersion => gameVersion;
    [SerializeField]
    string nickName = "Kitsios";
    public string NickName => nickName;

    [SerializeField]
    int maxPlayers = 5;
    public int MaxPlayers => maxPlayers;
}