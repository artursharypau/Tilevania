using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private float _jumpBufferTime = 0.2f;
        [SerializeField] private float _attackBufferTime = 0.8f;

        private float _jumpBufferStartTime;
        private float _attackBufferStartTime;
        private ushort _attackBuffer;

        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            _jumpBufferStartTime -= _jumpBufferTime;
            _attackBufferStartTime -= _attackBufferTime;
            _attackBuffer = 0;
        }

        private void Update()
        {
            if (Time.time > _attackBufferStartTime + _attackBufferTime)
            {
                _attackBuffer = 0;
            }
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

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                ++_attackBuffer;
                _attackBufferStartTime = Time.time;
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

        public bool IsAttackBufferEmpty()
        {
            return _attackBuffer == 0;
        }

        public void ConsumeAttack()
        {
            if (!IsAttackBufferEmpty())
            {
                --_attackBuffer;
            }
        }
    }
}
