﻿using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DragComponent : MonoBehaviour
{
    private GameObject currentlyDraggedGameObject;
    private GameObject hittedGameObject;

    public FixedJoystick FixedJoystick;
    
    // Update is called once per frame

    private void OnDisable() {
        if (hittedGameObject != null) {
            hittedGameObject.GetComponent<PhotonView>().RPC("Deselect", RpcTarget.AllBuffered);
        }
    }

    private void Start() {
        FixedJoystick = GameObject.FindObjectOfType<FixedJoystick>().GetComponent<FixedJoystick>();
    }

    void Update()
    {
        RaycastHit hit;
        bool isValidTouch = Input.touchCount > 0 && !Input.GetTouch(0).position.IsPointOverUiObject();


        if (currentlyDraggedGameObject is null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit) &&
                hit.transform.CompareTag("Draggable"))
            {
                if (hittedGameObject != hit.transform.gameObject && hittedGameObject != null)
                {
                    hittedGameObject.GetComponent<PhotonView>().RPC("Deselect", RpcTarget.AllBuffered);
                    // hittedGameObject.GetComponent<DraggableObject>().Deselect();
                }
                
                hittedGameObject = hit.transform.gameObject;
                hittedGameObject.GetComponent<PhotonView>().RPC("Select", RpcTarget.AllBuffered);
                // hittedGameObject.GetComponent<DraggableObject>().Select();

            }
            else
            {
                if (hittedGameObject != null)
                {
                    hittedGameObject.GetComponent<PhotonView>().RPC("Deselect", RpcTarget.AllBuffered);
                    // hittedGameObject.GetComponent<DraggableObject>().Deselect();
                    hittedGameObject = null;
                }
            }
            
            if (hittedGameObject)
            {
                if (isValidTouch)
                {
                    hittedGameObject.GetComponent<DraggableObject>().RequestOwnerChange();
                    currentlyDraggedGameObject = hittedGameObject;
                }
            }
        }

        if (currentlyDraggedGameObject != null)
        {
            if (isValidTouch)
            {
                currentlyDraggedGameObject.transform.position = transform.position + transform.forward* 1f;
                currentlyDraggedGameObject.transform.eulerAngles = new Vector3( 0, Mathf.Atan2( FixedJoystick.Vertical, FixedJoystick.Horizontal) * 180 / Mathf.PI, 0 );
            }
            else
            {
                currentlyDraggedGameObject = null;
            }
        }
    }
}
