using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Mengelola UI menu utama, termasuk tombol mulai permainan dan tombol keluar dari aplikasi.
/// </summary>
public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
#if UNITY_EDITOR
    [SerializeField] private SceneAsset gameScene;
#endif
    private string gameSceneName;

#if UNITY_EDITOR
    /// <summary>
    /// Memvalidasi komponen saat di editor untuk memastikan nama scene tersimpan.
    /// </summary>
    private void OnValidate()
    {
        if (gameScene != null)
            gameSceneName = gameScene.name;
    }
#endif

    /// <summary>
    /// Menginisialisasi pendengar untuk tombol mulai dan keluar saat objek aktif pertama kali.
    /// </summary>
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    /// <summary>
    /// Menangani tombol mulai diklik dengan memuat scene permainan.
    /// </summary>
    private void OnStartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Menangani tombol keluar diklik dengan menutup aplikasi.
    /// </summary>
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
