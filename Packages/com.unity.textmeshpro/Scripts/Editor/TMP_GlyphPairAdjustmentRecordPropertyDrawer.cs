﻿using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;

namespace TMPro.EditorUtilities
{
    [CustomPropertyDrawer(typeof(TMP_GlyphPairAdjustmentRecord))]
    public class TMP_GlyphPairAdjustmentRecordPropertyDrawer : PropertyDrawer
    {
        private bool isEditingEnabled = false;
        private bool isSelectable = false;

        private string m_FirstCharacter = string.Empty;
        private string m_SecondCharacter = string.Empty;
        private string m_PreviousInput;

        static GUIContent s_CharacterTextFieldLabel = new GUIContent("Char:", "Enter the character or its UTF16 or UTF32 Unicode character escape sequence. For UTF16 use \"\\uFF00\" and for UTF32 use \"\\UFF00FF00\" representation.");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prop_FirstAdjustmentRecord = property.FindPropertyRelative("m_FirstAdjustmentRecord");
            SerializedProperty prop_SecondAdjustmentRecord = property.FindPropertyRelative("m_SecondAdjustmentRecord");

            SerializedProperty prop_FirstGlyphIndex = prop_FirstAdjustmentRecord.FindPropertyRelative("m_GlyphIndex");
            SerializedProperty prop_FirstGlyphValueRecord = prop_FirstAdjustmentRecord.FindPropertyRelative("m_GlyphValueRecord");

            SerializedProperty prop_SecondGlyphIndex = prop_SecondAdjustmentRecord.FindPropertyRelative("m_GlyphIndex");
            SerializedProperty prop_SecondGlyphValueRecord = prop_SecondAdjustmentRecord.FindPropertyRelative("m_GlyphValueRecord");

            SerializedProperty prop_FontFeatureLookupFlags = property.FindPropertyRelative("m_FeatureLookupFlags");

            position.yMin += 2;

            float width = position.width / 2;
            float padding = 5.0f;

            Rect rect;

            isEditingEnabled = GUI.enabled;
            isSelectable = label.text == "Selectable" ? true : false;

            if (isSelectable)
                GUILayoutUtility.GetRect(position.width, 75);
            else
                GUILayoutUtility.GetRect(position.width, 55);

            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.richText = true;

