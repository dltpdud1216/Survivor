using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 조건 검색을 위해 추가

namespace Survivor
{
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName;      // 구분용 이름
        public GameObject prefab;     // 몬스터 프리팹
        public int appearanceLevel;   // 이 몬스터가 등장하기 시작하는 최소 레벨
        [Range(0, 100)]
        public float spawnWeight;     // 등장 확률 비중 (높을수록 자주 나옴)
    }

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<EnemyData> enemyList; // 4종류 몬스터 등록
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnDistance = 12f;

        [Header("References")]
        [SerializeField] private PlayerStats playerStats;

        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemyBasedOnLevel();
                timer = 0;
            }
        }

        private void SpawnEnemyBasedOnLevel()
        {
            if (playerStats == null) return;

            // 1. 현재 레벨에서 등장 가능한 몬스터들만 필터링
            var availableEnemies = enemyList.Where(e => e.appearanceLevel <= playerStats.level).ToList();

            if (availableEnemies.Count == 0) return;

            // 2. 가중치 기반 랜덤 선택 (더 논리적인 확률 분배)
            GameObject selectedPrefab = GetRandomEnemy(availableEnemies);

            // 3. 소환 위치 계산 및 생성 (기존 로직 유지)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 spawnPos = (Vector2)playerStats.transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistance;

            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }

        private GameObject GetRandomEnemy(List<EnemyData> enemies)
        {
            float totalWeight = enemies.Sum(e => e.spawnWeight);
            float pivot = Random.Range(0, totalWeight);
            float currentWeight = 0;

            foreach (var enemy in enemies)
            {
                currentWeight += enemy.spawnWeight;
                if (pivot <= currentWeight) return enemy.prefab;
            }

            return enemies[0].prefab;
        }
    }
}