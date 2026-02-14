using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private float _jumpBufferTime = 0.2f;

        private float _jumpBufferStartTime;

        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            _jumpBufferStartTime -= _jumpBufferTime;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _jumpBufferStartTime = Time.time;
            }
        }

        public bool IsJumpActive()
        {
            return Time.time < _jumpBufferStartTime + _jumpBufferTime;
        }

        public void ConsumeJump()
        {
            _jumpBufferStartTime = -1f;
        }
    }
}
