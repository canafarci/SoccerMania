using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallController : MonoBehaviourPunCallbacks/* , IPunObservable */
{
    private Rigidbody rb;
    public float TimeSinceLastInteraction = 0f;
    Quaternion networkRotation;
    Vector3 networkPosition;
    private float movementSpeed;

    private void Update()
    {
        if (photonView.IsMine)
        {
            TimeSinceLastInteraction += Time.deltaTime;

            if (TimeSinceLastInteraction >= 10f)
            {
                GameManager.Instance.TiltButton.SetActive(true);
            }
            else
            {
                GameManager.Instance.TiltButton.SetActive(false);
            }
        }
        else
        {
            TimeSinceLastInteraction = 0f;
            GameManager.Instance.TiltButton.SetActive(false);
        }

        /* if (!photonView.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * movementSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100);
            return;
        } */
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GameManager.Instance.PlayingBall = this.gameObject;

        foreach (Push _p in FindObjectsOfType<Push>())
        {
            _p.ball = this;
        }

        foreach (BallAnchor _ba in FindObjectsOfType<BallAnchor>())
        {
            _ba.ball = this;
        }

        foreach (KeeperRotation _kr in FindObjectsOfType<KeeperRotation>())
        {
            _kr.ball = this;
        }
    }

    public void TiltBall()
    {
        float _tiltAmount = 30f;
        Vector3 _rndVector = new Vector3(Random.Range(-_tiltAmount, _tiltAmount), Random.Range(-_tiltAmount, _tiltAmount), Random.Range(-_tiltAmount, _tiltAmount));
        rb.AddForce(_rndVector, ForceMode.Impulse);
    }

    /* public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            movementSpeed = (float)info.SentServerTime;
        }
    } */
}