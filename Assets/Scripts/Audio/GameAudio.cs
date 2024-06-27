using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Linq;
using System;

public class GameAudio : MonoBehaviour
{
    [SerializeField] EventReference FootstepEvent;
    [SerializeField] float rate;
    [SerializeField] GameObject Player;
    [SerializeField] Animator Player_Anim;

    float time;

    public void PlayFootStep()
    {
        RuntimeManager.PlayOneShotAttached(FootstepEvent, Player);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (Player_Anim.GetBool("isWalking") == true && Player_Anim.GetBool("isJumping") == false)
        {
            if (time >= rate)
            {
                PlayFootStep();
                time = 0;
            }

        }
    }

    public void UpdateCache(IEnumerable<int> source)
    {
        if (source.Any())
        {
            int maxValue = source.Max();
            Console.WriteLine("Max value: " + maxValue);
        }
        else
        {
            Console.WriteLine("Sequence contains no elements.");
        }
    }
}