using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    bool KnockedOver = false;
    PlayerController playerController;

void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

void Update()
    {
        //A pin is only considered knocked over if its past halfway in it's rotation
        if (transform.up.y < 0.5f && !KnockedOver)
        {
            playerController.PinFall();
            KnockedOver = true;
        }
    }
}
