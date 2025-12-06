using UnityEngine;
using UnityEditor;

// Этот класс создает свое собственное окно в Unity
public class AudioDashboard : EditorWindow
{
    // Ссылки на префабы, из которых мы будем тянуть настройки
    private GameObject _playerPrefab;
    private GameObject _enemyPrefab;

    // Скроллбар для окна, если настроек будет слишком много
    private Vector2 _scrollPos;

    // Пункт меню, чтобы открыть окно
    [MenuItem("Tools/Audio Dashboard 🎧")]
    public static void ShowWindow()
    {
        GetWindow<AudioDashboard>("Audio Config");
    }

    void OnGUI()
    {
        // Красивый заголовок
        GUILayout.Label("Центр управления звуком", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Поля для перетаскивания префабов
        GUILayout.Label("1. Перетащи сюда свои префабы (из папки Project):", EditorStyles.helpBox);
        _playerPrefab = (GameObject)EditorGUILayout.ObjectField("Player Prefab", _playerPrefab, typeof(GameObject), false);
        _enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab", _enemyPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        GUILayout.Label("2. Настройки звуков:", EditorStyles.boldLabel);

        // Начало зоны прокрутки
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        if (_playerPrefab != null)
        {
            DrawPlayerAudioSettings();
        }
        else
        {
            EditorGUILayout.HelpBox("Назначь Player Prefab, чтобы видеть его звуки.", MessageType.Info);
        }

        EditorGUILayout.Space();
        DrawLine();
        EditorGUILayout.Space();

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

        // --- Блок 1: PlayerController (Шаги) ---
        var playerController = _playerPrefab.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Создаем "обертку" SerializedObject, чтобы изменения сохранялись корректно (с поддержкой Ctrl+Z)
            SerializedObject so = new SerializedObject(playerController);
            so.Update(); // Обновляем данные

            EditorGUILayout.LabelField("Движение (PlayerController)", EditorStyles.miniBoldLabel);

            // Отрисовываем конкретные свойства
            EditorGUILayout.PropertyField(so.FindProperty("footstepSounds"), new GUIContent("Звуки шагов"), true);
            EditorGUILayout.PropertyField(so.FindProperty("stepInterval"), new GUIContent("Интервал шага (сек)"));

            so.ApplyModifiedProperties(); // Применяем изменения
        }

        EditorGUILayout.Space();

        // --- Блок 2: PlayerCuteness (Геймплей) ---
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

        // --- Блок 1: EnemyAI (Атака) ---
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

        // --- Блок 2: EnemyHealth (Получение урона) ---
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

    // Вспомогательный метод для рисования разделительной линии
    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}