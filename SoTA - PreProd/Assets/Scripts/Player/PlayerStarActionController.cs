using FMOD.Studio;
using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStarActionController : MonoBehaviour
{

    // COMPONENTS
    private Transform starTransform;
    private StarActions starActions;
    private PlayerInput playerInput;
    private PlayerController playerController;

    // TWEAKABLE VARIABLES
    [SerializeField] private bool recallAllowed = false;
    [SerializeField] private bool gravityPullAllowed = false;
    [SerializeField] private bool strongThrowAllowed = false;

    [SerializeField] private float normalThrowRange = 4;
    [SerializeField] private float strongThrowRange = 10;

    [SerializeField] private float starPickupRange = 1.0f;
    [SerializeField] private float recallRange = 4.0f;

    [SerializeField] private float gravityPullRange = 3.0f;
    [SerializeField] private float gravityPullAcceptanceRadius = 0.01f;
    [SerializeField] private float gravityPullSpeed = 5.0f;

    [SerializeField] private float aimSensitivity = 0.5f;
    [SerializeField] private float aimRotationByDegrees = 45;


    // STORING/VALUE VARIABLES
    private bool isAiming = false;
    private bool strongThrow = false;
    private bool Controller = false;

    private float healthChangeTimer = 0.0f;

    private Vector3 mouseDownPosition;
    private Vector3 mouseReleasePosition;
    private Vector3 rotationAxis = Vector3.up;

    private Vector3 aimInput;
    private Vector3 throwDirection;
    private Vector3 throwTargetDestination;

    private LineRenderer lineRenderer;

    private EventInstance lowHealthWarningSFX;

    private IEnumerator GravityPull_IEnumerator;

    // ENGINE METHODS ====================================== // 

    void Start()
    {
        GameObject star = GameObject.FindGameObjectWithTag("Star");
        starActions = star.GetComponent<StarActions>();
        starTransform = star.GetComponent<Transform>();

        playerController = this.GetComponent<PlayerController>();
        playerInput = this.GetComponent<PlayerInput>();

        InitializeLineRenderer();
        
        if (playerInput.currentActionMap.name == "PlayerControlController")
        {
            Controller = true;
        }

        lowHealthWarningSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.LowHealthWarningSFX);
    }

    void Update()
    {
        if (isAiming)
        {
            if (Controller)
            {
                throwDirection = aimInput;
            }
            else
            {
                mouseReleasePosition = Input.mousePosition;
                throwDirection = mouseDownPosition - mouseReleasePosition; // Drag direction
                throwDirection.z = throwDirection.y; // Map vertical screen movement to Z-axis movement
                throwDirection.y = 0; // Keep movement on XZ plane

                throwDirection *= aimSensitivity / 100; //controlling the length of the throw was way too sensitive without this
            }

            if (strongThrow && throwDirection.sqrMagnitude > MathF.Pow(strongThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * strongThrowRange;
            }
            else if (!strongThrow && throwDirection.sqrMagnitude > MathF.Pow(normalThrowRange, 2))
            {
                throwDirection = throwDirection.normalized * normalThrowRange;
            }

            throwDirection = HelperScript.RotateVector3(throwDirection, aimRotationByDegrees, rotationAxis);

            DrawAimLine();
        }
        else
        {
            HideAimLine();
        }

        healthChangeTimer += Time.deltaTime;
        ManagePlayerHealth();

        PlayLowHealthWarningSound();
    }

    // METHODS ====================================== //

    void DrawAimLine()
    {
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + throwDirection);
    }

    void HideAimLine()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }
    }

    IEnumerator GravityPullToDestination(Vector3 targetDestination)
    {
        playerController.inputLocked = true; //Locks input and movement during gravity pull
        playerController.disableGravityDuringPull = true;         //Disables gravity

        float threshold = 0.1f; //Distance threshold to stop moving
        Vector3 lastPosition = transform.position; //Position of player when starting gravity pull

        while (true)
        {
            // Calculate direction and movement (no gravity applied, only horizontal movement)
            Vector3 direction = (targetDestination - transform.position).normalized;
            Vector3 move = direction * gravityPullSpeed;

            playerController.CharacterController.Move(move * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(transform.position, targetDestination);

            if (distanceToTarget <= threshold || Vector3.Distance(transform.position, lastPosition) < 0.01f)
            {
                transform.position = targetDestination; //Set position directly to the Star to avoid any small overshoot
                break;
            }

            lastPosition = transform.position; //Update the last position of player

            yield return null;
        }

        playerController.inputLocked = false;  //Re-enable input after the pull
        starActions.Recall();

        playerController.disableGravityDuringPull = false; //Enable gravity again
    }

    private void InteruptGravityPullToDestination()
    {
        if (GravityPull_IEnumerator != null)
        {
            StopCoroutine(GravityPull_IEnumerator);
            starActions.Recall();
        }

    }

    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void ManagePlayerHealth()
    {
        float changeHealthAtTime = 1.0f;

        if (playerController.currentHealth > 0 && starActions.IsOnPlayer == false && healthChangeTimer >= changeHealthAtTime)
        {
            playerController.currentHealth--;
            healthChangeTimer = 0.0f;
        }
        if (playerController.currentHealth < playerController.maxHealth && starActions.IsOnPlayer && healthChangeTimer >= changeHealthAtTime)
        {
            playerController.currentHealth++;
            healthChangeTimer = 0.0f;
        }
        if (healthChangeTimer >= changeHealthAtTime)
        {
            healthChangeTimer = 0.0f;
        }
    }

    void PlayLowHealthWarningSound()
    {
        if (playerController.currentHealth < 5 && playerController.currentHealth > 0 && !starActions.IsOnPlayer)
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                lowHealthWarningSFX.start();
            }
        } else
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                lowHealthWarningSFX.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    void ThrowStar()
    {
        if (starActions.IsOnPlayer && isAiming)
        {
            isAiming = false;
            throwTargetDestination = transform.position + throwDirection;
            starActions.Throw(throwTargetDestination, throwDirection.normalized);

        }
    }

    // INPUT RELATED MEHTODS ====================================== //

    void OnCarryStarToggle(InputValue input)
    {
        if (Vector3.Distance(transform.position, starTransform.position) <= starPickupRange)
        {
            starActions.CarryToggle();
        }
    }

    void OnRecallStar(InputValue input)
    {
        if (!recallAllowed)
        {
            return;
        }

        if (Vector3.Distance(transform.position, starTransform.position) <= recallRange)
        {
            starActions.Recall();
        }
    }

    void OnLeftMouseDown(InputValue input)
    {

        if (starActions.IsOnPlayer)
        {
            isAiming = true;

            mouseDownPosition = Input.mousePosition;
        }
    }

    void OnRightMouseDown(InputValue input)
    {
        if (!strongThrowAllowed)
        {
            return;
        }

        if (starActions.IsOnPlayer)
        {
            isAiming = true;
            strongThrow = true;

            mouseDownPosition = Input.mousePosition;
        }
    }

    void OnRightMouseRelease(InputValue input)
    {
        if (!strongThrowAllowed)
        {
            return;
        }

        isAiming = false;
        strongThrow = false;

        if (starActions.IsOnPlayer)
        {
            throwTargetDestination = transform.position + throwDirection;

            starActions.Throw(throwTargetDestination, throwDirection.normalized);
        }
    }

    void OnGravityPull(InputValue input)
    {

        if (!gravityPullAllowed ||starActions.IsOnPlayer)
        {
            return;
        }

        if (Vector3.Distance(transform.position, starTransform.position) <= gravityPullRange)
        {
            GravityPull_IEnumerator = GravityPullToDestination(starTransform.position);
            StartCoroutine(GravityPull_IEnumerator);
        }
    }
    
    void OnAimInput(InputValue input) //For Controller
    {
        if (starActions.IsOnPlayer)
        {
            isAiming = true;
            Vector2 input2d = input.Get<Vector2>();
            aimInput = new Vector3(input2d.x, 0, input2d.y);
            if (strongThrow)
            {
                aimInput *= strongThrowRange;
            }
            else
            {
                aimInput *= normalThrowRange;
            }
        }
    }

    void OnAimRelease(InputValue input)
    {
        isAiming = false;
        aimInput = Vector3.zero;
    }

    void OnThrowRelease()
    {
        ThrowStar();
    }

    void OnStrongThrow()
    {
        if (!strongThrowAllowed)
        {
            return;
        }
        strongThrow = true;
    }

    void OnStrongThrowRelease()
    {
        strongThrow = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Spikes"))
        {
            InteruptGravityPullToDestination();
        }
    }
}
