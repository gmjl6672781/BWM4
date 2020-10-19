using System;
using UnityEngine;
public class EdgeCheckAnimation : MonoBehaviour
{
    private float delta = 0.04f;
    public Color edgeColor = new Color(0f, 1f, 1f, 1f);
    private bool isRuning = false;
    private Color lastEdgeColor;
    private Material m_Material;
    private float offset = 0.0015f;
    public Shader shader;
    public float speed = 3f;
    private float startTime = 0f;
    [Range(0f, 8.5f)]
    private float valueWave = 0f;
    [Range(0f, 8.5f)]
    private float valueWave1 = 4f;
    public Texture2D waveTexture;

    private void OnDestroy()
    {
        if (this.m_Material != null)
        {
            UnityEngine.Object.DestroyImmediate(this.m_Material);
        }
        this.isRuning = false;
        
    }

    private void OnEnable()
    {
        this.startTime = Time.time;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.isRuning)
        {
            Graphics.Blit(source, destination, this.material);
        }
        else if (material)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            //base.enabled = false;
        }
        
        if (this.shader == null)
        {
            this.shader = Shader.Find("Hidden/EdgeCheck");
        }
        if ((this.shader == null) || !this.shader.isSupported)
        {
            //base.enabled = false;
        }
        if (this.waveTexture == null)
        {
            //base.enabled = false;
            Debug.LogError("There is No waveTexture in EdgeCheck!", this);
        }
        this.material.SetFloat("_delta", this.delta);
        this.material.SetTexture("_WaveTex", this.waveTexture);
        this.material.SetColor("_Color", this.edgeColor);
        this.lastEdgeColor = this.edgeColor;
        this.isRuning = true;
        this.startTime = Time.time;
    }

    private void OnStartShow()
    {
        enabled = false;
    }

    private void OnChangeTrackState()
    {
        enabled = true;
    }

    private void Update()
    {
        if (this.lastEdgeColor != this.edgeColor)
        {
            this.material.SetColor("_Color", this.edgeColor);
            this.lastEdgeColor = this.edgeColor;
        }
        this.material.SetFloat("_Offset", this.offset);
        this.material.SetVector("_WaveTex_ST", new Vector4(1f / this.valueWave, 1f / this.valueWave, (1f - (1f / this.valueWave)) / 2f, (1f - (1f / this.valueWave)) / 2f));
        this.material.SetVector("_WaveTex1_ST", new Vector4(1f / this.valueWave1, 1f / this.valueWave1, (1f - (1f / this.valueWave1)) / 2f, (1f - (1f / this.valueWave1)) / 2f));
        this.valueWave = Mathf.Repeat((Time.time - this.startTime) * this.speed, 9f) + 0.5f;
        this.valueWave1 = Mathf.Repeat(((Time.time - this.startTime) * this.speed) + 4f, 9f) + 0.5f;
    }

    public Color EdgeColor
    {
        get
        {
            return this.edgeColor;
        }
        set
        {
            this.edgeColor = value;
        }
    }

    protected Material material
    {
        get
        {
            if (this.m_Material == null)
            {
                this.m_Material = new Material(this.shader);
                this.m_Material.hideFlags = HideFlags.HideAndDontSave;// 0x3d;
            }
            return this.m_Material;
        }
    }


}
