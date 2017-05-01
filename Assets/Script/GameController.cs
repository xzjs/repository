using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour
{
    public GameObject FpsController, MainCamera;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("tab"))
	    {
	        FirstPersonController first = FpsController.GetComponent<FirstPersonController>();
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

    /// <summary>
    /// 切换视角
    /// </summary>
    public void ChangeCamera()
    {
        if (MainCamera.activeSelf)
        {
            MainCamera.SetActive(false);
            FpsController.SetActive(true);
        }
        else
        {
            MainCamera.SetActive(true);;
            FpsController.SetActive(false);
        }
    }
}
