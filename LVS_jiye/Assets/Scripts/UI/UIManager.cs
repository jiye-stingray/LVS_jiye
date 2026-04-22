using UnityEngine;

public class UIManager : MonoBehaviour
{
    public VirtualJoystick Joystick { get; private set; }

    public void RegisterJoystick(VirtualJoystick joystick)
    {
        Joystick = joystick;
    }

    public void UnregisterJoystick()
    {
        Joystick = null;
    }
}
