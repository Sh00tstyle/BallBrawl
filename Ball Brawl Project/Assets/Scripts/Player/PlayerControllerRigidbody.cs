using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using FMODUnity;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerControllerRigidbody : NetworkBehaviour
{

    [Header("Sounds")]
    [SerializeField]
    [EventRef]
    private string _footsteps;
    [SerializeField]
    [EventRef]
    private string _dash;
    [SerializeField]
    [EventRef]
    private string _jetpackStart;
    [SerializeField]
    [EventRef]
    private string _jetpackLoop;

    [Header("General Movement")]
    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    private bool _grounded = false;

    [Header("Dash")]
    public float dashStrength = 125;
    public float dashCooldown = 2f;
    private float _dashCooldownTimer = 0;

    [Header("Jetpack")]
    public float jumpForce = 2.0f;
    public float jetpackActivationDelay = 1.5f;
    public float maxFlightCharge = 5;
    public float flightChargeDepletionRatePerSecond = 1.5f;
    public float flightChargeRechargeRatePerSecond = 0.75f;
    public float flightUpwardsForceMultiplier = 10;
    public float flightMovementPenalty = 2;
    public float gravityMultiplierOnPress = 3;
    private float _currentFlightCharge;
    private float _jetpackActivationTimer;

    [Header("Rotation")]
    public float rotationSensitivity = 10f;
    private float _rotationX;
    private Quaternion _originalRotation;

    private PlayerInteractionScript _playerInteraction;

    private Rigidbody _rigidbody;
    private AnimationsPlayer _anim;
    private AnimationsHands _animHands;

    private Vector3 targetVelocity;

    private bool _jetpackInput;
    private bool _dashInput;

    void Awake()
    {
        _originalRotation = transform.localRotation;
        _currentFlightCharge = maxFlightCharge;
        _jetpackActivationTimer = 0;

        _playerInteraction = GetComponent<PlayerInteractionScript>();

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;

        targetVelocity = Vector3.zero;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _animHands = GetComponentInChildren<AnimationsHands>();
        _anim = GetComponentInChildren<AnimationsPlayer>();
    }

    private void Update()
    {
        //Get intput
        targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift)) _dashInput = true;
        else _dashInput = false;

        if (Input.GetKey(KeyCode.Space)) _jetpackInput = true;
        else _jetpackInput = false;

        //Unlock the Cursor when the user press escape
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKey(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (PauseManagerScript.Instance.IsPaused || PauseManagerScript.Instance.BlockInput) return;

        //Put input it into WorldSpace
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        //If we are flying apply a movement penalty so we ensure that we dont manouver quite as fast midair
        if (!_grounded) targetVelocity /= flightMovementPenalty;

        //Testing, if it solves a collision issue
        //targetVelocity = PreventBoundaryStuck(targetVelocity);

        GroundMovement(targetVelocity);

        AirMovement(_jetpackInput);

        Dash(targetVelocity, _dashInput);

        if (_grounded)
        {
            HudOverlayManager.Instance.SetJetpackActive();
        }
        else
        {
            HudOverlayManager.Instance.SetDescendActive();
        }

        HudOverlayManager.Instance.UpdateFuel(_currentFlightCharge / maxFlightCharge);

        DoRotation();

        if (_anim != null) _anim.UpdateAnimator(transform.InverseTransformDirection(targetVelocity), _grounded);

        _grounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == Tags.GROUND)
        {
            AudioManager.stopInstance(_jetpackLoop, gameObject);
            _grounded = true;
            _jetpackActivationTimer = 0;
        }
    }

    //Ensure that we rotate the player itself so we use the rotation stuff here
    private void DoRotation()
    {
        _rotationX += Input.GetAxis("Mouse X") * rotationSensitivity;
        _rotationX = ClampAngle(_rotationX, -360, 360);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
        transform.localRotation = _originalRotation * xQuaternion;
    }

    public void SetRotationPlayer(float newRotation)
    {
        _rotationX = newRotation;
        DoRotation(); //applying the rotation
    }

    [ClientRpc]
    public void RpcReceivePush(Vector3 direction, float force)
    {
        _rigidbody.velocity = Vector3.zero; //resetting velocity, so you wont be pushed so hard
        _rigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);

        if (_playerInteraction.IsHolding)
        {
            _playerInteraction.CmdReleaseBall();
            _playerInteraction.CmdSetIsHolding(false);
        }
    }

    [ClientRpc]
    public void RpcResetVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    [ClientRpc]
    public void RpcResetCooldowns()
    {
        _dashCooldownTimer = 0f;
        _currentFlightCharge = maxFlightCharge;

        HudOverlayManager.Instance.SetDashOffCooldown();
        HudOverlayManager.Instance.SetJetpackActive();
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void GroundMovement(Vector3 targetVelocity)
    {
        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = _rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        //Clamp it at the maxVelocityChange
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        //AddForce, because we already put it into WorldSpaceCoordinates, otherwise we had to use AddRelativeForce
        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Dash(Vector3 velocity, bool UserInput)
    {
        if (UserInput && _dashCooldownTimer == 0)
        {
            if (velocity.magnitude == 0)
            {
                _rigidbody.AddForce(transform.forward * dashStrength, ForceMode.Impulse);
                //print("No velocity, dash forward");
            }
            else
            {
                _rigidbody.AddForce(velocity.normalized * dashStrength, ForceMode.Impulse);
                //print("Dash into velocity direction: " + velocity); 
            }
            _animHands.TriggerDashAnimation();
            AudioManager.PlayEvent(_dash, gameObject, true);
            _dashCooldownTimer = dashCooldown;
        }

        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;

            if (_dashCooldownTimer < 0)
            {
                _dashCooldownTimer = 0;
                HudOverlayManager.Instance.SetDashOffCooldown();
            }
            else
            {
                HudOverlayManager.Instance.SetDashOnCooldown(1 - _dashCooldownTimer / dashCooldown, _dashCooldownTimer);
            }
        }
    }

    private void AirMovement(bool UserInput)
    {
        float _currentGravity = gravity;

        //If we are ... well ... grounded
        if (_grounded)
        {
            if (UserInput)
            {
                AudioManager.PlayEvent(_jetpackStart, gameObject, true);
                AudioManager.PlayEvent(_jetpackLoop, gameObject, true, false);
                _rigidbody.AddRelativeForce(transform.up * jumpForce, ForceMode.Impulse);
                _grounded = false;
            }
        }

        //If we are airborn
        else
        {
            _jetpackActivationTimer += Time.deltaTime;
            AudioManager.setParameter(_jetpackLoop, gameObject, "Velocity", _rigidbody.velocity.y / 10f);
            //If wanna jump and can jump
            if (UserInput && _currentFlightCharge > 0f && _jetpackActivationTimer > jetpackActivationDelay)
            {
                //Substract the rate from our charge
                _currentFlightCharge -= flightChargeDepletionRatePerSecond * Time.deltaTime;
                if (_currentFlightCharge < 0) _currentFlightCharge = 0;
                //Apply force
                if (_currentFlightCharge > 0.5f) _rigidbody.AddRelativeForce(transform.up * flightUpwardsForceMultiplier * Time.deltaTime, ForceMode.Impulse);
            }

            if (Input.GetKey(KeyCode.LeftControl)) _currentGravity *= gravityMultiplierOnPress;
            _grounded = false;
        }

        //Otherwise recharge
        if (!UserInput || _currentFlightCharge == 0)
        {
            if (_currentFlightCharge < maxFlightCharge) _currentFlightCharge += flightChargeRechargeRatePerSecond * Time.deltaTime;
            if (_currentFlightCharge > maxFlightCharge) _currentFlightCharge = maxFlightCharge;
        }

        // We apply gravity manually for more tuning control
        _rigidbody.AddRelativeForce(new Vector3(0, -_currentGravity * _rigidbody.mass, 0));
    }

    private Vector3 PreventBoundaryStuck(Vector3 targetVelocity)
    {
        Vector3 move = targetVelocity;

        // Don't use the vertical velocity
        move.y = 0;
        // Calculate the approximate distance that will be traversed
        float distance = move.magnitude * Time.fixedDeltaTime;
        // Normalize horizontalMove since it should be used to indicate direction
        move.Normalize();
        RaycastHit hit;

        // Check if the body's current velocity will result in a collision
        if (Physics.Raycast(new Ray(transform.position, move), out hit, move.magnitude))
        {
            if (hit.collider.tag == Tags.WALL)
            {
                // If so, stop the movement
                //_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                //gravity = 20;
                return new Vector3(targetVelocity.normalized.x * 0.5f, 0, targetVelocity.normalized.z * 0.5f);
            }
            else
            {
                //gravity = 10;
                return targetVelocity;
            }
        }
        return targetVelocity;
    }

    private void PlayerSounds(string name)
    {
        if (name == "footstep")
            AudioManager.PlayEvent(_footsteps, gameObject);
    }
}