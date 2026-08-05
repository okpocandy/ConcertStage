using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cameras;

    private void Start()
    {
        SwitchCamera(0);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.digit1Key.wasPressedThisFrame) SwitchCamera(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SwitchCamera(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SwitchCamera(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SwitchCamera(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SwitchCamera(4);
        if (keyboard.digit6Key.wasPressedThisFrame) SwitchCamera(5);
    }

    private void SwitchCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = i == index ? 100 : 0;
        }
    }
}
