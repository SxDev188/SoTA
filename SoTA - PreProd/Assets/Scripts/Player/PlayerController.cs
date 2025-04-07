using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class PlayerController : MonoBehaviour
{
    public int currentHealth;

    [SerializeField] public int maxHealth = 10;
    bool justRespawned;

    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float boulderPushSpeed = 3.0f;
    [SerializeField] private float movementRotationByDegrees = 45;

    private Rigidbody playerRigidbody;
    
    private bool isMoving = false;
    private bool isMovementLocked = false;
    private bool isAttachedToBoulder = false;

    private bool isInDeathZone = false;

    public bool IsInDeathZone
    {
        get { return isInDeathZone; }
    }

    private Vector3 movementLockAxis;
    private Vector3 movementDirection;
    private Vector2 lastMoveDirection;
    private Vector3 rotationAxis = Vector3.up;
    private Vector3 movementInput = Vector3.zero;

    private EventInstance playerSlither; //Audio

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerSlither = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SlitherSound);
        currentHealth = maxHealth;
    }

    void Update()
    {
        //UpdateSound(); no more sound :(
    }

    private void FixedUpdate()
    {
        if (justRespawned)
        {
            justRespawned = false;
            return;
        }

        if (isMovementLocked && movementLockAxis != Vector3.zero)
        {
            movementInput = Vector3.Scale(movementInput, movementLockAxis);
        }

        if (isAttachedToBoulder) //don't move if attached to a boulder
        {
            return;
        }

        if (isMovementLocked && isMoving) //aka is pushing/pulling boulder
        {
            //this movement does not depend on where player is facing, only movementInput
            playerRigidbody.MovePosition(transform.position + movementInput * boulderPushSpeed * Time.deltaTime);
        }
        else if (isMoving)
        {
            //this ONLY MOVES FORWARD, direction is determined by where character is looking
            playerRigidbody.MovePosition(transform.position + transform.forward * movementInput.magnitude * moveSpeed * Time.deltaTime);
        }
    }

    void OnMoveInput(InputValue input)
    {
        isMoving = true;

        Vector2 input2d = input.Get<Vector2>();
        movementInput = new Vector3(input2d.x, 0, input2d.y);

        if (!isMovementLocked)
        {
            movementInput = RotateVector3(movementInput, movementRotationByDegrees, rotationAxis);
            LookAtMovementDirection();
        }

        if (input2d != Vector2.zero) // Used for CameraPan
        {
            lastMoveDirection = input2d.normalized;
        }
    }

    void OnMoveRelease(InputValue input)
    {
        InteruptMovement();
    }

    public void InteruptMovement()
    {
        isMoving = false;
        movementInput = Vector3.zero;
    }

    void LookAtMovementDirection()
    {
        if (movementInput != Vector3.zero)
        {
            movementDirection = (transform.position + movementInput) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(movementDirection, rotationAxis);

            transform.rotation = rotation;
        }

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation);         //this line might be needed for controller
    }

    public void LockMovement(Vector3 axis) //For boulder movement
    {
        isMovementLocked = true;
        movementLockAxis = axis;
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
        movementLockAxis = Vector3.zero;
        InteruptMovement();
    }

    public bool GetIsMovementLocked()
    {
        return isMovementLocked;
    }

    public Vector3 RayBoulderInteraction(float interactionRange, GameObject interactedBoulder)
    {
        RaycastHit hit;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        for (int i = 0; i < directions.Length; i++)
        {
            //here I removed the transform.TransformDirection() so that the raycast ignores the player characters orientation/rotation - goobie
            if (Physics.Raycast(transform.position, directions[i], out hit, interactionRange))
            {
                Debug.DrawRay(transform.position, directions[i] * hit.distance, Color.red);

                if (hit.transform.gameObject == interactedBoulder && hit.transform.tag == "Boulder") //tag check here is probably not necessary but just a precaution
                {
                    return directions[i];
                }
            }
        }

        //means the boulder will not be attached to the player
        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes") || other.CompareTag("Abyss"))
        {
            isInDeathZone = true;
            currentHealth = 0;
            justRespawned = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spikes") || other.CompareTag("Abyss"))
        {
            isInDeathZone = false;
        }
    }

    private void UpdateSound()
    {
        if (isMoving)
        {
            PLAYBACK_STATE playbackState;
            playerSlither.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerSlither.start();
            }
        }
    }

    private Vector3 RotateVector3(Vector3 vectorToRotate, float degrees, Vector3 rotationAxis)
    {
        Vector3 rotatedVector = Quaternion.AngleAxis(degrees, rotationAxis) * vectorToRotate;
        return rotatedVector;
    }

    public Vector2 GetLastMoveDirection() //Used for CameraPan
    {
        return lastMoveDirection;
    }
    
    public Vector3 GetBoulderPushDirection() //Used for boulder push/pull
    {
        if (isAttachedToBoulder && isMovementLocked && movementLockAxis != Vector3.zero)
        {
            Vector3 boulderPushDirection = Vector3.Scale(movementInput, movementLockAxis);
            boulderPushDirection = boulderPushDirection.normalized;
            return boulderPushDirection;
        }

        return Vector3.zero;
    }
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }


    public void AttachToBoulder()
    {
        isAttachedToBoulder = true;
    }

    public void DetachFromBoulder()
    {
        isAttachedToBoulder = false;
    }

}
