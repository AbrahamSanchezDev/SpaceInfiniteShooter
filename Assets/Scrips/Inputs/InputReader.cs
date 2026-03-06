using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour {
    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference siphonAction;

    public Vector2 MoveValue { get; private set; }
    public bool IsSiphoning { get; private set; }

    private void OnEnable() {
        moveAction.action.Enable();
        siphonAction.action.Enable();
    }

    private void OnDisable() {
        moveAction.action.Disable();
        siphonAction.action.Disable();
    }

    private void Update() {
        MoveValue = moveAction.action.ReadValue<Vector2>();
        // Check if button is held
        IsSiphoning = siphonAction.action.IsPressed();
    }
}