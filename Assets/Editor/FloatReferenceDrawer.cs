using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        Rect contentRect = EditorGUI.PrefixLabel(position, label);

        SerializedProperty useConstantProperty = property.FindPropertyRelative("_useConstant");

        DrawMenu(ref contentRect, useConstantProperty.boolValue, () => SetType(useConstantProperty, true), () => SetType(useConstantProperty, false));

        DrawProperty(contentRect, property, useConstantProperty.boolValue);

        EditorGUI.EndProperty();
    }

    private static SerializedProperty GetSerializedValue(SerializedProperty property, bool useConstant)
    {
        SerializedProperty propertyValue = useConstant ? property.FindPropertyRelative("_constantValue") : property.FindPropertyRelative("_variable");
        return (propertyValue);
    }

    private static void DrawMenu(ref Rect contentPosition, bool useConstant, Action constantAction, Action variableAction)
    {
        Texture2D menuIcon = EditorGUIUtility.isProSkin ? (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png") : (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
        Rect menuRect = new Rect(contentPosition.x, contentPosition.y + 4f, menuIcon.width, menuIcon.height);
        GUIHelper.DrawTexture(menuRect, menuIcon);
        contentPosition.x += menuRect.width + 4f;
        contentPosition.xMax -= (menuRect.width + 4f);

        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {   
            if (menuRect.Contains(e.mousePosition))
            {
                ShowHeaderContextMenu(new Vector2(menuRect.x, menuRect.yMax), useConstant, constantAction, variableAction);
                e.Use();
            }
        }
    }

    private static void DrawProperty(Rect contentPosition, SerializedProperty property, bool useConstant)
    {
        EditorGUI.PropertyField(contentPosition, GetSerializedValue(property, useConstant), GUIContent.none);
    }

    private static void ShowHeaderContextMenu(Vector2 position, bool useConstant, Action constantAction, Action variableAction)
    {
        Assert.IsNotNull(constantAction);
        Assert.IsNotNull(variableAction);

        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Constant"), useConstant, () => constantAction());
        menu.AddItem(new GUIContent("Variable"), !useConstant, () => variableAction());

        menu.DropDown(new Rect(position, Vector2.zero));
    }

    private static void SetType(SerializedProperty property, bool constant)
    {
        property.boolValue = constant;
        property.serializedObject.ApplyModifiedProperties();
    }
}
