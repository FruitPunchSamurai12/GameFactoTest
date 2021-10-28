using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsCanvases : MonoBehaviour
{
    [SerializeField]
    CanvasGroup createOrJoinRoom;

    [SerializeField]
    CanvasGroup currentRoom;

    [SerializeField]
    CanvasGroup loadScreen;

    public void Connected()
    {
        HideCanvasGroup(loadScreen);
        ShowCanvasGroup(createOrJoinRoom);
    }

    public void EnterRoom()
    {
        HideCanvasGroup(createOrJoinRoom);
        ShowCanvasGroup(currentRoom);
    }

    public void ExitRoom()
    {
        HideCanvasGroup(currentRoom);
        ShowCanvasGroup(createOrJoinRoom);
    }


    void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }


}
