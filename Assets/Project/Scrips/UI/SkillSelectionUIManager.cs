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

        private List<MonoBehaviour> playerScripts = new List<MonoBehaviour>();
        private List<SkillData> currentOptions = new List<SkillData>();
        private static bool isFirstSelectionDone = false;

        private Dictionary<string, string> skillNameMap = new Dictionary<string, string>() {
            { "독 안개", "MlasmaSkill" }, { "화염구", "FireballSkill" }, { "얼음 화살", "FrostBoltSkill" },
            { "벼락", "VoltCrashSkill" }, { "회전하는 칼날", "SpinningBladeSkill" },
            { "자석", "MagnetSkill" }, { "헤르메스의 신발", "HermesShoesSkill" }
        };

        // 😤 [추가] 게임 시작 시 모든 스킬 레벨 초기화
        private void Awake()
        {
            isFirstSelectionDone = false; // 첫 선택 여부 초기화
            foreach (var skill in allSkillDatabase)
            {
                if (skill != null) skill.level = 0; // 😤 무조건 0레벨(기본)부터 시작!
            }
        }

        void Start() { SetupButtons(); ShowRandomSkillSelection(); }

        public void ShowRandomSkillSelection()
        {
            if (skillPanel != null) skillPanel.SetActive(true);
            Time.timeScale = 0f;
            RefreshPlayerReference();

            // 레벨 4 미만(기본1+강화3)만 등장
            var availableSkills = allSkillDatabase.Where(s => s != null && s.level < 4).ToList();

            List<SkillData> filteredPool = !isFirstSelectionDone ?
                availableSkills.Where(s => s.type == SkillType.Active).ToList() :
                availableSkills.Where(s => s.type == SkillType.Active || s.type == SkillType.Passive).ToList();

            int count = Mathf.Min(skillChoiceButtons.Count, filteredPool.Count);
            currentOptions = filteredPool.OrderBy(x => Random.value).Take(count).ToList();

            UpdateUIAndListeners();
        }

        private void UpdateUIAndListeners()
        {
            foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false);
            for (int i = 0; i < currentOptions.Count; i++)
            {
                SkillData data = currentOptions[i];
                skillChoiceButtons[i].gameObject.SetActive(true);

                // UI 텍스트: level 0이면 "New!", 1이상이면 "Lv.X"
                string levelText = data.level == 0 ? "New!" : $"Lv.{data.level + 1}";
                skillChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{data.skillName} ({levelText})";

                var iconImage = skillChoiceButtons[i].GetComponentsInChildren<Image>(true).FirstOrDefault(x => x.gameObject.name == "Icon");
                if (iconImage != null) iconImage.sprite = data.skillIcon;

                skillChoiceButtons[i].onClick.RemoveAllListeners();
                skillChoiceButtons[i].onClick.AddListener(() => OnSkillSelected(data));
            }
        }

        public void OnSkillSelected(SkillData selected)
        {
            // 1. 먼저 현재 레벨(0) 기준으로 스크립트 활성화 (기본 수치 적용)
            ActivateSkillScript(selected.skillName);

            // 2. 그 다음 레벨업 (0 -> 1)
            selected.level++;

            isFirstSelectionDone = true;
            ResumeGame();
        }

        private void ActivateSkillScript(string displayName)
        {
            string searchKey = displayName.Trim();
            if (!skillNameMap.TryGetValue(searchKey, out string targetClassName))
                targetClassName = searchKey.Replace(" ", "");

            foreach (var script in playerScripts)
            {
                if (script != null && string.Equals(script.GetType().Name, targetClassName, System.StringComparison.OrdinalIgnoreCase))
                {
                    script.enabled = true;
                    if (script is MagnetSkill magnet) magnet.ApplyMagnetEffect();
                    break;
                }
            }
        }

        public void RefreshPlayerReference()
        {
            if (playerObject == null) playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) playerScripts = playerObject.GetComponentsInChildren<MonoBehaviour>(true).ToList();
        }

        private void SetupButtons() { foreach (var btn in skillChoiceButtons) btn.gameObject.SetActive(false); }
        private void ResumeGame() { if (skillPanel != null) skillPanel.SetActive(false); Time.timeScale = 1f; }
    }
}