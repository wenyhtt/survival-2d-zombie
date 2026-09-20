using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
#if UNITY_EDITOR
    [SerializeField] private SceneAsset gameScene;
#endif
    private string gameSceneName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameScene != null)
            gameSceneName = gameScene.name;
    }
#endif

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
