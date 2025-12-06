using Mono.Cecil.Cil;
using UnityEngine;
public class PlayerCuteness : MonoBehaviour
{
    [Header("Cuteness Settings")]
    public float maxCuteness = 100f; // Максимум милоты (Game Over)
    public float startCuteness = 0f; // Старт (Брутально)
    [Header("Gameplay Balance")]
    public float passiveCutenessGain = 1f; // Сколько милоты капает само по себе в секунду (нагнетание)

    // Текущее значение (свойство only-get для других скриптов, менять может только этот скрипт)
    public float CurrentCuteness { get; private set; }

    private bool _isGameOver = false;

    void Start()
    {
        CurrentCuteness = startCuteness;
    }

    // --- ПОДПИСКА НА СОБЫТИЯ ---
    // Это паттерн Observer. Мы слушаем, когда умирают враги.

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= OnEnemyKilled;
    }
    // ---------------------------

    void Update()
    {
        if (_isGameOver) return;

        // Постоянное нагнетание милоты со временем (опционально, усложняет игру)
        AddCuteness(passiveCutenessGain * Time.deltaTime);
    }

    // Этот метод вызывается автоматически, когда где-то умирает EnemyHealth
    private void OnEnemyKilled(float brutailityRestoreAmount)
    {
        if (_isGameOver) return;

        // Убийство врага СНИЖАЕТ милоту (делает игру брутальнее)
        RemoveCuteness(brutailityRestoreAmount);
    }

    // Метод для вызова врагами при атаке
    public void AddCuteness(float amount)
    {
        if (_isGameOver) return;

        CurrentCuteness += amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);

        CheckGameOver();
    }

    public void RemoveCuteness(float amount)
    {
        if (_isGameOver) return;

        CurrentCuteness -= amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);
    }

    // Вспомогательный метод для UI (возвращает от 0 до 1)
    public float GetCutenessNormalized()
    {
        return CurrentCuteness / maxCuteness;
    }

    private void CheckGameOver()
    {
        if (CurrentCuteness >= maxCuteness)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        _isGameOver = true;
        Debug.Log("GAME OVER: Вы превратились в зайчика! Слишком мило!");

        // Здесь можно отключить PlayerController
        GetComponent<PlayerController>().enabled = false;

        // TODO: Запустить анимацию превращения и показать экран проигрыша
    }
}
