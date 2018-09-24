// Amplify Shader Editor - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Outline", "Miscellaneous", "Uses vertices to simulate an outline around the object" )]
	public sealed class OutlineNode : ParentNode
	{
		private const string ErrorMessage = "Outline node should only be connected to vertex ports.";

		[SerializeField]
		private bool m_noFog = true;

		[SerializeField]
		private string[] AvailableOutlineModes = { "Vertex Offset", "Vertex Scale", "Custom" };

		[SerializeField]
		private int[] AvailableOutlineValues = { 0, 1, 2 };

		[SerializeField]
		private int m_currentSelectedMode = 0;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT3, "Out" );
			AddInputPort( WirePortDataType.FLOAT3, false, "Color" );
			AddInputPort( WirePortDataType.FLOAT, false, "Width" );
			m_textLabelWidth = 115;
			m_hasLeftDropdown = true;
			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.UsingCustomOutlineColor = true;

			if( m_inputPorts[ 1 ].IsConnected )
				dataCollector.UsingCustomOutlineWidth = true;
		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();

			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			EditorGUI.BeginChangeCheck();
			m_currentSelectedMode = m_upperLeftWidget.DrawWidget( this, m_currentSelectedMode, AvailableOutlineModes );
			if( EditorGUI.EndChangeCheck() )
			{
				SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
				UpdatePorts();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, () =>
			{
				EditorGUI.BeginChangeCheck();
				m_currentSelectedMode = EditorGUILayoutIntPopup( "Type", m_currentSelectedMode, AvailableOutlineModes, AvailableOutlineValues );
				if( EditorGUI.EndChangeCheck() )
				{
					SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
					UpdatePorts();
				}
				m_noFog = EditorGUILayoutToggle( "No Fog", m_noFog );
			} );
		}

		void UpdatePorts()
		{
			if( m_currentSelectedMode == 2 ) //custom mode
			{
				m_inputPorts[ 1 ].ChangeProperties( "Offset", WirePortDataType.FLOAT3, false );
			}
			else
			{
				m_inputPorts[ 1 ].ChangeProperties( "Width", WirePortDataType.FLOAT, false );
			}

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsFragmentCategory )
			{
				UIUtils.ShowMessage( ErrorMessage );
				return "0";
			}
			if( m_outputPorts[ 0 ].IsLocalValue )
				return "0";

			m_outputPorts[ 0 ].SetLocalValue( "0" );

			MasterNodeDataCollector outlineDataCollector = new MasterNodeDataCollector();
			outlineDataCollector.IsOutlineDataCollector = true;
			if( m_inputPorts[ 1 ].IsConnected )
			{
				outlineDataCollector.PortCategory = MasterNodePortCategory.Vertex;
				string outlineWidth = m_inputPorts[ 1 ].GenerateShaderForOutput( ref outlineDataCollector, m_inputPorts[ 1 ].DataType, true, true );
				outlineDataCollector.AddToVertexLocalVariables( UniqueId, PrecisionType.Float, m_inputPorts[ 1 ].DataType, "outlineVar", outlineWidth );
			
				outlineDataCollector.AddVertexInstruction( outlineDataCollector.SpecialLocalVariables, UniqueId, false );
				outlineDataCollector.ClearSpecialLocalVariables();

				outlineDataCollector.AddVertexInstruction( outlineDataCollector.VertexLocalVariables, UniqueId, false );
				outlineDataCollector.ClearVertexLocalVariables();

				// need to check whether this breaks other outputs or not
				ContainerGraph.ResetNodesLocalVariables();
			}

			outlineDataCollector.PortCategory = MasterNodePortCategory.Fragment;
			string outlineColor = m_inputPorts[ 0 ].GeneratePortInstructions( ref outlineDataCollector );// "\to.Emission = " + m_inputPorts[ 0 ].GeneratePortInstructions( ref outlineDataCollector ) + ";";

			bool addTabs = outlineDataCollector.DirtySpecialLocalVariables;
			outlineDataCollector.AddInstructions( "\t" + outlineDataCollector.SpecialLocalVariables.TrimStart( '\t' ) );
			outlineDataCollector.ClearSpecialLocalVariables();
			outlineDataCollector.AddInstructions( ( addTabs ? "\t\t\t" : "" ) + "o.Emission = " + outlineColor + ";" );

			if( dataCollector.UsingWorldNormal )
				outlineDataCollector.AddInstructions( ( addTabs ? "\n\t\t\t" : "" ) + "o.Normal = float3(0,0,-1);" );

			ContainerGraph.CurrentStandardSurface.OutlineHelper.InputList = outlineDataCollector.InputList;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Inputs = outlineDataCollector.Inputs;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.DirtyInput = outlineDataCollector.DirtyInputs;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Pragmas = outlineDataCollector.Pragmas;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Uniforms = outlineDataCollector.Uniforms;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.UniformList = outlineDataCollector.UniformsList;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.VertexData = outlineDataCollector.VertexData;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Instructions = outlineDataCollector.Instructions;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.Functions = outlineDataCollector.Functions;

			ContainerGraph.CurrentStandardSurface.OutlineHelper.OffsetMode = m_currentSelectedMode;
			ContainerGraph.CurrentStandardSurface.OutlineHelper.CustomNoFog = m_noFog;

			for( int i = 0; i < outlineDataCollector.PropertiesList.Count; i++ )
			{
				dataCollector.AddToProperties( UniqueId, outlineDataCollector.PropertiesList[ i ].PropertyName, outlineDataCollector.PropertiesList[ i ].OrderIndex );
			}

			return "0";
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentSelectedMode = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_noFog = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			SetAdditonalTitleText( string.Format( Constants.SubTitleTypeFormatStr, AvailableOutlineModes[ m_currentSelectedMode ] ) );
			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelectedMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_noFog );
		}
	}
}
