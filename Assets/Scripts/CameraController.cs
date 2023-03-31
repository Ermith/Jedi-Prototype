using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Tooltip("Jedi to be followed by the camera.")]
    public JediCharacter Target;

    [SerializeField, Tooltip("How long is the selfie stick. Negative numbers to be behind the character.")]
    public float Offset;

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
