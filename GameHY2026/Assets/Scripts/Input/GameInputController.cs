using UnityEngine;
using UnityEngine.InputSystem;

namespace PinkTaxGame
{
    public class GameInputController : MonoBehaviour
    {
        [Header("Player Input")]
        [SerializeField] private PlayerInput playerInput;

        [Header("Gameplay Actions")]
        [SerializeField] private InputActionReference pointAction;
        [SerializeField] private InputActionReference primaryPressAction;
        [SerializeField] private InputActionReference submitAction;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference dragPressAction;

        public Vector2 PointerPosition => default;
        public bool PrimaryPressIsHeld => default;

        public void EnableGameplayInput() { }
        public void DisableGameplayInput() { }
        public void SetGameplayInputEnabled(bool enabled) { }
        public void Submit() { }
        public void Cancel() { }
    }
}
