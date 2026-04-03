using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Survivor
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance;

        [SerializeField] private CanvasGroup faderCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float minWaitTime = 1.5f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }
        }

        public void FadeTo(string sceneName)
        {
            faderCanvasGroup.alpha = 1f;
            faderCanvasGroup.blocksRaycasts = true;
            StartCoroutine(FadeOutIn(sceneName));
        }

        private IEnumerator FadeOutIn(string sceneName)
        {
            // 씬 로딩 (이때 이미 SkillSelectionUIManager의 Start가 실행됨!) 😤
            yield return SceneManager.LoadSceneAsync(sceneName);

            // 로딩 화면 유지
            yield return new WaitForSecondsRealtime(minWaitTime);

            // 페이드 인 (이 과정 중에 뒤에서는 이미 스킬창이 떠 있는 상태임)
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                faderCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                yield return null;
            }

            faderCanvasGroup.alpha = 0f;
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}