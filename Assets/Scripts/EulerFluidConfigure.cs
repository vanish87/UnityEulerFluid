using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;
using UnityTools.Common;

namespace EulerFluid
{
	public class EulerFluidConfigure : Configure<EulerFluidConfigure.Data>
	{
		[System.Serializable]
		public class Data : GPUContainer
		{
            [Shader(Name = "_TimeStep")] public float timeStep = 0.01f;
			[Shader(Name = "_FieldSize")] public int3 fieldSize = new int3(512, 512, 1);
			[Shader(Name = "_VelocityDissipation")] public float velocityDissipation = 0.999f;
			[Shader(Name = "_PressureDissipation")] public float pressureDissipation = 0.999f;
			[Shader(Name = "_DiffuseIteration")] public int diffuseIteration = 10;
			[Shader(Name = "_PressureIteration")] public int pressureIteration = 50;
			[Shader(Name = "_Viscosity")] public float viscosity = -1;
		}
	}
}