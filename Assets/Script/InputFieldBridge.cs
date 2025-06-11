using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;

public class InputFieldBridge : MonoBehaviour
{
    public MRTKUGUIInputField inputField;
    public LobbySceneManager manager;

    public void CallCreateRoom()
    {
        manager.CreateRoom(inputField.text);
    }

    public void CallJoinRoom()
    {
        manager.JoinRoom(inputField.text);
    }
}
