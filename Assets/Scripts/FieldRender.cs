using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Common;

namespace UnityTools.Rendering
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
	public class FieldRender : MonoBehaviour
	{
        [SerializeField] protected Mesh quad;
        [SerializeField] protected Shader shader;
        protected DisposableMaterial material;
        protected MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        protected IField field;
		protected IField Field => this.field ??= this.GetComponent<IField>() ?? this.GetComponentInParent<IField>();

        protected void OnEnable()
        {
            this.material?.Dispose();
            this.material = new DisposableMaterial(this.shader);
            this.meshRenderer = this.GetComponent<MeshRenderer>();
            this.meshFilter = this.GetComponent<MeshFilter>();
            this.meshRenderer.material = this.material;
            this.meshFilter.sharedMesh = this.quad;
        }
        protected void Update()
        {
            this.material.Data.mainTexture = this.Field?.Field;
        }
        protected void OnDisable()
        {
            this.material?.Dispose();
        }

	}
}
