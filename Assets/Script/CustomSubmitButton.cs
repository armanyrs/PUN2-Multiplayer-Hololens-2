using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine.UI; // For standard UI Button
using TMPro;          // For TMP_InputField (which is likely inside MRTKUGUIInputField)

public class CustomSubmitButton : MonoBehaviour
{
    [Tooltip("The TextMeshPro Input Field from your MRTKUGUIInputField prefab.")]
    public MRTKUGUIInputField targetMrtkInputField; // Assign this in the Inspector

    [Tooltip("The AzureAiHandler instance that will process the query.")]
    public AzureAiHandler azureAiHandlerInstance; // Assign this in the Inspector
    public RobotScript robotCharacter;

    private Button uiButton;

    void Start()
    {
        uiButton = GetComponent<Button>();
        if (uiButton == null)
        {
            Debug.LogError("CustomSubmitButton script needs to be on a GameObject with a UI Button component.");
            return;
        }

        if (targetMrtkInputField == null)
        {
            Debug.LogError("Target MRTK Input Field (TMP_InputField) not assigned to CustomSubmitButton.");
            return;
        }

        if (azureAiHandlerInstance == null)
        {
            Debug.LogError("Azure AI Handler instance not assigned to CustomSubmitButton.");
            return;
        }

        uiButton.onClick.AddListener(PerformSubmit);
    }

    public void PerformSubmit()
    {
        if (targetMrtkInputField == null || azureAiHandlerInstance == null)
        {
            Debug.LogError("Dependencies not set for PerformSubmit.");
            return;
        }

        string currentText = targetMrtkInputField.text;
        robotCharacter.ResetTimer();

        if (!string.IsNullOrEmpty(currentText))
        {
            // Call the AzureAiHandler to process the query and display it
            azureAiHandlerInstance.ProcessQueryAndDisplayInChat(currentText);

            // Optional: Clear the input field after submission
            targetMrtkInputField.text = "";

            // Optional: Deactivate the input field to potentially dismiss the HoloLens keyboard
            // targetMrtkInputField.DeactivateInputField();
            // Or, to simulate deselection which might also dismiss keyboard:
            // if (UnityEngine.EventSystems.EventSystem.current != null) {
            //     UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            // }
        }
        else
        {
            Debug.Log("Input field is empty. Nothing to submit via custom button.");
        }
    }

    void OnDestroy()
    {
        if (uiButton != null)
        {
            uiButton.onClick.RemoveListener(PerformSubmit);
        }
    }
}