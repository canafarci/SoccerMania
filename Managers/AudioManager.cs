using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    //public
    [Space(10)]
    [Header("THEME SFXs")]

    public AudioClip[] playableLevelMusic;
    public AudioClip[] mainMenuMusic;
    [Space(10)]
    [Header("PLAYER SFXs")]
    public AudioClip[] BallKickSFX;
    public AudioClip GameWinSFX, GameLostSFX, CoinTossSFX, WhistleSFX, GoalSFX, EndGameSFX;
    //private
    public AudioSource audioSourceTheme, ballKickSource;
    public bool isMainMenu, gameIsOver;
    private bool isFirstTimeMusicPlaying = true;
    private int randomMusicIndex, previousMusicIndex;

    private static int ssIndex;

    //singleton
    public static AudioManager Instance { get; private set; }
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

        gameIsOver = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ScreenCapture.CaptureScreenshot("ss/" + ssIndex.ToString() + ".png");
            ssIndex++;
        }

        if (!gameIsOver)
            CheckAndPlayMusic();
    }

    public void PlayBallKickSFX()
    {
        ballKickSource.clip = BallKickSFX[Random.Range(0, BallKickSFX.Length)];
        ballKickSource.Play();
    }

    private void CheckAndPlayMusic()
    {
        //check if audiosource is playing, if not loop music
        if (!audioSourceTheme.isPlaying)
        {
            //Check if main menu and play music accordingly
            if (isMainMenu)
            {
                audioSourceTheme.clip = mainMenuMusic[0];
                audioSourceTheme.Play();
            }
            if (!isMainMenu)
            {
                PlayRandomMusic(playableLevelMusic);
            }
        }
    }

    private void PlayRandomMusic(AudioClip[] music, bool loop = true)
    {
        randomMusicIndex = Random.Range(0, music.Length);

        if (randomMusicIndex == previousMusicIndex)
        {
            PlayRandomMusic(music, loop);
        }

        audioSourceTheme.clip = music[randomMusicIndex];
        previousMusicIndex = randomMusicIndex;

        audioSourceTheme.Play();
    }



    private void OnSceneLoaded(Scene __loadedScene, LoadSceneMode arg1)
    {
        if (__loadedScene.buildIndex != 0)
        {
            isMainMenu = false;

            audioSourceTheme.Stop();
        }
        else
        {
            isMainMenu = true;
        }
    }

}
