using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class RobotEntryPoint : MonoBehaviour
{
    public GameObject sceneContent;
    public GameObject mainCamera;
    [Header("prefabs")]
    public GameObject globalManagerPrefab;
    public GameObject handMenuPrefab;
    public GameObject robotCharacterPrefab;
    public GameObject chatboxUiPrefab;

    GameObject globalManager;
    GameObject handMenu;
    GameObject robotCharacter;
    GameObject chatboxUi;
    AzureAiHandler azureAiHandler;
    DictationController dictationController;
    CustomSubmitButton customSubmitButton;
    RobotScript robotScript;
    RoboPetFollowController roboPetFollowController;
    HandMenuPrefabGetter handMenuPrefabGetter;
    MRTKUGUIInputField mrtkuguiInputField;
    GameObject menuSpawn;
    GameObject menuChat;
    Interactable spawnButton;
    Interactable chatButton;
    Interactable speakButton;

    void Start()
    {
        globalManager = Instantiate(globalManagerPrefab, sceneContent.transform);
        handMenu = Instantiate(handMenuPrefab, sceneContent.transform);
        robotCharacter = Instantiate(robotCharacterPrefab, sceneContent.transform);
        chatboxUi = Instantiate(chatboxUiPrefab, sceneContent.transform);

        azureAiHandler = globalManager.GetComponent<AzureAiHandler>();
        dictationController = globalManager.GetComponent<DictationController>();
        customSubmitButton = chatboxUi.GetComponentInChildren<CustomSubmitButton>();
        robotScript = robotCharacter.GetComponent<RobotScript>();
        roboPetFollowController = robotCharacter.GetComponent<RoboPetFollowController>();
        handMenuPrefabGetter = handMenu.GetComponent<HandMenuPrefabGetter>();

        mrtkuguiInputField = chatboxUi.GetComponentInChildren<MRTKUGUIInputField>();

        menuSpawn = handMenuPrefabGetter.GetMenu(HandMenuPrefabGetter.ButtonType.spawnButton);
        menuChat = handMenuPrefabGetter.GetMenu(HandMenuPrefabGetter.ButtonType.chatButton);
        spawnButton = handMenuPrefabGetter.GetButton(HandMenuPrefabGetter.ButtonType.spawnButton);
        chatButton = handMenuPrefabGetter.GetButton(HandMenuPrefabGetter.ButtonType.chatButton);
        speakButton = handMenuPrefabGetter.GetButton(HandMenuPrefabGetter.ButtonType.speakButton);

        RouteComponents();
    }

    void RouteComponents()
    {
        customSubmitButton.targetMrtkInputField = mrtkuguiInputField;
        customSubmitButton.azureAiHandlerInstance = azureAiHandler;
        roboPetFollowController.userTarget = mainCamera.transform;
        azureAiHandler.chatDisplayContent = chatboxUi.GetComponent<ChatboxUiPrefabGetter>().scrollContent.GetComponent<RectTransform>();

        spawnButton.OnClick.AddListener(SpawnButtonClick);
        chatButton.OnClick.AddListener(ChatButtonClick);
        speakButton.OnClick.AddListener(dictationController.ToggleDictation);

        robotScript.onKilled.AddListener(MenuSpawnActivate);
        robotScript.onSpawned.AddListener(MenuChatActivate);

        dictationController.targetInputField = mrtkuguiInputField;
        dictationController.robotCharacter = robotScript;

        mrtkuguiInputField.onValueChanged.AddListener(robotScript.ResetTimer);
        mrtkuguiInputField.onSubmit.AddListener(InputSubmit);
        mrtkuguiInputField.onEndEdit.AddListener(robotScript.ResetTimer);

        customSubmitButton.robotCharacter = robotScript;
    }

    void SpawnButtonClick()
    {
        robotScript.SpawnRobot();
        // chatboxUiPrefab.GetComponent<FollowMeToggle>().SetFollowMeBehavior(true);
    }

    void InputSubmit(string _)
    {
        robotScript.ResetTimer();
        customSubmitButton.PerformSubmit();
    }

    void ChatButtonClick()
    {
        chatboxUi.GetComponent<ToggleGameObject>().ShowIt();
        chatboxUi.GetComponent<FollowMeToggle>().SetFollowMeBehavior(true);
        robotScript.ResetTimer();
    }

    void speakButtonClick()
    {
        dictationController.ToggleDictation();
    }

    void MenuSpawnActivate()
    {
        menuChat.SetActive(false);
        chatboxUi.GetComponent<ToggleGameObject>().HideIt();
        menuSpawn.SetActive(true);
    }

    void MenuChatActivate()
    {
        menuChat.SetActive(true);
        menuSpawn.SetActive(false);
    }
}