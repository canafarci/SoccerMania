using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPlayer : MonoBehaviour
{
    public LayerMask layerMask;
    private Transform BaseTransform, PlayerTransform;
    public RigidbodyAddForce RigidbodyAddForce;
    public Vector3 Direction;
    bool transformsInitiated = false;

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (transformsInitiated)
            {
                Vector3 _currentRot = PlayerTransform.localRotation.eulerAngles;
                _currentRot.x = 90;

                PlayerTransform.localRotation = Quaternion.Euler(_currentRot);
                RigidbodyAddForce.Direction = Vector3.zero;
            }
           
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, layerMask))
            {
                if (!transformsInitiated)
                {
                    BaseTransform = _hit.transform.parent.GetChild(1).GetChild(0);
                    PlayerTransform = _hit.transform.parent.GetChild(1).GetChild(0).GetChild(0);
                    transformsInitiated = true;
                }

                BaseTransform.transform.LookAt(_hit.point);
                BaseTransform.transform.localRotation = Quaternion.Euler(BaseTransform.transform.localRotation.eulerAngles - new Vector3(0, 0, 180));
                print("turning");                  
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            transformsInitiated = false;
            BaseTransform = null;
            PlayerTransform = null;
            RigidbodyAddForce.Direction = Direction;
        }
    }
}
