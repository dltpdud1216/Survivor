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

        [Header("Item Drop Settings")]
        public GameObject itemPrefab;
        [Range(0, 1)] public float dropRate = 0.5f;

        [Header("Visual Feedback")]
        public Color hitColor = Color.red;
        public float flashDuration = 0.1f;

        private float currentSpeed;
        private Transform player;
        private Vector3 originScale;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Rigidbody2D rb;

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
            if (player == null) return;

            Vector3 direction = player.position - transform.position;
            direction.z = 0;
            Vector3 moveDir = direction.normalized;

            if (rb != null)
            {
                // 플레이어가 밀어도 굴하지 않고 내 갈 길 가는 로직 😤
                rb.linearVelocity = moveDir * currentSpeed;
            }
            else
            {
                transform.position += moveDir * currentSpeed * Time.deltaTime;
            }

            if (moveDir.x != 0)
            {
                // 뒷걸음질 방지 로직 😤
                float flipX = moveDir.x > 0 ? Mathf.Abs(originScale.x) : -Mathf.Abs(originScale.x);
                transform.localScale = new Vector3(flipX, originScale.y, originScale.z);
            }
        }

        public void TakeDamage(float dmg)
        {
            hp -= dmg;
            StopCoroutine("HitFlashRoutine");
            StartCoroutine("HitFlashRoutine");
            if (hp <= 0) Die();
        }

        private IEnumerator HitFlashRoutine()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = hitColor;
                yield return new WaitForSeconds(flashDuration);
                spriteRenderer.color = originalColor;
            }
        }

        private void Die()
        {
            if (itemPrefab != null && Random.value <= dropRate)
            {
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        public void ApplySlow(float amount, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(SlowRoutine(amount, duration));
        }

        private IEnumerator SlowRoutine(float amount, float duration)
        {
            currentSpeed = speed * (1f - amount);
            yield return new WaitForSeconds(duration);
            currentSpeed = speed;
        }
    }
}