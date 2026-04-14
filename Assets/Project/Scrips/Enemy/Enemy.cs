using UnityEngine;
using System.Collections;

namespace Survivor
{
    public class Enemy : MonoBehaviour
    {
        [Header("Stat Settings")]
        public float hp = 100f;
        public float speed = 3f;
        public int level = 1;
        public float damage = 10f;        // 😤 플레이어에게 줄 데미지
        public float attackDelay = 0.5f; // 😤 데미지 주기 (0.5초마다)

        [Header("Item Drop Settings")]
        public GameObject itemPrefab;
        [Range(0, 1)] public float dropRate = 0.5f;

        [Header("Visual Feedback")]
        public Color hitColor = Color.red;
        public Color freezeColor = Color.blue; // 😤 빙결 시 변할 색상
        public float flashDuration = 0.1f;

        private float currentSpeed;
        private Transform player;
        private Vector3 originScale;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Rigidbody2D rb;

        private float lastAttackTime; // 마지막 공격 시간 체크
        private bool isFrozen = false; // 😤 현재 빙결 상태인지 체크

        void Start()
        {
            currentSpeed = speed;
            originScale = transform.localScale;

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) originalColor = spriteRenderer.color;

            rb = GetComponent<Rigidbody2D>();

            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;

            if (currentSpeed <= 0) currentSpeed = 3f;
        }

        void FixedUpdate()
        {
            if (player == null || isFrozen) // 😤 빙결 상태면 이동 중지
            {
                if (isFrozen && rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector3 direction = player.position - transform.position;
            direction.z = 0;
            Vector3 moveDir = direction.normalized;

            if (rb != null)
            {
                rb.linearVelocity = moveDir * currentSpeed;
            }
            else
            {
                transform.position += moveDir * currentSpeed * Time.deltaTime;
            }

            if (moveDir.x != 0)
            {
                float flipX = moveDir.x > 0 ? Mathf.Abs(originScale.x) : -Mathf.Abs(originScale.x);
                transform.localScale = new Vector3(flipX, originScale.y, originScale.z);
            }
        }

        // 😤 [플레이어와 충돌 중일 때 데미지 주기]
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Time.time >= lastAttackTime + attackDelay)
                {
                    if (collision.gameObject.TryGetComponent<PlayerStats>(out PlayerStats stats))
                    {
                        stats.TakeDamage(damage);
                        lastAttackTime = Time.time;
                        Debug.Log($"💥 플레이어 피격! 남은 체력: {stats.currentHP}");
                    }
                }
            }
        }

        public void TakeDamage(float dmg)
        {
            hp -= dmg;

            // 😤 빙결 중에는 히트 플래시(빨간색)를 잠시 끄거나 색이 겹치지 않게 처리
            if (!isFrozen)
            {
                StopCoroutine("HitFlashRoutine");
                StartCoroutine("HitFlashRoutine");
            }

            if (hp <= 0) Die();
        }

        private IEnumerator HitFlashRoutine()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = hitColor;
                yield return new WaitForSeconds(flashDuration);
                if (!isFrozen) spriteRenderer.color = originalColor;
                else spriteRenderer.color = freezeColor;
            }
        }

        private void Die()
        {
            if (itemPrefab != null && Random.value <= dropRate)
            {
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
            }

            if (MonsterKillCounter.Instance != null)
            {
                MonsterKillCounter.Instance.AddKill();
            }

            Destroy(gameObject);
        }

        // 😤 [추가 핵심] 프로스트 볼에 맞았을 때 호출할 함수
        public void ApplyFreeze(float duration)
        {
            StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            isFrozen = true;
            currentSpeed = 0;

            if (spriteRenderer != null) spriteRenderer.color = freezeColor;

            yield return new WaitForSeconds(duration);

            isFrozen = false;
            currentSpeed = speed;

            if (spriteRenderer != null) spriteRenderer.color = originalColor;
        }

        public void ApplySlow(float amount, float duration)
        {
            if (isFrozen) return; // 빙결 중이면 슬로우 무시
            StartCoroutine(SlowRoutine(amount, duration));
        }

        private IEnumerator SlowRoutine(float amount, float duration)
        {
            currentSpeed = speed * (1f - amount);
            yield return new WaitForSeconds(duration);
            if (!isFrozen) currentSpeed = speed;
        }
    }
}