using UnityEngine;
using TMPro; // TextMeshPro를 사용한다면 필수!

namespace Survivor
{
    public class MonsterKillCounter : MonoBehaviour
    {
        // 😤 싱글톤으로 만들어서 어디서든 접근하기 쉽게 합니다.
        public static MonsterKillCounter Instance;

        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI killCountText;

        private int currentKillCount = 0;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            UpdateKillUI();
        }

        // 😤 몬스터가 죽을 때마다 이 함수를 부를 겁니다.
        public void AddKill()
        {
            currentKillCount++;
            UpdateKillUI();
        }

        private void UpdateKillUI()
        {
            if (killCountText != null)
            {
                // 😤 "KILLS : 123" 형식으로 표시
                killCountText.text = $"{currentKillCount}";
            }
        }
    }
}