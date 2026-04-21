using UnityEngine;
using UnityEngine.EventSystems;

namespace LVS.UI
{
    /// <summary>
    /// 동적 위치 가상 조이스틱.
    /// JoystickArea(투명 Image) 오브젝트에 부착.
    /// 터치한 위치에 배경이 나타나고, 그 지점 기준으로 핸들이 이동.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;

        [Header("Settings")]
        [SerializeField] private float _handleRange = 0.8f;  // 핸들이 배경 반경 대비 이동 비율
        [SerializeField] private float _deadZone = 0.1f;     // 이 값 이하 입력 무시

        private Canvas _canvas;
        private RectTransform _canvasRect;
        private Camera _camera;
        private Vector2 _input;

        /// <summary>정규화된 조이스틱 방향 (magnitude 0~1)</summary>
        public Vector2 Direction => _input;

        /// <summary>입력 중 여부</summary>
        public bool IsPressed { get; private set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _camera = _canvas.renderMode == RenderMode.ScreenSpaceCamera
                ? _canvas.worldCamera
                : null;

            _background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 터치 위치(스크린) → 캔버스 로컬 좌표 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, eventData.position, _camera, out Vector2 localPoint);

            _background.anchoredPosition = localPoint;
            _background.gameObject.SetActive(true);

            IsPressed = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 배경 중심의 스크린 좌표
            Vector2 backgroundScreenPos = RectTransformUtility.WorldToScreenPoint(_camera, _background.position);
            Vector2 radius = _background.sizeDelta * 0.5f;

            _input = (eventData.position - backgroundScreenPos) / (radius * _canvas.scaleFactor);
            _input = Vector2.ClampMagnitude(_input, _handleRange);

            _handle.anchoredPosition = _input * radius * _handleRange;

            // 데드존 보정: 데드존 밖의 입력을 0~1로 재매핑
            float magnitude = _input.magnitude;
            if (magnitude < _deadZone)
                _input = Vector2.zero;
            else
                _input = _input.normalized * ((magnitude - _deadZone) / (1f - _deadZone));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            _input = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
            _background.gameObject.SetActive(false);
        }
    }
}
