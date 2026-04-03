using UnityEngine;

namespace Survivor
{
    public class Blade : MonoBehaviour
    {
        [HideInInspector] public float currentDamage; // 매니저 스크립트에서 설정해줌

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 태그가 Enemy인 오브젝트와 충돌 시
            if (collision.CompareTag("Enemy"))
            {
                if (collision.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(currentDamage);
                    // 여기서 타격 이펙트를 소환하면 UX가 훨씬 좋아집니다!
                }
            }
        }
    }
}