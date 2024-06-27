using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpForce = 30f; // 점프대의 점프 힘

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 Rigidbody2D를 가져옵니다.
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        // Rigidbody2D가 있는 경우에만 점프 힘을 가합니다.
        if (rb != null)
        {
            // 현재 속도를 초기화하고 위로 점프 힘을 줍니다.
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
