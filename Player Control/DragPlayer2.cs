using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DragPlayer2 : MonoBehaviourPunCallbacks
{
    public bool isSelected = false;
    PlayerTeamController pC;
    private bool hitInitiated = false;
    float endRot;
    Quaternion startRot;
    Push push;
    public bool playable;
    public bool isKeeper = false;

    private void Awake()
    {
        pC = GetComponentInParent<PlayerTeamController>();
        startRot = transform.rotation;
        push = GetComponentInChildren<Push>();
    }

    private void Update()
    {
        if (playable && isSelected && pC.controllable)
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit2;
            
            if (Physics.Raycast(_ray, out _hit2))
            {
                if (!push.ballIsStuck)
                {
                    push.StartPlaceBallCoroutine();
                }
                else
                {
                    Quaternion lookRot = Quaternion.LookRotation(_hit2.point - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10);

                    //transform.LookAt(_hit2.point);
                    if (!isKeeper)
                    {
                        if (transform.eulerAngles.x > 320)
                        {
                            transform.eulerAngles = new Vector3(320, transform.eulerAngles.y, transform.eulerAngles.z);
                        }
                    }
                    else if (isKeeper)
                    {
                        if (transform.eulerAngles.x > 300)
                        {
                            transform.eulerAngles = new Vector3(300, transform.eulerAngles.y, transform.eulerAngles.z);
                        }
                    }

                    endRot = transform.eulerAngles.y;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isSelected = false;
                //Hit();
                photonView.RPC("RPC_Hit", RpcTarget.AllBuffered);
            }
        } 
    }

    [PunRPC]
    public void RPC_Hit()
    {
        push.pusherHit = true;
        hitInitiated = true;
        
        AudioManager.Instance.PlayBallKickSFX();

        if (hitInitiated)
        {
            hitInitiated = false;
            StartLerp();
        }
    }

    private void Hit()
    {
        push.pusherHit = true;
        hitInitiated = true;
        if (hitInitiated)
        {
            hitInitiated = false;
            StartLerp();
        }
    }

    public void StartLerp()
    {
        StartCoroutine(CompleteLerp());
    }

    IEnumerator CompleteLerp()
    {
        //90-20
        push.SetFloatAngle(360 - transform.eulerAngles.x);
        float time = 0f;
        while (time < 0.5)
        {
            time += Time.deltaTime * 5;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(startRot.eulerAngles.x, endRot, startRot.eulerAngles.z), time);
            yield return null;
        }
        push.pusherHit = false;
        push.forceApplied = false;
    }
}
