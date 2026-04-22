using UnityEngine;
using UnityEngine.EventSystems;

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

    public Vector2 Direction => _input;
    public bool IsPressed { get; private set; }

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();
        _camera = _canvas.renderMode == RenderMode.ScreenSpaceCamera
            ? _canvas.worldCamera
            : null;

        _background.gameObject.SetActive(false);
        Manager.Instance.UI.RegisterJoystick(this);
    }

    private void OnDestroy()
    {
        Manager.Instance.UI.UnregisterJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, eventData.position, _camera, out Vector2 localPoint);

        _background.anchoredPosition = localPoint;
        _background.gameObject.SetActive(true);

        IsPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 backgroundScreenPos = RectTransformUtility.WorldToScreenPoint(_camera, _background.position);
        Vector2 radius = _background.sizeDelta * 0.5f;

        _input = (eventData.position - backgroundScreenPos) / (radius * _canvas.scaleFactor);
        _input = Vector2.ClampMagnitude(_input, _handleRange);

        _handle.anchoredPosition = _input * radius * _handleRange;

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
