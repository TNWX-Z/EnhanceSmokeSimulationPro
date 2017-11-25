//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright © 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
using UnityEngine.UI;
using TNWX.Fluid;
using UnityEngine;
/// <summary>
/// Control and Input config Values
/// </summary>
public class UIManager : MonoBehaviour {

    public Slider slider_ResolutionChange;
    public Slider slider_DesityDiffusion;
    public Slider slider_VelosityDiffusion;
    public Slider slider_PressureStrongly;
    public Slider slider_MotionCurlIter;
    public Slider slider_Vorticity;
    public Slider slider_BrushSize;
    public Slider slider_BrushForce;

    private FluidConfig config;

    void Awake()
    {
        this.config = Camera.main.GetComponent<FluidSimulation>().Config;

        ResolutionChange();
        DensityDiffusion();
        VelosityDiffusion();
        PressureStrongly();
        MotionCurlIter();
        Vorticity();
        BrushSize();
        BrushForce();
    }
    void Start()
    {
        this.config = Camera.main.GetComponent<FluidSimulation>().Config;
    }

    public void ResolutionChange()
    {
        this.config.Buffer_SizeScale = (int)this.slider_ResolutionChange.value; 
    }
    public void DensityDiffusion()
    {
        this.config.DensityDiffusion = this.slider_DesityDiffusion.value;
    }
    public void VelosityDiffusion()
    {
        this.config.VelocityDiffusion = this.slider_VelosityDiffusion.value;
    }
    public void PressureStrongly()
    {
        this.config.PressureConcentration = this.slider_PressureStrongly.value;
    }
    public void MotionCurlIter()
    {
        this.config.Iterations = (int)this.slider_MotionCurlIter.value;
    }
    public void Vorticity()
    {
        this.config.Vorticity = this.slider_Vorticity.value;
    }
    public void BrushSize()
    {
        this.config.SplatRadius = this.slider_BrushSize.value;
    }
    public void BrushForce()
    {
        this.config.StrongDirectionCoeff = this.slider_BrushForce.value;
    }

}
