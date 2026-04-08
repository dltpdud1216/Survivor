using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Survivor
{
    public class SkillSelectionUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private List<Button> skillChoiceButtons = new List<Button>();
        [SerializeField] private GameObject skillPanel;

        [Header("Skill Pool")]
        [SerializeField] private List<SkillData> allSkillDatabase = new List<SkillData>();

        [Header("Player Reference")]
        [SerializeField] private GameObject playerObject;

        private List<MonoBehaviour> playerActiveSkills = new List<MonoBehaviour>();
        private List<SkillData> currentOptions = new List<SkillData>();
        private bool isFirstSelect = true;

        // 😤 사진에 있는 스크립트 이름(Skill 포함)과 정확히 매칭되도록 수정했습니다.
        private Dictionary<string, string> skillNameMap = new Dictionary<string, string>() {
            { "독 안개", "MlasmaSkill" },
            { "화염구", "FireballSkill" },
            { "얼음 화살", "FrostBoltSkill" },
            { "벼락", "VoltCrashSkill" },
            { "회전하는 칼날", "SpinningBladeSkill" },
            { "신성한 심판", "DivineJudgementSkill" },
            { "영원한 지옥불", "EternalInfernoSkill" },
            { "빙하의 별", "GlacialStarSkill" },
            { "궤도 톱날", "OrbitingSawSkill" },
            { "죽음의 발걸음", "ToxicTrailSkill" },
            { "거인의 장갑", "GiantGauntletsSkill" },
            { "헤르메스의 신발", "HermesShoesSkill" },
            { "행운의 부적", "LuckyCharmSkill" },
            { "마법 시계", "MagicHourglassSkill" },
            { "자석", "MagnetSkill" },
            { "힘의 원석", "PowerCrystalSkill" }
        };

        void Start()
        {
            RefreshPlayerReference();
            SetupButtons();
            ShowRandomSkillSelection();
        }

        private void SetupButtons()
        {
            for (int i = 0; i < skillChoiceButtons.Count; i++)
            {
                int index = i;
                if (skillChoiceButtons[i] == null) continue;

                skillChoiceButtons[i].onClick.RemoveAllListeners();
                skillChoiceButtons[i].onClick.AddListener(() => {
                    OnSkillSelected(index);
                });
            }
        }

        public void RefreshPlayerReference()
        {
            if (playerObject == null) playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                // 자식 오브젝트에 꺼져있는 스크립트까지 전부 리스트업
                playerActiveSkills = playerObject.GetComponentsInChildren<MonoBehaviour>(true).ToList();
                Debug.Log($"✅ 플레이어 스캔 완료: {playerActiveSkills.Count}개 감지");
            }
        }

        public void ShowRandomSkillSelection()
        {
            if (skillPanel != null) skillPanel.SetActive(true);
            Time.timeScale = 0f;

            var filteredPool = allSkillDatabase.Where(s => s != null && s.level < 8).ToList();

            if (isFirstSelect)
            {
                var activeOnly = filteredPool.Where(s => s.type == SkillType.Active).ToList();
                if (activeOnly.Count >= skillChoiceButtons.Count)
                {
                    filteredPool = activeOnly;
                }
            }

            currentOptions = filteredPool.OrderBy(x => Random.value).Take(skillChoiceButtons.Count).ToList();
            UpdateUI();
        }

        public void OnSkillSelected(int index)
        {
            if (index >= 0 && index < currentOptions.Count)
            {
                SkillData selected = currentOptions[index];
                selected.level++;

                Debug.Log($"👍 선택됨: {selected.skillName}");
                ActivateSkillScript(selected.skillName);
            }

            if (isFirstSelect) isFirstSelect = false;
            ResumeGame();
        }

        private void ActivateSkillScript(string displayName)
        {
            string searchKey = displayName.Trim();

            // 😤 딕셔너리에서 클래스 이름 가져오기
            if (!skillNameMap.TryGetValue(searchKey, out string targetClassName))
            {
                targetClassName = searchKey.Replace(" ", ""); // 맵에 없으면 공백만 제거
            }

            bool found = false;
            foreach (var script in playerActiveSkills)
            {
                if (script == null) continue;

                // 😤 클래스 이름을 대소문자 무시하고 비교
                string currentClassName = script.GetType().Name;

                if (string.Equals(currentClassName, targetClassName, System.StringComparison.OrdinalIgnoreCase))
                {
                    script.enabled = true;
                    Debug.Log($"✨ 스크립트 활성화 성공: {currentClassName}");
                    found = true;
                    break;
                }
            }

            if (!found) Debug.LogError($"❌ 매칭 실패: '{targetClassName}' 클래스를 찾을 수 없습니다. (선택 이름: {searchKey})");
        }

        private void ResumeGame()
        {
            if (skillPanel != null) skillPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        private void UpdateUI()
        {
            foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false);

            for (int i = 0; i < currentOptions.Count; i++)
            {
                if (i >= skillChoiceButtons.Count) break;

                skillChoiceButtons[i].gameObject.SetActive(true);

                var txt = skillChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = currentOptions[i].skillName;

                var images = skillChoiceButtons[i].GetComponentsInChildren<Image>(true);
                var icon = images.FirstOrDefault(x => x.gameObject.name == "Icon");
                if (icon != null) icon.sprite = currentOptions[i].skillIcon;
            }
        }
    }
}