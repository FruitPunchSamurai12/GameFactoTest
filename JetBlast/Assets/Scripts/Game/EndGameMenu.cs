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

    WinZone winZone;

    void Start()
    {
        winZone = FindObjectOfType<WinZone>();
        winZone.onGameEnd += HandleGameEnd;
        GameManager.Instance.onLocalPlayerDeath += HandleLocalPlayerDeath;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onLocalPlayerDeath -= HandleLocalPlayerDeath;
    }

    void HandleLocalPlayerDeath()
    {
        winZone.onGameEnd -= HandleGameEnd;
        endGamePanel.SetActive(true);
        nameOfWinnerText.SetText("You died horribly!");
        messageToPlayerText.SetText("Better luck next time!");
    }

    void HandleGameEnd(int winnerPlayerNumber)
    {
        endGamePanel.SetActive(true);
        if(PhotonNetwork.LocalPlayer.GetPlayerNumber()==winnerPlayerNumber)
        {
            nameOfWinnerText.SetText("You won!");
            messageToPlayerText.SetText("Congrats!");
            AudioManager.Instance.PlaySoundEffect3D("Victory",PlayerController.LocalPlayerInstance.transform.position);
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
