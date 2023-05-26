using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private GameController gameController;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }
    void Start()
    {
        if (photonView.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                gameController.SetConnectionBools(true);

                FindObjectOfType<GameController>().index = Random.Range(0, 2);
            }
            else
            {
                gameController.SetConnectionBools(false);
            }
        }
    }

    public void CallOnScore(bool __bluePlayerHasScored)
    {
        if (photonView.IsMine)
        {

            if (__bluePlayerHasScored)
            {
                GameManager.Instance.BluePlayerScore++;
                GameManager.Instance.BluePlayerScoreText.text = GameManager.Instance.BluePlayerScore.ToString();
            }

            else if (!__bluePlayerHasScored)
            {
                GameManager.Instance.RedPlayerScore++;
                GameManager.Instance.RedPlayerScoreText.text = GameManager.Instance.RedPlayerScore.ToString();
            }

            if (__bluePlayerHasScored)
            {
                GameManager.Instance.StartOnScoreRoutine(true);
            }

            else
            {
                GameManager.Instance.StartOnScoreRoutine(false);
            }
        }
    }

    public void TiltBallCalled()
    {
        StartCoroutine(ShakeRoutine());

        if (GameManager.Instance.PlayingBall.GetComponent<PhotonView>().Owner == this.photonView.Owner)
        {
            GameManager.Instance.PlayingBall.GetComponent<BallController>().TiltBall();
        }
    }

    IEnumerator ShakeRoutine()
    {
        //Handheld.Vibrate();
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2f;
        yield return new WaitForSeconds(0.5f);
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
    }
}