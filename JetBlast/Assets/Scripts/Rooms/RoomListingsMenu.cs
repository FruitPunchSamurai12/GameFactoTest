using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    RoomListing roomListingPrefab;
    [SerializeField]
    Transform content;

    List<RoomListing> listings = new List<RoomListing>();


    public override void OnJoinedRoom()
    {
        content.DestroyChildren();
        listings.Clear();
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = listings.FindIndex(t => t.RoomInfo.Name == info.Name);
                if(index!=-1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            }
            else
            {
                int index = listings.FindIndex(t => t.RoomInfo.Name == info.Name);
                if (index == -1)
                {
                    RoomListing listing = Instantiate(roomListingPrefab, content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        listings.Add(listing);
                    }
                }
                else
                {
                    listings[index].SetRoomInfo(info);
                }
            }
        }
    }

}
