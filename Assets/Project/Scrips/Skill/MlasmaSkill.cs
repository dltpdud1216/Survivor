using UnityEngine;

namespace Survivor
{
    public class MlasmaSkill : MonoBehaviour
    {
        [SerializeField] private GameObject mlasmaPrefab;
        private GameObject spawnedMlasma;

        void OnEnable()
        {
            if (mlasmaPrefab != null && spawnedMlasma == null)
            {
                // 플레이어의 자식으로 소환
                spawnedMlasma = Instantiate(mlasmaPrefab, transform.position, Quaternion.identity, transform);
            }
        }
    }
}