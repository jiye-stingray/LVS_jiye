using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

    private Camera _camera;
    private Transform _target;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        Manager.Instance.InitCameraController(this);
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
