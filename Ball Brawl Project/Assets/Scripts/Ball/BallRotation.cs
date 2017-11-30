using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRotation : MonoBehaviour
{

    public float offset;
    private float offsetX;
    private float offsetY;
    private float offsetZ;

    private void Start()
    {
        offsetX = Random.Range(0f, 10f);
        offsetZ = Random.Range(0f, 10f);
        offsetY = Random.Range(0f, 10f);
    }

    void Update()
    {
        transform.Rotate(new Vector3(offsetX, offsetY, offsetZ) * Time.deltaTime * 100f * offset, Space.World);
    }
}
