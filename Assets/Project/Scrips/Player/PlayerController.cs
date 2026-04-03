using UnityEngine;
using UnityEngine.InputSystem;

namespace Survivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float smoothTime = 0.1f; // 멈출 때의 부드러움 정도

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 moveInput;
        private Vector2 smoothMoveInput;
        private Vector2 moveInputVelocity;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // 탑다운 뷰를 위한 리지드바디 필수 설정 (논리적 설계)
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            // 유니티 6 권장 설정: 충돌 정확도 향상
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Input System 메시지 수신 (On-Screen Stick과 WASD 모두 대응)
        void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        void FixedUpdate()
        {
            // 1. 부드러운 가속/감속 처리 (UX 디테일)
            smoothMoveInput = Vector2.SmoothDamp(smoothMoveInput, moveInput, ref moveInputVelocity, smoothTime);

            // 2. 물리 이동 처리 (Unity 6의 최신 API 적용)
            rb.linearVelocity = smoothMoveInput * moveSpeed;

            // 3. 좌우 반전 처리 (Scale 대신 Flip을 써서 크기 왜곡 방지)
            if (moveInput.x != 0)
            {
                // x가 0보다 작으면(왼쪽 이동) flipX를 켭니다.
                spriteRenderer.flipX = (moveInput.x < 0);
            }
        }
    }
}