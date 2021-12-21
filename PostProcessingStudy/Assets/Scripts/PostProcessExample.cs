using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessExample : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] Color screenTint = Color.white;
    [SerializeField] float colorExpose = 1;
    [SerializeField] float cursorMaskRadius = 5.0f;

    int kernalHandle;
    RenderTexture outputTexture;
    Vector4 mousePos;

    private void Awake()
    {
        kernalHandle = computeShader.FindKernel("CSBloom");
    }

    private void OnEnable()
    {
        CreateTexture();
        SetShaderParameters();
    }

    private void OnDisable()
    {
        ClearTexture();
    }

    void CreateTexture()
    {
        outputTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
    }

    void ClearTexture()
    {
        if (outputTexture != null)
            outputTexture.Release();
    }

    void SetShaderParameters()
    {
        computeShader.SetTexture(kernalHandle, "Result", outputTexture);
    }

    void UpdateParameters(RenderTexture source)
    {
        computeShader.SetTexture(kernalHandle, "srcTexture", source);
        computeShader.SetVector("Tint", screenTint);
        computeShader.SetFloat("ColorExpose", colorExpose);

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

        Graphics.Blit(outputTexture, destination);
    }
}
