using UnityEngine;

namespace Survivor
{
    public class FrostBoltSkill : MonoBehaviour
    {
        public GameObject frostBoltPrefab;
        public float damage = 10f;
        public float fireRate = 0.8f;

        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                Shoot();
                timer = 0;
            }
        }

        void Shoot()
        {
            // 플레이어의 이동 방향(정면)으로 발사 (InputManager나 Rigidbody에서 방향 가져오기)
            // 임시로 오른쪽으로 발사하게 설정 (수정 가능)
            Vector2 shootDir = Vector2.right;

            GameObject go = Instantiate(frostBoltPrefab, transform.position, Quaternion.identity);
            go.GetComponent<Projectile>().Setup(shootDir, damage, 12f, true); // true는 슬로우 효과 여부
        }
    }
}