//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright © 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
using UnityEngine;
using TNWX.Tools;
/// <summary>
/// Smoke Simulation Kernel
/// </summary>
namespace TNWX.Fluid
{
    [RequireComponent(typeof(Camera))]
    public class FluidSimulation : MonoBehaviour
    {
        [Header("--------Set All Materials Of Smoke Simulaiton (放置所有的烟雾模拟材质)-------")]
        public Material mat_Advection;
        public Material mat_Splat;
        public Material mat_Curl;
        public Material mat_Vorticity;
        public Material mat_Divergence;
        public Material mat_Clear;
        public Material mat_Pressure;
        public Material mat_GradienSubract;
        public Material mat_Display;

        private Material[] mat_All = null;

        [Header("-------Configure(参数调节)-------")]
        public FluidConfig Config = new FluidConfig();

        private float ScreenAspect
        {
            get
            {
                return Screen.width / Screen.height;
            }
        }
        private int Buffer_width = Screen.width;
        private int Buffer_height = Screen.height;

        private Vector2 _texelSize;

        private RenderTexture[] Buf_Density01;
        private RenderTexture[] Buf_Velocity01;
        private RenderTexture Buf_Divergence;
        private RenderTexture Buf_Curl;
        private RenderTexture[] Buf_Pressure01;

        /// <summary>
        ///Parameter information update （参数信息更新)
        /// </summary>
        private float dt_time = 0.02f;
        private float mouse_dx = 0.0f;
        private float mouse_dy = 0.0f;
        private Vector3 uv_mousePos = Vector3.zero;
        private float count_Frame = 0f;

#region init & create (创建与初始化)
        private void SetAllMaterial(params Material[] _mat)
        {
            if (_mat == null)
            {
                return;
            }
            mat_All = _mat;
        }

        private bool CheakAllMaterialsActive()
        {
            if (!mat_Advection || !mat_Splat || !mat_Curl || !mat_Vorticity || !mat_Divergence || !mat_Clear || !mat_Pressure || !mat_GradienSubract || !mat_Display)
            {
                Debug.Log("有材质丢失，没有设置上......");
                return false;
            }
            return true;
        }
        private void SetAllStaticValues2Material()
        {
            for (int i = 0; i < this.mat_All.Length; i++)
            {
                this.mat_All[i].SetVector("_texelSize", this._texelSize);
            }
        }
        //初始化所有Buffer
        private void initAllBuffer()
        {
            this.Buffer_width = Mathf.Max(1,(Screen.width >> Config.Buffer_SizeScale) - (int)(Config.Buffer_SizeSubCoff * this.ScreenAspect));
            this.Buffer_height = Mathf.Max(1,(Screen.height >> Config.Buffer_SizeScale) - Config.Buffer_SizeSubCoff);

            this._texelSize = new Vector2(1f / this.Buffer_width, 1f / this.Buffer_height);

            Buf_Density01 = CreateHeaper.CreateDoubleRenderBuffer(this.Buffer_width, this.Buffer_height, RenderTextureFormat.ARGBHalf, "DensityBuffer");
            Buf_Velocity01 = CreateHeaper.CreateDoubleRenderBuffer(this.Buffer_width, this.Buffer_height, RenderTextureFormat.RGHalf, "VelocityBuffer");
            Buf_Divergence = CreateHeaper.CreateRenderBuffer(this.Buffer_width, this.Buffer_height, RenderTextureFormat.RHalf, "DivergenceBuffer");
            Buf_Curl = CreateHeaper.CreateRenderBuffer(this.Buffer_width, this.Buffer_height, RenderTextureFormat.RGHalf, "CurlBuffer");
            Buf_Pressure01 = CreateHeaper.CreateDoubleRenderBuffer(this.Buffer_width, this.Buffer_height, RenderTextureFormat.RHalf, "PressureBuffer");

            //一次性把所有值传到位
            SetAllStaticValues2Material();
        }
#endregion
        void Awake()
        {
            SetAllMaterial(
                this.mat_Advection,
                this.mat_Splat,
                this.mat_Curl,
                this.mat_Vorticity,
                this.mat_Divergence,
                this.mat_Clear,
                this.mat_Pressure,
                this.mat_GradienSubract
           );
        }
        void Start()
        {
            initAllBuffer();
            //The registration process （运行流程注册）
            simulationRun += AdvectionRun;
            simulationRun += SplatRun;
            simulationRun += CurlRun;
            simulationRun += VorticityRun;
            simulationRun += DivergenceRun;
            simulationRun += ClearRun;
            simulationRun += PressureRun;
            simulationRun += GradienSubtractRun;
        }
        /// <summary>
        ///Information really time update 必要的信息实时更新
        /// </summary>
#region Information Update (信息实时更新)
        void UpdateInformation()
        {
            this.dt_time = Time.deltaTime;
            this.mouse_dx = (Input.mousePosition.x - this.uv_mousePos.x) * this.Config.StrongDirectionCoeff;
            this.mouse_dy = (Input.mousePosition.y - this.uv_mousePos.y) * this.Config.StrongDirectionCoeff;
            this.uv_mousePos.x = Input.mousePosition.x;
            this.uv_mousePos.y = Input.mousePosition.y;
            this.uv_mousePos.z = Input.GetMouseButton(0) ? -1 : 1;

            switch (this.Config.colorStatus)
            {
                case CreateHeaper.ColorStatus.none:
                    break;
                case CreateHeaper.ColorStatus.randomColor:
                    if (this.uv_mousePos.z > 0)
                        this.Config.EffectColor = new Color(Random.value + 0.2f, Random.value + 0.2f, Random.value + 0.2f);
                    break;
                case CreateHeaper.ColorStatus.randomColorFul:
                    if (this.uv_mousePos.z < 0)
                        this.Config.EffectColor = new Color(Random.value, Random.value, Random.value);
                    break;
            }

            this.count_Frame++;
        }
        #endregion

#region Simulation Life Cycle (烟雾/流体 模拟生命周期)
        public delegate void SimulationRun();
        private SimulationRun simulationRun;

