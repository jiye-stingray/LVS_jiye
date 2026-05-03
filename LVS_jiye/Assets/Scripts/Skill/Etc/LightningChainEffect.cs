using System.Collections;
using UnityEngine;

public class LightningChainEffect : MonoBehaviour
{
    [SerializeField] private float _defaultDuration = 0.4f;

    private string _poolKey;
    private Coroutine _returnCoroutine;

    public void Init(string poolKey, float duration = -1f)
    {
        _poolKey = poolKey;
        float d = duration > 0f ? duration : _defaultDuration;

        if (_returnCoroutine != null)
            StopCoroutine(_returnCoroutine);
        _returnCoroutine = StartCoroutine(ReturnAfterDelay(d));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Manager.Instance.Pool.Return(_poolKey, gameObject);
    }

    private void OnDisable()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }
}
