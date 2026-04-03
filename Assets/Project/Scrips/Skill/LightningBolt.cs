using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour
{
    private LineRenderer line;

    public void Setup(Vector3 targetPos, float damage)
    {
        line = GetComponent<LineRenderer>();

        // 1. 벼락 선 그리기
        line.positionCount = 2;
        line.SetPosition(0, targetPos + Vector3.up * 10f); // 하늘 위
        line.SetPosition(1, targetPos); // 적 위치

        // 2. 실제 데미지 판정 (주변 적들까지 때리고 싶으면 OverlapCircle 추천)
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.5f);
        if (hit != null && hit.CompareTag("Enemy"))
        {
            // hit.GetComponent<Enemy>().TakeDamage(damage); 
            Debug.Log("벼락 맞음! ⚡️");
        }

        // 3. 0.1초 뒤에 오브젝트 삭제 (순간적으로 보이고 사라짐)
        Destroy(gameObject, 0.1f);
    }
}