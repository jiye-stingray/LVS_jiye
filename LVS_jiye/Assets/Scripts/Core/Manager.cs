using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("[Manager] Instance is null. Make sure Manager exists in the scene.");
            return _instance;
        }
    }

    public UIManager UI { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitManagers();
    }

    private void InitManagers()
    {
        UI = GetComponentInChildren<UIManager>();
    }
}
