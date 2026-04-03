/*using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Survivor
{
    public class SkillSelectionUIManager : MonoBehaviour
    {
        [Header("UI Elements (Hierarchy 연결)")]
        [SerializeField] private List<Button> SampleButtons = new List<Button>();

        [Header("Skill Pool (데이터 연결)")]
        [SerializeField] private List<SkillData> SampleBase = new List<SkillData>();

        private List<SkillData> SamplecurrentOptions = new List<SkillData>();

        void Start()
        {
            gameObject.SetActive(false);
            for (int i = 0; i < SampleButtons.Count; i++)
            {
                int index = i;
                SampleButtons[i].onClick.RemoveAllListeners();
                SampleButtons[i].onClick.AddListener(() => SampleOnSkillSelected(index));
            }
        }

        public void SampleShowRandomSkillSelection()
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);

            if (SampleBase.Count < 4) return;

            SamplecurrentOptions.Clear();
            List<int> randomIndices = SampleGetRandomUniqueIndices(4, SampleBase.Count);

            for (int i = 0; i < 4; i++)
            {
                SkillData skillData = SampleBase[randomIndices[i]];
                SamplecurrentOptions.Add(skillData);

                // 자식 오브젝트 찾기
                Transform iconTr = SampleButtons[i].transform.Find("Icon");
                Transform textTr = SampleButtons[i].transform.Find("Text (TMP)");

                if (iconTr != null && textTr != null)
                {
                    Image iconImage = iconTr.GetComponent<Image>();
                    TextMeshProUGUI nameText = textTr.GetComponent<TextMeshProUGUI>();

                    // [데이터 주입]
                    if (skillData.skillIcon != null)
                    {
                        iconImage.sprite = skillData.skillIcon; // 인스펙터 칸 변경 확인용
                        
                        // ★ 투명도 및 활성화 강제 설정
                        iconImage.gameObject.SetActive(true);
                        Color c = iconImage.color;
                        c.a = 1f;
                        iconImage.color = c;

                        // ★ UI 강제 갱신 (화면에 안 보일 때 해결책)
                        iconImage.SetAllDirty();
                    }
                    
                    nameText.text = skillData.skillName;
                }
            }
            
            // ★ 캔버스 전체 업데이트 강제 실행
            Canvas.ForceUpdateCanvases();
        }

        public void SampleOnSkillSelected(int choiceIndex)
        {
            if (choiceIndex >= SamplecurrentOptions.Count) return;
            
            // 스킬 적용 로직 (필요시 작성)
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        private List<int> SampleGetRandomUniqueIndices(int count, int maxCount)
        {
            List<int> result = new List<int>();
            List<int> possible = new List<int>();
            for (int i = 0; i < maxCount; i++) possible.Add(i);
            for (int i = 0; i < count; i++)
            {
                int rand = Random.Range(0, possible.Count);
                result.Add(possible[rand]);
                possible.RemoveAt(rand);
            }
            return result;
        }
    }
}*/