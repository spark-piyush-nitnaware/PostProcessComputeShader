using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PostProcTintVal
{
    Horizontal = 0,
    Vertical,
    Diagonal,
    Radial
}

public class PostProcessScreenTint : PostProcessBase
{
    [SerializeField] Color ColorA = Color.white;
    [SerializeField] Color ColorB = Color.white;
    [SerializeField] PostProcTintVal postProcTintType;
    [SerializeField] float radialScale = 5.0f;

    int kernalVerticalHandle;
    int kernalDiagonalHandle;
    int kernalRadialHandle;

    int currKernalHandle;

    protected override void Awake()
    {
        base.Awake();
        kernalHandle = computeShader.FindKernel("CSScreenHorizontalColorOverlay");
        kernalVerticalHandle = computeShader.FindKernel("CSScreenVerticalColorOverlay");
        kernalDiagonalHandle = computeShader.FindKernel("CSScreenDiagonalColorOverlay");
        kernalRadialHandle = computeShader.FindKernel("CSScreenRadialColorOverlay");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetShaderParameters();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    int GetKernalHandle()
    {
        int[] kernalHandles = { kernalHandle, kernalVerticalHandle, kernalDiagonalHandle, kernalRadialHandle };
        return kernalHandles[(int)postProcTintType];
    }

    void SetShaderParameters()
    {
        computeShader.SetTexture(kernalHandle, "Result", OutputTexture);
        computeShader.SetTexture(kernalVerticalHandle, "Result", OutputTexture);
        computeShader.SetTexture(kernalDiagonalHandle, "Result", OutputTexture);
        computeShader.SetTexture(kernalRadialHandle, "Result", OutputTexture);
    }

    void UpdateParameters(RenderTexture source)
    {
        computeShader.SetTexture(currKernalHandle, "SrcTexture", source);
        computeShader.SetVector("ColorOverlayA", ColorA);
        computeShader.SetVector("ColorOverlayB", ColorB);

        computeShader.SetFloat("screenWidth", Camera.main.pixelWidth);
        computeShader.SetFloat("screenHeight", Camera.main.pixelHeight);

        if(postProcTintType == PostProcTintVal.Radial)
        {
            computeShader.SetFloat("RadialScale", radialScale);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        currKernalHandle = GetKernalHandle();

        UpdateParameters(source);

        uint x, y;
        computeShader.GetKernelThreadGroupSizes(currKernalHandle, out x, out y, out _);
        computeShader.Dispatch(currKernalHandle, (int)(Camera.main.pixelWidth / x), (int)(Camera.main.pixelHeight / y), 1);

        Graphics.Blit(OutputTexture, destination);
    }
}
