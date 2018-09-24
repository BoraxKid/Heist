// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Normal", "Surface Data", "Per pixel world normal vector, accepts a <b>Normal</b> vector in tangent space (ie: normalmap)" )]
	public sealed class WorldNormalVector : ParentNode
	{
		private const string NormalVecValStr = "newWorldNormal";
		private const string NormalVecDecStr = "float3 {0} = {1};";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_previewShaderGUID = "5f55f4841abb61e45967957788593a9d";
			m_drawPreviewAsSphere = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_outputPorts[ 0 ].IsLocalValue )
						return m_outputPorts[ 0 ].LocalValue;


					string value = dataCollector.TemplateDataCollectorInstance.GetWorldNormal( m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
					RegisterLocalVariable( 0, value, ref dataCollector, "worldNormal" + OutputId );
					return m_outputPorts[ 0 ].LocalValue;
				}
				else
				{
					return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldNormal() );
				}
			}

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				
				string result = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
					dataCollector.ForceNormal = true;

					result = "WorldNormalVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar ) + " )";

					int connCount = 0;
					for( int i = 0; i < m_outputPorts.Count; i++ )
					{
						connCount += m_outputPorts[ i ].ConnectionCount;
					}

					if( connCount > 1 || outputId > 0 )
					{
						dataCollector.AddToLocalVariables( UniqueId, string.Format( NormalVecDecStr, NormalVecValStr + OutputId, result ) );
						return GetOutputVectorItem( 0, outputId, NormalVecValStr + OutputId );
					}
				}
				else
				{
					if( !dataCollector.DirtyNormal )
					{
						result = Constants.InputVarStr + ".worldNormal";
					}
					else
					{
						dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
						result = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
						dataCollector.ForceNormal = true;
					}


				}

				return GetOutputVectorItem( 0, outputId, result );
			}
			else
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					string inputTangent = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

					string normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					string tangent = GeneratorUtils.GenerateWorldTangent( ref dataCollector, UniqueId );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3x3 tangentToWorld = CreateTangentToWorldPerVertex( " + normal + ", " + tangent + ", " + Constants.VertexShaderInputStr + ".tangent.w );" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 tangentNormal" + OutputId + " = " + inputTangent + ";" );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 modWorldNormal" + OutputId + " = (tangentToWorld[0] * tangentNormal" + OutputId + ".x + tangentToWorld[1] * tangentNormal" + OutputId + ".y + tangentToWorld[2] * tangentNormal" + OutputId + ".z);" );
					return GetOutputVectorItem( 0, outputId, "modWorldNormal" + OutputId );
				}
				else
				{
					string result = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					return GetOutputVectorItem( 0, outputId, result );
				}
			}
		}
	}
}
