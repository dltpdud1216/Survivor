using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Survivor
{
    public class VolumeController : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Master")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Button masterMuteBtn;   // 소리 끄는 버튼 (OnMute)
        [SerializeField] private Button masterUnmuteBtn; // 소리 켜는 버튼 (UnMute)

        [Header("BGM")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Button bgmMuteBtn;
        [SerializeField] private Button bgmUnmuteBtn;

        [Header("SFX")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button sfxMuteBtn;
        [SerializeField] private Button sfxUnmuteBtn;

        private float lastMasterVol = 0.5f;
        private float lastBgmVol = 0.5f;
        private float lastSfxVol = 0.5f;

        private void Start()
        {
            // 1. 슬라이더 이벤트 등록
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            bgmSlider.onValueChanged.AddListener(val => { ApplyVolume("BGMVol", val); UpdateBtnUI(bgmMuteBtn, bgmUnmuteBtn, val); });
            sfxSlider.onValueChanged.AddListener(val => { ApplyVolume("SFXVol", val); UpdateBtnUI(sfxMuteBtn, sfxUnmuteBtn, val); });

            // 2. 뮤트(끄기) 버튼 클릭: 현재 값 저장하고 0으로
            masterMuteBtn.onClick.AddListener(() => { lastMasterVol = masterSlider.value; masterSlider.value = 0; });
            bgmMuteBtn.onClick.AddListener(() => { lastBgmVol = bgmSlider.value; bgmSlider.value = 0; });
            sfxMuteBtn.onClick.AddListener(() => { lastSfxVol = sfxSlider.value; sfxSlider.value = 0; });

            // 3. 언뮤트(켜기) 버튼 클릭: 저장된 값으로 복구
            masterUnmuteBtn.onClick.AddListener(() => { masterSlider.value = (lastMasterVol > 0.05f) ? lastMasterVol : 0.5f; });
            bgmUnmuteBtn.onClick.AddListener(() => { bgmSlider.value = (lastBgmVol > 0.05f) ? lastBgmVol : 0.5f; });
            sfxUnmuteBtn.onClick.AddListener(() => { sfxSlider.value = (lastSfxVol > 0.05f) ? lastSfxVol : 0.5f; });

            // 초기 UI 갱신
            UpdateBtnUI(masterMuteBtn, masterUnmuteBtn, masterSlider.value);
            UpdateBtnUI(bgmMuteBtn, bgmUnmuteBtn, bgmSlider.value);
            UpdateBtnUI(sfxMuteBtn, sfxUnmuteBtn, sfxSlider.value);
        }

        private void OnMasterSliderChanged(float val)
        {
            ApplyVolume("MasterVol", val);
            UpdateBtnUI(masterMuteBtn, masterUnmuteBtn, val);

            // 마스터 슬라이더가 직접 움직일 때만 서브 슬라이더들을 강제로 맞춤
            bgmSlider.value = val;
            sfxSlider.value = val;
        }

        private void UpdateBtnUI(Button muteBtn, Button unmuteBtn, float vol)
        {
            // 소리가 꺼져 있으면 (0 근처) -> Unmute(켜기) 버튼만 활성화
            bool isMuted = vol <= 0.001f;
            muteBtn.gameObject.SetActive(!isMuted);
            unmuteBtn.gameObject.SetActive(isMuted);
        }

        private void ApplyVolume(string param, float val)
        {
            float dB = Mathf.Log10(Mathf.Max(val, 0.0001f)) * 20;
            audioMixer.SetFloat(param, dB);
        }
    }
}