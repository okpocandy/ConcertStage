using UnityEngine;
using Unity.Cinemachine;

public class SplineCameraMover : MonoBehaviour
{
    [SerializeField] private CinemachineSplineDolly splineDolly;
    [SerializeField] private float duration = 5f;

    private float elapsed;
    private bool isPlaying;

    public void PlayFromStart()
    {
        if (splineDolly == null)
        {
            return;
        }

        splineDolly.CameraPosition = 0f;
        elapsed = 0f;
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
        
         if (splineDolly == null)
            return;

        splineDolly.CameraPosition = 0f;

        // 이전 프레임의 위치를 사용한 Damping 초기화
        splineDolly.VirtualCamera?.CancelDamping(true);
    }

    private void Update()
    {
        if (!isPlaying || splineDolly == null)
        {
            return;
        }

        elapsed += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsed / duration);
        splineDolly.CameraPosition = progress;

        if (progress >= 1f)
        {
            isPlaying = false;
        }
    }
}
