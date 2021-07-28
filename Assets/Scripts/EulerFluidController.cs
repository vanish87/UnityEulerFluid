using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityTools.Common;
using UnityTools.ComputeShaderTool;
using UnityTools.Rendering;

namespace EulerFluid
{
	public class EulerFluidController : MonoBehaviour, IFieldTexture
	{
		[System.Serializable]
		public class GPUData : GPUContainer
		{
			[Shader(Name = "_VelocityRead")] public GPUBufferVariable<float3> velocityRead = new GPUBufferVariable<float3>();
			[Shader(Name = "_VelocityWrite")] public GPUBufferVariable<float3> velocityWrite = new GPUBufferVariable<float3>();
			[Shader(Name = "_PressureRead")] public GPUBufferVariable<float> pressureRead = new GPUBufferVariable<float>();
			[Shader(Name = "_PressureWrite")] public GPUBufferVariable<float> pressureWrite = new GPUBufferVariable<float>();
			[Shader(Name = "_VelocityDivergenceRead")] public GPUBufferVariable<float> velocityDivergenceRead = new GPUBufferVariable<float>();
			[Shader(Name = "_VelocityDivergenceWrite")] public GPUBufferVariable<float> velocityDivergenceWrite = new GPUBufferVariable<float>();
			[Shader(Name = "_VelocityTexture")] public RenderTexture velocityTexture;
			[Shader(Name = "_MousePos")] public float2 pos;
			[Shader(Name = "_MouseVel")] public float2 vel;

		}

		public enum Kernel
		{
			AdvectVelocity,
			AdvectPressure,
			Diffuse,
			AddForce,
			ComputeDivergence,
			ComputePressure,
			SubPressureGradient,
			ClearPressure,
		}

		[SerializeField] protected ComputeShader fluidCS;
		[SerializeField] protected GPUData gpuData = new GPUData();

		protected EulerFluidConfigure Configure => this.configure ??= this.GetComponent<EulerFluidConfigure>();

		public Texture FieldAsTexture => this.gpuData.velocityTexture;

		protected EulerFluidConfigure configure;

		protected ComputeShaderDispatcher<Kernel> dispatcher;

		protected float2 prevPos;

		protected void Init()
		{
			this.Configure.Initialize();
			var s = this.Configure.D.fieldSize;
            var bsize = s.x * s.y * s.z;

			this.gpuData.velocityRead.InitBuffer(bsize);
			this.gpuData.velocityWrite.InitBuffer(bsize);

			this.gpuData.pressureRead.InitBuffer(bsize);
			this.gpuData.pressureWrite.InitBuffer(bsize);

			this.gpuData.velocityDivergenceRead.InitBuffer(bsize);
			this.gpuData.velocityDivergenceWrite.InitBuffer(this.gpuData.velocityDivergenceRead);

			this.gpuData.velocityTexture = new RenderTexture(s.x, s.y, s.z, RenderTextureFormat.ARGBFloat);
			this.gpuData.velocityTexture.enableRandomWrite = true;

			this.dispatcher = new ComputeShaderDispatcher<Kernel>(this.fluidCS);
			foreach (Kernel k in Enum.GetValues(typeof(Kernel)))
			{
				this.dispatcher.AddParameter(k, this.gpuData);
				this.dispatcher.AddParameter(k, this.Configure.D);
			}
		}

		protected void Deinit()
		{
			this.gpuData?.Release();
		}
        protected void Step()
        {
            this.Advect();
            this.SolveDiffuse();
            this.AddForce();
            this.ComputePressure();
            this.SubtractPressureGradient();
        }

        protected void Advect()
        {
            var s = this.Configure.D.fieldSize;
            this.dispatcher.Dispatch(Kernel.AdvectVelocity, s.x, s.y, s.z);
            this.dispatcher.Dispatch(Kernel.AdvectPressure, s.x, s.y, s.z);

            GPUBufferVariable<float3>.SwapBuffer(this.gpuData.velocityRead, this.gpuData.velocityWrite);
            GPUBufferVariable<float>.SwapBuffer(this.gpuData.pressureRead, this.gpuData.pressureWrite);
        }

        protected void SolveDiffuse()
        {
            var s = this.Configure.D.fieldSize;
            foreach(var i in Enumerable.Range(0, this.Configure.D.diffuseIteration))
            {
				this.dispatcher.Dispatch(Kernel.Diffuse, s.x, s.y, s.z);
				GPUBufferVariable<float3>.SwapBuffer(this.gpuData.velocityRead, this.gpuData.velocityWrite);
            }
        }

        protected void AddForce()
        {
			this.gpuData.pos = new float2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
			this.gpuData.vel = (this.gpuData.pos - this.prevPos) / Time.deltaTime;
			this.prevPos = this.gpuData.pos;

            var s = this.Configure.D.fieldSize;
            this.dispatcher.Dispatch(Kernel.AddForce, s.x, s.y, s.z);
            GPUBufferVariable<float3>.SwapBuffer(this.gpuData.velocityRead, this.gpuData.velocityWrite);
        }

        protected void ComputePressure()
        {
            var s = this.Configure.D.fieldSize;
            this.dispatcher.Dispatch(Kernel.ComputeDivergence, s.x, s.y, s.z);

			this.dispatcher.Dispatch(Kernel.ClearPressure, s.x, s.y, s.z);
			GPUBufferVariable<float>.SwapBuffer(this.gpuData.pressureRead, this.gpuData.pressureWrite);

            foreach(var i in Enumerable.Range(0, this.Configure.D.pressureIteration))
            {
				this.dispatcher.Dispatch(Kernel.ComputePressure, s.x, s.y, s.z);
				GPUBufferVariable<float>.SwapBuffer(this.gpuData.pressureRead, this.gpuData.pressureWrite);
            }

        }
        protected void SubtractPressureGradient()
        {
			this.gpuData.velocityTexture.Clear();

            var s = this.Configure.D.fieldSize;
            this.dispatcher.Dispatch(Kernel.SubPressureGradient, s.x, s.y, s.z);
            GPUBufferVariable<float3>.SwapBuffer(this.gpuData.velocityRead, this.gpuData.velocityWrite);
        }

		protected void OnEnable()
		{
			this.Init();
		}
		protected void OnDisable()
		{
			this.Deinit();
		}
		protected void Update()
		{
			this.Step();
		}
	}
}
