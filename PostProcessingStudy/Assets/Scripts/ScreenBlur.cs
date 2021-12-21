using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenBlur : PostProcessBase
{
    [SerializeField, Min(1)] float blurRadius = 1;

    int horizontalBlurKernalHandle;

    RenderTexture horizontalResult = null;

    protected override void Awake()
    {
        base.Awake();
        horizontalBlurKernalHandle = computeShader.FindKernel("CSHorizontalBlur");
        kernalHandle = computeShader.FindKernel("CSScreenBlur");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetShaderParameters();
    }

    protected override void CreateTexture()
    {
        base.CreateTexture();
        horizontalResult = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        horizontalResult.enableRandomWrite = true;
        horizontalResult.Create();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void ClearTexture()
    {
        base.ClearTexture();
        horizontalResult.Release();
    }

    void SetShaderParameters()
    {
        computeShader.SetTexture(kernalHandle, "Result", OutputTexture);
    }

    void UpdateParameters(RenderTexture source)
    {
        computeShader.SetTexture(kernalHandle, "SrcTexture", source);
        computeShader.SetTexture(kernalHandle, "HorizontalResult", horizontalResult);

        computeShader.SetTexture(horizontalBlurKernalHandle, "SrcTexture", source);
        computeShader.SetTexture(horizontalBlurKernalHandle, "HorizontalResult", horizontalResult);

        computeShader.SetFloat("BlurRadius", blurRadius);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateParameters(source);

        uint x, y;

        computeShader.GetKernelThreadGroupSizes(horizontalBlurKernalHandle, out x, out y, out _);

        x = (uint)Camera.main.pixelWidth / x;
        y = (uint)Camera.main.pixelHeight / y;

        computeShader.Dispatch(horizontalBlurKernalHandle, (int)x, (int)y, 1);
        computeShader.Dispatch(kernalHandle, (int)x, (int)y, 1);

        Graphics.Blit(OutputTexture, destination);
    }
}
