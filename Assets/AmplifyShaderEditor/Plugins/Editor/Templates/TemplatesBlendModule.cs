// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
    [Serializable]
    public sealed class TemplatesBlendModule : TemplateModuleParent
    {
        private const string BlendModeStr = " Blend Mode";

        private const string BlendModesRGBStr = "Blend RGB";
        private const string BlendModesAlphaStr = "Blend Alpha";

        private const string BlendOpsRGBStr = "Blend Op RGB";
        private const string BlendOpsAlphaStr = "Blend Op Alpha";

        private const string SourceFactorStr = "Src";
        private const string DstFactorStr = "Dst";

        private const string SingleBlendFactorStr = "Blend {0} {1}";
        private const string SeparateBlendFactorStr = "Blend {0} {1} , {2} {3}";

        private const string SingleBlendOpStr = "BlendOp {0}";
        private const string SeparateBlendOpStr = "BlendOp {0} , {1}";

        private string[] m_commonBlendTypesArr;
        private List<CommonBlendTypes> m_commonBlendTypes = new List<CommonBlendTypes>
        {
            new CommonBlendTypes("<OFF>",               AvailableBlendFactor.Zero,              AvailableBlendFactor.Zero ),
            new CommonBlendTypes("Custom",              AvailableBlendFactor.Zero,              AvailableBlendFactor.Zero ) ,
            new CommonBlendTypes("Alpha Blend",         AvailableBlendFactor.SrcAlpha,          AvailableBlendFactor.OneMinusSrcAlpha ) ,
            new CommonBlendTypes("Premultiplied",      AvailableBlendFactor.One,               AvailableBlendFactor.OneMinusSrcAlpha ),
            new CommonBlendTypes("Additive",            AvailableBlendFactor.One,               AvailableBlendFactor.One ),
            new CommonBlendTypes("Soft Additive",       AvailableBlendFactor.OneMinusDstColor,  AvailableBlendFactor.One ),
            new CommonBlendTypes("Multiplicative",      AvailableBlendFactor.DstColor,          AvailableBlendFactor.Zero ),
            new CommonBlendTypes("2x Multiplicative",   AvailableBlendFactor.DstColor,          AvailableBlendFactor.SrcColor )
        };

        [SerializeField]
        private bool m_blendModeEnabled = false;

        // Blend Factor
        // RGB
        [SerializeField]
        private int m_currentRGBIndex = 0;


        [SerializeField]
        private AvailableBlendFactor m_sourceFactorRGB = AvailableBlendFactor.Zero;

        [SerializeField]
        private AvailableBlendFactor m_destFactorRGB = AvailableBlendFactor.Zero;

        // Alpha
        [SerializeField]
        private int m_currentAlphaIndex = 0;

        [SerializeField]
        private AvailableBlendFactor m_sourceFactorAlpha = AvailableBlendFactor.Zero;

        [SerializeField]
        private AvailableBlendFactor m_destFactorAlpha = AvailableBlendFactor.Zero;

        //Blend Ops
        [SerializeField]
        private bool m_blendOpEnabled = false;

        [SerializeField]
        private AvailableBlendOps m_blendOpRGB = AvailableBlendOps.OFF;

        [SerializeField]
        private AvailableBlendOps m_blendOpAlpha = AvailableBlendOps.OFF;

        public TemplatesBlendModule() : base( "Blend Mode and Ops" )
        {
            m_commonBlendTypesArr = new string[ m_commonBlendTypes.Count ];
            for( int i = 0; i < m_commonBlendTypesArr.Length; i++ )
            {
                m_commonBlendTypesArr[ i ] = m_commonBlendTypes[ i ].Name;
            }
        }

        public void ConfigureFromTemplateData( TemplateBlendData blendData )
        {
            if( blendData.ValidBlendMode )
            {
                m_blendModeEnabled = true;

                m_sourceFactorRGB = blendData.SourceFactorRGB;
                m_destFactorRGB = blendData.DestFactorRGB;
                m_sourceFactorAlpha = blendData.SourceFactorAlpha;
                m_destFactorAlpha = blendData.DestFactorAlpha;
                if( blendData.SeparateBlendFactors )
                {
                    CheckRGBIndex();
                    CheckAlphaIndex();
                }
                else
                {
                    CheckRGBIndex();
                    m_currentAlphaIndex = 0;
                }
            }
            else
            {
                m_blendModeEnabled = false;
            }

            if( blendData.ValidBlendOp )
            {
                m_blendOpEnabled = true;
                m_blendOpRGB = blendData.BlendOpRGB;
                m_blendOpAlpha = blendData.BlendOpAlpha;
            }
            else
            {
                m_blendOpEnabled = false;
            }
        }

        public override void ShowUnreadableDataMessage()
        {
            bool blendModeIsVisible = EditorVariablesManager.ExpandedBlendModeModule.Value;
            NodeUtils.DrawPropertyGroup( ref blendModeIsVisible, BlendModeStr, base.ShowUnreadableDataMessage );
            EditorVariablesManager.ExpandedBlendModeModule.Value = blendModeIsVisible;
        }

        public override void Draw( UndoParentNode owner )
        {
            bool blendModeIsVisible = EditorVariablesManager.ExpandedBlendModeModule.Value;
            NodeUtils.DrawPropertyGroup( ref blendModeIsVisible, BlendModeStr, () =>
            {
                if( m_blendModeEnabled )
                {
                    // RGB
                    EditorGUI.BeginChangeCheck();
                    m_currentRGBIndex = owner.EditorGUILayoutPopup( BlendModesRGBStr, m_currentRGBIndex, m_commonBlendTypesArr );
                    if( EditorGUI.EndChangeCheck() )
                    {
                        if( m_currentRGBIndex > 1 )
                        {
                            m_sourceFactorRGB = m_commonBlendTypes[ m_currentRGBIndex ].SourceFactor;
                            m_destFactorRGB = m_commonBlendTypes[ m_currentRGBIndex ].DestFactor;
                        }
                    }
                    EditorGUI.BeginDisabledGroup( m_currentRGBIndex == 0 );

                    EditorGUI.BeginChangeCheck();
                    float cached = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40;

                    EditorGUILayout.BeginHorizontal();
                    m_sourceFactorRGB = (AvailableBlendFactor)owner.EditorGUILayoutEnumPopup( SourceFactorStr, m_sourceFactorRGB );
                    EditorGUI.indentLevel--;
                    EditorGUIUtility.labelWidth = 25;
                    m_destFactorRGB = (AvailableBlendFactor)owner.EditorGUILayoutEnumPopup( DstFactorStr, m_destFactorRGB );
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndHorizontal();

                    EditorGUIUtility.labelWidth = cached;
                    if( EditorGUI.EndChangeCheck() )
                    {
                        CheckRGBIndex();
                    }

                    EditorGUI.EndDisabledGroup();
                    // Alpha
                    EditorGUILayout.Separator();

                    EditorGUI.BeginChangeCheck();
                    m_currentAlphaIndex = owner.EditorGUILayoutPopup( BlendModesAlphaStr, m_currentAlphaIndex, m_commonBlendTypesArr );
                    if( EditorGUI.EndChangeCheck() )
                    {
                        if( m_currentAlphaIndex > 0 )
                        {
                            m_sourceFactorAlpha = m_commonBlendTypes[ m_currentAlphaIndex ].SourceFactor;
                            m_destFactorAlpha = m_commonBlendTypes[ m_currentAlphaIndex ].DestFactor;
                        }
                    }
                    EditorGUI.BeginDisabledGroup( m_currentAlphaIndex == 0 );

                    EditorGUI.BeginChangeCheck();
                    cached = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40;
                    EditorGUILayout.BeginHorizontal();
                    m_sourceFactorAlpha = (AvailableBlendFactor)owner.EditorGUILayoutEnumPopup( SourceFactorStr, m_sourceFactorAlpha );
                    EditorGUI.indentLevel--;
                    EditorGUIUtility.labelWidth = 25;
                    m_destFactorAlpha = (AvailableBlendFactor)owner.EditorGUILayoutEnumPopup( DstFactorStr, m_destFactorAlpha );
                    EditorGUI.indentLevel++;
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = cached;

                    if( EditorGUI.EndChangeCheck() )
                    {
                        CheckAlphaIndex();
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Separator();
                }

                if( m_blendOpEnabled )
                {
                    m_blendOpRGB = (AvailableBlendOps)owner.EditorGUILayoutEnumPopup( BlendOpsRGBStr, m_blendOpRGB );
                    EditorGUILayout.Separator();
                    m_blendOpAlpha = (AvailableBlendOps)owner.EditorGUILayoutEnumPopup( BlendOpsAlphaStr, m_blendOpAlpha );
                }
            } );

            EditorVariablesManager.ExpandedBlendModeModule.Value = blendModeIsVisible;
        }

        void CheckRGBIndex()
        {
            int count = m_commonBlendTypes.Count;
            m_currentRGBIndex = 1;
            for( int i = 1; i < count; i++ )
            {
                if( m_commonBlendTypes[ i ].SourceFactor == m_sourceFactorRGB && m_commonBlendTypes[ i ].DestFactor == m_destFactorRGB )
                {
                    m_currentRGBIndex = i;
                    return;
                }
            }

        }

        void CheckAlphaIndex()
        {
            int count = m_commonBlendTypes.Count;
            m_currentAlphaIndex = 1;
            for( int i = 1; i < count; i++ )
            {
                if( m_commonBlendTypes[ i ].SourceFactor == m_sourceFactorAlpha && m_commonBlendTypes[ i ].DestFactor == m_destFactorAlpha )
                {
                    m_currentAlphaIndex = i;
                    if( m_currentAlphaIndex > 0 && m_currentRGBIndex == 0 )
                        m_currentRGBIndex = 1;
                    return;
                }
            }

            if( m_currentAlphaIndex > 0 && m_currentRGBIndex == 0 )
                m_currentRGBIndex = 1;
        }

        public void ReadBlendModeFromString( ref uint index, ref string[] nodeParams )
        {
            m_currentRGBIndex = Convert.ToInt32( nodeParams[ index++ ] );
            m_sourceFactorRGB = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
            m_destFactorRGB = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );

            m_currentAlphaIndex = Convert.ToInt32( nodeParams[ index++ ] );
            m_sourceFactorAlpha = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
            m_destFactorAlpha = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
        }

        public void ReadBlendOpFromString( ref uint index, ref string[] nodeParams )
        {
            m_blendOpRGB = (AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), nodeParams[ index++ ] );
            m_blendOpAlpha = (AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), nodeParams[ index++ ] );
            m_blendOpEnabled = ( m_blendOpRGB != AvailableBlendOps.OFF );
        }

        public void WriteBlendModeToString( ref string nodeInfo )
        {
            IOUtils.AddFieldValueToString( ref nodeInfo, m_currentRGBIndex );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_sourceFactorRGB );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_destFactorRGB );

            IOUtils.AddFieldValueToString( ref nodeInfo, m_currentAlphaIndex );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_sourceFactorAlpha );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_destFactorAlpha );
        }

        public void WriteBlendOpToString( ref string nodeInfo )
        {
            IOUtils.AddFieldValueToString( ref nodeInfo, m_blendOpRGB );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_blendOpAlpha );
        }

        public string CurrentBlendFactorSingle
        {
            get
            {
                return ( m_currentRGBIndex > 0 ) ? string.Format( SingleBlendFactorStr, m_sourceFactorRGB, m_destFactorRGB ) : string.Empty;
            }
        }

        public string CurrentBlendFactorSeparate
        {
            get
            {
                return string.Format( SeparateBlendFactorStr, ( m_currentRGBIndex > 0 ? m_sourceFactorRGB : AvailableBlendFactor.One ), ( m_currentRGBIndex > 0 ? m_destFactorRGB : AvailableBlendFactor.Zero ), m_sourceFactorAlpha, m_destFactorAlpha );
            }
        }
        public string CurrentBlendFactor
        {
            get
            {
                return ( ( m_currentAlphaIndex > 0 ) ? CurrentBlendFactorSeparate : CurrentBlendFactorSingle );
            }
        }


        public string CurrentBlendOpSingle
        {
            get
            {
                return ( m_blendOpRGB != AvailableBlendOps.OFF ) ? string.Format( SingleBlendOpStr, m_blendOpRGB ) : string.Empty;
            }
        }

        public string CurrentBlendOpSeparate
        {
            get
            {
                return string.Format( SeparateBlendOpStr, ( ( m_currentRGBIndex > 0 && m_blendOpRGB != AvailableBlendOps.OFF ) ? m_blendOpRGB : AvailableBlendOps.Add ), m_blendOpAlpha );
            }
        }

        public string CurrentBlendOp { get { return ( ( m_blendOpAlpha != AvailableBlendOps.OFF ) ? CurrentBlendOpSeparate : CurrentBlendOpSingle ); } }
        public bool Active { get { return m_blendModeEnabled && ( m_currentRGBIndex > 0 || m_currentAlphaIndex > 0 ); } }
    }
}
