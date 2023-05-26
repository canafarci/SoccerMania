using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    private bool masterIsConnected, clientIsConnected;
    public int index;
    private AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetConnectionBools(bool __isMasterClient)
    {
        photonView.RPC("RPC_SetConnectionBools", RpcTarget.AllBuffered, __isMasterClient);
    }

    [PunRPC]
    public void RPC_SetConnectionBools(bool __isMasterClient, PhotonMessageInfo __info)
    {
        if (__isMasterClient && photonView.IsMine)
        {
            masterIsConnected = true;
            print(__info.Sender.NickName + "  " + "connected");
        }

        else if (!__isMasterClient && photonView.IsMine)
        {
            clientIsConnected = true;
            print(__info.Sender.NickName + "  " + "connected");
        }

        if (masterIsConnected && clientIsConnected && photonView.IsMine)
        {
            //do stuff
            print(" all players connected");

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_GameStart", RpcTarget.AllBufferedViaServer, index);
            }
        }
    }

    [PunRPC]
    public void RPC_GameStart(int __index)
    {
        StartCoroutine(GameStartRoutine(__index));
        GameManager.Instance.StartFadeRoutine(true);
        print("__index is " + __index);
    }

    IEnumerator GameStartRoutine(int __index)
    {

        yield return new WaitForSeconds(4f);
        audioSource.clip = AudioManager.Instance.CoinTossSFX;
        audioSource.Play();
        yield return new WaitForSeconds(2f);

        if (__index == 0)
        {
            GameManager.Instance.GameStartText.text = "Red Player Starts!";
        }
        else if (__index == 1)
        {
            GameManager.Instance.GameStartText.text = "Blue Player Starts!";
        }

        yield return new WaitForSeconds(2f);

        if (__index == 0)
        {
            SpawnBall(GameManager.Instance.RedPlayerSpawnTransform);
        }
        else if (__index == 1)
        {
            SpawnBall(GameManager.Instance.BluePlayerSpawnTransform);
        }

        GameManager.Instance.GameStartTextObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        GameManager.Instance.gameIsStarted = true;
    }

    public void SpawnBall(Transform __spawnTransform)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (__spawnTransform == GameManager.Instance.RedPlayerSpawnTransform)
            {
                GameObject __ball = PhotonNetwork.InstantiateRoomObject(GameManagerStrings.BallPrefab, __spawnTransform.position, Quaternion.identity);
            }
            else
            {
                GameObject __ball = PhotonNetwork.InstantiateRoomObject(GameManagerStrings.BallPrefab, __spawnTransform.position, Quaternion.identity);
                __ball.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.CurrentRoom.Players[1]);
            }
            
        }
    }
}
