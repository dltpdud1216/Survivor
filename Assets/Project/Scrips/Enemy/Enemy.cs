using UnityEngine;
using System.Collections; // ★ 코루틴을 쓰기 위해 필수!

namespace Survivor
{
    public class Enemy : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float health = 20f;
        [SerializeField] private GameObject gemPrefab;

        [Header("Hit Effect")]
        [SerializeField] private Color hitColor = Color.red; // ★ 부딪혔을 때 색상
        [SerializeField] private float hitDuration = 0.1f;    // ★ 색이 유지되는 시간

        private Transform playerTransform;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        // ★ 코루틴 중복 실행 방지용
        private Coroutine hitEffectCoroutine;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        void FixedUpdate()
        {
            if (playerTransform == null) return;

            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            if (direction.x != 0)
            {
                spriteRenderer.flipX = (direction.x < 0);
            }
        }

        // 데미지를 입었을 때 호출
        public void TakeDamage(float damage)
        {
            health -= damage;

            // ★ 데미지 입을 때 깜빡이는 이펙트 실행
            if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine); // 이미 깜빡이는 중이면 멈추고 새로 시작
            hitEffectCoroutine = StartCoroutine(HitEffectRoutine());

            if (health <= 0) Die();
        }

        // ★ 빨갛게 깜빡이는 마법의 코루틴
        private IEnumerator HitEffectRoutine()
        {
            // 1. 스프라이트 색상을 빨간색으로 변경
            spriteRenderer.color = hitColor;

            // 2. hitDuration(0.1초) 동안 대기
            yield return new WaitForSeconds(hitDuration);

            // 3. 다시 하얀색(원래 색)으로 돌려놓기
            spriteRenderer.color = Color.white;
            hitEffectCoroutine = null;
        }

        private void Die()
        {
            if (Random.value < 0.7f && gemPrefab != null)
            {
                Instantiate(gemPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}