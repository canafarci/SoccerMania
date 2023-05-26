using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(AudioSource))]
public class NetTrigger : MonoBehaviourPunCallbacks
{
    public Players thisPlayer;
    private BoxCollider boxCollider;
    private Coroutine resetColliderCorooutine = null;
    private AudioSource audioSource;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (resetColliderCorooutine != null)
                StopCoroutine(resetColliderCorooutine);

            resetColliderCorooutine = StartCoroutine(ResetTriggersRoutine());


            if (thisPlayer == Players.RedPlayer)
            {
                CallOnScore(true);
            }
            else if (thisPlayer == Players.BluePlayer)
            {
                CallOnScore(false);
            }
        }
    }


    public void CallOnScore(bool __bluePlayerHasScored)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_OnScore", RpcTarget.AllBuffered, __bluePlayerHasScored);
        }
    }


    [PunRPC]
    public void RPC_OnScore(bool __bluePlayerHasScored)
    {
        audioSource.clip = AudioManager.Instance.GoalSFX;
        audioSource.Play();

        if (resetColliderCorooutine != null)
            StopCoroutine(resetColliderCorooutine);

        resetColliderCorooutine = StartCoroutine(ResetTriggersRoutine());

        foreach (PlayerController _pc in FindObjectsOfType<PlayerController>())
        {
            _pc.CallOnScore(__bluePlayerHasScored);
        }
    }

    IEnumerator ResetTriggersRoutine()
    {
        boxCollider.enabled = false;
        yield return new WaitForSeconds(2.2f);
        boxCollider.enabled = true;
    }
}
