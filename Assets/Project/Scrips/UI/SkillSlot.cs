using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Survivor
{
    public class SkillSlot : MonoBehaviour
    {
        public Image slotImage;
        public TextMeshProUGUI levelText;
        public int assignedSkillID = -1;

        void Awake()
        {
            if (slotImage != null) slotImage.enabled = false;
            if (levelText != null) levelText.text = "";
            assignedSkillID = -1;
        }

        public void UpdateSlot(Sprite icon, int level)
        {
            if (slotImage == null) return;

            slotImage.sprite = icon;
            slotImage.enabled = true;
            slotImage.color = Color.white;

            // 😤 진화 스킬(Evolution)은 level이 1로 들어오므로 숫자가 안 뜸
            // 일반 스킬은 2레벨부터 '1' (1강) 표시
            if (levelText != null)
            {
                levelText.text = (level > 1) ? (level - 1).ToString() : "";
            }
        }
    }
}