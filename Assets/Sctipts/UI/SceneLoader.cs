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
        // Показываем экран загрузки
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            // Обновляем UI загрузки
            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }
            
            if (loadingText != null)
            {
                loadingText.text = $"Загрузка... {progress * 100:F0}%";
            }
            
            if (operation.progress >= 0.9f)
            {
                // Ждем немного перед переходом
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Скрываем экран загрузки
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
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