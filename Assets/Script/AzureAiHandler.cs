using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro; // Required for TextMeshProUGUI

public class AzureAiHandler : MonoBehaviour
{
    [Header("Azure AI Configuration")]
    [Tooltip("The full URL of your Azure AI service endpoint.")]
    public string azureEndpointUrl = "PASTE_YOUR_AZURE_ENDPOINT_URL_HERE";

    [Tooltip("Your Azure AI service API key or Bearer token. If Bearer token, prefix with 'Bearer '.")]
    public string apiKey = "PASTE_YOUR_API_KEY_OR_BEARER_TOKEN_HERE";

    [Tooltip("The HTTP header name for your API key. Common: Ocp-Apim-Subscription-Key, api-key, Authorization")]
    public string apiKeyHeaderName = "Authorization"; // Changed for Bearer token

    [Header("Chat Display")]
    [Tooltip("The 'Content' GameObject of your ScrollView where new chat message prefabs will be parented.")]
    public RectTransform chatDisplayContent; // Assign the ScrollView's Content object

    [Tooltip("The prefab for a single chat message. Must have a TextMeshProUGUI component in its children.")]
    public GameObject chatMessagePrefab; // Assign your chat message prefab

    // --- Helper class for structuring the request JSON ---
    [System.Serializable]
    private class RequestPayload
    {
        // Changed from inputText to question
        public string question; 
    }

    // --- Helper class for parsing the response JSON ---
    [System.Serializable]
    private class ResponsePayload
    {
        // Changed from outputText to answer
        public string answer; 
        // Add any other fields your AI might return
    }

    // Public method to call to send the query and update chat
    public void ProcessQueryAndDisplayInChat(string userQuery)
    {
        if (string.IsNullOrEmpty(userQuery))
        {
            Debug.LogWarning("User query is empty. Not sending to Azure AI.");
            return;
        }

        if (string.IsNullOrEmpty(azureEndpointUrl) || azureEndpointUrl == "PASTE_YOUR_AZURE_ENDPOINT_URL_HERE")
        {
            Debug.LogError("Azure Endpoint URL is not set in the AzureAiHandler component.");
            InstantiateChatMessage("<color=red>System Error:</color> Azure endpoint not configured.\n\n");
            return;
        }
        if (string.IsNullOrEmpty(apiKey) || apiKey == "PASTE_YOUR_API_KEY_OR_BEARER_TOKEN_HERE")
        {
            Debug.LogError("Azure API Key/Token is not set in the AzureAiHandler component.");
            InstantiateChatMessage("<color=red>System Error:</color> Azure API key/token not configured.\n\n");
            return;
        }

        // 1. Display user's query immediately
        InstantiateChatMessage($"<color=#76D7C4>You:</color> {userQuery}\n"); // Example user color

        Debug.Log($"Sending query to Azure AI: \"{userQuery}\"");
        StartCoroutine(PostRequestCoroutine(userQuery));
    }

    private IEnumerator PostRequestCoroutine(string queryText)
    {
        // Use 'question' field for the payload
        RequestPayload payload = new RequestPayload { question = queryText }; 
        string jsonRequestBody = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);

        using (UnityWebRequest www = new UnityWebRequest(azureEndpointUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            // The apiKey should now contain "Bearer your_token_string" if using Bearer tokens
            www.SetRequestHeader(apiKeyHeaderName, apiKey); 

            Debug.Log($"Sending JSON: {jsonRequestBody} to {azureEndpointUrl} with Header '{apiKeyHeaderName}: {apiKey.Substring(0, Mathf.Min(apiKey.Length, 15))}...'"); // Log only part of the key for security

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Azure AI Raw Response: " + responseText);

                string aiResponseToDisplay = responseText; // Default to raw
                try
                {
                    ResponsePayload responsePayload = JsonUtility.FromJson<ResponsePayload>(responseText);
                    // Use 'answer' field from the response
                    if (responsePayload != null && !string.IsNullOrEmpty(responsePayload.answer)) 
                    {
                        aiResponseToDisplay = responsePayload.answer;
                        Debug.Log("Azure AI Parsed Answer: " + aiResponseToDisplay);
                    }
                    else
                    {
                        Debug.LogWarning("Could not parse 'answer' from Azure AI response or it was empty. Using raw response. JSON might be: " + responseText);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing Azure AI JSON response: " + e.Message + ". Using raw response. JSON was: " + responseText);
                }
                InstantiateChatMessage($"<color=#5DADE2>AI:</color> {aiResponseToDisplay}\n\n"); // Example AI color
            }
            else
            {
                Debug.LogError($"Azure AI Request Error: {www.error} (Code: {www.responseCode})");
                string errorDetails = www.error;
                if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    errorDetails += $"\nError Body: {www.downloadHandler.text}";
                    Debug.LogError($"Error Body: {www.downloadHandler.text}");
                }
                InstantiateChatMessage($"<color=red>AI Error:</color> {errorDetails}\n\n");
            }
        }
    }

    private void InstantiateChatMessage(string message)
    {
        if (chatMessagePrefab == null)
        {
            Debug.LogError("ChatMessagePrefab not assigned in AzureAiHandler. Cannot display message: " + message);
            return;
        }
        if (chatDisplayContent == null)
        {
            Debug.LogError("ChatDisplayContent (ScrollView Content area) not assigned in AzureAiHandler. Cannot display message: " + message);
            return;
        }

        GameObject newChatGO = Instantiate(chatMessagePrefab, chatDisplayContent); // Parent to the Content area
        TextMeshProUGUI messageTMP = newChatGO.GetComponentInChildren<TextMeshProUGUI>(); 

        if (messageTMP != null)
        {
            messageTMP.text = message;
            // Ensure Rich Text is enabled on the TextMeshProUGUI component in your prefab for colors to work.
        }
        else
        {
            Debug.LogError("ChatMessagePrefab does not have a TextMeshProUGUI component in its children.", newChatGO);
        }

        // Optional: Auto-scroll to bottom
        // For this to work reliably with dynamic content, you might need to wait for the end of the frame
        // so the layout group has a chance to update.
        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        // Wait until the end of the frame so UI layout can update
        yield return new WaitForEndOfFrame(); 
        
        UnityEngine.UI.ScrollRect scrollRect = chatDisplayContent.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f; // 0 is bottom, 1 is top
        }
    }
}