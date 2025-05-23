using UnityEngine;
using Photon.Pun;

public class MainSceneManager : MonoBehaviourPunCallbacks
{

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am MasterClient, spawning the cheese!");
            PhotonNetwork.Instantiate("Cheese", Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("I am NOT MasterClient, cheese will be spawned by someone else.");
        }

    }
}