        //Pass! 计算平流运动
        void AdvectionRun()
        {
            //1.1
            this.mat_Advection.SetVector("BufferSize", new Vector2(this.Buffer_width, this.Buffer_height));

            this.mat_Advection.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);
            this.mat_Advection.SetTexture("Tex_VelocityAndDesity", this.Buf_Velocity01[1]);
            this.mat_Advection.SetFloat("dt_time", this.dt_time);
            this.mat_Advection.SetFloat("Diffusion", this.Config.VelocityDiffusion);

            this.mat_Advection.BlitAndSwapBuffer(this.Buf_Velocity01);
            //1.2
            this.mat_Advection.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);
            this.mat_Advection.SetTexture("Tex_VelocityAndDesity", this.Buf_Density01[1]);
            this.mat_Advection.SetFloat("Diffusion", this.Config.DensityDiffusion);

            this.mat_Advection.BlitAndSwapBuffer(this.Buf_Density01);
        }
        //Pass! 笔刷
        void SplatRun()
        {
            if (!Input.GetMouseButton(0))
                return;
            //1.1
            this.mat_Splat.SetTexture("Tex_VelocityAndDesity", this.Buf_Velocity01[1]);
            this.mat_Splat.SetVector("color", new Vector3(this.mouse_dx, this.mouse_dy, 1f));
            this.mat_Splat.SetVector("pointpos", new Vector2(this.uv_mousePos.x, this.uv_mousePos.y));
            this.mat_Splat.SetFloat("radius", this.Config.SplatRadius);

            this.mat_Splat.BlitAndSwapBuffer(this.Buf_Velocity01);
            //1.2
            this.mat_Splat.SetTexture("Tex_VelocityAndDesity", this.Buf_Density01[1]);
            this.mat_Splat.SetVector("color", this.Config.EffectColor * 0.3f);

            this.mat_Splat.BlitAndSwapBuffer(this.Buf_Density01);
        }
        //Pass! 卷曲
        void CurlRun()
        {
            this.mat_Curl.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);

            this.mat_Curl.Blit(Buf_Curl);
        }
        //Pass! 旋度
        void VorticityRun()
        {
            this.mat_Vorticity.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);
            this.mat_Vorticity.SetTexture("Tex_Curl", this.Buf_Curl);
            this.mat_Vorticity.SetFloat("curl", this.Config.Vorticity);
            this.mat_Vorticity.SetFloat("dt_time", this.dt_time);

            this.mat_Vorticity.BlitAndSwapBuffer(this.Buf_Velocity01);
        }
        //Pass! 散度
        void DivergenceRun()
        {
            this.mat_Divergence.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);

            this.mat_Divergence.Blit(this.Buf_Divergence);
        }
        //Pass! 消散
        void ClearRun()
        {
            this.mat_Clear.SetTexture("_Tex", this.Buf_Pressure01[1]);
            this.mat_Clear.SetFloat("dissipate", this.Config.PressureConcentration);

            this.mat_Clear.BlitAndSwapBuffer(Buf_Pressure01);
        }
        //Pass! 压强
        void PressureRun()
        {
            this.mat_Pressure.SetTexture("Tex_Divergence", this.Buf_Divergence);
            for (int i = 0; i < this.Config.Iterations; i++)
            {
                this.mat_Pressure.SetTexture("Tex_Pressure", this.Buf_Pressure01[1]);

                this.mat_Pressure.BlitAndSwapBuffer(this.Buf_Pressure01);
            }
        }
        //Pass! 梯度衰减
        void GradienSubtractRun()
        {
            this.mat_GradienSubract.SetTexture("Tex_Pressure", this.Buf_Pressure01[1]);
            this.mat_GradienSubract.SetTexture("Tex_Velocity", this.Buf_Velocity01[1]);

            this.mat_GradienSubract.BlitAndSwapBuffer(this.Buf_Velocity01);
        }
        //Pass! Build in My Magic Color
        void DisplayRun(RenderTexture _dst)
        {
            this.mat_Display.Blit(_dst, Buf_Density01[1]);
        }
