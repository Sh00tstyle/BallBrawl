using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHands : MonoBehaviour {

    Animator _animator;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TriggerDashAnimation() {
        _animator.SetTrigger("isDashing");
    }

    public void TriggerPushAnimation() {
        _animator.SetTrigger("isThrowing");
    }

    public void SetHoldAnimation(bool isHolding) {
        _animator.SetBool("isHolding", isHolding);
    } 
}
