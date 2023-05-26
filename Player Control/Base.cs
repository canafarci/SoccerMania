using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Base : MonoBehaviourPunCallbacks
{
    //[SerializeField] bool isPlayerOne = true;
    PlayerTeamController pC;
    public DragPlayer2 player;

    private Coroutine switchOwnershipRoutine = null;

    void Awake()
    {
        pC = GetComponentInParent<PlayerTeamController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            print("called1");

            if (photonView.IsMine)
            {
                print("called2");

                if (switchOwnershipRoutine != null)
                    StopCoroutine(switchOwnershipRoutine);

                switchOwnershipRoutine = StartCoroutine(SwitchOwnership(other));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            other.GetComponent<BallController>().TimeSinceLastInteraction = 0f;
        }
    }

    IEnumerator SwitchOwnership(Collider other)
    {
        yield return new WaitForSecondsRealtime(0.35f);
        pC.SetControlable(true);
        player.playable = true;
        other.transform.GetComponent<PhotonView>().TransferOwnership(photonView.Owner);
        photonView.RPC("RPC_SwitchBallOwnership", RpcTarget.AllBuffered, photonView.Owner);
    }

    [PunRPC]
    public void RPC_SwitchBallOwnership(Photon.Realtime.Player __player)
    {
        print("called3");
        GameManager.Instance.PlayingBall.GetComponent<PhotonView>().TransferOwnership(__player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            player.isSelected = false;
            pC.SetControlable(false);
            player.playable = false;
            //player.StartLerp();

            if (switchOwnershipRoutine != null)
                StopCoroutine(switchOwnershipRoutine);
        }
    }
}
