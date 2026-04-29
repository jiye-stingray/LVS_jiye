public class WaveManager
{
    private WaveSpawner _spawner;

    public void Init(WaveSpawner spawner)
    {
        _spawner = spawner;
    }

    public void StartWave() => _spawner?.StartWave();
    public void StopWave()  => _spawner?.StopWave();
}
