using System.Collections;
using UnityEngine;

public class Push : MonoBehaviour
{
    public bool pusherHit = false;
    [SerializeField] float forceStrength = 100;
    public bool forceApplied = false;
    public BallController ball;
    public bool ballIsStuck = false;
    private float forceAngle;

    private void Update()
    {
        if (ballIsStuck)
        {
            ball.transform.position = transform.GetChild(0).position;
            ball.transform.rotation = Quaternion.identity;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (pusherHit && !forceApplied)
        {
            ballIsStuck = false;
            float randomFactor = Random.Range(-1200, 1200);
            collision.gameObject.GetComponent<Rigidbody>().AddRelativeForce((forceAngle * forceStrength * -this.transform.forward * Time.deltaTime) + (this.transform.right * randomFactor * Time.deltaTime) ,ForceMode.Impulse);
            forceApplied = true;
        }
    }

    public void SetFloatAngle(float _forceAngle)
    {
        forceAngle = _forceAngle;
        forceAngle = 110 - forceAngle;
    }

    public void StartPlaceBallCoroutine()
    {
        StartCoroutine(PlaceBall());
    }
    
    IEnumerator PlaceBall()
    {
        float time = 0f;
        while (time < 0.5)
        {
            time += Time.deltaTime * 5;
            ball.transform.position = Vector3.Lerp(ball.transform.position, transform.GetChild(0).position, time);
            yield return null;
        }
        ballIsStuck = true;
    }
}
