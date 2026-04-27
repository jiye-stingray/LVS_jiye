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

    public UIManager UI       = new UIManager();
    public UserInfoData User  = new UserInfoData();
    public ObjectPool Pool    = new ObjectPool();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
