using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerControllerRigidbody : NetworkBehaviour {

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

    private Rigidbody _rigidbody;
    private AnimationsPlayer _anim;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _originalRotation = transform.localRotation;
        _currentFlightCharge = maxFlightCharge;
        _jetpackActivationTimer = 0;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
        _anim = GetComponentInChildren<AnimationsPlayer>();
    }

    void FixedUpdate() {
        if (!isLocalPlayer) return;
        //if (GameManagerScript.IsPaused) return;

        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Put it into WorldSpace
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        //If we are flying apply a movement penalty so we ensure that we dont manouver quite as fast midair
        if (!_grounded) targetVelocity /= flightMovementPenalty;

        GroundMovement(targetVelocity);

        AirMovement();

        Dash(targetVelocity);

        //Unlock the Cursor when the user press escape
        if (Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetKey(KeyCode.O)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        DoRotation();

        if (_anim != null) _anim.UpdateAnimator(targetVelocity, _grounded);

        _grounded = false;
    }

    void OnCollisionStay() {
        _grounded = true;
        _jetpackActivationTimer = 0;
    }

    //Ensure that we rotate the player itself so we use the rotation stuff here
    private void DoRotation() {
        _rotationX += Input.GetAxis("Mouse X") * rotationSensitivity;
        _rotationX = ClampAngle(_rotationX, -360, 360);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
        transform.localRotation = _originalRotation * xQuaternion;
    }

    public void SetRotationPlayer(float newRotation) {
        _rotationX = newRotation;
    }

    [ClientRpc]
    public void RpcReceivePush(Vector3 direction, float force) {
        _rigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    public static float ClampAngle(float angle, float min, float max) {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F)) {
            if (angle < -360F) {
                angle += 360F;
            }
            if (angle > 360F) {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void GroundMovement(Vector3 targetVelocity) {
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

    private void Dash (Vector3 velocity) {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer == 0) {
            if (velocity.magnitude == 0) {
                _rigidbody.AddForce(transform.forward * dashStrength, ForceMode.Impulse);
                //print("No velocity, dash forward");
            }
            else {
                _rigidbody.AddForce(velocity.normalized * dashStrength, ForceMode.Impulse);
                //print("Dash into velocity direction: " + velocity); 
            }

            _dashCooldownTimer = dashCooldown;
        }

        if(_dashCooldownTimer > 0) {
            _dashCooldownTimer -= Time.deltaTime;
            if (_dashCooldownTimer < 0) _dashCooldownTimer = 0;
        }
    }

    private void AirMovement() {
        float _currentGravity = gravity;

        //If we are ... well ... grounded
        if(_grounded) {
            if(Input.GetKey(KeyCode.Space)) {
                _rigidbody.AddRelativeForce(transform.up * jumpForce, ForceMode.Impulse);
                _grounded = false;
            }
        }

        //If we are airborn
        else {
            _jetpackActivationTimer += Time.deltaTime;

            //If wanna jump and can jump
            if (Input.GetKey(KeyCode.Space) && _currentFlightCharge > 0f && _jetpackActivationTimer > jetpackActivationDelay) {
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
        if(!Input.GetKey(KeyCode.Space) || _currentFlightCharge == 0) {
            if(_currentFlightCharge < maxFlightCharge) _currentFlightCharge += flightChargeRechargeRatePerSecond * Time.deltaTime;
            if (_currentFlightCharge > maxFlightCharge) _currentFlightCharge = maxFlightCharge;
        }

        // We apply gravity manually for more tuning control
        _rigidbody.AddRelativeForce(new Vector3(0, -_currentGravity * _rigidbody.mass, 0));
    }
} 