using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Common;
using UnityTools.ComputeShaderTool;

namespace EulerFluid
{
	public class EulerFluidController : MonoBehaviour
	{
		[System.Serializable]
		public class GPUData : GPUContainer
		{
			[Shader(Name = "Velocity")] public TextureField velocityField = new TextureField();
		}

		public enum Kernel
		{
			Advert,

		}

		[SerializeField] protected GPUData gpuData = new GPUData();
		[SerializeField] protected ComputeShader fluidCS;

		protected EulerFluidConfigure Configure => this.configure ??= this.GetComponent<EulerFluidConfigure>();
		protected EulerFluidConfigure configure;

		protected ComputeShaderDispatcher<Kernel> dispatcher;

		protected void Init()
		{
			this.Configure.Initialize();
			this.gpuData.velocityField.Init(this.Configure.D.fieldSize);

			this.dispatcher = new ComputeShaderDispatcher<Kernel>(this.fluidCS);
			foreach (Kernel k in Enum.GetValues(typeof(Kernel)))
			{
				this.dispatcher.AddParameter(k, this.gpuData);
			}
		}

		protected void Deinit()
		{
			this.gpuData?.Release();
		}


		protected void OnEnable()
		{
			this.Init();
		}
		protected void OnDisable()
		{
			this.Deinit();
		}
	}
}
