using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    //An enum is a datatype that we can specifiy its values and use
    public enum PowerupType { SpeedUp, SpeedDown}

    public PowerupType myPowerup;           //This Objects powerup type
    public float powerupDuration = 7f;      //The duration of the powerup
    public float SpeedUpMultiplier = 2;
    public float SpeedDownMultiplier = 3;
    PlayerController playerController;      //a reference to out player controller
    
    void Start()
    {
        //find and assign the player controller object to this local reference
        playerController = FindObjectOfType<PlayerController>();
    }

    public void UsePowerup()
    {
        //If this powerup is the speedUp powerup, increase the player controller speed by double
        if (myPowerup == PowerupType.SpeedUp)
            playerController.speed = playerController.baseSpeed * SpeedUpMultiplier;

        //If this powerup is the speedDown powerup, decrease the player controller speed times 2
        if (myPowerup == PowerupType.SpeedDown)
            playerController.speed = playerController.baseSpeed / SpeedDownMultiplier;

        //Start a coroutine to reset the powerups effects
        StartCoroutine(ResetPowerup());
    }

    IEnumerator ResetPowerup()
    {
        yield return new WaitForSeconds(powerupDuration);

        //If this powerup relates to speed, reset our player controller speed to it's base level
        if(myPowerup == PowerupType.SpeedUp || myPowerup == PowerupType.SpeedDown)
                playerController.speed = playerController.baseSpeed;
    }
}
