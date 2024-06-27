using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float disappearDelay; // ������� �� ��� �ð�
    public float reappearDelay; // �ٽ� ��Ÿ���� �ð�

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
        // �÷��� ��Ȱ��ȭ
        platformRenderer.enabled = false;
        platformCollider.enabled = false;
        Invoke("AppearAfterTime", reappearDelay);
    }

    void AppearAfterTime()
    {
        // �÷��� Ȱ��ȭ
        platformRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
