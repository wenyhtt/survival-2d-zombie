using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool stopBGM = true;

    [Header("Settings")]
    [SerializeField] private bool pauseGameOnGameOver = true;
    [SerializeField] private float delayBeforeShow = 0f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool returnToMainMenuOnQuit = true;

    private void Awake()
    {
        // Ensure game time is running normally on scene load
        Time.timeScale = 1f;

        AutoAssignReferences();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void OnEnable()
    {
        Health.OnPlayerDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Health.OnPlayerDied -= HandlePlayerDeath;
    }

    private void AutoAssignReferences()
    {
        if (gameOverPanel == null)
        {
            Transform panelTrans = transform.Find("Panel");
            gameOverPanel = panelTrans != null ? panelTrans.gameObject : gameObject;
        }

        if (restartButton == null && gameOverPanel != null)
        {
            Transform playTrans = gameOverPanel.transform.Find("Play");
            if (playTrans != null) restartButton = playTrans.GetComponent<Button>();
        }

        if (quitButton == null && gameOverPanel != null)
        {
            Transform quitTrans = gameOverPanel.transform.Find("Quit");
            if (quitTrans != null) quitButton = quitTrans.GetComponent<Button>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
    }

    private void HandlePlayerDeath()
    {
        if (delayBeforeShow > 0f)
        {
            StartCoroutine(ShowGameOverRoutine());
        }
        else
        {
            ShowGameOver();
        }
    }

    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeShow);
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (stopBGM)
        {
            GameObject bgmObj = GameObject.Find("AudioManager");
            if (bgmObj != null)
            {
                AudioSource bgmSource = bgmObj.GetComponent<AudioSource>();
                if (bgmSource != null) bgmSource.Stop();
            }
        }

        if (gameOverSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSFX);
        }

        if (pauseGameOnGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;
        if (returnToMainMenuOnQuit && !string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Application.Quit();
        }
    }
}
