using UnityEngine;

namespace Survivor
{
    public class FireZone : MonoBehaviour
    {
        private float damage;
        private float timer;
        private float interval = 0.5f;

        public void Setup(float dmg, float radius)
        {
            this.damage = dmg;

            // 😤 절대 transform.localScale = Vector3.one * radius; 같은 거 하지 마세요!
            // 세영님이 프리팹에서 배치한 그 사이즈 그대로 쓰게 놔둡니다.
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                timer += Time.deltaTime;
                if (timer >= interval)
                {
                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                    timer = 0f;
                }
            }
        }

        private void Start()
        {
            // 2초 뒤 장판 소멸
            Destroy(gameObject, 2f);
        }
    }
}