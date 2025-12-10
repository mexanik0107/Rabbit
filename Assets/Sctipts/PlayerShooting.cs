using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Audio;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public HUD hud;

    [Header("Audio")]
    // Перетащи сюда группу SFX
    public AudioMixerGroup sfxGroup;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyClickSound;

    [Header("Weapon Stats")]
    public float damage = 2f;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float spreadAngle = 5f;

    [Header("Ammo Settings")]
    public int maxMagazineSize = 10;
    public int maxTotalAmmo = 60;
    public int startTotalAmmo = 30;
    public float reloadTime = 1.5f;

    public int CurrentClip { get; private set; }
    public int CurrentTotalAmmo { get; private set; }
    public bool IsReloading { get; private set; }

    private AudioSource _audioSource;
    private float _nextFireTime = 0f;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

        // Привязываем к микшеру
        if (sfxGroup != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }

        if (hud == null) hud = FindObjectOfType<HUD>();
    }

    void Start()
    {
        CurrentClip = maxMagazineSize;
        CurrentTotalAmmo = startTotalAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (IsReloading) return;

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartReload();
            return;
        }

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
        if (CurrentClip <= 0)
        {
            if (CurrentTotalAmmo > 0) StartReload();
            else PlaySound(emptyClickSound);
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        float randomSpread = Random.Range(-spreadAngle, spreadAngle);
        Quaternion finalRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, finalRotation);

        Projectile projectile = bulletObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.speed = bulletSpeed;
            projectile.damage = damage;
        }

        CurrentClip--;
        UpdateAmmoUI();
        PlaySound(shootSound, randomizePitch: true);
    }

    public void StartReload()
    {
        if (IsReloading) return;
        if (CurrentClip == maxMagazineSize) return;
        if (CurrentTotalAmmo <= 0) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        PlaySound(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxMagazineSize - CurrentClip;
        int ammoToReload = Mathf.Min(ammoNeeded, CurrentTotalAmmo);

        CurrentClip += ammoToReload;
        CurrentTotalAmmo -= ammoToReload;

        IsReloading = false;
        UpdateAmmoUI();
    }

    public void AddAmmo(int amount)
    {
        CurrentTotalAmmo += amount;
        if (CurrentTotalAmmo > maxTotalAmmo) CurrentTotalAmmo = maxTotalAmmo;

        UpdateAmmoUI();

        if (CurrentClip == 0) StartReload();
    }

    private void UpdateAmmoUI()
    {
        if (hud != null) hud.UpdateAmmo(CurrentClip, CurrentTotalAmmo);
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