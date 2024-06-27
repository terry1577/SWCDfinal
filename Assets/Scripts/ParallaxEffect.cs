using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform[] backgrounds; // 배경 레이어들
    public float[] parallaxScales; // 각 배경 레이어의 패럴랙스 계수
    public float smoothing = 1f; // 부드러운 움직임을 위한 계수

    private Vector3 previousCamPos; // 이전 프레임의 카메라 위치
    private Transform cam; // 카메라의 트랜스폼

    void Awake()
    {
        // 카메라의 트랜스폼을 가져옵니다.
        cam = Camera.main.transform;
    }

    void Start()
    {
        // 이전 프레임의 카메라 위치를 초기화합니다.
        previousCamPos = cam.position;

        // 각 배경 레이어에 대해 패럴랙스 계수를 초기화합니다.
        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    void Update()
    {
        // 각 배경 레이어에 대해 패럴랙스 효과를 적용합니다.
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float targetPosX = backgrounds[i].position.x + parallax;

            Vector3 targetPos = new Vector3(targetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPos, smoothing * Time.deltaTime);
        }

        // 이전 프레임의 카메라 위치를 현재 위치로 갱신합니다.
        previousCamPos = cam.position;
    }
}
