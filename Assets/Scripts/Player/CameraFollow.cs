using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 플레이어 오브젝트의 Transform

    void Update()
    {
        if (target != null)
        {
            // 카메라의 위치를 플레이어 오브젝트의 위치로 설정
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
