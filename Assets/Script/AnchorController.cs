using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class AnchorController : MonoBehaviour
{
    private GameObject anchorGO;

    void Start()
    {
        // Coba load anchor
        var anchorData = PlayerPrefs.GetString("anchor_pos", "");
        if (!string.IsNullOrEmpty(anchorData))
        {
            string[] split = anchorData.Split(',');
            Vector3 pos = new Vector3(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2]));
            transform.position = pos;
        }
    }

    public void SaveAnchor()
    {
        Debug.Log("Saving anchor position...");
        Vector3 pos = transform.position;
        string data = $"{pos.x},{pos.y},{pos.z}";
        PlayerPrefs.SetString("anchor_pos", data);
        PlayerPrefs.Save();
    }
}
