using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;

namespace EulerFluid
{
	public class EulerFluidConfigure : Configure<EulerFluidConfigure.Data>
	{
		[System.Serializable]
		public class Data
		{
			public int3 fieldSize = new int3(512, 512, 1);
		}
	}
}