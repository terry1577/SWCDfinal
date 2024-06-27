using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    [SerializeField] EventReference GunshotEvent;
    [SerializeField] GameObject Player;
    public GameObject bullet;
    public Transform bulletpos;

    void OnFire()
    {
        Quaternion rotation = bulletpos.rotation * Quaternion.Euler(0f, 0f, -90f);
        Instantiate(bullet, bulletpos.position, rotation);
        PlaygunSound();
    }

    public void PlaygunSound()
    {
        RuntimeManager.PlayOneShotAttached(GunshotEvent, Player);
    }
}
