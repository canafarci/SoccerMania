using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTeamController : MonoBehaviourPunCallbacks
{
    public bool controllable = false;
    public LayerMask layerMask;
    private bool selectHeld = false;
    RaycastHit _hit;

    private void LateUpdate()
    {
        if (controllable == true && photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, layerMask))
                {
                    if (_hit.collider != null && _hit.collider.GetComponent<Base>().player.playable == true)
                    {
                        selectHeld = true;
                    }
                }
            }

            if (selectHeld)
            {
                _hit.collider.GetComponent<Base>().player.isSelected = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (_hit.collider != null)
                {
                    _hit.collider.GetComponent<Base>().player.isSelected = false;
                }
                selectHeld = false;
            }
        }
    }

    public void SetControlable(bool _controllable)
    {
        controllable = _controllable;
    }
}
