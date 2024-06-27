using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] public float dampingFactor; // ���� ��� ��
    [SerializeField] public float duration; // ������ ���� (��)
    [SerializeField] public int sampleRate = 44100; // ����� ���÷���Ʈ
    [SerializeField] public float frequency;
    [SerializeField] string TargetTag;
    [SerializeField] public float contactForceMultiplier = 1;
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

        // Ensure AudioSource component exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == TargetTag)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // �ڽ��� Rigidbody2D ��������
                float m1 = rb.mass;

                // �浹�� ��ü�� Rigidbody2D ��������
                Rigidbody2D rb2 = collision.collider.GetComponent<Rigidbody2D>();
                float m2 = (rb2 != null) ? rb2.mass : 1.0f;

                // �浹�� ���� �� ��������
                float contactForceMagnitude = contact.normalImpulse / Time.fixedDeltaTime;
                if (contactForceMagnitude == 0)
                {
                    contactForceMagnitude = 10;
                }
                contactForceMagnitude *= contactForceMultiplier;

                // �Ƴ��α� ��ġ�� ��� ���� ��� A*
                float A_star = 1.0f; // ���������� �ʱ� ���� 1.0���� ����
                float finalAmplitude = (A_star * contactForceMagnitude / (m1 + m2));


                ////// ������ �ü��� ������Ʈ�� �Ÿ��� �����ؼ� ���� amplitude�� ���� �����ϴ� �κ�
                // ������ �ü� ��ġ ���
                Vector2 gazeWorldPoint = NewBehaviourScript.latestGazeWorldPoint;

                // �浹�� �߻��� ��ġ
                Vector2 collisionPoint = contact.point;

                // �ü� ��ġ�� �浹 ��ġ ������ �Ÿ� ���
                float distance = Vector2.Distance(gazeWorldPoint, collisionPoint);
                Debug.Log("Distance: " + distance);

                // �Ÿ��� ���� ���� ���� (�������� 1�� ������, �ּ��� 0�� �����)
                float distanceFactor = Mathf.Clamp01(1.0f - (distance / 10.0f)); // ���⼭ 5.0f�� ������ �� �ִ� �Ÿ� ������
                distanceFactor = Mathf.Max(distanceFactor, 0.01f); // �ּҰ��� 0.1�� ����
                finalAmplitude *= distanceFactor;
                Debug.Log("distanceFactor: " + distanceFactor);
                Debug.Log("Adjusted Amplitude: " + finalAmplitude);
                ////// �������


                // �ʱ� ���� ����
                initialAmplitude = finalAmplitude;
                decayedAmplitude = finalAmplitude;

                // ���� ���� �� ���
                GenerateAndPlaySound();
            }
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
    }
}
