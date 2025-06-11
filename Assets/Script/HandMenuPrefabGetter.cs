using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class HandMenuPrefabGetter : MonoBehaviour
{
    [SerializeField] private GameObject menuSpawn;
    [SerializeField] private GameObject menuChat;
    [Space]
    [SerializeField] private Interactable spawnButton;
    [SerializeField] private Interactable chatButton;
    [SerializeField] private Interactable speakButton;
    [HideInInspector]
    public enum ButtonType
    {
        spawnButton,
        chatButton,
        speakButton
    }

    public Interactable GetButton(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.spawnButton:
                return spawnButton;
            case ButtonType.chatButton:
                return chatButton;
            case ButtonType.speakButton:
                return speakButton;
            default:
                return spawnButton;
        }
    }

    public GameObject GetMenu(ButtonType menuButton)
    {
        if (menuButton == ButtonType.spawnButton)
        {
            return menuSpawn;
        }
        else
        {
            return menuChat;
        }
    }
}