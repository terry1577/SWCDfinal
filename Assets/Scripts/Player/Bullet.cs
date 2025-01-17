using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject PlayerCenter;
    private Rigidbody2D rb;
    public float bulletspeed;

    [SerializeField] public float dampingFactor = 10f; // 감쇠 계수 τ
    [SerializeField] public float duration = 10f; // 사운드의 길이 (초)
    [SerializeField] public int sampleRate = 44100; // 오디오 샘플레이트
    [SerializeField] public float frequency = 350.0f;
    private float initialAmplitude;
    private bool isPlaying;
    public float minAmplitude = 0.01f; // 사인 주파수의 진폭 값이 이 값보다 작아지면 사운드를 멈춤
    private float decayedAmplitude; // 현재 진폭

    private AudioSource audioSource;
    private AudioClip audioClip;
    private float startTime; // 사운드 재생 시작 시간

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

        // 5초 후에 총알을 파괴하는 타이머를 시작
        Invoke("DestroyBulletAfterTimeout", 5f);
    }

    void Update()
    {
        if (isPlaying)
        {
            // 감쇠율에 따라 amplitude 값을 조절하여 사운드를 감쇠시킴
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

        isPlaying = true; // 사운드 재생중인지 확인
        startTime = Time.time; // 사운드 재생 시작 시간 초기화

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

        // 렌더러와 충돌 컴포넌트를 비활성화하여 오브젝트를 보이지 않게 처리
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }

        // 소리 재생이 끝난 후 오브젝트를 파괴
        Destroy(gameObject, audioClip.length);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnAttack(collision.transform);

            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 자신의 Rigidbody2D 가져오기
                float m1 = rb.mass;

                // 충돌한 물체의 Rigidbody2D 가져오기
                Rigidbody2D rb2 = collision.collider.GetComponent<Rigidbody2D>();
                float m2 = (rb2 != null) ? rb2.mass : 1.0f;

                // 충돌에 사용된 힘 가져오기
                float contactForceMagnitude = contact.normalImpulse / Time.fixedDeltaTime;

                // 아날로그 장치의 출력 전압 상수 A*
                float A_star = 2.0f; // 예제에서는 초기 값을 1.0으로 설정
                float finalAmplitude = (A_star * contactForceMagnitude / (m1 + m2));


                ////// 유저의 시선과 오브젝트의 거리를 측정해서 최종 amplitude의 값을 조정하는 부분
                // 유저의 시선 위치 얻기
                Vector2 gazeWorldPoint = NewBehaviourScript.latestGazeWorldPoint;

                // 충돌이 발생한 위치
                Vector2 collisionPoint = contact.point;

                // 시선 위치와 충돌 위치 사이의 거리 계산
                float distance = Vector2.Distance(gazeWorldPoint, collisionPoint);

                // 거리에 따라 진폭 조정 (가까울수록 1에 가깝고, 멀수록 0에 가까움)
                float distanceFactor = Mathf.Clamp01(1.0f - (distance / 5.0f)); // 여기서 5.0f는 조정할 수 있는 거리 스케일
                distanceFactor = Mathf.Max(distanceFactor, 0.01f); // 최소값을 0.1로 설정
                finalAmplitude *= distanceFactor;
                ////// 여기까지


                // 초기 진폭 설정
                initialAmplitude = finalAmplitude;
                decayedAmplitude = finalAmplitude;

                // 사운드 생성 및 재생
                GenerateAndPlaySound();

                // 5초 타이머를 취소하고 소리 재생이 끝난 후 파괴 타이머로 대체
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
        // 일정 시간이 지난 후 총알을 파괴
        Destroy(gameObject);
    }
}
