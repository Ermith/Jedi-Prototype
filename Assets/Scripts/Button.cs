using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField, Tooltip("On button press, activates smart objects. What is executed depends on the smart objects. (They are smart)")]
    public List<SmartObject> Targets;
    // Start is called before the first frame update
    void Start()
    {
        //Targets = new List<SmartObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.Raycast(transform.position, transform.up, out RaycastHit hitInfo, 1 << 9)
            || hitInfo.distance > 1f)
        {
            foreach (SmartObject target in Targets)
                target.Deactivate();
        
            return;
        }

        foreach (SmartObject target in Targets)
            target.Activate();
    }
}