#endregion

        void OnRenderImage(RenderTexture _src, RenderTexture _dst)
        {
            if (!CheakAllMaterialsActive())
            {
                Graphics.Blit(_src, _dst);
                return;
            }

            //运行 Smoke Simulation
            if (simulationRun == null)
            {
                Graphics.Blit(_src, _dst);
                return;
            }

            int tmp_width = Mathf.Max(1, (Screen.width >> Config.Buffer_SizeScale) - (int)(Config.Buffer_SizeSubCoff * this.ScreenAspect));
            int tmp_height = Mathf.Max(1, (Screen.height >> Config.Buffer_SizeScale) - Config.Buffer_SizeSubCoff);
            if(this.Buffer_width != tmp_width || this.Buffer_height != tmp_height)
            {
                initAllBuffer();
            }

            UpdateInformation();
            simulationRun.Invoke();
            DisplayRun(_dst);
        }

        /// <summary>
        ///Realese all buffers 释放所有缓冲纹理
        /// </summary>
#region Release and Remove Buffer（释放并移除所有缓存纹理）
        void Release(RenderTexture _rt)
        {
            if (_rt)
            {
                _rt.Release();
                _rt = null;
            }
        }
        void ReleaseArray(RenderTexture[] _rts)
        {
            if(_rts == null)
            {
                return;
            }
            for (int i = 0; i < _rts.Length; i++)
            {
                Release(_rts[i]);
            }
        }
        void ReleaseAll()
        {
            ReleaseArray(Buf_Density01);
            ReleaseArray(Buf_Velocity01);
            Release(Buf_Divergence);
            Release(Buf_Curl);
            ReleaseArray(Buf_Pressure01);
        }

        void OnDisable()
        {
            ReleaseAll();
        }
#endregion

    }
}
