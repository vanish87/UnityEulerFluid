
using UnityEditor;
using UnityEngine;

namespace EulerFluid
{
	[CustomEditor(typeof(EulerFluidConfigure))]
	public class FluidSPH3DConfigureEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var configure = target as EulerFluidConfigure;
			if (configure == null) return;

			if (GUILayout.Button("Save"))
			{
				configure.Save();
			}
			if (GUILayout.Button("Load"))
			{
				configure.Load();
				configure.NotifyChange();
			}
			base.OnInspectorGUI();
		}
	}
}
