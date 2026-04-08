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

        private float currentSpeed;
        private Transform player;
        private Vector3 originScale;

        void Start()
        {
            currentSpeed = speed;
            originScale = transform.localScale;

            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;

            // 😤 혹시 모르니 시작할 때 속도가 0이거나 음수인지 체크
            if (currentSpeed <= 0) currentSpeed = 3f;
        }

        void Update()
        {
            if (player == null) return;

            // 1. 플레이어 방향 계산 (회피 방지 로직) 😤
            // 단순히 (목적지 - 내위치)를 해서 거리와 상관없이 방향만 추출합니다.
            Vector3 direction = player.position - transform.position;
            direction.z = 0; // 2D니까 Z축은 죽여버립니다.

            Vector3 moveDir = direction.normalized;

            // 2. 이동 실행 (속도에 레벨이 곱해지거나 나눠지지 않게 고정)
            transform.position += moveDir * currentSpeed * Time.deltaTime;

            // 3. 방향에 따른 좌우 반전
            if (moveDir.x != 0)
            {
                float flipX = moveDir.x > 0 ? -Mathf.Abs(originScale.x) : Mathf.Abs(originScale.x);
                transform.localScale = new Vector3(flipX, originScale.y, originScale.z);
            }
        }

        public void TakeDamage(float dmg)
        {
            hp -= dmg;
            if (hp <= 0) Die();
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
            // 😤 슬로우 시에도 speed 기본값을 참조하게 해서 계산 꼬임 방지
            currentSpeed = speed * (1f - amount);
            yield return new WaitForSeconds(duration);
            currentSpeed = speed;
        }
    }
}