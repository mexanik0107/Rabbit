using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed;   // �������� ������� �� ������ ��� ��������
    [HideInInspector] public float damage;  // ���� ������� �� ������ ��� ��������

    private Rigidbody2D _rb;
    private float _lifetime = 5f; // ������ �� ������ ������: ���� �������� ����� 5 ���, ���� �� � ���� �� �������

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // ������� �������� "�����" ������������ ���������� ����
        // (� 2D "����" ������� ������ ��������� �� ����������� ������)
        _rb.linearVelocity = transform.up * speed;

        // ���������� ���� ����� �����, ����� �� �������� �����
        Destroy(gameObject, _lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������, ���� �� � ������� �������� (��� ��� ������ EnemyHealth)
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // ���������� ���� ��� ���������
        }
        else if (!collision.CompareTag("Player") && !collision.isTrigger)
        {
            // ���������� ����, ���� ��� ������ � ����� (�� � ������ � �� � �������)
            Destroy(gameObject);
        }
    }
}