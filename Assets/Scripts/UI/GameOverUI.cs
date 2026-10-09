using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Mengelola tampilan layar Game Over, termasuk memunculkan panel, memainkan efek suara,
/// dan menyediakan tombol untuk memulai ulang atau kembali ke menu utama.
/// </summary>
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

    /// <summary>
    /// Dipanggil saat skrip diinisialisasi, mengatur referensi awal dan status tombol.
    /// </summary>
    private void Awake()
    {
        // Pastikan waktu permainan berjalan normal saat memuat scene
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

    /// <summary>
    /// Dipanggil saat objek diaktifkan, mendaftar ke peristiwa kematian pemain.
    /// </summary>
    private void OnEnable()
    {
        Health.OnPlayerDied += HandlePlayerDeath;
    }

    /// <summary>
    /// Dipanggil saat objek dinonaktifkan, membatalkan pendaftaran dari peristiwa kematian pemain.
    /// </summary>
    private void OnDisable()
    {
        Health.OnPlayerDied -= HandlePlayerDeath;
    }

    /// <summary>
    /// Secara otomatis menetapkan referensi komponen UI dan audio yang belum diisi.
    /// </summary>
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

    /// <summary>
    /// Menangani gameover pemain atau langsung menampilkan antarmuka Game Over.
    /// </summary>
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

    /// <summary>
    /// Menampilkan antarmuka Game Over, memutar suara, menghentikan musik latar, dan menjeda permainan jika diatur.
    /// </summary>
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

    /// <summary>
    /// Dipanggil saat tombol restart diklik, mengatur ulang skala waktu dan memuat ulang scene saat ini.
    /// </summary>
    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Dipanggil saat tombol keluar diklik, kembali ke menu utama atau keluar dari aplikasi.
    /// </summary>
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
