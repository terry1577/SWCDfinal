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
    public Image gazeIndicator; // UI 점 이미지
    public Canvas uiCanvas; // UI 캔버스
    public Camera mainCamera; // 메인 카메라

    public static Vector2 latestGazeWorldPoint = Vector2.zero; // 시선 위치 저장

    private void Start()
    {
        TobiiGameIntegrationApi.TrackTracker("tobii-prp://IS5FF-100203250282");
        // Canvas의 Sorting Order를 높여서 다른 오브젝트보다 앞에 오도록 설정
        uiCanvas.sortingOrder = 10; // 필요한 만큼 큰 값으로 설정

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

        // 최신 시선 위치 업데이트
        latestGazeWorldPoint = worldPoint;

        // UI 점 이미지의 위치를 변경하여 눈의 위치에 표시
        gazeIndicator.rectTransform.position = worldPoint3D; // UI 요소의 위치는 Vector3를 사용.
    }
}
