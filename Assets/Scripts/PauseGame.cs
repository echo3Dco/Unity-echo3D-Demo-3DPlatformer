using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public bool gamePaused = false;
    public AudioSource levelMusic;
    public GameObject pauseMenu;
    public GameObject mainCamera;
    public AudioSource pauseOpen;
    public AudioSource pauseClose;

    PlayerControls playerControls;

    private InputAction pause;

    private void Awake()
    {
        playerControls = new PlayerControls();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        pause = playerControls.Player.Pause;
        pause.Enable();
    }

    private void OnDisable()
    {
        pause.Disable();
    }

    // Update is called once per frame
    void Update()
    {

        if (pause.triggered)
        {
            if(gamePaused == false)
            {
                Time.timeScale = 0;
                gamePaused = true;
                Cursor.visible = true;
                levelMusic.Pause();
                pauseOpen.Play();
                pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                gamePaused = false;
                Cursor.visible = false;
                pauseClose.Play();
                levelMusic.UnPause();
                pauseMenu.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gamePaused = false;
        Cursor.visible = false;
        levelMusic.UnPause();
        pauseMenu.SetActive(false);
    }
    public void Restart()
    {
        Time.timeScale = 1;
        gamePaused = false;
        Cursor.visible = false;
        levelMusic.UnPause();
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
        Time.timeScale = 1;
        gamePaused = false;
        Cursor.visible = false;
        levelMusic.UnPause();
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(1);
    }
}
