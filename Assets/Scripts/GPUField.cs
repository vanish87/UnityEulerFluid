using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools.Rendering;

namespace UnityTools.Common
{
    public interface IField
    {
		Texture Field { get; }
    }
	public class GPUField : MonoBehaviour, IField
	{
		public Texture Field => this.data;
		[SerializeField] protected int3 fieldSize = new int3(512, 512, 1);
        [SerializeField] protected RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
        [SerializeField] protected Texture data;

		protected virtual void Init()
        {
            this.data?.DestoryObj();
            var rt = new RenderTexture(this.fieldSize.x, this.fieldSize.y, this.fieldSize.z, this.format);
            rt.enableRandomWrite = true;
            this.data = rt;
        }
        protected virtual void Deinit()
        {
            this.data?.DestoryObj();
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