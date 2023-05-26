using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainMenuDollyTrackController : MonoBehaviour
{
    public GameObject UIPanel, StartButton;
    private CinemachineVirtualCamera CinemachineVirtualCamera;
    private CinemachineTrackedDolly DollyCam;

    public static bool isFirstStart = true;

    private void Awake() 
    {
        CinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        DollyCam = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Start() 
    {
        if (!isFirstStart)
            ActivateUI();
    }
    
    private void Update()
    {
        AnimateDollyCam();
    }

    private void AnimateDollyCam()
    {
        DollyCam.m_PathPosition += 0.01f * Time.deltaTime;
        if (DollyCam.m_PathPosition >= 1f)
        {
            DollyCam.m_PathPosition = 0f;
        }
    }

    public void OnStartGameButtonClicked()
    {
        isFirstStart = false;
        ActivateUI();
    }

    private void ActivateUI()
    {
        StartButton.SetActive(false);
        UIPanel.SetActive(true);
    }
}
