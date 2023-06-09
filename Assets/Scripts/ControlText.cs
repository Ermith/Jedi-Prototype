using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlText : MonoBehaviour
{
    [SerializeField, Tooltip("Similar to ControlButton.")]
    public string Name = "Move";

    // Start is called before the first frame update
    void Start()
    {
        var action = FindObjectOfType<JediCharacter>().GetComponent<PlayerInput>().currentActionMap.FindAction(Name);
        var binding = action.bindings[0];
        string control = InputControlPath.ToHumanReadableString(binding.path,
            InputControlPath.HumanReadableStringOptions.OmitDevice); 
        GetComponent<TMP_Text>().text += $"\n[{control}]";
        Debug.Log(control);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
