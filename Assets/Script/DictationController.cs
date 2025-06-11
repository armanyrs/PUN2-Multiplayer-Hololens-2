using UnityEngine;
using TMPro; // For your TMP_InputField
using Microsoft.MixedReality.Toolkit.Input; // For DictationHandler (though event signature seems different)
using System.Collections;
using Microsoft.MixedReality.Toolkit.Experimental.UI; // Required for Coroutines

public class DictationController : MonoBehaviour
{
    [Header("MRTK Components")]
    [Tooltip("Assign the DictationHandler component from your scene. This script assumes DictationHandler is on a different GameObject or this one.")]
    public DictationHandler dictationHandler;

    [Header("UI and AI Links")]
    [Tooltip("Assign the TMP_InputField where dictated text should go (e.g., from MRKeyboardInputField_UGUI).")]
    public MRTKUGUIInputField targetInputField;

    // [Tooltip("Assign your AzureAiHandler script instance.")]
    // public AzureAiHandler azureAiHandler;
    public CustomSubmitButton submitButton;
    public RobotScript robotCharacter;

    // This flag helps our Toggle logic, but dictationHandler.IsListening is the source of truth for actual state
    private bool userWantsDictationActive = false;
    private string lastRecognizedPhrase = ""; // To hold the latest complete phrase

    void Start()
    {
        if (dictationHandler == null)
        {
            // Try to find it on the same GameObject if not assigned
            dictationHandler = GetComponent<DictationHandler>();
            if (dictationHandler == null)
            {
                Debug.LogError("DictationController: DictationHandler not assigned and not found on this GameObject! Please assign it in the Inspector.");
                enabled = false; // Disable script if critical component is missing
                return;
            }
            else
            {
                Debug.LogWarning("DictationController: DictationHandler was not assigned in Inspector, but found on this GameObject. Assigning automatically.");
            }
        }

        // Subscribe to DictationHandler events
        // Assuming the events are UnityEvent<string> based on the error CS1503
        dictationHandler.OnDictationHypothesis.AddListener(HandleDictationHypothesis);
        dictationHandler.OnDictationResult.AddListener(HandleDictationResult);
        dictationHandler.OnDictationComplete.AddListener(HandleDictationComplete);
        dictationHandler.OnDictationError.AddListener(HandleDictationError);

        if (targetInputField == null) Debug.LogWarning("DictationController: Target Input Field not assigned. Dictated text won't populate it automatically.");
        // if (azureAiHandler == null) Debug.LogWarning("DictationController: Azure AI Handler not assigned. Dictated text won't be auto-sent to AI.");
    }

    /// <summary>
    /// Public method to be called by a button (e.g., a microphone icon button) to start or stop dictation.
    /// </summary>
    public void ToggleDictation()
    {
        if (!dictationHandler.IsListening)
        {
            // If not listening, user wants to start
            userWantsDictationActive = true;
            StartCoroutine(RequestStartDictationCoroutine());
        }
        else
        {
            // If listening, user wants to stop
            userWantsDictationActive = false; // User no longer wants it active, even if it's stopping naturally
            RequestStopDictation();
        }
    }

    private IEnumerator RequestStartDictationCoroutine()
    {
        // If the handler somehow thinks it's listening (e.g., stuck state), try to stop it first.
        if (dictationHandler.IsListening)
        {
            Debug.LogWarning("RequestStartDictationCoroutine: DictationHandler reports it's already listening. Attempting to stop first.");
            dictationHandler.StopRecording();
            yield return null; // Wait a frame for the stop command to attempt processing

            // Check again after attempting to stop
            if (dictationHandler.IsListening)
            {
                Debug.LogError("RequestStartDictationCoroutine: DictationHandler STILL listening after explicit stop attempt. Cannot start new session now. Dictation might be stuck.");
                if (targetInputField != null && targetInputField.text == "Listening...")
                {
                    targetInputField.text = "Dictation stuck. Try again.";
                }
                userWantsDictationActive = false; // Reset intent as we failed to start
                yield break; // Exit the coroutine
            }
        }

        lastRecognizedPhrase = ""; // Clear previous result
        if (targetInputField != null)
        {
            targetInputField.text = "Listening..."; // Provide visual feedback
        }

        if (robotCharacter != null)
        {
            robotCharacter.PauseTimer();
        }
        dictationHandler.StartRecording();
        // userWantsDictationActive is already true from ToggleDictation
        Debug.Log("Dictation start requested.");
    }

