using UnityEngine;

namespace Survivor
{
    public class MlasmaSkill : MonoBehaviour
    {
        [Header("Skill Data")]
        public SkillData skillData;

        [Header("Prefabs")]
        [SerializeField] private GameObject mlasmaPrefab;        // 일반 독 안개
        [SerializeField] private GameObject evolvedMlasmaPrefab; // 진화 독 안개

        [Header("Settings")]
        [SerializeField] private float baseDamage = 5f;

        private GameObject spawnedMlasma;
        private int lastProcessedLevel = -1;
        private int lastProcessedId = -1;

        void Update()
        {
            if (skillData == null) return;

            // 데이터나 레벨 변경 감지
            if (lastProcessedLevel != skillData.level || lastProcessedId != skillData.id)
            {
                lastProcessedLevel = skillData.level;
                lastProcessedId = skillData.id;
                ApplyStats();
            }
        }

        void OnEnable()
        {
            if (spawnedMlasma == null)
            {
                SpawnMlasma();
            }
        }

        // 😤 [중요] CheatKeyManager에서 에러 안 나게 하려면 이 함수가 반드시 있어야 함!
        public void ForceEvolveMlasma()
        {
            Debug.Log("🟣 MlasmaSkill: 치트키 강제 진화 실행!");
            SpawnMlasma();
        }

        private void SpawnMlasma()
        {
            if (spawnedMlasma != null) Destroy(spawnedMlasma);

            // 진화 데이터면 진화 프리팹, 아니면 일반 프리팹 선택
            GameObject targetPrefab = (skillData != null && skillData.type == SkillType.Evolution) ? evolvedMlasmaPrefab : mlasmaPrefab;

            if (targetPrefab != null)
            {
                spawnedMlasma = Instantiate(targetPrefab, transform.position, Quaternion.identity, transform);
                ApplyStats();
                Debug.Log($"🟣 독 안개 소환 완료! (진화 여부: {skillData.type == SkillType.Evolution})");
            }
        }

        private void ApplyStats()
        {
            if (spawnedMlasma == null) return;

            Projectile proj = spawnedMlasma.GetComponent<Projectile>();
            if (proj != null)
            {
                float multiplier = Mathf.Pow(1.5f, skillData.level);
                if (skillData != null && skillData.type == SkillType.Evolution) multiplier *= 2.0f;

                proj.type = Projectile.ProjectileType.Explosion;
                proj.Setup(Vector3.zero, baseDamage, 0f, multiplier);
            }
        }
    }
}