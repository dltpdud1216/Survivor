using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ★ 마우스 입력을 위해 필수!
using TMPro; // TextMeshPro를 쓴다면 추가

namespace Survivor
{
    // 마우스 오버 이벤트를 받기 위해 인터페이스를 상속받습니다.
    public class FloatingText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Floating Settings")]
        [SerializeField] private float floatSpeed = 2f;      // 움직이는 속도
        [SerializeField] private float floatDistance = 15f;   // 움직이는 거리 (위아래 범위)

        [Header("Scale Settings")]
        [SerializeField] private float targetScale = 1.2f;    // 마우스 올렸을 때 커질 크기 (배율)
        [SerializeField] private float scaleSpeed = 5f;       // 크기 변화 속도

        private Vector3 startPosition;
        private Vector3 originalScale;
        private bool isMouseOver = false;

        void Start()
        {
            // 게임 시작 시 초기 위치와 크기를 저장합니다.
            startPosition = transform.position;
            originalScale = transform.localScale;
        }

        void Update()
        {
            // 마우스가 올라가 있지 않을 때만 위아래로 움직입니다.
            if (!isMouseOver)
            {
                FloatMovement();
            }

            // 크기 변화 처리 (Smooth하게)
            HandleScale();
        }

        private void FloatMovement()
        {
            // 사인파(Sin)를 이용해 부드럽게 위아래로 움직이는 좌표 계산
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatDistance;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        private void HandleScale()
        {
            // 마우스 오버 상태에 따라 목표 크기 설정
            Vector3 desiredScale = isMouseOver ? originalScale * targetScale : originalScale;

            // 크기를 부드럽게(Lerp) 목표값으로 변화시킵니다.
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.timeScale * scaleSpeed);
        }

        // --- [마우스 오버 이벤트] ---

        // 마우스가 글자 위에 올라갔을 때 호출됨
        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOver = true;
        }

        // 마우스가 글자에서 벗어났을 때 호출됨
        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOver = false;

        }
    }
}