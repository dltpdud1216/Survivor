using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private bool isSlow;

    public void Setup(Vector2 dir, float dmg, float spd, bool slow = false)
    {
        direction = dir;
        damage = dmg;
        speed = spd;
        isSlow = slow;

        // 투사체가 날아가는 방향을 바라보게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 5f); // 화면 밖으로 나갈 걸 대비해 5초 뒤 삭제
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 몬스터 데미지 로직 실행
            // collision.GetComponent<Enemy>().TakeDamage(damage);

            if (isSlow)
            {
                // 슬로우 로직 추가 (예: 적 이동속도 절반으로)
                Debug.Log("얼음 화살! 적 이동속도 감소 ❄️");
            }

            Destroy(gameObject); // 적중 시 투사체 삭제
        }
    }
}