using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Realtime;

public class EndGameMenu : MonoBehaviourPun
{
    [SerializeField]
    GameObject endGamePanel;
    [SerializeField]
    TextMeshProUGUI nameOfWinnerText;
    [SerializeField]
    TextMeshProUGUI messageToPlayerText;

    void Start()
    {
        FindObjectOfType<WinZone>().onGameEnd += HandleGameEnd;
    }

    void HandleGameEnd(int winnerPlayerNumber)
    {
        endGamePanel.SetActive(true);
        if(PhotonNetwork.LocalPlayer.GetPlayerNumber()==winnerPlayerNumber)
        {
            nameOfWinnerText.SetText("You won!");
            messageToPlayerText.SetText("Congrats!");
            AudioManager.Instance.PlaySoundEffect2D("Victory");
        }
        else
        {
            Player winner = PhotonNetwork.PlayerList.FirstOrDefault(t => t.GetPlayerNumber() == winnerPlayerNumber);
            string winnerName = "";     
            if (winner.CustomProperties.ContainsKey("Nickname"))
                winnerName = winner.CustomProperties["Nickname"].ToString();
            nameOfWinnerText.SetText($"{winnerName} won!");
            messageToPlayerText.SetText("Better luck next time!");
        }
    }

    public void OnClickBackToMenu()
    {
        AudioManager.Instance.PlaySoundEffect2D("Button");
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
