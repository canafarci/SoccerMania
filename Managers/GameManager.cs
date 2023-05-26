using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int RedPlayerScore, BluePlayerScore = 0;
    public GameObject PlayingBall;
    public TextMeshProUGUI RedPlayerScoreText, BluePlayerScoreText, EndGameText, GameStartText, RematchText;
    public GameObject GoalTextObject, EndGamePanel, GameStartTextObject, TiltButton, RematchPanel, GameOverPanel,
                        RematchAcceptButton, RematchRefuseButton;
    public Transform FirstStartTransform, RedPlayerSpawnTransform, BluePlayerSpawnTransform;
    public CanvasGroup fader;
    public FaderScoreTextSetter faderScoreTextSetter;
    public bool gameIsStarted, isRedPlayer = false;
    private GameController gameController;
    private bool gameFinished = false;
    private Coroutine fadeCoroutine = null;
    private AudioSource audioSource;
    private string hostNameText, clientNameText;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        //Singleton Method
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        gameController = FindObjectOfType<GameController>();

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
        {
            isRedPlayer = true;
        }
        else
        {
            isRedPlayer = false;
        }
    }

    public void StartFadeRoutine(bool __isFadingIn, float __factor = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(__isFadingIn, __factor));
    }

    IEnumerator FadeRoutine(bool __fadingIn, float __factor = 1f)
    {
        yield return new WaitForSeconds(3f);

        bool _fadeActive = true;

        if (__fadingIn)
        {
            while (_fadeActive)
            {
                yield return new WaitForSecondsRealtime(0.04f * __factor);

                fader.alpha -= 0.02f;

                if (Mathf.Approximately(fader.alpha, 0f))
                {
                    _fadeActive = false;
                }
                print("fade in looping");
            }
        }

        else if (!__fadingIn)
        {
            while (_fadeActive)
            {
                yield return new WaitForSecondsRealtime(0.04f);
                fader.alpha += 0.02f;
                if (Mathf.Approximately(fader.alpha, 1f))
                {
                    _fadeActive = false;
                }

                print("fade out looping");
            }
        }

        yield break;
    }

    public void StartOnScoreRoutine(bool __bluePlayerHasScored)
    {
        if (__bluePlayerHasScored)
        {
            StartCoroutine(OnScoreRoutine(Players.RedPlayer));
        }
        else
        {
            StartCoroutine(OnScoreRoutine(Players.BluePlayer));
        }
    }

    IEnumerator OnScoreRoutine(Players __player)
    {
        GoalTextObject.SetActive(true);
        //Handheld.Vibrate();

        yield return new WaitForSeconds(2f);

        GoalTextObject.SetActive(false);

        CheckEndGame();

        if (!gameFinished)
        {
            audioSource.clip = AudioManager.Instance.WhistleSFX;
            audioSource.Play();

            if (__player == Players.RedPlayer)
            {
                GameManager.Instance.PlayingBall.transform.position = GameManager.Instance.RedPlayerSpawnTransform.position;

                Rigidbody _rb = GameManager.Instance.PlayingBall.GetComponent<Rigidbody>();
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            else
            {
                GameManager.Instance.PlayingBall.transform.position = GameManager.Instance.BluePlayerSpawnTransform.position;

                Rigidbody _rb = GameManager.Instance.PlayingBall.GetComponent<Rigidbody>();
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }


    }

    private void CheckEndGame()
    {
        foreach (KeyValuePair<int, Photon.Realtime.Player> _pl in PhotonNetwork.CurrentRoom.Players)
        {
            if (_pl.Value == PhotonNetwork.MasterClient)
            {
                hostNameText = _pl.Value.NickName;
            }
            else
            {
                clientNameText = _pl.Value.NickName;
            }
        }

        if (BluePlayerScore >= 5)
        {
            gameFinished = true;
            photonView.RPC("RPC_EndGame", RpcTarget.AllBuffered, clientNameText, false);
        }

        else if (RedPlayerScore >= 5)
        {
            gameFinished = true;
            photonView.RPC("RPC_EndGame", RpcTarget.AllBuffered, hostNameText, true);
        }

    }

    [PunRPC]
    public void RPC_EndGame(string __winner, bool __redIsWinner)
    {
        EndGamePanel.SetActive(true);

        EndGameText.text = __winner + " " + " is the Winner!";

        foreach (NetTrigger _nt in FindObjectsOfType<NetTrigger>())
        {
            _nt.enabled = false;
            _nt.transform.GetComponent<BoxCollider>().isTrigger = false;
            _nt.transform.GetComponent<BoxCollider>().enabled = false;
        }

        if (__redIsWinner && isRedPlayer)
        {
            audioSource.clip = AudioManager.Instance.GameWinSFX;
            audioSource.Play();
        }
        else if (!__redIsWinner && isRedPlayer)
        {
            audioSource.clip = AudioManager.Instance.GameLostSFX;
            audioSource.Play();
        }
        else if (__redIsWinner && !isRedPlayer)
        {
            audioSource.clip = AudioManager.Instance.GameLostSFX;
            audioSource.Play();
        }
        else if (!__redIsWinner && !isRedPlayer)
        {
            audioSource.clip = AudioManager.Instance.GameWinSFX;
            audioSource.Play();
        }

        StartCoroutine(ChangeEndGameAudio());

        faderScoreTextSetter.SetEndGameScore();

        EndGamePanel.SetActive(true);
    }

    IEnumerator ChangeEndGameAudio()
    {
        yield return new WaitForSeconds(5f);

        AudioManager.Instance.gameIsOver = true;
        AudioManager.Instance.audioSourceTheme.clip = AudioManager.Instance.EndGameSFX;
        AudioManager.Instance.audioSourceTheme.Play();
    }

    public void OnRematchAccepted()
    {
        photonView.RPC("RPC_OnRematchAccepted", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_OnRematchAccepted()
    {
        StartCoroutine(ReloadSceneRoutine());
    }

    IEnumerator ReloadSceneRoutine()
    {
        gameIsStarted = false;

        GameManager.Instance.StartFadeRoutine(false, 4f);

        yield return new WaitForSeconds(2f);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(1);
    }
    public void OnRematchRefuseButtonClicked()
    {
        photonView.RPC("RPC_OnRematchRefused", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    public void RPC_OnRematchRefused()
    {
        StartCoroutine(OtherPlayerRefusedRoutine());
    }

    IEnumerator OtherPlayerRefusedRoutine()
    {
        GameManager.Instance.RematchText.text = "Opponent Disconnected";
        yield return new WaitForSeconds(2f);
        StartCoroutine(LoadMainMenuRoutine());
    }

    IEnumerator LoadMainMenuRoutine()
    {
        GameManager.Instance.StartFadeRoutine(false, 2f);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
}
