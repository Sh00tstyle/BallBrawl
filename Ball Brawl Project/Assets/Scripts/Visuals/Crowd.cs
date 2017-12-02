using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public float cheering = 1f;

    [SerializeField]
    private List<CrowdIndividual> CrowdMass;
    [SerializeField]
    private float baseSpeed = 3f;
    [SerializeField]
    private float MinSize = 0.5f;
    [SerializeField]
    private float maxSize = 1f;

    [System.Serializable]
    public class CrowdIndividual
    {
        public Vector3 startPosition;
        public GameObject gameobject;
        public float maxSpeed;
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //Changing speeds with some offset to make it look more random
            CrowdIndividual newCrowdIndividual = new CrowdIndividual();
            newCrowdIndividual.startPosition = transform.GetChild(i).transform.position;
            newCrowdIndividual.gameobject = transform.GetChild(i).gameObject;
            newCrowdIndividual.maxSpeed = baseSpeed + Random.Range(0f, 3f);
            CrowdMass.Add(newCrowdIndividual);

            //Changing sprite size
            transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("_Scale", Random.Range(MinSize, maxSize));
        }
    }

    void Update()
    {
        for (int i = 0; i < CrowdMass.Count; i++)
        {
            transform.GetChild(i).transform.position = new Vector3(CrowdMass[i].startPosition.x, CrowdMass[i].startPosition.y + (Mathf.Sin(Time.time * CrowdMass[i].maxSpeed * cheering) / 2), CrowdMass[i].startPosition.z);
        }
    }
}
