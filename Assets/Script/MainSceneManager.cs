using UnityEngine;
using Photon.Pun;
using TMPro;

public class MainSceneManager : MonoBehaviourPunCallbacks
{
    [Header("Anchor spawn transform (isi dari scene)")]
    public Transform caseAnchor;
    public Transform caseTriggerAnchor;
    public Transform cpuFanAnchor;
    public Transform cpuFanTriggerAnchor;
    public Transform deskAnchor;
    public Transform fanAnchor;
    public Transform fanTriggerAnchor;
    public Transform hddAnchor;
    public Transform hddTriggerAnchor;
    public Transform motherboardAnchor;
    public Transform motherboardTriggerAnchor;
    public Transform pcAnchor;
    public Transform placardVariantAnchor;
    public Transform vgaAnchor;
    public Transform vgaTriggerAnchor;

    private GameObject placardVariantAnchorGameObject;
    
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am MasterClient, spawning all objects...");

            PhotonNetwork.Instantiate("Case", caseAnchor.position, caseAnchor.rotation);
            PhotonNetwork.Instantiate("Case Trigger", caseTriggerAnchor.position, caseTriggerAnchor.rotation);
            PhotonNetwork.Instantiate("Cpu Fan", cpuFanAnchor.position, cpuFanAnchor.rotation);
            PhotonNetwork.Instantiate("Cpu Fan Trigger", cpuFanTriggerAnchor.position, cpuFanTriggerAnchor.rotation);
            PhotonNetwork.Instantiate("Desk", deskAnchor.position, deskAnchor.rotation);
            PhotonNetwork.Instantiate("Fan", fanAnchor.position, fanAnchor.rotation);
            PhotonNetwork.Instantiate("Fan Trigger", fanTriggerAnchor.position, fanTriggerAnchor.rotation);
            PhotonNetwork.Instantiate("HDD", hddAnchor.position, hddAnchor.rotation);
            PhotonNetwork.Instantiate("HDD Trigger", hddTriggerAnchor.position, hddTriggerAnchor.rotation);
            PhotonNetwork.Instantiate("MotherBoard", motherboardAnchor.position, motherboardAnchor.rotation);
            PhotonNetwork.Instantiate("Motherboard Trigger", motherboardTriggerAnchor.position, motherboardTriggerAnchor.rotation);
            PhotonNetwork.Instantiate("PC", pcAnchor.position, pcAnchor.rotation);
            placardVariantAnchorGameObject = PhotonNetwork.Instantiate("Placard Variant", placardVariantAnchor.position, placardVariantAnchor.rotation);
            PhotonNetwork.Instantiate("VGA", vgaAnchor.position, vgaAnchor.rotation);
            PhotonNetwork.Instantiate("VGA Trigger", vgaTriggerAnchor.position, vgaTriggerAnchor.rotation);
        }
        else
        {
            Debug.Log("I am NOT MasterClient, waiting for Master to spawn objects.");
        }
    }

    public TextMeshProUGUI getScoreText()
    {
        return placardVariantAnchorGameObject.GetComponentInChildren<TextMeshProUGUI>();
    }
}
