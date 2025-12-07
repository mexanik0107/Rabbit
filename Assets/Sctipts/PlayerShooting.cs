using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Нужно для работы Coroutines (IEnumerator)

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Weapon Stats")]
    public float damage = 2f;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float spreadAngle = 5f;

    [Header("Ammo Settings")]
    public int maxMagazineSize = 10;   // Емкость магазина
    public int maxTotalAmmo = 60;      // Максимум патронов в запасе
    public int startTotalAmmo = 30;    // Стартовый запас
    public float reloadTime = 1.5f;    // Время перезарядки в секундах

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;      // Звук начала перезарядки
    public AudioClip emptyClickSound;  // Звук, когда совсем нет патронов
    private AudioSource _audioSource;

    // Публичные свойства для UI
    public int CurrentClip { get; private set; }
    public int CurrentTotalAmmo { get; private set; }
    public bool IsReloading { get; private set; } // Чтобы UI мог показать полоску перезарядки

    private float _nextFireTime = 0f;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        CurrentClip = maxMagazineSize;
        CurrentTotalAmmo = startTotalAmmo;
        UpdateAmmoDebug();
    }

    void Update()
    {
        // Если мы уже перезаряжаемся, блокируем любое управление оружием
        if (IsReloading) return;

        // --- Ручная Перезарядка (Кнопка R) ---
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartReload();
            return;
        }

        // --- Стрельба (ЛКМ) ---
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= _nextFireTime)
            {
                TryShoot();
                _nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void TryShoot()
    {
        // 1. Если магазин пуст
        if (CurrentClip <= 0)
        {
            // Автоматическая перезарядка, если есть патроны в запасе
            if (CurrentTotalAmmo > 0)
            {
                StartReload();
            }
            else
            {
                // Если и в запасе пусто — просто щелкаем
                PlaySound(emptyClickSound);
            }
            return;
        }

        // 2. Если патроны есть — стреляем
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Расчет разброса
        float randomSpread = Random.Range(-spreadAngle, spreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread);
        Quaternion finalRotation = firePoint.rotation * spreadRotation;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, finalRotation);

        Projectile projectile = bulletObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.speed = bulletSpeed;
            projectile.damage = damage;
        }

        CurrentClip--;

        PlaySound(shootSound, randomizePitch: true);

        // Лог для отладки
        if (CurrentClip == 0) Debug.Log("Магазин пуст!");
    }

    // Публичный метод, чтобы можно было вызвать перезарядку извне (если понадобится)
    public void StartReload()
    {
        // Проверки перед стартом:
        if (IsReloading) return; // Уже в процессе
        if (CurrentClip == maxMagazineSize) return; // Магазин полон
        if (CurrentTotalAmmo <= 0) return; // Нечем заряжать

        // Запускаем корутину (процесс, растянутый во времени)
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        Debug.Log("Перезарядка...");

        PlaySound(reloadSound);

        // Ждем указанное время (reloadTime)
        yield return new WaitForSeconds(reloadTime);

        // --- Логика пополнения после ожидания ---

        int ammoNeeded = maxMagazineSize - CurrentClip;
        int ammoToReload = Mathf.Min(ammoNeeded, CurrentTotalAmmo);

        CurrentClip += ammoToReload;
        CurrentTotalAmmo -= ammoToReload;

        IsReloading = false;
        Debug.Log("Перезарядка завершена!");
        UpdateAmmoDebug();
    }

    public void AddAmmo(int amount)
    {
        CurrentTotalAmmo += amount;
        if (CurrentTotalAmmo > maxTotalAmmo)
        {
            CurrentTotalAmmo = maxTotalAmmo;
        }
        Debug.Log($"Подобраны патроны: +{amount}. Всего: {CurrentTotalAmmo}");

        // Опционально: если магазин был пуст и мы подобрали патроны, можно автоматически начать перезарядку
        if (CurrentClip == 0)
        {
            StartReload();
        }
    }

    private void UpdateAmmoDebug()
    {
        Debug.Log($"Ammo: {CurrentClip} / {CurrentTotalAmmo}");
    }

    private void PlaySound(AudioClip clip, bool randomizePitch = false)
    {
        if (clip != null && _audioSource != null)
        {
            _audioSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
            _audioSource.PlayOneShot(clip);
        }
    }
}