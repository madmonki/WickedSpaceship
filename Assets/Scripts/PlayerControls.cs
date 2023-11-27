using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{  
    [Header("General Setup Settings")]
    [SerializeField] InputAction movement;
    [Tooltip("How fast ship moves up and down based upon player input")] 
    [SerializeField] float controlSpeed = 10f;
    [Tooltip("How far player moves horizontally")] [SerializeField] float xRange = 10f;
    [Tooltip("How far player moves vertically")] [SerializeField] float yRange = 10f;

    [Header("Laser gun array")]
    [Tooltip("Add all player lasers here")]
    [SerializeField] GameObject[] lasers;


    [Header("Screen position based tuning")]    
    [SerializeField] float positionPitchFactor = -2f;
    [SerializeField] float positionYawFactor = 2.5f;

    [Header("Player input based tuning")]    
    [SerializeField] float controlPitchFactor = -15f;
    [SerializeField] float controlRollFactor = -20f;
    [SerializeField] float rotationFactor;

    [SerializeField] InputAction fire;

    float xThrow, yThrow;

    void OnEnable()
    {
        movement.Enable();
        fire.Enable();
    }

    void OnDisable()
    {
        movement.Disable();
        fire.Disable();
    }

    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
        ProcessFiring();
    }

    void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueToControlThrow = yThrow * controlPitchFactor;

        float pitch = pitchDueToPosition + pitchDueToControlThrow;
        float yaw = transform.localPosition.x * positionYawFactor;
        float roll = xThrow * controlRollFactor;

        // On next line, we get the target rotation but we don't assign it to transform.localRotation
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
        // here, we use Quaternion.RotateTowards from the current rotation
        // to the target rotation. NOTE that the rotationFactor has to be small, such as 1, otherwise the rotation will be too fast and will be janky.
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationFactor);
    }

    void ProcessTranslation()
    {
        xThrow = movement.ReadValue<Vector2>().x;
        yThrow = movement.ReadValue<Vector2>().y;

        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float yOffset = yThrow * Time.deltaTime * controlSpeed;
        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);

        transform.localPosition = new Vector3
        (clampedXPos, clampedYPos, transform.localPosition.z);

        // float horizontalThrow = Input.GetAxis("Horizontal");
        //Debug.Log(horizontalThrow);

        // float verticalThrow = Input.GetAxis("Vertical");
        //Debug.Log(verticalThrow);
    }

    void ProcessFiring()
    {
        if(fire.ReadValue<float>() > 0.5)
        {
            SetLasersActive(true);    
        }
        else
        {
            SetLasersActive(false);
        }
    }

    void SetLasersActive(bool isActive)
    {
        foreach (GameObject laser in lasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }

}

