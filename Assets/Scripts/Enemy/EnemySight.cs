﻿using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {

    public float fieldOfViewAngle = 110.0f;
    public float angle;
    public bool playerInSight;
    public bool useFOV;
    public bool useSphereCollider;
    public Vector3 personalLastSighting;
    public Vector3 direction;
    public GameObject goRoomThermostat;

    private bool inTrigger = false;
    private Vector3 previousSighting;
    private GameObject goCharacter;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;
    private Transform myTransform;


	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;

        if (goRoomThermostat)
        {
            scriptRoomHeat = goRoomThermostat.GetComponent<RoomHeatVariables>();
        }
        else
        {
            Debug.LogError("Room thermostat not assigned in Inspector.");
        }
        goCharacter = GameObject.Find("Character");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (useFOV)
        {
            FieldOfView();
        }
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == goCharacter)
        {
            inTrigger = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (useSphereCollider)
        {
            playerInSight = false;

            if (scriptCharEnergy.currentEnergy >= 0)
            {
                if (other.gameObject == goCharacter && IsOutOfTemperatureThreshold(scriptCharEnergy.currentEnergy))
                {
                    playerInSight = true;
                    personalLastSighting = goCharacter.transform.position;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.collider.gameObject == goCharacter)
        {
            inTrigger = false;
            playerInSight = false;
        }
    }

    void FieldOfView()
    {
        //if the other conditions are not met, playerInSight should be false
        playerInSight = false;
        if (scriptCharEnergy.currentEnergy >= 0)
        {
            direction = goCharacter.transform.position - myTransform.position;
            angle = Vector3.Angle(direction, myTransform.forward);

            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;

                if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == goCharacter)
                    {
                        playerInSight = true;
                        personalLastSighting = goCharacter.transform.position;   //Update this so that one script has the position for all to reference
                    }
                }
            }
        }
    }

    bool IsOutOfTemperatureThreshold(float temp)
    {
        if (temp < scriptRoomHeat.minStealthTemp || temp > scriptRoomHeat.maxStealthTemp)
            return true;
        else
            return false;
    }
}
