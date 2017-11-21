using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject player;
    public Camera cam;
    public GameObject hand;
    public float maxTime = 3f;
    public Slider slider;

    public bool holdingBall;
    public float holdingTime;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Ball" && Input.GetMouseButtonDown(1))
        {
            other.transform.position = hand.transform.position;
            other.transform.parent = hand.transform;
            other.GetComponent<Rigidbody>().isKinematic = true;
            holdingBall = true;
        }
    }

    private void Update()
    {
        if (hand.transform.childCount > 0)
        {
            Holding();
            if (Input.GetMouseButtonDown(0))
                Shooting();
        }
    }

    private void Holding()
    {
        if (holdingBall && holdingTime < maxTime)
        {
            holdingTime = holdingTime + 0.01f;
            slider.value = slider.value + 0.01f;
        }
        if (holdingTime >= maxTime)
            Shooting();
    }

    private void Shooting()
    {
        {
            Rigidbody rb = hand.transform.GetChild(0).GetComponent<Rigidbody>();
            rb.isKinematic = false;
            float addedSpeed = (rb.velocity.magnitude + 1000f * (1 + holdingTime / 5));
            float inaccuracy = Random.Range(-holdingTime / 8, holdingTime / 8);
            Vector3 cameraOffset = new Vector3(cam.transform.forward.x + inaccuracy, cam.transform.forward.y + inaccuracy, cam.transform.forward.z);
            rb.AddForce(cameraOffset * addedSpeed);

            // Resetting the ball & timer
            hand.transform.GetChild(0).transform.parent = null;
            holdingBall = false;
            holdingTime = 0f;
            slider.value = 0f;

            // Debug
            Debug.Log("Fired the ball with a magnitude of " + addedSpeed);
        }
    }
}
