using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수
using TMPro;           // 레벨 텍스트 표시용 (선택 사항)

namespace Survivor
{
    public class ExperienceUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TextMeshProUGUI levelText; // 레벨 표시용 텍스트

        [Header("Player Reference")]
        [SerializeField] private PlayerStats playerStats;

        void Start()
        {
            if (playerStats == null)
            {
                playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            }

            // 초기 UI 업데이트
            UpdateXPUI();
        }

        void Update()
        {
            // 매 프레임 업데이트 (더 최적화하려면 이벤트를 써도 되지만, 지금은 직관적으로!)
            UpdateXPUI();
        }

        private void UpdateXPUI()
        {
            if (playerStats == null || xpSlider == null) return;

            // Slider의 Value는 0~1 사이의 비율로 계산 (현재 경험치 / 다음 레벨 목표치)
            xpSlider.value = playerStats.currentExp / playerStats.nextLevelExp;

            // 레벨 텍스트 업데이트
            if (levelText != null)
            {
                levelText.text = $"LV. {playerStats.level}";
            }
        }
    }
}