using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = GameObject.Find("Managers");
                if(go != null)
                {
                    _instance = go.GetComponent<Manager>();
                }
                else
                {
                    var manager = new GameObject("Manager");
                    manager.AddComponent<Manager>();
                    _instance = manager.GetComponent<Manager>();
                }

                if(_instance == null)
                    Debug.LogError("[Manager] Instance is null. Make sure Manager exists in the scene.");
            }
            return _instance;
        }
    }

    public UIManager UI        = new UIManager();
    public UserInfoData User   = new UserInfoData();
    public ObjectPool Pool     = new ObjectPool();
    public SkillManager skill  = new SkillManager();
    public WaveManager Wave    = new WaveManager();
    public PlayerController Player { get; private set; }
    public WaveSpawner WaveSpawner { get; private set; }
    public CameraController Camera { get; private set; }

    public void InitPlayerController(PlayerController player)
    {
        Player = player;
        Camera?.SetTarget(player.transform);
    }

    public void InitWaveController(WaveSpawner waveSpawner)
    {
        WaveSpawner = waveSpawner;
    }

    public void InitCameraController(CameraController camera)
    {
        Camera = camera;
        if (Player != null)
            Camera.SetTarget(Player.transform);
    }

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
