using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii;
using Tobii.GameIntegration.Net;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Device;

public class NewBehaviourScript : MonoBehaviour
{
    public Image gazeIndicator; // UI �� �̹���
    public Canvas uiCanvas; // UI ĵ����
    public Camera mainCamera; // ���� ī�޶�

    public static Vector2 latestGazeWorldPoint = Vector2.zero; // �ü� ��ġ ����

    private void Start()
    {
        TobiiGameIntegrationApi.TrackTracker("tobii-prp://IS5FF-100203250282");
        // Canvas�� Sorting Order�� ������ �ٸ� ������Ʈ���� �տ� ������ ����
        uiCanvas.sortingOrder = 10; // �ʿ��� ��ŭ ū ������ ����

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        TobiiGameIntegrationApi.Update();

        if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gazePoint))
        {
            // Clamp gazePoint values to be within the range [-1, 1]
            float clampedX = Mathf.Clamp(gazePoint.X, -1f, 1f);
            float clampedY = Mathf.Clamp(gazePoint.Y, -1f, 1f);

            // Convert gaze point to screen coordinates
            Vector2 screenPoint = ConvertGazeToScreenPoint(clampedX, clampedY);

            // Optionally, visualize the gaze point in the Unity scene
            MoveGazeIndicator(screenPoint);
        }
    }

    private Vector2 ConvertGazeToScreenPoint(float gazeX, float gazeY)
    {
        // Tobii gazePoint.X and gazePoint.Y are between -1 and 1
        // We need to convert these to screen coordinates
        float x = (gazeX + 1) * 0.5f * UnityEngine.Screen.width;
        float y = (gazeY + 1) * 0.5f * UnityEngine.Screen.height;

        return new Vector2(x, y);
    }

    private void MoveGazeIndicator(Vector2 screenPoint)
    {
        // Convert screen point to world point
        Vector3 worldPoint3D = mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, mainCamera.nearClipPlane));
        Vector2 worldPoint = new Vector2(worldPoint3D.x, worldPoint3D.y);

        // �ֽ� �ü� ��ġ ������Ʈ
        latestGazeWorldPoint = worldPoint;

        // UI �� �̹����� ��ġ�� �����Ͽ� ���� ��ġ�� ǥ��
        gazeIndicator.rectTransform.position = worldPoint3D; // UI ����� ��ġ�� Vector3�� ���.
    }
}
