using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Загрузка")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Text loadingText;

    private void Awake()
    {
        // Singleton, который не уничтожается при переходе между сценами
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);

        // Асинхронная операция загрузки
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Не переключать сразу, ждать

        while (!operation.isDone)
        {
            // Unity загружает сцену до 0.9, потом ждет активации.
            // Нормализуем значение, чтобы слайдер был от 0 до 1.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingSlider != null) loadingSlider.value = progress;
            if (loadingText != null) loadingText.text = $"Загрузка... {progress * 100:F0}%";

            if (operation.progress >= 0.9f)
            {
                // Искусственная задержка (опционально), чтобы игрок успел прочитать советы
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null) loadingScreen.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}