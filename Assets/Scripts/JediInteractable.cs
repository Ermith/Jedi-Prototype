using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JediInteractable : MonoBehaviour
{
    public bool SnapDirection = false;
    public CharacterController Controller = null;

    private Rigidbody _rigidBody;
    private CharacterController _characterController;
    private Vector3 _pushForce;

    public void JediPush(Vector3 fromPosition, float strength)
    {
        Vector3 pushDirection = (transform.position - fromPosition).normalized;
        if (SnapDirection) pushDirection = SnapDirectionXZ(pushDirection);

        _pushForce = pushDirection * strength;

        if (_characterController != null)
            DOTween.To(() => _pushForce, (Vector3 force) => _pushForce = force, Vector3.zero, 1f);
    }

    private Vector3 SnapDirectionXZ(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            return new Vector3(Mathf.Sign(direction.x), 0, 0);

        return new Vector3(0, 0, Mathf.Sign(direction.z));
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _characterController =
            (Controller == null)
            ? GetComponent<CharacterController>()
            : Controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rigidBody != null)
        {
            _rigidBody.AddForce(_pushForce);
            _pushForce = Vector3.zero;
        }

        if (_characterController != null)
        {
            _characterController.SimpleMove(_pushForce * Time.deltaTime * 2.1f);
            //_pushForce = Vector3.zero;
        }
    }
}
