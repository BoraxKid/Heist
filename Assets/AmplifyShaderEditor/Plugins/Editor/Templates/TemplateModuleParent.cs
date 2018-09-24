// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateModuleParent
	{
        private const string UnreadableDataMessagePrefix = "Unreadable data on Module ";
        protected string m_unreadableMessage;

        [SerializeField]
		protected bool m_validData = false;
        public TemplateModuleParent( string moduleName ) { m_unreadableMessage = UnreadableDataMessagePrefix + moduleName; }
        public virtual void Draw( UndoParentNode owner ) { }
		public virtual void ReadFromString( ref uint index, ref string[] nodeParams ) { }
		public virtual void WriteToString( ref string nodeInfo ) { }
		public virtual string GenerateShaderData() { return string.Empty; }
		public virtual void Destroy() { }
		public bool ValidData { get { return m_validData; } }

        public virtual void ShowUnreadableDataMessage()
        {
            EditorGUILayout.HelpBox( m_unreadableMessage, MessageType.Info );
        }
	}
}
