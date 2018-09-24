using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class StencilBufferOpHelper
	{
        public static readonly string[] StencilComparisonValues = 
        {
            "<Default>",
            "Greater" ,
            "GEqual" ,
            "Less" ,
            "LEqual" ,
            "Equal" ,
            "NotEqual" ,
            "Always" ,
            "Never"
        };

        public static readonly string[] StencilComparisonLabels =
        {
            "<Default>",
            "Greater" ,
            "Greater or Equal" ,
            "Less" ,
            "Less or Equal" ,
            "Equal" ,
            "Not Equal" ,
            "Always" ,
            "Never"
        };


        public static readonly string[] StencilOpsValues = 
        {
            "<Default>",
            "Keep",
            "Zero",
            "Replace",
            "IncrSat",
            "DecrSat",
            "Invert",
            "IncrWrap",
            "DecrWrap"
        };

        public static readonly string[] StencilOpsLabels =
        {
            "<Default>",
            "Keep",
            "Zero",
            "Replace",
            "IncrSat",
            "DecrSat",
            "Invert",
            "IncrWrap",
            "DecrWrap"
        };


        private const string FoldoutLabelStr = " Stencil Buffer";
		private GUIContent ReferenceValueContent = new GUIContent( "Reference", "The value to be compared against (if Comparison is anything else than always) and/or the value to be written to the buffer (if either Pass, Fail or ZFail is set to replace)" );
		private GUIContent ReadMaskContent = new GUIContent( "Read Mask", "An 8 bit mask as an 0-255 integer, used when comparing the reference value with the contents of the buffer (referenceValue & readMask) comparisonFunction (stencilBufferValue & readMask)" );
		private GUIContent WriteMaskContent = new GUIContent( "Write Mask", "An 8 bit mask as an 0-255 integer, used when writing to the buffer" );
		private const string ComparisonStr = "Comparison";
		private const string PassStr = "Pass";
		private const string FailStr = "Fail";
		private const string ZFailStr = "ZFail";

		private const string ComparisonFrontStr = "Comparison Front";
		private const string PassFrontStr = "Pass Front";
		private const string FailFrontStr = "Fail Front";
		private const string ZFailFrontStr = "ZFail Front";

		private const string ComparisonBackStr = "Comparison Back";
		private const string PassBackStr = "Pass Back";
		private const string FailBackStr = "Fail Back";
		private const string ZFailBackStr = "ZFail Back";

		
		[SerializeField]
		private bool m_active;

		[SerializeField]
		private int m_refValue;

		// Read Mask
		private const int ReadMaskDefaultValue = 255;
		[SerializeField]
		private int m_readMask = ReadMaskDefaultValue;

		//Write Mask
		private const int WriteMaskDefaultValue = 255;
		[SerializeField]
		private int m_writeMask = WriteMaskDefaultValue;

		//Comparison Function
		private const int ComparisonDefaultValue = 0;
		[SerializeField]
		private int m_comparisonFunctionIdx = ComparisonDefaultValue;

		[SerializeField]
		private int m_comparisonFunctionBackIdx = ComparisonDefaultValue;

		//Pass Stencil Op
		private const int PassStencilOpDefaultValue = 0;
		[SerializeField]
		private int m_passStencilOpIdx = PassStencilOpDefaultValue;

		[SerializeField]
		private int m_passStencilOpBackIdx = PassStencilOpDefaultValue;

		//Fail Stencil Op 
		private const int FailStencilOpDefaultValue = 0;
		[SerializeField]
		private int m_failStencilOpIdx = FailStencilOpDefaultValue;
		[SerializeField]
		private int m_failStencilOpBackIdx = FailStencilOpDefaultValue;

		//ZFail Stencil Op
		private const int ZFailStencilOpDefaultValue = 0;
		[SerializeField]
		private int m_zFailStencilOpIdx = ZFailStencilOpDefaultValue;
		[SerializeField]
		private int m_zFailStencilOpBackIdx = ZFailStencilOpDefaultValue;

		public string CreateStencilOp( UndoParentNode owner )
		{
			string result = "\t\tStencil\n\t\t{\n";
			result += string.Format( "\t\t\tRef {0}\n", m_refValue );
			if ( m_readMask != ReadMaskDefaultValue )
			{
				result += string.Format( "\t\t\tReadMask {0}\n", m_readMask );
			}

			if ( m_writeMask != WriteMaskDefaultValue )
			{
				result += string.Format( "\t\t\tWriteMask {0}\n", m_writeMask );
			}

			if( ( owner as StandardSurfaceOutputNode ).CurrentCullMode == CullMode.Off &&
               ( m_comparisonFunctionBackIdx != ComparisonDefaultValue ||
                m_passStencilOpBackIdx != PassStencilOpDefaultValue ||
                m_failStencilOpBackIdx != FailStencilOpDefaultValue ||
                m_zFailStencilOpBackIdx != ZFailStencilOpDefaultValue ) )
			{
				if( m_comparisonFunctionIdx != ComparisonDefaultValue )
					result += string.Format( "\t\t\tCompFront {0}\n", StencilComparisonValues[ m_comparisonFunctionIdx ] );
				if( m_passStencilOpIdx != PassStencilOpDefaultValue )
					result += string.Format( "\t\t\tPassFront {0}\n", StencilOpsValues[ m_passStencilOpIdx ] );
				if( m_failStencilOpIdx != FailStencilOpDefaultValue )
					result += string.Format( "\t\t\tFailFront {0}\n", StencilOpsValues[ m_failStencilOpIdx ] );
				if( m_zFailStencilOpIdx != ZFailStencilOpDefaultValue )
					result += string.Format( "\t\t\tZFailFront {0}\n", StencilOpsValues[ m_zFailStencilOpIdx ] );

				if( m_comparisonFunctionBackIdx != ComparisonDefaultValue )
					result += string.Format( "\t\t\tCompBack {0}\n", StencilComparisonValues[ m_comparisonFunctionBackIdx ] );
				if( m_passStencilOpBackIdx != PassStencilOpDefaultValue )
					result += string.Format( "\t\t\tPassBack {0}\n", StencilOpsValues[ m_passStencilOpBackIdx ] );
				if( m_failStencilOpBackIdx != FailStencilOpDefaultValue )
					result += string.Format( "\t\t\tFailBack {0}\n", StencilOpsValues[ m_failStencilOpBackIdx ] );
				if( m_zFailStencilOpBackIdx != ZFailStencilOpDefaultValue )
					result += string.Format( "\t\t\tZFailBack {0}\n", StencilOpsValues[ m_zFailStencilOpBackIdx ] );
			}
			else
			{
				if ( m_comparisonFunctionIdx != ComparisonDefaultValue )
					result += string.Format( "\t\t\tComp {0}\n", StencilComparisonValues[ m_comparisonFunctionIdx ] );
				if ( m_passStencilOpIdx != PassStencilOpDefaultValue )
					result += string.Format( "\t\t\tPass {0}\n", StencilOpsValues[ m_passStencilOpIdx ] );
				if ( m_failStencilOpIdx != FailStencilOpDefaultValue )
					result += string.Format( "\t\t\tFail {0}\n", StencilOpsValues[ m_failStencilOpIdx ] );
				if ( m_zFailStencilOpIdx != ZFailStencilOpDefaultValue )
					result += string.Format( "\t\t\tZFail {0}\n", StencilOpsValues[ m_zFailStencilOpIdx ] );
			}


			result += "\t\t}\n";
			return result;
		}

		public void Draw( UndoParentNode owner )
		{
			bool foldoutValue = EditorVariablesManager.ExpandedStencilOptions.Value;
			NodeUtils.DrawPropertyGroup( owner, ref foldoutValue, ref m_active, FoldoutLabelStr, () =>
			 {
				 m_refValue = owner.EditorGUILayoutIntSlider( ReferenceValueContent, m_refValue, 0, 255 );
				 m_readMask = owner.EditorGUILayoutIntSlider( ReadMaskContent, m_readMask, 0, 255 );
				 m_writeMask = owner.EditorGUILayoutIntSlider( WriteMaskContent, m_writeMask, 0, 255 );
				 if( (owner as StandardSurfaceOutputNode).CurrentCullMode == CullMode.Off )
				 {
					 m_comparisonFunctionIdx = owner.EditorGUILayoutPopup( ComparisonFrontStr, m_comparisonFunctionIdx, StencilComparisonLabels );
					 m_passStencilOpIdx = owner.EditorGUILayoutPopup( PassFrontStr, m_passStencilOpIdx, StencilOpsLabels );
					 m_failStencilOpIdx = owner.EditorGUILayoutPopup( FailFrontStr, m_failStencilOpIdx, StencilOpsLabels );
					 m_zFailStencilOpIdx = owner.EditorGUILayoutPopup( ZFailFrontStr, m_zFailStencilOpIdx, StencilOpsLabels );
					 EditorGUILayout.Separator();
					 m_comparisonFunctionBackIdx = owner.EditorGUILayoutPopup( ComparisonBackStr, m_comparisonFunctionBackIdx, StencilComparisonLabels );
					 m_passStencilOpBackIdx = owner.EditorGUILayoutPopup( PassBackStr, m_passStencilOpBackIdx, StencilOpsLabels );
					 m_failStencilOpBackIdx = owner.EditorGUILayoutPopup( FailBackStr, m_failStencilOpBackIdx, StencilOpsLabels );
					 m_zFailStencilOpBackIdx = owner.EditorGUILayoutPopup( ZFailBackStr, m_zFailStencilOpBackIdx, StencilOpsLabels );
				 } else
				 {
					 m_comparisonFunctionIdx = owner.EditorGUILayoutPopup( ComparisonStr, m_comparisonFunctionIdx, StencilComparisonLabels );
					 m_passStencilOpIdx = owner.EditorGUILayoutPopup( PassStr, m_passStencilOpIdx, StencilOpsLabels );
					 m_failStencilOpIdx = owner.EditorGUILayoutPopup( FailStr, m_failStencilOpIdx, StencilOpsLabels );
					 m_zFailStencilOpIdx = owner.EditorGUILayoutPopup( ZFailStr, m_zFailStencilOpIdx, StencilOpsLabels );
				 }
			 } );
			EditorVariablesManager.ExpandedStencilOptions.Value = foldoutValue;
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_active = Convert.ToBoolean( nodeParams[ index++ ] );
			m_refValue = Convert.ToInt32( nodeParams[ index++ ] );
			m_readMask = Convert.ToInt32( nodeParams[ index++ ] );
			m_writeMask = Convert.ToInt32( nodeParams[ index++ ] );
			m_comparisonFunctionIdx = Convert.ToInt32( nodeParams[ index++ ] );
			m_passStencilOpIdx = Convert.ToInt32( nodeParams[ index++ ] );
			m_failStencilOpIdx = Convert.ToInt32( nodeParams[ index++ ] );
			m_zFailStencilOpIdx = Convert.ToInt32( nodeParams[ index++ ] );

			if( UIUtils.CurrentShaderVersion() > 13203 )
			{
				m_comparisonFunctionBackIdx = Convert.ToInt32( nodeParams[ index++ ] );
				m_passStencilOpBackIdx = Convert.ToInt32( nodeParams[ index++ ] );
				m_failStencilOpBackIdx = Convert.ToInt32( nodeParams[ index++ ] );
				m_zFailStencilOpBackIdx = Convert.ToInt32( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_active );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_refValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_readMask );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_writeMask );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_comparisonFunctionIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_passStencilOpIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_failStencilOpIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zFailStencilOpIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_comparisonFunctionBackIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_passStencilOpBackIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_failStencilOpBackIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zFailStencilOpBackIdx );
		}

		public bool Active
		{
			get { return m_active; }
			set { m_active = value; }
		}
	}
}
