using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class ButtonController : MonoBehaviourPunCallbacks
{
    public void OnTiltButtonClicked()
    {
        photonView.RPC("RPC_OnTiltButtonClicked", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void RPC_OnTiltButtonClicked()
    {
         foreach (PlayerController _pc in FindObjectsOfType<PlayerController>())
        {
            _pc.TiltBallCalled();
        }
    }

    public void OnReturnToMainMenuClicked()
    {
        StartCoroutine(LoadMainMenuRoutine());
    }

    IEnumerator LoadMainMenuRoutine()
    {
        GameManager.Instance.StartFadeRoutine(false);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    public void RPC_OnRematchButtonClicked()
    {
        GameManager.Instance.RematchPanel.SetActive(true);
        GameManager.Instance.GameOverPanel.SetActive(false);

        foreach (KeyValuePair<int, Photon.Realtime.Player> _pl in  PhotonNetwork.CurrentRoom.Players)
        {
            if (PhotonNetwork.LocalPlayer.NickName != _pl.Value.NickName)
            {
                string __otherPlayerName = _pl.Value.NickName;
                GameManager.Instance.RematchText.text = __otherPlayerName + " offers a rematch!";
            }
        }
    }

    public void OnRematchRefuseButtonClicked()
    {
        StartCoroutine(LoadMainMenuRoutine());
        GameManager.Instance.OnRematchRefuseButtonClicked();
    }

    public void OnRematchAccepted()
    {
        GameManager.Instance.OnRematchAccepted();
    }

    public void OnRematchButtonClicked()
    {
        GameManager.Instance.RematchPanel.SetActive(true);
        GameManager.Instance.GameOverPanel.SetActive(false);
        GameManager.Instance.RematchAcceptButton.SetActive(false);
        GameManager.Instance.RematchRefuseButton.SetActive(false);
        GameManager.Instance.RematchText.text = "Awaiting Response...";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            StartCoroutine(OtherPlayerRefusedRoutine());
        }
        else
        {
            photonView.RPC("RPC_OnRematchButtonClicked", RpcTarget.OthersBuffered);
        }
    }

    IEnumerator OtherPlayerRefusedRoutine()
    {
        GameManager.Instance.RematchText.text = "Opponent Disconnected";
        yield return new WaitForSeconds(2f);
        StartCoroutine(LoadMainMenuRoutine());
    }

    
}
