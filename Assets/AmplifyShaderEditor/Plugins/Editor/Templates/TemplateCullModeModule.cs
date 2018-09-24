// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public sealed class TemplateCullModeModule : TemplateModuleParent
	{
        public TemplateCullModeModule() : base("Cull Mode"){ }

        private static readonly string CullModeStr = "Cull Mode";

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;
		
		public override void Draw( UndoParentNode owner )
		{
			m_cullMode = (CullMode)owner.EditorGUILayoutEnumPopup( CullModeStr, m_cullMode );
		}

		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_cullMode = (CullMode)Enum.Parse( typeof( CullMode ), nodeParams[ index++ ] );
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
		}

		public override string GenerateShaderData()
		{
			return "Cull " + m_cullMode.ToString();
		}

		public void ConfigureFromTemplateData( TemplateCullModeData data )
		{
			m_cullMode = data.CullModeData;
		}

        public CullMode CurrentCullMode { get { return m_cullMode; } }
	}
}
