using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Linq;

public class ObjectSlotTrigger : MonoBehaviour
{
    [Header("Nama objek yang boleh masuk (exact match)")]
    public string acceptedObjectName = "MyPuzzleObject";

    [Header("Offset posisi snap (jika perlu)")]
    public Vector3 positionOffset;

    [Header("Apakah object dikunci setelah masuk?")]
    public bool lockObject = true;

    [Header("Skor yang diberikan saat berhasil")]
    public int scoreReward = 100;

    private bool isFilled = false;

    void Start()
    {
        acceptedObjectName += "(Clone)";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFilled) return;  // Cegah double trigger
        if (other.gameObject.name != acceptedObjectName) return;

        // Snap posisi dan rotasi
        other.transform.position = transform.position + positionOffset;
        other.transform.rotation = transform.rotation;
        other.transform.SetParent(transform);

        if (lockObject)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            ObjectManipulator manipulator = other.GetComponent<ObjectManipulator>();
            if (manipulator != null)
            {
                manipulator.enabled = false;
            }
        }

        // Tambah skor di ScoreManager
        ScoreManager.instance.AddScore(scoreReward);

        isFilled = true;

        Debug.Log($"✅ Object '{other.name}' berhasil ditempatkan di slot '{gameObject.name}', score +{scoreReward}");
    }
}
