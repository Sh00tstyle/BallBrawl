using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerControllerRigidbody : NetworkBehaviour {

    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool _grounded = false;

    public float dashStrength = 250;

    public float maxFlightCharge = 5;
    public float flightChargeDepletionRatePerSecond = 1.5f;
    public float flightChargeRechargeRatePerSecond = 0.75f;
    public float flightUpwardsForceMultiplier = 10;
    public float flightMovementPenalty = 2;
    private float _currentFlightCharge;

    public float rotationSensitivity = 10f;
    private float _rotationX;
    private Quaternion originalRotation;

    private Rigidbody _rigidbody;
    private Camera _cam;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalRotation = transform.localRotation;
        _currentFlightCharge = maxFlightCharge;

        _rigidbody = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
    }

    void FixedUpdate() {
        if (!isLocalPlayer) return;
        if (GameManagerScript.IsPaused) return;

        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Put it into WorldSpace
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        //If we are flying apply a movement penalty so we ensure that we dont manouver quite as fast midair
        if (!_grounded) targetVelocity /= flightMovementPenalty;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = _rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        //Clamp it at the maxVelocityChange
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        //AddForce, because we already put it into WorldSpaceCoordinates, otherwise we had to use AddRelativeForce
        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);


        Phara();

        if (Input.GetKeyDown(KeyCode.LeftShift)) Dash(targetVelocity);

        //Unlock the Cursor when the user press escape
        if (Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // We apply gravity manually for more tuning control
        _rigidbody.AddRelativeForce(new Vector3(0, -gravity * _rigidbody.mass, 0));

        DoRotation();

        _grounded = false;
    }

    void OnCollisionStay() {
        _grounded = true;
    }

    float CalculateJumpVerticalSpeed() {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    //Ensure that we rotate the player itself so we use the rotation stuff here
    private void DoRotation() {
        _rotationX += Input.GetAxis("Mouse X") * rotationSensitivity;
        _rotationX = ClampAngle(_rotationX, -360, 360);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
        transform.localRotation = originalRotation * xQuaternion;
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

    private void Dash (Vector3 velocity) {
        if(velocity.magnitude == 0) {
            _rigidbody.AddRelativeForce(transform.forward * dashStrength, ForceMode.Impulse);
            print("No velocity, dash forward");
        }
        else {
            _rigidbody.AddForce(velocity.normalized * dashStrength, ForceMode.Impulse);
            print("Dash into velocity direction: " + velocity); 
        }
    }

    private void Phara() {

        //If wanna jump and can jump
        if (Input.GetKey(KeyCode.Space) && _currentFlightCharge > 0f) {
            //Substract the rate from our charge
            _currentFlightCharge -= flightChargeDepletionRatePerSecond * Time.deltaTime;
            if (_currentFlightCharge < 0) _currentFlightCharge = 0;
            //Apply force
            if (_currentFlightCharge > 0.5f) _rigidbody.AddRelativeForce(transform.up * flightUpwardsForceMultiplier * Time.deltaTime, ForceMode.Impulse);
        }

        //Otherwise recharge
        if(!Input.GetKey(KeyCode.Space) || _currentFlightCharge == 0) {
            if(_currentFlightCharge < maxFlightCharge) _currentFlightCharge += flightChargeRechargeRatePerSecond * Time.deltaTime;
            if (_currentFlightCharge > maxFlightCharge) _currentFlightCharge = maxFlightCharge;
        }
        print(_currentFlightCharge);
    }
}