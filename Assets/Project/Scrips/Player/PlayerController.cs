using UnityEngine;
using UnityEngine.InputSystem;

namespace Survivor
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float smoothTime = 0.1f;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 moveInput;
        private Vector2 smoothMoveInput;
        private Vector2 moveInputVelocity;

        // 😤 패시브 스탯 참조 추가
        private PlayerStats stats;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // 😤 같은 오브젝트에 있는 PlayerStats를 연결
            stats = GetComponent<PlayerStats>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        void FixedUpdate()
        {
            // 1. 부드러운 가속/감속 처리
            smoothMoveInput = Vector2.SmoothDamp(smoothMoveInput, moveInput, ref moveInputVelocity, smoothTime);

            // 😤 2. [헤르메스의 신발 적용] 
            // 기본 속도에 패시브 배율을 곱해서 최종 속도를 결정합니다.
            float finalSpeed = moveSpeed;
            if (stats != null)
            {
                finalSpeed *= stats.moveSpeedMultiplier;
            }

            // 3. 물리 이동 처리 (Unity 6 API)
            rb.linearVelocity = smoothMoveInput * finalSpeed;

            // 4. 좌우 반전 처리
            if (moveInput.x != 0)
            {
                spriteRenderer.flipX = (moveInput.x < 0);
            }
        }
    }
}