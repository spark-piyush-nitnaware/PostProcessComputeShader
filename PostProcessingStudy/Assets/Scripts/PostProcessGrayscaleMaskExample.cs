using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessGrayscaleMaskExample : MonoBehaviour
{
    [SerializeField] protected ComputeShader computeShader;
    [SerializeField] float cursorMaskRadius = 50.0f;
    Vector4 mousePos;

    protected RenderTexture OutputTexture;
    protected int kernalHandle;

    protected void Awake()
    {
        kernalHandle = computeShader.FindKernel("CSGrayscaleMask");
    }

    protected void OnEnable()
    {
        CreateTexture();
        SetShaderParameters();
    }

    void CreateTexture()
    {
        OutputTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        OutputTexture.enableRandomWrite = true;
        OutputTexture.Create();
    }

    void SetShaderParameters()
    {
        computeShader.SetTexture(kernalHandle, "Result", OutputTexture);
    }

    protected void OnDisable()
    {
        ClearTexture();
    }

    void ClearTexture()
    {
        OutputTexture.Release();
    }

    void UpdateParameters(RenderTexture source)
    {
        computeShader.SetTexture(kernalHandle, "SrcTexture", source);

        mousePos.Set(Input.mousePosition.x, Input.mousePosition.y, 0, 0);
        computeShader.SetVector("MaskPos", mousePos);
        computeShader.SetFloat("MaskRadius", cursorMaskRadius);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateParameters(source);

        uint x, y;
        computeShader.GetKernelThreadGroupSizes(kernalHandle, out x, out y, out _);
        computeShader.Dispatch(kernalHandle, (int)(Camera.main.pixelWidth / x), (int)(Camera.main.pixelHeight / y), 1);

        Graphics.Blit(OutputTexture, destination);
    }
}
