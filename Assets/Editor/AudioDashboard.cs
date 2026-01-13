using UnityEngine;
using UnityEditor;

// Этот класс наследуется от EditorWindow, что позволяет создавать свои окна внутри Unity.
public class AudioDashboard : EditorWindow
{
    // Ссылки на префабы Игрока и Врага, настройки которых мы хотим менять.
    private GameObject _playerPrefab;
    private GameObject _enemyPrefab;

    // Позиция прокрутки (нужна, если настроек станет слишком много и они не влезут в окно).
    private Vector2 _scrollPos;

    // Этот атрибут добавляет пункт в верхнее меню Unity: Tools -> Audio Dashboard.
    [MenuItem("Tools/Audio Dashboard 🎧")]
    public static void ShowWindow()
    {
        // Создает или фокусирует существующее окно.
        GetWindow<AudioDashboard>("Audio Config");
    }

    // OnGUI — это метод, который отрисовывает интерфейс окна (кнопки, поля и т.д.).
    void OnGUI()
    {
        GUILayout.Label("Центр управления звуком", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Блок выбора префабов
        GUILayout.Label("1. Перетащи сюда префабы из папки Project:", EditorStyles.helpBox);
        // ObjectField позволяет перетаскивать объекты Unity в поле.
        _playerPrefab = (GameObject)EditorGUILayout.ObjectField("Player Prefab", _playerPrefab, typeof(GameObject), false);
        _enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab", _enemyPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        GUILayout.Label("2. Настройки звуков:", EditorStyles.boldLabel);

        // Начинаем зону прокрутки
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        // Если префаб игрока назначен, рисуем его настройки
        if (_playerPrefab != null)
        {
            DrawPlayerAudioSettings();
        }
        else
        {
            EditorGUILayout.HelpBox("Назначь Player Prefab, чтобы видеть его звуки.", MessageType.Info);
        }

        EditorGUILayout.Space();
        DrawLine(); // Рисуем разделитель
        EditorGUILayout.Space();

        // Если префаб врага назначен, рисуем его настройки
        if (_enemyPrefab != null)
        {
            DrawEnemyAudioSettings();
        }
        else
        {
            EditorGUILayout.HelpBox("Назначь Enemy Prefab, чтобы видеть его звуки.", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPlayerAudioSettings()
    {
        GUILayout.Label($"Настройки Игрока ({_playerPrefab.name})", EditorStyles.boldLabel);

        // Попытка получить компонент PlayerController
        var playerController = _playerPrefab.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // SerializedObject позволяет безопасно менять данные (работает Ctrl+Z и сохранение префаба).
            SerializedObject so = new SerializedObject(playerController);
            so.Update(); // Синхронизируем данные

            EditorGUILayout.LabelField("Движение (PlayerController)", EditorStyles.miniBoldLabel);

            // Отрисовываем поля скрипта PlayerController
            EditorGUILayout.PropertyField(so.FindProperty("footstepSounds"), new GUIContent("Звуки шагов"), true);
            EditorGUILayout.PropertyField(so.FindProperty("stepInterval"), new GUIContent("Интервал шага (сек)"));

            so.ApplyModifiedProperties(); // Применяем изменения обратно в префаб
        }

        EditorGUILayout.Space();

        // Попытка получить компонент PlayerCuteness
        var playerCuteness = _playerPrefab.GetComponent<PlayerCuteness>();
        if (playerCuteness != null)
        {
            SerializedObject so = new SerializedObject(playerCuteness);
            so.Update();

            EditorGUILayout.LabelField("Милота / Геймплей (PlayerCuteness)", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(so.FindProperty("damageSound"), new GUIContent("Звук получения милоты"));
            EditorGUILayout.PropertyField(so.FindProperty("restoreSound"), new GUIContent("Звук убийства врага"));
            EditorGUILayout.PropertyField(so.FindProperty("gameOverSound"), new GUIContent("Звук Game Over"));

            so.ApplyModifiedProperties();
        }
    }

    private void DrawEnemyAudioSettings()
    {
        GUILayout.Label($"Настройки Врага ({_enemyPrefab.name})", EditorStyles.boldLabel);

        // Настройки AI врага
        var enemyAI = _enemyPrefab.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            SerializedObject so = new SerializedObject(enemyAI);
            so.Update();

            EditorGUILayout.LabelField("Бой (EnemyAI)", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("attackSound"), new GUIContent("Звук Атаки"));

            so.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();

        // Настройки здоровья врага
        var enemyHealth = _enemyPrefab.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            SerializedObject so = new SerializedObject(enemyHealth);
            so.Update();

            EditorGUILayout.LabelField("Здоровье (EnemyHealth)", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("hitSound"), new GUIContent("Звук получения урона"));
            EditorGUILayout.PropertyField(so.FindProperty("deathSound"), new GUIContent("Звук смерти"));

            so.ApplyModifiedProperties();
        }
    }

    // Вспомогательный метод для рисования серой линии-разделителя
    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}