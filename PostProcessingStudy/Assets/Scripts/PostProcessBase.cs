using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessBase : MonoBehaviour
{
    [SerializeField] protected ComputeShader computeShader;

    protected RenderTexture OutputTexture;
    protected int kernalHandle;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {
        CreateTexture();
    }

    protected virtual void OnDisable()
    {
        ClearTexture();
    }

    protected virtual void CreateTexture()
    {
        OutputTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        OutputTexture.enableRandomWrite = true;
        OutputTexture.Create();
    }

    protected virtual void ClearTexture()
    {
        OutputTexture.Release();
    }

}
