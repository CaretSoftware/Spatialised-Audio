using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 1.15f;
    public float runSpeed = 4.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float gravity = 150.0f;
    public float speedThresholdForSound;
    [SerializeField] private AudioClip[] floorboardStepSounds;
    [SerializeField] private AudioClip[] cleanStepSounds;
    [SerializeField] private AudioClip[] creakSounds;
    
    //(2 sources for playing step and creak sounds simultaneously)
    public AudioSource as_Steps;                                    //AudioSource for playing steps only 
    public AudioSource as_Creaks;                                   //AudioSource for playing creak sounds only when stepping on floorboards

    private Transform _creakSource;
    private Transform _stepSource;
    
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    private bool canMove = true;
    private Transform _transform;


    private void Awake() {
        _transform = transform;
        
        _stepSource = as_Steps.transform;
        _creakSource = as_Creaks.transform;

        _stepSource.transform.parent = null;
        _creakSource.transform.parent = null;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateMe()
    {
        //if (Time.timeScale < Mathf.Epsilon) return;
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0) ;
        }

        if (characterController.velocity.sqrMagnitude > speedThresholdForSound)
        {
            PlayDynamicFootstep();
        }
    }

    [SerializeField] public string colliderType;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Code for identifying what material we're on
        if (hit.gameObject.GetComponent<Collider>().gameObject.GetComponent<SurfaceColliderType>())
        {
            colliderType = hit.gameObject.GetComponent<SurfaceColliderType>().GetTerrainType();
        }
    }

    private void PlayDynamicFootstep()
    {
        switch (colliderType)
        {
            case "Floorboards":
                PlayFloorboardStepSounds();
                PlayCreakSounds();
                break;
            case "Clean":
                PlayCleanStepSounds();
                break;
            default:
                PlayFloorboardStepSounds();
                PlayCreakSounds();
                break;
        }
    }

    private int randomInt = int.MaxValue;
    
    private void PlayFloorboardStepSounds()
    {
        if (!as_Steps.isPlaying) {
            _stepSource.position = _transform.position + characterController.velocity.normalized * .5f;
            
            randomInt = Random.Range(0, 4);
            as_Steps.pitch = 1f;
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, floorboardStepSounds.Length);
            as_Steps.clip = floorboardStepSounds[n];
            as_Steps.PlayOneShot(as_Steps.clip);
            // move picked sound to index 0 so it's not picked next time
            floorboardStepSounds[n] = floorboardStepSounds[0];
            floorboardStepSounds[0] = as_Steps.clip;
        }
    }

    private void PlayCreakSounds()
    {
        if (!as_Creaks.isPlaying && randomInt == 0)
        {
            _creakSource.position = _transform.position + characterController.velocity.normalized * .5f;
            
            // float f = Random.Range(0f, 1f);
            //Debug.Log(f);
            // if(f > 0.99f) {
            // pick & play a random creak sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, creakSounds.Length);
            as_Creaks.clip = creakSounds[n];
            as_Creaks.PlayOneShot(as_Creaks.clip);
            // move picked sound to index 0 so it's not picked next time
            creakSounds[n] = creakSounds[0];
            creakSounds[0] = as_Creaks.clip;
            // }
        }
    }

    private void PlayCleanStepSounds()
    {
        if (!as_Steps.isPlaying)
        {
            _stepSource.position = _transform.position + characterController.velocity.normalized * .5f;

            as_Steps.pitch = 0.7f;
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, cleanStepSounds.Length);
            as_Steps.clip = cleanStepSounds[n];
            as_Steps.PlayOneShot(as_Steps.clip);
            // move picked sound to index 0 so it's not picked next time
            cleanStepSounds[n] = cleanStepSounds[0];
            cleanStepSounds[0] = as_Steps.clip;
        }
    }
}