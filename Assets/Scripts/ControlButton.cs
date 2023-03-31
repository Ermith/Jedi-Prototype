using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UIButton = UnityEngine.UI.Button;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class ControlButton : MonoBehaviour
{
    [SerializeField, Tooltip("Action the button is bind ti. e.g. Attack")]
    public InputActionReference actionReference;

    [SerializeField, Tooltip("Binding number in the action. Depends on the action. Like movement - forward = 0, left = 1 ...")]
    public int BindingIndex = 0;

    private TMP_Text _label;
    private UIButton _button;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<UIButton>();
        _label = GetComponentInChildren<TMP_Text>();

        //int bindingIndex = action.action.GetBindingIndexForControl(action.action.controls[0]);
        _label.text = InputControlPath.ToHumanReadableString(actionReference.action.bindings[BindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void Rebind()
    {
        _label.text = "...";
        _button.interactable = false;
        actionReference.action.PerformInteractiveRebinding(BindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(RebindComplete)
            .Start();
    }

    private void RebindComplete(RebindingOperation operation)
    {
        int bindingIndex = actionReference.action.GetBindingIndexForControl(actionReference.action.controls[0]);

        _label.text = InputControlPath.ToHumanReadableString(actionReference.action.bindings[BindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        _button.interactable = true;
        operation.Dispose();
    }
}
