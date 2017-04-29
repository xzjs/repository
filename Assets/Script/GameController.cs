﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour
{
    public GameObject FPSController;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("tab"))
	    {
	        FirstPersonController first = FPSController.GetComponent<FirstPersonController>();
	        first.enabled = !first.enabled;
	        if (Cursor.lockState == CursorLockMode.Locked)
	        {
	            Cursor.lockState = CursorLockMode.None;
	            Cursor.visible = true;
	        }
	        else
	        {
	            Cursor.lockState=CursorLockMode.Locked;
	            Cursor.visible = false;
	        }
	    }
	}
}
