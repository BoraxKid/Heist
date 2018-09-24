// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Attenuation", "Light", "Contains light attenuation for all types of light", NodeAvailabilityFlags = ( int ) NodeAvailability.CustomLighting )]
	public sealed class LightAttenuation : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = "This node only returns correct information using a custom light model, otherwise returns 1";
			m_previewShaderGUID = "4b12227498a5c8d46b6c44ea018e5b56";
			m_drawPreviewAsSphere = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			//Debug.Log( dataCollector.CurrentCanvasMode );
			if ( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
                return "1";

			dataCollector.UsingLightAttenuation = true;
			return "ase_lightAtten";
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
            if ( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader || ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
                m_showErrorMessage = true;
			else
				m_showErrorMessage = false;
		}
	}
}
