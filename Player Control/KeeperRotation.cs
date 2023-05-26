using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperRotation : MonoBehaviour
{
    public BallController ball;

    void Update()
    {
        if (!GameManager.Instance.gameIsStarted)
            return;
        
        SetRotation();
    }

    private void SetRotation()
    {
        Quaternion lookRot = Quaternion.LookRotation(ball.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, lookRot.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * 7);
    }
}
