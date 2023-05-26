using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(GameManagerStrings.PlayerPrefab, Vector3.zero, Quaternion.identity);

            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(GameManagerStrings.Player1Prefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate(GameManagerStrings.Player2Prefab, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
