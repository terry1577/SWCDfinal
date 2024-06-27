using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpForce = 30f; // �������� ���� ��

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ������Ʈ�� Rigidbody2D�� �����ɴϴ�.
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        // Rigidbody2D�� �ִ� ��쿡�� ���� ���� ���մϴ�.
        if (rb != null)
        {
            // ���� �ӵ��� �ʱ�ȭ�ϰ� ���� ���� ���� �ݴϴ�.
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
