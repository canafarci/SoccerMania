using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TextPlayerNameSetter : MonoBehaviour
{
    public Players thisPlayer;
    private TextMeshProUGUI playerNameText;

    private string hostNameText, clientNameText;


    private void Awake() 
    {
        playerNameText = GetComponent<TextMeshProUGUI>();
    }

    private void Start() 
    {
        foreach (KeyValuePair<int, Photon.Realtime.Player> _pl in  PhotonNetwork.CurrentRoom.Players)
        {
            if (_pl.Value == PhotonNetwork.MasterClient)
            {
                hostNameText = _pl.Value.NickName;
            }
            else
            {
                clientNameText = _pl.Value.NickName;
            }
        }

        if (thisPlayer == Players.RedPlayer)
        {
            playerNameText.text = hostNameText;
        }
        else
        {
            playerNameText.text = clientNameText;
        }
    }
}