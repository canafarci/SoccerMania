using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnchor : MonoBehaviour
{
    public BallController ball;
    public Push push;
    [SerializeField] float highAnchorPoint;
    [SerializeField] float lowAnchorPoint;

    [SerializeField] bool isBlueKeeper;
    [SerializeField] bool isRedKeeper;

    void Update()
    {
        if (!GameManager.Instance.gameIsStarted)
            return;
            
        if (ball == null)
            return;
            
        if (!push.ballIsStuck)
        {
            SetRotation();
        }
        ClampRotation();
    }

    private void SetRotation()
    {
        Quaternion lookRot = Quaternion.LookRotation(ball.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, lookRot.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * 5f);
    }

    private void ClampRotation()
    {
        if (isBlueKeeper)
        {
            if (transform.eulerAngles.y > highAnchorPoint)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, highAnchorPoint, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.y < lowAnchorPoint)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, lowAnchorPoint, transform.eulerAngles.z);
            }
        }

        else if (isRedKeeper)
        {
            if (transform.eulerAngles.y < 100 && transform.eulerAngles.y > highAnchorPoint)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, highAnchorPoint, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.y > 300 && transform.eulerAngles.y < lowAnchorPoint)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, lowAnchorPoint, transform.eulerAngles.z);
            }
        }
    }
}
