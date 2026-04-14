using UnityEngine;

namespace Survivor
{
    public class Projectile : MonoBehaviour
    {
        public enum ProjectileType { Guided, Straight, Explosion }
        public ProjectileType type;

        public float damage = 20f;
        public float speed = 7f;
        public float explosionRadius = 3.0f;
        public float slowAmount = 0.5f;
        public int pierceCount = 0;
        public float rotationOffset = 0f;
        public GameObject evolutionPrefab;
        public bool isEvolvedProjectile = false;

        private Vector3 direction;
        private bool isInitialized = false;

        // 😤 [수정] OnEnable에서 1로 만들던 쓰레기 코드 삭제

        public void Setup(Vector3 dir, float dmg, float projSpeed, float multiplier = 1f, bool evolved = false)
        {
            this.direction = dir.normalized;
            this.damage = dmg;
            this.speed = projSpeed;
            this.isEvolvedProjectile = evolved;

            // 😤 [수정] 여기서 transform.localScale = Vector3.one 하던 거 싹 지웠습니다.
            // 이제 세영님이 프리팹에 설정한 0.19 값이 그대로 유지됩니다.

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

            isInitialized = true;
            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            if (!isInitialized) return;
            transform.position += direction * speed * Time.deltaTime;
        }

        // 😤 [수정] LateUpdate에서 강제로 1로 만들던 쓰레기 로직도 완전 삭제
        private void LateUpdate()
        {
            // 스케일 간섭 안 함. 프리팹 설정대로 흐르게 둠.
        }

        public void ThunderHit() { Explode(false); }
        public void ThunderSlow() { Explode(true); }

        public void Explode(bool applySlow)
        {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var res in results)
            {
                if (res.CompareTag("Enemy"))
                {
                    Enemy e = res.GetComponent<Enemy>();
                    if (e != null) { e.TakeDamage(damage); if (applySlow) e.ApplySlow(slowAmount, 1.5f); }
                }
            }
        }
    }
}