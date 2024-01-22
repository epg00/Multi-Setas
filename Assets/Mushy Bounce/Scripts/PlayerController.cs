using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

    // Create a variable to store the aspect ratio of your screen
    //float aspect = (float)Screen.width / Screen.height;
     
    // Store the world height of your screen like so
    //float worldHeight = myCamera.orthographicSize * 2;
     
    // Then you can define your world width
    //float worldWidth = worldHeight * aspect;

public class PlayerController : NetworkBehaviour
{
    
    [Header("Control Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxX;
    private float clickedScreenX;
    private float clickedPlayerX;


    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private float animatorSpeedMultiplier;
    [SerializeField] private float animatorSpeedLerp;

    [Header("Events")]
    public static Action onBump;

    
    void Start(){
        if(IsOwner){ //Para ver que el que se usa es el que se mueve
            IdleServerRpc();            
        }        
    }

    [ServerRpc]
    private void IdleServerRpc(){
        animator.Play("Idle");
    }


    void Update()
    {
        ManageControl();
    }

    private void ManageControl(){

        if(Input.GetMouseButtonDown(0)){
            clickedScreenX = Input.mousePosition.x;
            clickedPlayerX = transform.position.x;
        }

        else if(Input.GetMouseButton(0)){
            float xDifference = Input.mousePosition.x - clickedScreenX;
            xDifference /= Screen.width; //Para que funcione en todos los dispositivos
            xDifference *= moveSpeed;

            float newXPosition = clickedPlayerX + xDifference;

            newXPosition = Mathf.Clamp(newXPosition,-maxX,maxX);

            transform.position = new Vector2(newXPosition, transform.position.y);
        
            //Debug.Log("Xdifference: "+ xDifference);
            UpdatePlayerAnimation();
        }

        else if(Input.GetMouseButtonUp(0)){
            animator.speed = 1;
            IdleServerRpc();
        }
    }
    
    private float previousScreenX;

    private void UpdatePlayerAnimation(){

        float xDifference = (Input.mousePosition.x - previousScreenX)/Screen.width;
        xDifference *= animatorSpeedMultiplier;
        xDifference = Mathf.Abs(xDifference);

        float targetAnimatorSpeed = Mathf.Lerp(animator.speed,xDifference,Time.deltaTime*animatorSpeedLerp);

        animator.speed = targetAnimatorSpeed;
        RunServerRpc();

        previousScreenX = Input.mousePosition.x;
    }

    public void Bump(){
        BumpClientRpc();
    }

    [ClientRpc]
    private void BumpClientRpc(){
        bodyAnimator.Play("Bump");
        onBump?.Invoke();
    }  

    [ServerRpc]
    private void RunServerRpc(){
        animator.Play("Run");
    }

    
}