            // First Glyph
            GUI.enabled = isEditingEnabled;
            if (isSelectable)
            {
                rect = new Rect(position.x + 70, position.y, position.width, 49);

                float labelWidth = GUI.skin.label.CalcSize(new GUIContent("ID: " + prop_FirstGlyphIndex.intValue)).x;
                EditorGUI.LabelField(new Rect(position.x + (64 - labelWidth) / 2, position.y + 60, 64f, 18f), new GUIContent("ID: <color=#FFFF80>" + prop_FirstGlyphIndex.intValue + "</color>"), style);

                GUI.enabled = isEditingEnabled;
                EditorGUIUtility.labelWidth = 30f;

                rect = new Rect(position.x + 70, position.y + 10, (width - 70) - padding, 18);
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_XPlacement"), new GUIContent("OX:"));

                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_YPlacement"), new GUIContent("OY:"));

                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_XAdvance"), new GUIContent("AX:"));

                DrawGlyph((uint)prop_FirstGlyphIndex.intValue, new Rect(position.x, position.y, position.width, position.height), property);
            }
            else
            {
                rect = new Rect(position.x, position.y, width / 2 * 0.8f - padding, 18);
                EditorGUIUtility.labelWidth = 40f;

                // First Character Lookup
                GUI.SetNextControlName("FirstCharacterField");
                EditorGUI.BeginChangeCheck();
                string firstCharacter = EditorGUI.TextField(rect, s_CharacterTextFieldLabel, m_FirstCharacter);

                if (GUI.GetNameOfFocusedControl() == "FirstCharacterField")
                {
                    if (ValidateInput(firstCharacter))
                    {
                        uint unicode = GetUnicodeCharacter(firstCharacter);
                        
                        // Get font asset through serialized property instead of direct access
                        SerializedProperty fontAssetProp = property.serializedObject.FindProperty("fontAsset");
                        if (fontAssetProp != null)
                        {
                            TMP_FontAsset fontAsset = fontAssetProp.objectReferenceValue as TMP_FontAsset;
                            if (fontAsset != null && fontAsset.characterLookupTable.ContainsKey(unicode))
                            {
                                prop_FirstGlyphIndex.intValue = (int)fontAsset.characterLookupTable[unicode].glyphIndex;
                            }
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                    m_FirstCharacter = firstCharacter;

                // First Glyph Index
                rect.x += width / 2 * 0.8f;

                EditorGUIUtility.labelWidth = 25f;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, prop_FirstGlyphIndex, new GUIContent("ID:"));
                if (EditorGUI.EndChangeCheck()) { }

                GUI.enabled = isEditingEnabled;
                EditorGUIUtility.labelWidth = 25f;

                rect = new Rect(position.x, position.y + 20, width * 0.5f - padding, 18);
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_XPlacement"), new GUIContent("OX"));

                rect.x += width * 0.5f;
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_YPlacement"), new GUIContent("OY"));

                rect.x = position.x;
                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_FirstGlyphValueRecord.FindPropertyRelative("m_XAdvance"), new GUIContent("AX"));
            }

            // Second Glyph
            GUI.enabled = isEditingEnabled;
            if (isSelectable)
            {
                float labelWidth = GUI.skin.label.CalcSize(new GUIContent("ID: " + prop_SecondGlyphIndex.intValue)).x;
                EditorGUI.LabelField(new Rect(position.width / 2 + 20 + (64 - labelWidth) / 2, position.y + 60, 64f, 18f), new GUIContent("ID: <color=#FFFF80>" + prop_SecondGlyphIndex.intValue + "</color>"), style);

                GUI.enabled = isEditingEnabled;
                EditorGUIUtility.labelWidth = 30f;

                rect = new Rect(position.width / 2 + 20 + 70, position.y + 10, (width - 70) - padding, 18);
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_XPlacement"), new GUIContent("OX:"));

                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_YPlacement"), new GUIContent("OY:"));

                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_XAdvance"), new GUIContent("AX:"));

                DrawGlyph((uint)prop_SecondGlyphIndex.intValue, new Rect(position.width / 2 + 20, position.y, position.width, position.height), property);
            }
            else
            {
                rect = new Rect(position.width / 2 + 20, position.y, width / 2 * 0.8f - padding, 18);
                EditorGUIUtility.labelWidth = 40f;

                // Second Character Lookup
                GUI.SetNextControlName("SecondCharacterField");
                EditorGUI.BeginChangeCheck();
                string secondCharacter = EditorGUI.TextField(rect, s_CharacterTextFieldLabel, m_SecondCharacter);

                if (GUI.GetNameOfFocusedControl() == "SecondCharacterField")
                {
                    if (ValidateInput(secondCharacter))
                    {
                        uint unicode = GetUnicodeCharacter(secondCharacter);
                        
                        // Get font asset through serialized property instead of direct access
                        SerializedProperty fontAssetProp = property.serializedObject.FindProperty("fontAsset");
                        if (fontAssetProp != null)
                        {
                            TMP_FontAsset fontAsset = fontAssetProp.objectReferenceValue as TMP_FontAsset;
                            if (fontAsset != null && fontAsset.characterLookupTable.ContainsKey(unicode))
                            {
                                prop_SecondGlyphIndex.intValue = (int)fontAsset.characterLookupTable[unicode].glyphIndex;
                            }
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                    m_SecondCharacter = secondCharacter;

                // Second Glyph Index
                rect.x += width / 2 * 0.8f;

                EditorGUIUtility.labelWidth = 25f;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, prop_SecondGlyphIndex, new GUIContent("ID:"));
                if (EditorGUI.EndChangeCheck()) { }

                GUI.enabled = isEditingEnabled;
                EditorGUIUtility.labelWidth = 25f;

                rect = new Rect(position.width / 2 + 20, position.y + 20, width * 0.5f - padding, 18);
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_XPlacement"), new GUIContent("OX"));

                rect.x += width * 0.5f;
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_YPlacement"), new GUIContent("OY"));

                rect.x = position.width / 2 + 20;
                rect.y += 20;
                EditorGUI.PropertyField(rect, prop_SecondGlyphValueRecord.FindPropertyRelative("m_XAdvance"), new GUIContent("AX"));
            }

            // Font Feature Lookup Flags
            if (isSelectable)
            {
                EditorGUIUtility.labelWidth = 55f;

                rect.x = position.width - 255;
                rect.y += 23;
                rect.width = 270;

                FontFeatureLookupFlags flags = (FontFeatureLookupFlags)prop_FontFeatureLookupFlags.intValue;

                EditorGUI.BeginChangeCheck();
                flags = (FontFeatureLookupFlags)EditorGUI.EnumFlagsField(rect, new GUIContent("Options:"), flags);
                if (EditorGUI.EndChangeCheck())
                {
                    prop_FontFeatureLookupFlags.intValue = (int)flags;
                }
            }
        }

        bool ValidateInput(string source)
        {
            int length = string.IsNullOrEmpty(source) ? 0 : source.Length;
            Event evt = Event.current;
            char c = evt.character;

            if (c != '\0')
            {
                switch (length)
                {
                    case 0:
                        break;
                    case 1:
                        if (source != m_PreviousInput)
                            return true;

                        if ((source[0] == '\\' && (c == 'u' || c == 'U')) == false)
                            evt.character = '\0';
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
                            evt.character = '\0';
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        if (source[1] == 'u' || (c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
                            evt.character = '\0';

                        if (length == 6 && source[1] == 'u' && source != m_PreviousInput)
                            return true;
                        break;
                    case 10:
                        if (source != m_PreviousInput)
                            return true;

                        evt.character = '\0';
                        break;
                }
            }

            m_PreviousInput = source;
            return false;
        }

        uint GetUnicodeCharacter(string source)
        {
            uint unicode;

            if (source.Length == 1)
                unicode = source[0];
            else if (source.Length == 6)
                unicode = (uint)TMP_TextUtilities.StringHexToInt(source.Replace("\\u", ""));
            else
                unicode = (uint)TMP_TextUtilities.StringHexToInt(source.Replace("\\U", ""));

            return unicode;
        }

        void DrawGlyph(uint glyphIndex, Rect position, SerializedProperty property)
        {
            // Get font asset through serialized property instead of direct access
            SerializedProperty fontAssetProp = property.serializedObject.FindProperty("fontAsset");
            if (fontAssetProp == null) return;
            
            TMP_FontAsset fontAsset = fontAssetProp.objectReferenceValue as TMP_FontAsset;
            if (fontAsset == null) return;

            if (!fontAsset.glyphLookupTable.TryGetValue(glyphIndex, out Glyph glyph)) return;

            int atlasIndex = glyph.atlasIndex;
            Texture2D atlasTexture = fontAsset.atlasTextures.Length > atlasIndex ? fontAsset.atlasTextures[atlasIndex] : null;
            if (atlasTexture == null) return;

            Material mat = GetGlyphMaterial(fontAsset);
            if (mat == null) return;

            mat.mainTexture = atlasTexture;
            
            bool isBitmapMode = (int)fontAsset.atlasRenderMode == 1; // 1 = Bitmap mode
            if (!isBitmapMode)
            {
                mat.SetFloat(ShaderUtilities.ID_GradientScale, fontAsset.atlasPadding + 1);
            }

            Rect glyphDrawPosition = new Rect(position.x, position.y + 2, 64, 60);
            GlyphRect glyphRect = glyph.glyphRect;
            int padding = fontAsset.atlasPadding;

            int glyphOriginX = glyphRect.x - padding;
            int glyphOriginY = glyphRect.y - padding;
            int glyphWidth = glyphRect.width + padding * 2;
            int glyphHeight = glyphRect.height + padding * 2;

            float normalizedHeight = fontAsset.faceInfo.ascentLine - fontAsset.faceInfo.descentLine;
            float scale = glyphDrawPosition.width / normalizedHeight;

            Rect texCoords = new Rect(
                (float)glyphOriginX / atlasTexture.width, 
                (float)glyphOriginY / atlasTexture.height, 
                (float)glyphWidth / atlasTexture.width, 
                (float)glyphHeight / atlasTexture.height);

            if (Event.current.type == EventType.Repaint)
            {
                glyphDrawPosition.x += (glyphDrawPosition.width - glyphWidth * scale) / 2;
                glyphDrawPosition.y += (glyphDrawPosition.height - glyphHeight * scale) / 2;
                glyphDrawPosition.width = glyphWidth * scale;
                glyphDrawPosition.height = glyphHeight * scale;

                Graphics.DrawTexture(glyphDrawPosition, atlasTexture, texCoords, 0, 0, 0, 0, new Color(1f, 1f, 1f), mat);
            }
        }

        private Material GetGlyphMaterial(TMP_FontAsset fontAsset)
        {
            if (fontAsset.material != null)
                return new Material(fontAsset.material);

            bool isBitmapMode = (int)fontAsset.atlasRenderMode == 1; // 1 = Bitmap mode
            string shaderName = isBitmapMode ? "TextMeshPro/Bitmap" : "TextMeshPro/Distance Field";

            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {shaderName}");
                return null;
            }

            return new Material(shader);
        }
    }
}