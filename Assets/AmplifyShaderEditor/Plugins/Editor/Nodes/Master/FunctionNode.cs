// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Function Node", "Functions", "Function Node", KeyCode.None, false, 0, int.MaxValue, typeof( AmplifyShaderFunction ) )]
	public class FunctionNode : ParentNode
	{
		[SerializeField]
		private AmplifyShaderFunction m_function;

		//[SerializeField]
		private ParentGraph m_functionGraph;

		[SerializeField]
		private int m_functionGraphId = -1;

		[SerializeField]
		private List<FunctionInput> m_allFunctionInputs;
		private Dictionary<int, FunctionInput> m_allFunctionInputsDict = new Dictionary<int, FunctionInput>();

		[SerializeField]
		private List<FunctionOutput> m_allFunctionOutputs;
		private Dictionary<int, FunctionOutput> m_allFunctionOutputsDict = new Dictionary<int, FunctionOutput>();

		[SerializeField]
		private ReordenatorNode m_reordenator;

		[SerializeField]
		private string m_filename;

		[SerializeField]
		private string m_headerTitle = string.Empty;

		[SerializeField]
		private int m_orderIndex;

		[SerializeField]
		private string m_functionCheckSum;

		[SerializeField]
		private string m_functionGUID = string.Empty;

		[SerializeField]
		private List<string> m_includes = new List<string>();

		[SerializeField]
		private List<string> m_pragmas = new List<string>();

		private bool m_parametersFoldout = true;
		private ParentGraph m_outsideGraph = null;

		[SerializeField]
		private FunctionOutput m_mainPreviewNode;

		[SerializeField]
		bool m_initialGraphDraw = false;

		private bool m_refreshIdsRequired = false;

		string LastLine( string text )
		{
			string[] lines = text.Replace( "\r", "" ).Split( '\n' );
			return lines[ lines.Length - 1 ];
		}

		public void CommonInit( AmplifyShaderFunction function, int uniqueId )
		{
			SetBaseUniqueId( uniqueId );

			if( function == null )
				return;

			m_refreshIdsRequired = m_containerGraph.ParentWindow.IsLoading && ( UIUtils.CurrentShaderVersion() < 14004 );

			m_function = function;

			if( Function.FunctionName.Length > 1 )
			{
				bool lastIsUpper = Char.IsUpper( Function.FunctionName, 0 );
				System.Text.StringBuilder title = new System.Text.StringBuilder();
				title.Append( Function.FunctionName[ 0 ] );
				for( int i = 1; i < Function.FunctionName.Length; i++ )
				{
					bool currIsUpper = Char.IsUpper( Function.FunctionName, i );
					if( currIsUpper && !lastIsUpper && Function.FunctionName[ i - 1 ] != ' ' )
					{
						title.Append( " " );
					}
					lastIsUpper = currIsUpper;
					title.Append( Function.FunctionName[ i ] );
					SetTitleText( title.ToString() );
				}
			}
			else
			{
				SetTitleText( Function.FunctionName );
			}
			m_tooltipText = Function.Description;
			if( m_functionGraph == null )
			{
				m_functionGraph = new ParentGraph();
				m_functionGraph.ParentWindow = ContainerGraph.ParentWindow;
			}

			if( string.IsNullOrEmpty( m_functionGUID ) )
			{
				m_functionGUID = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_function ) );
			}

			m_functionGraphId = Mathf.Max( m_functionGraphId, ContainerGraph.ParentWindow.GraphCount );
			ContainerGraph.ParentWindow.GraphCount = m_functionGraphId + 1;
			m_functionGraph.SetGraphId( m_functionGraphId );

			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			AmplifyShaderEditorWindow.LoadFromMeta( ref m_functionGraph, ContainerGraph.ParentWindow.ContextMenuInstance, ContainerGraph.ParentWindow.CurrentVersionInfo, Function.FunctionInfo );
			//m_functionCheckSum = LastLine( m_function.FunctionInfo );
			m_functionCheckSum = AssetDatabase.GetAssetDependencyHash( AssetDatabase.GetAssetPath( m_function ) ).ToString();
			List<PropertyNode> propertyList = UIUtils.PropertyNodesList();
			m_allFunctionInputs = UIUtils.FunctionInputList();
			m_allFunctionOutputs = UIUtils.FunctionOutputList();

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			m_allFunctionInputs.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
			m_allFunctionOutputs.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

			int inputCount = m_allFunctionInputs.Count;
			for( int i = 0; i < inputCount; i++ )
			{
				if( m_refreshIdsRequired )
				{
					AddInputPort( m_allFunctionInputs[ i ].SelectedInputType, false, m_allFunctionInputs[ i ].InputName );
				}
				else
				{
					AddInputPort( m_allFunctionInputs[ i ].SelectedInputType, false, m_allFunctionInputs[ i ].InputName, -1, MasterNodePortCategory.Fragment, m_allFunctionInputs[ i ].UniqueId );
				}
				PortSwitchRestriction( m_inputPorts[ i ] );
				if( m_allFunctionInputs[ i ].InputPorts[ 0 ].IsConnected )
				{
					m_inputPorts[ i ].AutoDrawInternalData = false;
				}
				else
				{
					m_inputPorts[ i ].InternalData = m_allFunctionInputs[ i ].InputPorts[ 0 ].InternalData;
				}
			}

			int outputCount = m_allFunctionOutputs.Count;
			FunctionOutput first = null;
			for( int i = 0; i < outputCount; i++ )
			{
				if( i == 0 )
					first = m_allFunctionOutputs[ i ];

				if( m_allFunctionOutputs[ i ].PreviewNode )
				{
					m_mainPreviewNode = m_allFunctionOutputs[ i ];
				}

				if( m_refreshIdsRequired )
				{
					AddOutputPort( m_allFunctionOutputs[ i ].AutoOutputType, m_allFunctionOutputs[ i ].OutputName );
				}
				else
				{
					AddOutputPort( m_allFunctionOutputs[ i ].AutoOutputType, m_allFunctionOutputs[ i ].OutputName, m_allFunctionOutputs[ i ].UniqueId );
				}
				PortSwitchRestriction( m_outputPorts[ i ] );
			}

			if( m_mainPreviewNode == null )
				m_mainPreviewNode = first;

			//create reordenator to main graph
			bool inside = false;
			if( ContainerGraph.ParentWindow.CustomGraph != null )
				inside = true;

			if( /*hasConnectedProperties*/propertyList.Count > 0 )
			{
				m_reordenator = ScriptableObject.CreateInstance<ReordenatorNode>();
				m_reordenator.Init( "_" + Function.FunctionName, Function.FunctionName, propertyList, false );
				m_reordenator.OrderIndex = m_orderIndex;
				m_reordenator.HeaderTitle = Function.FunctionName;
				m_reordenator.IsInside = inside;
			}

			if( m_reordenator != null )
			{
				cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = null;
				UIUtils.RegisterPropertyNode( m_reordenator );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			m_textLabelWidth = 100;

			UIUtils.RegisterFunctionNode( this );

			m_previewShaderGUID = "aca70c900c50c004e8ef0b47c4fac4d4";
			m_useInternalPortData = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			if( !m_initialized )
				return;

			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( !m_allFunctionInputs[ i ].InputPorts[ 0 ].IsConnected || m_inputPorts[ i ].IsConnected )
				{
					m_allFunctionInputs[ i ].m_ignoreConnection = true;
					m_allFunctionInputs[ i ].InputPorts[ 0 ].PreparePortCacheID();
					m_allFunctionInputs[ i ].PreviewMaterial.SetTexture( m_allFunctionInputs[ i ].InputPorts[ 0 ].CachedPropertyId, m_inputPorts[ i ].InputPreviewTexture );
				}
				else
				{
					m_allFunctionInputs[ i ].m_ignoreConnection = false;
				}
			}

			if( m_mainPreviewNode != null )
			{
				if( m_drawPreviewAsSphere != m_mainPreviewNode.SpherePreview )
				{
					m_drawPreviewAsSphere = m_mainPreviewNode.SpherePreview;
					OnNodeChange();
				}
			}
		}

		public override void RenderNodePreview()
		{
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
			if( m_functionGraph != null )
			{
				for( int i = 0; i < m_functionGraph.AllNodes.Count; i++ )
				{
					ParentNode node = m_functionGraph.AllNodes[ i ];
					if( node != null )
					{
						node.RenderNodePreview();
					}
				}
			}
			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			SetPreviewInputs();

			int count = m_outputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				m_outputPorts[ i ].OutputPreviewTexture = m_allFunctionOutputs[ i ].PreviewTexture;
			}
		}

		public override RenderTexture PreviewTexture
		{
			get
			{
				if( m_mainPreviewNode != null )
					return m_mainPreviewNode.PreviewTexture;
				else
					return base.PreviewTexture;
			}
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();
			if( Function == null )
				return;

			if( ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface != null )
			{
				for( int i = 0; i < Function.AdditionalIncludes.IncludeList.Count; i++ )
				{
					ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalIncludes.OutsideList.Add( Function.AdditionalIncludes.IncludeList[ i ] );
					m_includes.Add( Function.AdditionalIncludes.IncludeList[ i ] );
				}

				for( int i = 0; i < Function.AdditionalPragmas.PragmaList.Count; i++ )
				{
					ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalPragmas.OutsideList.Add( Function.AdditionalPragmas.PragmaList[ i ] );
					m_pragmas.Add( Function.AdditionalPragmas.PragmaList[ i ] );
				}
			}

			if( m_refreshIdsRequired )
			{
				m_refreshIdsRequired = false;
				int inputCount = m_inputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					m_inputPorts[ i ].ChangePortId( m_allFunctionInputs[ i ].UniqueId );
				}

				int outputCount = m_outputPorts.Count;
				for( int i = 0; i < outputCount; i++ )
				{
					m_outputPorts[ i ].ChangePortId( m_allFunctionOutputs[ i ].UniqueId );
				}
			}
		}

		public void PortSwitchRestriction( WirePort port )
		{
			switch( port.DataType )
			{
				case WirePortDataType.OBJECT:
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					port.CreatePortRestrictions( WirePortDataType.FLOAT, WirePortDataType.FLOAT2, WirePortDataType.FLOAT3, WirePortDataType.FLOAT4, WirePortDataType.COLOR, WirePortDataType.INT, WirePortDataType.OBJECT );
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					port.CreatePortRestrictions( WirePortDataType.FLOAT3x3, WirePortDataType.FLOAT4x4, WirePortDataType.OBJECT );
				}
				break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				{
					port.CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );
				}
				break;
				default:
				break;
			}
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			m_outsideGraph = cachedGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			for( int i = 0; i < m_allFunctionOutputs.Count; i++ )
			{
				m_allFunctionOutputs[ i ].PropagateNodeData( nodeData, ref dataCollector );
			}

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			base.PropagateNodeData( nodeData, ref dataCollector );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterFunctionNode( this );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			AmplifyShaderFunction function = obj as AmplifyShaderFunction;
			CommonInit( function, UniqueId );
			RefreshExternalReferences();
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			FunctionInput functionInput = m_refreshIdsRequired ? m_allFunctionInputs[ portId ] : GetFunctionInputByUniqueId( portId );
			if( functionInput.AutoCast )
			{
				InputPort inputPort = m_refreshIdsRequired ? m_inputPorts[ portId ] : GetInputPortByUniqueId( portId );
				inputPort.MatchPortToConnection();

				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
				functionInput.ChangeOutputType( inputPort.DataType, false );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			for( int i = 0; i < m_allFunctionOutputs.Count; i++ )
			{
				m_outputPorts[ i ].ChangeType( m_allFunctionOutputs[ i ].InputPorts[ 0 ].DataType, false );
			}
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			FunctionInput functionInput = m_refreshIdsRequired ? m_allFunctionInputs[ inputPortId ] : GetFunctionInputByUniqueId( inputPortId );
			if( functionInput.AutoCast )
			{
				InputPort inputPort = m_refreshIdsRequired ? m_inputPorts[ inputPortId ] : GetInputPortByUniqueId( inputPortId );
				inputPort.MatchPortToConnection();

				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
				functionInput.ChangeOutputType( inputPort.DataType, false );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			for( int i = 0; i < m_allFunctionOutputs.Count; i++ )
			{
				m_outputPorts[ i ].ChangeType( m_allFunctionOutputs[ i ].InputPorts[ 0 ].DataType, false );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			if( Function == null )
				return;

			if( Function.Description.Length > 0 )
				NodeUtils.DrawPropertyGroup( ref m_parametersFoldout, "Parameters", DrawDescription );
		}

		private void DrawDescription()
		{
			EditorGUILayout.HelpBox( Function.Description, MessageType.Info );
		}

		public override void Destroy()
		{
			m_mainPreviewNode = null;

			base.Destroy();

			if( ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface != null )
			{
				for( int i = 0; i < m_includes.Count; i++ )
				{
					if( ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalIncludes.OutsideList.Contains( m_includes[ i ] ) )
					{
						ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalIncludes.OutsideList.Remove( m_includes[ i ] );
					}
				}

				for( int i = 0; i < m_pragmas.Count; i++ )
				{
					if( ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalPragmas.OutsideList.Contains( m_pragmas[ i ] ) )
					{
						ContainerGraph.ParentWindow.OutsideGraph.CurrentStandardSurface.AdditionalPragmas.OutsideList.Remove( m_pragmas[ i ] );
					}
				}
			}

			if( m_reordenator != null )
			{
				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = null;
				UIUtils.UnregisterPropertyNode( m_reordenator );
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

				m_reordenator.Destroy();
				m_reordenator = null;
			}

			UIUtils.UnregisterFunctionNode( this );

			ParentGraph cachedGraph2 = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			if( m_allFunctionInputs != null )
				m_allFunctionInputs.Clear();
			m_allFunctionInputs = null;

			if( m_allFunctionOutputs != null )
				m_allFunctionOutputs.Clear();
			m_allFunctionOutputs = null;

			if( m_functionGraph != null )
				m_functionGraph.Destroy();
			m_functionGraph = null;

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph2;
			m_function = null;

			m_allFunctionOutputsDict.Clear();
			m_allFunctionOutputsDict = null;

			m_allFunctionInputsDict.Clear();
			m_allFunctionInputsDict = null;
		}



		public override void Draw( DrawInfo drawInfo )
		{
			//CheckForChanges();
			CheckForChangesRecursively();

			if( !m_initialGraphDraw && drawInfo.CurrentEventType == EventType.Repaint )
			{
				m_initialGraphDraw = true;
				ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
				ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;
				if( m_functionGraph != null )
				{
					for( int i = 0; i < m_functionGraph.AllNodes.Count; i++ )
					{
						ParentNode node = m_functionGraph.AllNodes[ i ];
						if( node != null )
						{
							node.OnNodeLayout( drawInfo );
						}
					}
				}
				ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			}

			base.Draw( drawInfo );
		}

		public bool CheckForChanges( bool forceCheck = false, bool forceChange = false )
		{
			if( ( ContainerGraph.ParentWindow.CheckFunctions || forceCheck || forceChange ) && m_function != null )
			{
				//string newCheckSum = LastLine( m_function.FunctionInfo );
				string newCheckSum = AssetDatabase.GetAssetDependencyHash( AssetDatabase.GetAssetPath( m_function ) ).ToString();
				if( !m_functionCheckSum.Equals( newCheckSum ) || forceChange )
				{
					m_functionCheckSum = newCheckSum;
					ContainerGraph.OnDuplicateEvent += DuplicateMe;
					return true;
				}
			}
			return false;
		}

		public bool CheckForChangesRecursively()
		{
			if( m_functionGraph == null )
				return false;

			bool result = false;
			for( int i = 0; i < m_functionGraph.FunctionNodes.NodesList.Count; i++ )
			{
				if( m_functionGraph.FunctionNodes.NodesList[ i ].CheckForChangesRecursively() )
					result = true;
			}
			if( CheckForChanges( false, result ) )
				result = true;

			return result;
		}

		public void DuplicateMe()
		{
			bool previewOpen = m_showPreview;
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = null;
			if( ContainerGraph.ParentWindow.CurrentGraph.CurrentStandardSurface != null )
				ContainerGraph.ParentWindow.CurrentGraph.CurrentStandardSurface.InvalidateMaterialPropertyCount();
			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			ParentNode newNode = ContainerGraph.CreateNode( m_function, false, Vec2Position );
			newNode.RefreshExternalReferences();
			newNode.ShowPreview = previewOpen;
			if( ( newNode as FunctionNode ).m_reordenator && m_reordenator )
				( newNode as FunctionNode ).m_reordenator.OrderIndex = m_reordenator.OrderIndex;

			for( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if( m_outputPorts[ i ].IsConnected )
				{
					OutputPort newOutputPort = newNode.GetOutputPortByUniqueId( m_outputPorts[ i ].PortId );
					if( newNode.OutputPorts != null && newOutputPort != null )
					{
						for( int j = m_outputPorts[ i ].ExternalReferences.Count - 1; j >= 0; j-- )
						{
							ContainerGraph.CreateConnection( m_outputPorts[ i ].ExternalReferences[ j ].NodeId, m_outputPorts[ i ].ExternalReferences[ j ].PortId, newOutputPort.NodeId, newOutputPort.PortId );
						}
					}
				}
				//else
				//{
				//if( newNode.OutputPorts != null && newNode.OutputPorts[ i ] != null )
				//{
				//    ContainerGraph.DeleteConnection( false, newNode.UniqueId, newNode.OutputPorts[ i ].PortId, false, false, false );
				//}
				//}
			}

			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					InputPort newInputPort = newNode.GetInputPortByUniqueId( m_inputPorts[ i ].PortId );
					if( newNode.InputPorts != null && newInputPort != null )
					{
						ContainerGraph.CreateConnection( newInputPort.NodeId, newInputPort.PortId, m_inputPorts[ i ].ExternalReferences[ 0 ].NodeId, m_inputPorts[ i ].ExternalReferences[ 0 ].PortId );
					}
				}
			}

			ContainerGraph.OnDuplicateEvent -= DuplicateMe;

			if( Selected )
			{
				ContainerGraph.DeselectNode( this );
				ContainerGraph.SelectNode( newNode, true, false );
			}

			ContainerGraph.DestroyNode( this, false );
		}

		private FunctionOutput GetFunctionOutputByUniqueId( int uniqueId )
		{
			int listCount = m_allFunctionOutputs.Count;
			if( m_allFunctionOutputsDict.Count != m_allFunctionOutputs.Count )
			{
				m_allFunctionOutputsDict.Clear();
				for( int i = 0; i < listCount; i++ )
				{
					m_allFunctionOutputsDict.Add( m_allFunctionOutputs[ i ].UniqueId, m_allFunctionOutputs[ i ] );
				}
			}

			if( m_allFunctionOutputsDict.ContainsKey( uniqueId ) )
				return m_allFunctionOutputsDict[ uniqueId ];

			return null;
		}

		private FunctionInput GetFunctionInputByUniqueId( int uniqueId )
		{
			int listCount = m_allFunctionInputs.Count;
			if( m_allFunctionInputsDict.Count != m_allFunctionInputs.Count )
			{
				m_allFunctionInputsDict.Clear();
				for( int i = 0; i < listCount; i++ )
				{
					m_allFunctionInputsDict.Add( m_allFunctionInputs[ i ].UniqueId, m_allFunctionInputs[ i ] );
				}
			}

			if( m_allFunctionInputsDict.ContainsKey( uniqueId ) )
				return m_allFunctionInputsDict[ uniqueId ];

			return null;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			OutputPort outputPort = GetOutputPortByUniqueId( outputId );
			FunctionOutput functionOutput = GetFunctionOutputByUniqueId( outputId );

			if( outputPort.IsLocalValue )
				return outputPort.LocalValue;

			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			m_outsideGraph = cachedGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_functionGraph;

			if( m_reordenator != null && m_reordenator.RecursiveCount() > 0 && m_reordenator.HasTitle )
			{
				dataCollector.AddToProperties( UniqueId, "[Header(" + m_reordenator.HeaderTitle.Replace( "-", " " ) + ")]", m_reordenator.OrderIndex );
			}
			string result = string.Empty;
			for( int i = 0; i < m_allFunctionInputs.Count; i++ )
			{
				if( !m_allFunctionInputs[ i ].InputPorts[ 0 ].IsConnected || m_inputPorts[ i ].IsConnected )
					m_allFunctionInputs[ i ].OnPortGeneration += FunctionNodeOnPortGeneration;
			}

			result += functionOutput.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			for( int i = 0; i < m_allFunctionInputs.Count; i++ )
			{
				if( !m_allFunctionInputs[ i ].InputPorts[ 0 ].IsConnected || m_inputPorts[ i ].IsConnected )
					m_allFunctionInputs[ i ].OnPortGeneration -= FunctionNodeOnPortGeneration;
			}

			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;

			if( outputPort.ConnectionCount > 1 )
				RegisterLocalVariable( outputId, result, ref dataCollector );
			else
				outputPort.SetLocalValue( result, dataCollector.PortCategory );

			return outputPort.LocalValue;
		}

		private string FunctionNodeOnPortGeneration( ref MasterNodeDataCollector dataCollector, int index, ParentGraph graph )
		{
			ParentGraph cachedGraph = ContainerGraph.ParentWindow.CustomGraph;
			ContainerGraph.ParentWindow.CustomGraph = m_outsideGraph;
			string result = m_inputPorts[ index ].GeneratePortInstructions( ref dataCollector );
			ContainerGraph.ParentWindow.CustomGraph = cachedGraph;
			return result;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			if( Function != null )
				IOUtils.AddFieldValueToString( ref nodeInfo, m_function.name );
			else
				IOUtils.AddFieldValueToString( ref nodeInfo, m_filename );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_reordenator != null ? m_reordenator.RawOrderIndex : -1 );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_headerTitle );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_functionGraphId );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_functionGUID );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_filename = GetCurrentParam( ref nodeParams );
			m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_headerTitle = GetCurrentParam( ref nodeParams );

			if( UIUtils.CurrentShaderVersion() > 7203 )
			{
				m_functionGraphId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() > 13704 )
			{
				m_functionGUID = GetCurrentParam( ref nodeParams );
			}

			AmplifyShaderFunction loaded = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( AssetDatabase.GUIDToAssetPath( m_functionGUID ) );
			if( loaded != null )
			{
				CommonInit( loaded, UniqueId );
			}
			else
			{
				string[] guids = AssetDatabase.FindAssets( "t:AmplifyShaderFunction " + m_filename );
				if( guids.Length > 0 )
				{
					loaded = AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( AssetDatabase.GUIDToAssetPath( guids[ 0 ] ) );
					if( loaded != null )
					{
						CommonInit( loaded, UniqueId );
					}
					else
					{
						SetTitleText( "ERROR" );
					}
				}
				else
				{
					SetTitleText( "Missing Function" );
				}
			}
		}

		public override void ReadOutputDataFromString( ref string[] nodeParams )
		{
			if( Function == null )
				return;

			base.ReadOutputDataFromString( ref nodeParams );

			ConfigureInputportsAfterRead();
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if( Function == null )
				return;

			ContainerGraph.DeSelectAll();
			this.Selected = true;

			ContainerGraph.ParentWindow.OnLeftMouseUp();
			AmplifyShaderEditorWindow.LoadShaderFunctionToASE( Function, true );
			this.Selected = false;
		}

		private void ConfigureInputportsAfterRead()
		{
			if( InputPorts != null )
			{
				int inputCount = InputPorts.Count;
				for( int i = 0; i < inputCount; i++ )
				{
					InputPorts[ i ].ChangeProperties( m_allFunctionInputs[ i ].InputName, m_allFunctionInputs[ i ].SelectedInputType, false );
				}
			}

			if( OutputPorts != null )
			{
				int outputCount = OutputPorts.Count;
				for( int i = 0; i < outputCount; i++ )
				{
					OutputPorts[ i ].ChangeProperties( m_allFunctionOutputs[ i ].OutputName, m_allFunctionOutputs[ i ].AutoOutputType, false );
				}
			}
		}

		public bool HasProperties { get { return m_reordenator != null; } }

		public ParentGraph FunctionGraph
		{
			get
			{
				return m_functionGraph;
			}

			set
			{
				m_functionGraph = value;
			}
		}

		public AmplifyShaderFunction Function
		{
			get { return m_function; }
			set { m_function = value; }
		}

		public override void SetContainerGraph( ParentGraph newgraph )
		{
			base.SetContainerGraph( newgraph );
			if( m_functionGraph == null )
				return;
			for( int i = 0; i < m_functionGraph.AllNodes.Count; i++ )
			{
				m_functionGraph.AllNodes[ i ].SetContainerGraph( m_functionGraph );
			}
		}
	}
}
