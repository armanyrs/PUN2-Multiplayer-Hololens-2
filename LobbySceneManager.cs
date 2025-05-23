using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject loadingMenu;
    public GameObject lobbyMenu;

    void Start()
    {
        loadingMenu.SetActive(true);
        lobbyMenu.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        loadingMenu.SetActive(false);  
        lobbyMenu.SetActive(true);    
    }

    public void CreateRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
    }

    public void JoinRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room, loading main scene");
        PhotonNetwork.LoadLevel("main"); 
    }
}
