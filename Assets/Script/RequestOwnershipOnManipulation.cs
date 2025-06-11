using Photon.Pun;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class RequestOwnershipOnManipulation : MonoBehaviourPun
{
    private ObjectManipulator manipulator;
    private bool isOwner = false;

    private void Awake()
    {
        manipulator = GetComponent<ObjectManipulator>();
    }

    private void OnEnable()
    {
        manipulator.OnManipulationStarted.AddListener(RequestOwnership);
        manipulator.OnManipulationEnded.AddListener(ResetOwnership);
    }

    private void OnDisable()
    {
        manipulator.OnManipulationStarted.RemoveListener(RequestOwnership);
        manipulator.OnManipulationEnded.RemoveListener(ResetOwnership);
    }

    private void RequestOwnership(ManipulationEventData data)
    {
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            isOwner = photonView.IsMine; // Cek status ownership
        }
    }

    private void ResetOwnership(ManipulationEventData data)
    {
        isOwner = false;
    }
}