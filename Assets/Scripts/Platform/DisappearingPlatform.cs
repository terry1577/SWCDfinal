using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float disappearDelay; // 사라지기 전 대기 시간
    public float reappearDelay; // 다시 나타나는 시간

    private Renderer platformRenderer;
    private Collider2D platformCollider;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collided");
            Invoke("DisappearAfterTime", disappearDelay);
        }
    }

    void DisappearAfterTime()
    {
        Debug.Log("hi");
        // 플랫폼 비활성화
        platformRenderer.enabled = false;
        platformCollider.enabled = false;
        Invoke("AppearAfterTime", reappearDelay);
    }

    void AppearAfterTime()
    {
        // 플랫폼 활성화
        platformRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
