using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyAddForce : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 Direction;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        rb.AddForce(Direction, ForceMode.Force); 
    }
}
