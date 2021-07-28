using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace UnityTools.Common
{
	public class BufferField<T> : MonoBehaviour, IField<GPUBufferVariable<T>>, IFieldTexture
	{
		public static Texture BufferToTexture(GPUBufferVariable<T> buffer, int3 bufferSize)
		{
            var rt = new RenderTexture(bufferSize.x, bufferSize.y, bufferSize.z, RenderTextureFormat.ARGBFloat);
            rt.enableRandomWrite = true;
			rt.Create();
			return rt;
		}
		public GPUBufferVariable<T> Field => this.data;
		public int3 Size { get => this.fieldSize; set => this.fieldSize = value; }

		public Texture FieldAsTexture => this.textureData??=BufferToTexture(this.data, this.Size);

		[SerializeField] protected int3 fieldSize = new int3(512, 512, 1);
        protected GPUBufferVariable<T> data;
		protected Texture textureData;

		protected virtual void Init()
		{
			this.data?.Release();
			this.textureData?.DestoryObj();
			var bsize = this.Size.x * this.Size.y * this.Size.z;
			this.data = new GPUBufferVariable<T>("BufferField", bsize); 

		}
		protected virtual void Deinit()
		{
			this.data?.Release();
			this.textureData?.DestoryObj();
		}
	}
}