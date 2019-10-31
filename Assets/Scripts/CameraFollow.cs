using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float damping = 1;
    Vector3 offset;
    public Slider strength;
    private float startingStrength;

    void Start()
    {
        offset = target.transform.position - transform.position;
        startingStrength = strength.value;
    }

    void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, damping);

        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.transform.position - (rotation * offset);

        transform.LookAt(target.transform);
        GetComponent<Camera>().fieldOfView = 60 + (strength.value - startingStrength) * 100;

        
        
        
        //transform.rotation = Quaternion.Euler(45, 90, transform.rotation.z);

    }
}