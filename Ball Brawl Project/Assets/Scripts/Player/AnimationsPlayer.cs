using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class AnimationsPlayer : NetworkBehaviour {

    Animator _animator;
    NetworkAnimator _netAnim;
    Rigidbody _rigidbody;

    private void Start() {
        _animator = GetComponent<Animator>();
        _netAnim = GetComponent<NetworkAnimator>();
        _rigidbody = GetComponentInParent<Rigidbody>();
        
    }

    public void UpdateAnimator(Vector3 move, bool _grounded) {

        if (move.magnitude > 1f) move.Normalize();


        float AnimatorX = 0f;
        float AnimatorZ = 0f;
        if (move.x > 0.15f) AnimatorX = 1;
        else if (move.x < -0.15f) AnimatorX = -1;

        if (move.z > 0.15f) AnimatorZ = 1;
        else if (move.z < -0.15f) AnimatorZ = -1;

        // update the animator parameters
        _animator.SetFloat("X Move", AnimatorX, 0.1f, Time.deltaTime);
        _animator.SetFloat("Z Move", AnimatorZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("Y Move", _rigidbody.velocity.y, 0.1f, Time.deltaTime);
        _animator.SetBool("Grounded", _grounded);

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (_grounded && move.magnitude > 0) {
            _animator.speed = 1;
        }
        else {
            // don't use that while airborne
            _animator.speed = 1;
        }
    }

    public void TriggerPushAnimation() {
        _animator.SetTrigger("isThrowing");
    }

    public void SetDashing(bool isDashing) {
        _animator.SetBool("isDashing", isDashing);
    }
}
