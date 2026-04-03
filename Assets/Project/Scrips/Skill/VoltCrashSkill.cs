using UnityEngine;
using System.Collections;

namespace Survivor
{
    public class VoltCrashSkill : MonoBehaviour
    {
        public GameObject boltPrefab; // 라인 렌더러가 붙은 프리팹
        public float damage = 20f;
        public float coolDown = 2f;
        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= coolDown)
            {
                Strike();
                timer = 0;
            }
        }

        void Strike()
        {
            // 1. 랜덤한 적 찾기
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;

            GameObject target = enemies[Random.Range(0, enemies.Length)];

            // 2. 벼락 프리팹 생성
            GameObject bolt = Instantiate(boltPrefab);

            // 3. 벼락 스크립트에 정보 전달 (적 위치, 데미지)
            bolt.GetComponent<LightningBolt>().Setup(target.transform.position, damage);
        }
    }
}