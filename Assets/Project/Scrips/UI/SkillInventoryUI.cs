using UnityEngine;
using System.Collections.Generic;

namespace Survivor
{
    public class SkillInventoryUI : MonoBehaviour
    {
        public static SkillInventoryUI Instance;
        public List<SkillSlot> slots = new List<SkillSlot>();

        void Awake() { Instance = this; }

        public void RefreshUI(SkillData data)
        {
            if (data == null || slots.Count == 0) return;

            // 😤 진화 스킬 처리: 기존 재료 액티브 슬롯을 찾아서 변환
            if (data.type == SkillType.Evolution)
            {
                foreach (var slot in slots)
                {
                    if (slot != null && slot.assignedSkillID == data.requiredActiveId)
                    {
                        slot.assignedSkillID = data.id; // 진화 ID로 교체
                        slot.UpdateSlot(data.skillIcon, data.level);
                        return;
                    }
                }
            }

            // 1. 기존 슬롯 업데이트
            foreach (var slot in slots)
            {
                if (slot != null && slot.assignedSkillID == data.id)
                {
                    slot.UpdateSlot(data.skillIcon, data.level);
                    return;
                }
            }

            // 2. 새 슬롯 등록
            foreach (var slot in slots)
            {
                if (slot != null && slot.assignedSkillID == -1)
                {
                    slot.assignedSkillID = data.id;
                    slot.UpdateSlot(data.skillIcon, data.level);
                    return;
                }
            }
        }
    }
}