using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_PlayerShip : MonoBehaviour
{
    public Model_Player playerModel;

    private void Start()
    {
        Debug.Assert(playerModel != null, "Controller_PlayerShip is looking for a reference to Model_Player, but none has been added in the Inspector!");
    }

    private void Update()
    {
        _ApplySmoothingToMotion();
    }

    public void ShipUpdate()
    {
        _TakeInputs();
        _LimitToScreen();
    }

    public void ForceShipPos(Vector3 where)
    {
        playerModel.positionCurrent = playerModel.positionTarget = where;
    }
    public void ForceShipRot(float angle)
    {
        playerModel.rotationCurrent = playerModel.rotationTarget = angle;
    }

    private void _TakeInputs()
    {
        if (Input.GetKey(KeyCode.W))
            playerModel.positionTarget +=
                Vector3.forward * Time.deltaTime * (playerModel.shipSpeed * (1 - playerModel.vFactor));
        if (Input.GetKey(KeyCode.S))
            playerModel.positionTarget -=
                Vector3.forward * Time.deltaTime * (playerModel.shipSpeed * (1 + playerModel.vFactor));
        if (Input.GetKey(KeyCode.A))
        {
            playerModel.positionTarget -= Vector3.right * Time.deltaTime * playerModel.shipSpeed;
            playerModel.rotationCurrent = setRotation(false);
        }
        if (Input.GetKey(KeyCode.D)){
            playerModel.positionTarget += Vector3.right * Time.deltaTime * playerModel.shipSpeed;
            playerModel.rotationCurrent = setRotation(true);
        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            playerModel.rotationCurrent = stabilizeRotation();
        }
}

    private float setRotation(bool r)
    {
        if (r)
        {
            if (playerModel.rotationCurrent < playerModel.turnLimit)
            {
                return playerModel.rotationCurrent += playerModel.rotationSpeed;
            }
            return playerModel.turnLimit;
        }
        
        if (playerModel.rotationCurrent > -playerModel.turnLimit)
            {
                return playerModel.rotationCurrent -= playerModel.rotationSpeed;
            }
        return -playerModel.turnLimit;
    }

    private float stabilizeRotation()
    {
        if (playerModel.rotationCurrent < 5 && playerModel.rotationCurrent > -5)
        {
            ForceShipRot(0);
            return 0;
        }
        if (playerModel.rotationCurrent > 0)
        {
            return playerModel.rotationCurrent -= playerModel.rotationSpeed;
        }
        else
        {
            return playerModel.rotationCurrent += playerModel.rotationSpeed;
        }
    }

    private void _LimitToScreen()
    {
        if (playerModel.positionTarget.x < -playerModel.limitHorz)
            playerModel.positionTarget.x = -playerModel.limitHorz;
        if (playerModel.positionTarget.x > playerModel.limitHorz)
            playerModel.positionTarget.x = playerModel.limitHorz;

        if (playerModel.positionTarget.z < -playerModel.limitVert)
            playerModel.positionTarget.z = -playerModel.limitVert;
        if (playerModel.positionTarget.z > playerModel.limitVert)
            playerModel.positionTarget.z = playerModel.limitVert;
    }
    private void _ApplySmoothingToMotion()
    {
        
        playerModel.positionCurrent = Vector3.Lerp(
            playerModel.positionCurrent, 
            playerModel.positionTarget, 
            Time.deltaTime * playerModel.smoothingFactor);

        
        
        playerModel.actualRotation = Vector3.Lerp(
            new Vector3(0, playerModel.rotationCurrent, 0), 
            new Vector3(0, playerModel.rotationTarget, 0),
            Time.deltaTime * playerModel.smoothingFactor);
        
        playerModel.ship.transform.position = playerModel.positionCurrent;
        playerModel.shield.transform.position = playerModel.ship.transform.position;
        playerModel.ship.transform.rotation = Quaternion.Euler(playerModel.actualRotation);

        Debug.Log(playerModel.rotationCurrent);
    }
}
