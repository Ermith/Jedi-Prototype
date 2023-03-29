using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public JediCharacter Target;
    public float Offset;
    public float RotationSpeed = 1;

    private float _currentRotation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);

        transform.position = Target.transform.position + Offset * transform.forward;
        transform.RotateAround(Target.transform.position, Vector3.up, _currentRotation);
    }

    private void FixedUpdate()
    {
        _currentRotation = Target.CameraRotation;
    }
}
