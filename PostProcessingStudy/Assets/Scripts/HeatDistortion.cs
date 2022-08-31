using UnityEngine;

public class HeatDistortion : MonoBehaviour
{
    [SerializeField] Material heatDistortMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, heatDistortMat, 0);
    }
}
