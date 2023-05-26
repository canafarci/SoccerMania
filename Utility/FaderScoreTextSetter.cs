using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FaderScoreTextSetter : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake() 
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetEndGameScore()
    {
        text.text = GameManager.Instance.RedPlayerScore.ToString() + "- " + GameManager.Instance.BluePlayerScore.ToString();
    }
}
