using UnityEngine;
using UnityEngine.UI;

namespace Survivor
{
    public class PlayerHPBar : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;

        void Awake()
        {
            if (hpSlider == null) hpSlider = GetComponent<Slider>();

            // 😤 [강제 설정] 시작하자마자 슬라이더의 범위를 0~1로 고정
            if (hpSlider != null)
            {
                hpSlider.minValue = 0f;
                hpSlider.maxValue = 1f;
                hpSlider.value = 1f; // 시작할 땐 일단 꽉 채워둠
            }
        }

        public void UpdateHPBar(float currentHP, float maxHP)
        {
            if (hpSlider != null)
            {
                // 비율 계산 (100/100 = 1.0)
                float healthRatio = (maxHP > 0) ? currentHP / maxHP : 0;

                // 😤 슬라이더 값 갱신
                hpSlider.value = healthRatio;

                // 😤 [꿀팁] 가끔 UI가 안 변할 때를 대비해 그래픽 강제 업데이트
                hpSlider.fillRect.gameObject.SetActive(healthRatio > 0);

                Debug.Log($"[HP UI] 수치: {currentHP}/{maxHP} | 슬라이더 Value: {hpSlider.value}");
            }
        }
    }
}