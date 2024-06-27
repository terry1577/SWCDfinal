using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using System.Text;

public class AudioDeviceSelector : MonoBehaviour
{
    private FMOD.System lowLevelSystem;
    private int outputDeviceIndex = -1; // ���ϴ� ��� ��ġ�� �ε���

    void Start()
    {
        // FMOD �ý��� �ʱ�ȭ
        RuntimeManager.StudioSystem.getCoreSystem(out lowLevelSystem);

        // ��� ��ġ ��� ��������
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

            // ���ϴ� ��� ��ġ�� ���� (��: Ư�� �̸��� ���� ��ġ)
            if (name.Contains("6- COX SCARLET"))
            {
                outputDeviceIndex = i;
            }
        }

        // ��� ��ġ ����
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