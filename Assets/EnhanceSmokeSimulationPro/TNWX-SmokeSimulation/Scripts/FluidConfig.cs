//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright © 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
using UnityEngine;
using System;
using TNWX.Tools;

namespace TNWX.Fluid
{
    /// <summary>
    /// Configure Smoke Simulation Value
    /// </summary>
    [Serializable]
    public class FluidConfig
    {
        public CreateHeaper.ColorStatus colorStatus = CreateHeaper.ColorStatus.none;
        public Color EffectColor = Color.blue;

        [Range(0, 2)]
        public int Buffer_SizeScale = 0;
        public int Buffer_SizeSubCoff = 0;
        [Range(0.95f, 1.0f)]
        public float DensityDiffusion = 0.98f;
        [Range(0.95f, 1.0f)]
        public float VelocityDiffusion = 0.99f;
        [Range(0.0f, 1.0f)]
        public float PressureConcentration = 0.8f;
        [Range(1, 60)]
        public int Iterations = 50;
        [Range(0, 60)]
        public float Vorticity = 20f;
        [Range(0.0001f, 0.005f)]
        public float SplatRadius = 0.001f;
        [Range(1, 15)]
        public float StrongDirectionCoeff = 5f;


    }
}
