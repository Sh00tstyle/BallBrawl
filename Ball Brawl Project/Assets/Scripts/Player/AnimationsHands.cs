using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHands : MonoBehaviour {

    [SerializeField]
    private ParticleSystem Discharge;

    [SerializeField]
    private ParticleSystem Holding;

    private Animator _animator;
    private bool _isHolding;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_isHolding && !Holding.isPlaying) Holding.Play();
        if (!_isHolding && Holding.isPlaying) {
            Holding.Stop();
            Holding.Clear();
        }
	}

    public void TriggerDashAnimation() {
        _animator.SetTrigger("isDashing");
    }

    public void TriggerPushAnimation() {
        _animator.SetTrigger("isThrowing");
        Discharge.Play();
    }

    public void SetHoldAnimation(bool isHolding) {
        _animator.SetBool("isHolding", isHolding);
        _isHolding = isHolding;
    } 
}
