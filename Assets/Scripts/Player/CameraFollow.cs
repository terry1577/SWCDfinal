using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // �÷��̾� ������Ʈ�� Transform

    void Update()
    {
        if (target != null)
        {
            // ī�޶��� ��ġ�� �÷��̾� ������Ʈ�� ��ġ�� ����
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
