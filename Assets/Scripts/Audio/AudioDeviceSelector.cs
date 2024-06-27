using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using System.Text;

public class AudioDeviceSelector : MonoBehaviour
{
    private FMOD.System lowLevelSystem;
    private int outputDeviceIndex = -1; // 원하는 출력 장치의 인덱스

    void Start()
    {
        // FMOD 시스템 초기화
        RuntimeManager.StudioSystem.getCoreSystem(out lowLevelSystem);

        // 출력 장치 목록 가져오기
        int numDrivers;
        lowLevelSystem.getNumDrivers(out numDrivers);

        for (int i = 0; i < numDrivers; i++)
        {
            string name;
            System.Guid guid;
            int systemRate;
            SPEAKERMODE speakerMode;
            int speakerModeChannels;

            lowLevelSystem.getDriverInfo(i, out name, 256, out guid, out systemRate, out speakerMode, out speakerModeChannels);

            UnityEngine.Debug.Log("Driver " + i + ": " + name);

            // 원하는 출력 장치를 선택 (예: 특정 이름을 가진 장치)
            if (name.Contains("6- COX SCARLET"))
            {
                outputDeviceIndex = i;
            }
        }

        // 출력 장치 설정
        if (outputDeviceIndex != -1)
        {
            lowLevelSystem.setDriver(outputDeviceIndex);
        }
        else
        {
            UnityEngine.Debug.LogError("Desired output device not found.");
        }
    }
}