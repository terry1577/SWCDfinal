using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject PlayerCenter;
    private Rigidbody2D rb;
    public float bulletspeed;

    [SerializeField] public float dampingFactor = 10f; // ���� ��� ��
    [SerializeField] public float duration = 10f; // ������ ���� (��)
    [SerializeField] public int sampleRate = 44100; // ����� ���÷���Ʈ
    [SerializeField] public float frequency = 350.0f;
    private float initialAmplitude;
    private bool isPlaying;
    public float minAmplitude = 0.01f; // ���� ���ļ��� ���� ���� �� ������ �۾����� ���带 ����
    private float decayedAmplitude; // ���� ����

    private AudioSource audioSource;
    private AudioClip audioClip;
    private float startTime; // ���� ��� ���� �ð�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerCenter = GameObject.FindGameObjectWithTag("Player");

        Vector2 direction = transform.position - PlayerCenter.transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * bulletspeed;

        // Ensure AudioSource component exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 5�� �Ŀ� �Ѿ��� �ı��ϴ� Ÿ�̸Ӹ� ����
        Invoke("DestroyBulletAfterTimeout", 5f);
    }

    void Update()
    {
        if (isPlaying)
        {
            // �������� ���� amplitude ���� �����Ͽ� ���带 �����Ŵ
            float elapsedTime = Time.time - startTime;
            decayedAmplitude = initialAmplitude * Mathf.Exp(-dampingFactor * elapsedTime);
        }
    }

    void GenerateAndPlaySound()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing.");
            return;
        }

        isPlaying = true; // ���� ��������� Ȯ��
        startTime = Time.time; // ���� ��� ���� �ð� �ʱ�ȭ

        int sampleCount = Mathf.CeilToInt(duration * sampleRate);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float value = decayedAmplitude * Mathf.Sin(2 * Mathf.PI * frequency * time) * Mathf.Exp(-dampingFactor * time);
            samples[i] = value;
        }

        audioClip = AudioClip.Create("DampedOscillationSound", sampleCount, 1, sampleRate, false);
        audioClip.SetData(samples, 0);
        audioSource.clip = audioClip;
        audioSource.Play();

        // �������� �浹 ������Ʈ�� ��Ȱ��ȭ�Ͽ� ������Ʈ�� ������ �ʰ� ó��
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }

        // �Ҹ� ����� ���� �� ������Ʈ�� �ı�
        Destroy(gameObject, audioClip.length);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnAttack(collision.transform);

            foreach (ContactPoint2D contact in collision.contacts)
            {
                // �ڽ��� Rigidbody2D ��������
                float m1 = rb.mass;

                // �浹�� ��ü�� Rigidbody2D ��������
                Rigidbody2D rb2 = collision.collider.GetComponent<Rigidbody2D>();
                float m2 = (rb2 != null) ? rb2.mass : 1.0f;

                // �浹�� ���� �� ��������
                float contactForceMagnitude = contact.normalImpulse / Time.fixedDeltaTime;

                // �Ƴ��α� ��ġ�� ��� ���� ��� A*
                float A_star = 2.0f; // ���������� �ʱ� ���� 1.0���� ����
                float finalAmplitude = (A_star * contactForceMagnitude / (m1 + m2));


                ////// ������ �ü��� ������Ʈ�� �Ÿ��� �����ؼ� ���� amplitude�� ���� �����ϴ� �κ�
                // ������ �ü� ��ġ ���
                Vector2 gazeWorldPoint = NewBehaviourScript.latestGazeWorldPoint;

                // �浹�� �߻��� ��ġ
                Vector2 collisionPoint = contact.point;

                // �ü� ��ġ�� �浹 ��ġ ������ �Ÿ� ���
                float distance = Vector2.Distance(gazeWorldPoint, collisionPoint);

                // �Ÿ��� ���� ���� ���� (�������� 1�� ������, �ּ��� 0�� �����)
                float distanceFactor = Mathf.Clamp01(1.0f - (distance / 5.0f)); // ���⼭ 5.0f�� ������ �� �ִ� �Ÿ� ������
                distanceFactor = Mathf.Max(distanceFactor, 0.01f); // �ּҰ��� 0.1�� ����
                finalAmplitude *= distanceFactor;
                ////// �������


                // �ʱ� ���� ����
                initialAmplitude = finalAmplitude;
                decayedAmplitude = finalAmplitude;

                // ���� ���� �� ���
                GenerateAndPlaySound();

                // 5�� Ÿ�̸Ӹ� ����ϰ� �Ҹ� ����� ���� �� �ı� Ÿ�̸ӷ� ��ü
                CancelInvoke("DestroyBulletAfterTimeout");
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.OnDamaged();
        }
    }

    void DestroyBulletAfterTimeout()
    {
        // ���� �ð��� ���� �� �Ѿ��� �ı�
        Destroy(gameObject);
    }
}
