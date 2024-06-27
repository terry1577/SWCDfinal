using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform[] backgrounds; // ��� ���̾��
    public float[] parallaxScales; // �� ��� ���̾��� �з����� ���
    public float smoothing = 1f; // �ε巯�� �������� ���� ���

    private Vector3 previousCamPos; // ���� �������� ī�޶� ��ġ
    private Transform cam; // ī�޶��� Ʈ������

    void Awake()
    {
        // ī�޶��� Ʈ�������� �����ɴϴ�.
        cam = Camera.main.transform;
    }

    void Start()
    {
        // ���� �������� ī�޶� ��ġ�� �ʱ�ȭ�մϴ�.
        previousCamPos = cam.position;

        // �� ��� ���̾ ���� �з����� ����� �ʱ�ȭ�մϴ�.
        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    void Update()
    {
        // �� ��� ���̾ ���� �з����� ȿ���� �����մϴ�.
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float targetPosX = backgrounds[i].position.x + parallax;

            Vector3 targetPos = new Vector3(targetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPos, smoothing * Time.deltaTime);
        }

        // ���� �������� ī�޶� ��ġ�� ���� ��ġ�� �����մϴ�.
        previousCamPos = cam.position;
    }
}