    private void RequestStopDictation()
    {
        if (dictationHandler.IsListening)
        {
            if (robotCharacter != null)
            {
                robotCharacter.PlayTimer();
            }
            dictationHandler.StopRecording();
            Debug.Log("Dictation stop requested by user.");
        }
        // The actual state change and result processing will happen in HandleDictationComplete or HandleDictationError.
        // userWantsDictationActive is already false from ToggleDictation
    }

    private void HandleDictationHypothesis(string hypothesisText)
    {
        // Hypothesis provides interim, possibly inaccurate results.
        // Useful for showing live feedback if desired.
        // if (userWantsDictationActive && targetInputField != null && !string.IsNullOrEmpty(hypothesisText))
        // {
        //     targetInputField.text = hypothesisText + "..."; // Uncomment to show live hypothesis
        // }
        // Debug.Log($"Dictation Hypothesis: {hypothesisText}");
    }

    private void HandleDictationResult(string resultText)
    {
        // Result can sometimes give more stable partial results than hypothesis.
        // We'll store it and prefer the OnDictationComplete result if available.
        if (!string.IsNullOrEmpty(resultText))
        {
            lastRecognizedPhrase = resultText;
            // Debug.Log($"Dictation Intermediate Result: {lastRecognizedPhrase}");
        }
    }

    private void HandleDictationComplete(string completeText)
    {
        Debug.Log($"Dictation Complete. Raw text: \"{completeText}\"");
        // userWantsDictationActive is now effectively false as the session ended.
        // If user had clicked stop, userWantsDictationActive would already be false.
        // If it timed out, this ensures our internal flag reflects the session ended.
        userWantsDictationActive = false; 

        string finalResult = completeText;

        // If DictationResult from Complete is empty, but we had a more recent Result, use that.
        // This logic might need adjustment if 'completeText' is the only reliable source now.
        if (string.IsNullOrEmpty(finalResult) && !string.IsNullOrEmpty(lastRecognizedPhrase))
        {
            Debug.Log($"Using last recognized phrase: \"{lastRecognizedPhrase}\" as completeText was empty.");
            finalResult = lastRecognizedPhrase;
        }
        lastRecognizedPhrase = ""; // Reset for the next session

        if (!string.IsNullOrEmpty(finalResult))
        {
            Debug.Log($"Final recognized phrase: \"{finalResult}\"");
            if (targetInputField != null)
            {
                targetInputField.text = finalResult; // Put text in the input field
                Debug.Log("Dictated text placed in input field.");

                // Automatically send this text to Azure AI
                // if (azureAiHandler != null)
                // {
                //     azureAiHandler.ProcessQueryAndDisplayInChat(finalResult);
                //     // Optionally clear the input field after sending if desired
                //     // targetInputField.text = ""; 
                // }
                // else
                // {
                //      Debug.LogWarning("AzureAiHandler not assigned, cannot send dictated text to AI.");
                // }

                if (submitButton != null)
                {
                    submitButton.PerformSubmit();
                }
            }
        }
        else
        {
            Debug.Log("Dictation completed with no discernible phrase.");
            if (targetInputField != null && targetInputField.text == "Listening...")
            {
                targetInputField.text = ""; // Clear "Listening..."
            }
        }
    }

    private void HandleDictationError(string errorText)
    {
        Debug.LogError($"Dictation Error: {errorText}");
        userWantsDictationActive = false; // Session has ended due to error

        if (targetInputField != null)
        {
            if (targetInputField.text == "Listening...")
                 targetInputField.text = "Dictation error.";
            // else keep existing text or append error.
        }
        lastRecognizedPhrase = ""; // Clear any partial phrase
    }

    void OnDestroy()
    {
        if (dictationHandler != null)
        {
            // Unsubscribe from all events
            dictationHandler.OnDictationHypothesis.RemoveListener(HandleDictationHypothesis);
            dictationHandler.OnDictationResult.RemoveListener(HandleDictationResult);
            dictationHandler.OnDictationComplete.RemoveListener(HandleDictationComplete);
            dictationHandler.OnDictationError.RemoveListener(HandleDictationError);

            // If dictation is somehow still active when this object is destroyed, try to stop it.
            if (dictationHandler.IsListening)
            {
                dictationHandler.StopRecording();
            }
        }
    }
}
