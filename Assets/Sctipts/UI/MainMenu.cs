using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public CanvasGroup mainMenuPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Настройки")]
    public SettingsMenu settingsMenu;

    private const string GAME_SCENE_NAME = "GameScene";

    private void Start()
    {
        if (settingsMenu != null && settingsMenu.gameObject.scene.name == null)
        {
            Debug.LogError("ОШИБКА: SettingsMenu - это префаб! Перетащи объект со сцены.");
        }

        if (settingsMenu != null) settingsMenu.gameObject.SetActive(false);

        InitializeButtons();
        Show();
    }

    private void Update()
    {
        // Закрытие настроек по Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu != null && settingsMenu.gameObject.activeSelf)
            {
                settingsMenu.CloseSettings();
            }
        }
    }

    private void InitializeButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(() => {
                if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(GAME_SCENE_NAME);
                else SceneManager.LoadScene(GAME_SCENE_NAME);
            });

            // Защита от дурака: добавляем звук кнопке, если забыли в редакторе
            if (playButton.GetComponent<UIButtonSound>() == null) playButton.gameObject.AddComponent<UIButtonSound>();
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            if (settingsButton.GetComponent<UIButtonSound>() == null) settingsButton.gameObject.AddComponent<UIButtonSound>();
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(() => Application.Quit());
            if (exitButton.GetComponent<UIButtonSound>() == null) exitButton.gameObject.AddComponent<UIButtonSound>();
        }
    }

    public void OpenSettings()
    {
        if (settingsMenu == null) return;

        if (UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);

        Hide();

        // Передаем Action (лямбда-выражение), который выполнится при нажатии кнопки "Назад" в настройках
        settingsMenu.Open(onBackAction: () => {
            this.Show();
        });
    }

    public void Show()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 1;
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 0;
            mainMenuPanel.interactable = false;
            mainMenuPanel.blocksRaycasts = false;
        }
    }
}