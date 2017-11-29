using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsPlayer : MonoBehaviour {

    Animator _animator;
    Rigidbody _rigidbody;

    private void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    public void UpdateAnimator(Vector3 move, bool _grounded) {

        if (move.magnitude > 1f) move.Normalize();


        float AnimatorX = 0f;
        float AnimatorZ = 0f;
        if (move.x > 0) AnimatorX = 1;
        else if (move.x < 0) AnimatorX = -1;

        if (move.z > 0) AnimatorZ = 1;
        else if (move.z < 0) AnimatorZ = -1;

        // update the animator parameters
        _animator.SetFloat("X Move", AnimatorX, 0.1f, Time.deltaTime);
        _animator.SetFloat("Z Move", AnimatorZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("Y Move", _rigidbody.velocity.y, 0.1f, Time.deltaTime);
        _animator.SetBool("Grounded", _grounded);
        _animator.SetBool("Crouch", false);
        if (!_grounded) {
            _animator.SetFloat("Jump", _rigidbody.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * move.z;
        if (_grounded) {
            _animator.SetFloat("JumpLeg", jumpLeg);
        }

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
}
