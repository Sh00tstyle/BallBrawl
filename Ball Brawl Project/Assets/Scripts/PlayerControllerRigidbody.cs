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
    private bool grounded = false;

    public float sensitivityX = 10f;
    private float rotationX;
    private Quaternion originalRotation;

    private Rigidbody rigidbody;
    private Camera cam;

    public override void OnStartLocalPlayer() {
        //Init the local player
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        GetComponentInChildren<CameraController>().enabled = true;
    }

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalRotation = transform.localRotation;

        rigidbody = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    void FixedUpdate() {
        if (!isLocalPlayer) return;

        DoRotation();
        if (grounded) {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //Put it into WorldSpace
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            //Clamp it at the maxVelocityChange
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            //AddForce, because we already put it into WorldSpaceCoordinates, otherwise we had to use AddRelativeForce
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (canJump && Input.GetButton("Jump")) {
                rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }

            //Unlock the Cursor when the user press escape
            if(Input.GetKey(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // We apply gravity manually for more tuning control
        rigidbody.AddRelativeForce(new Vector3(0, -gravity * rigidbody.mass, 0));

        grounded = false;
    }

    void OnCollisionStay() {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed() {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    //Ensure that we rotate the player itself so we use the rotation stuff here
    private void DoRotation() {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationX = ClampAngle(rotationX, -360, 360);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
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
}