using UnityEngine;
using UnityEditor;
using fNbt;
using System.Text;

public class NbtMassArrayEdit : EditorWindow {
    public NbtByteArray byteTag;
    public NbtIntArray intTag;
    public string text;

    public static void EditArray(Rect buttonRect, NbtByteArray byteArray) {
        NbtMassArrayEdit win = ScriptableObject.CreateInstance<NbtMassArrayEdit>();
        win.titleContent = new GUIContent("Byte Array");
        win.byteTag = byteArray;
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < byteArray.Value.Length; i++) {
            if(i != 0) sb.Append(" ");
            sb.Append(byteArray.Value[i]);
        }
        win.text = sb.ToString();
        win.ShowAsDropDown(buttonRect, new Vector2(256, 128));
    }

    public static void EditArray(Rect buttonRect, NbtIntArray intArray) {
        NbtMassArrayEdit win = ScriptableObject.CreateInstance<NbtMassArrayEdit>();
        win.titleContent = new GUIContent("Int Array");
        win.intTag = intArray;
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < intArray.Value.Length; i++) {
            if(i != 0) sb.Append(" ");
            sb.Append(intArray.Value[i]);
        }
        win.text = sb.ToString();
        win.ShowAsDropDown(buttonRect, new Vector2(256, 128));
    }

    /// <summary>
    /// Called by Unity to render the elements inside this EditorWindow's window
    /// </summary>
    void OnGUI() {
        if(!EditorGUIUtility.isProSkin) {
            Color oldColor = GUI.color;
            GUI.color = new Color(0.9f, 0.9f, 0.9f, 1.0f);
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorGUIUtility.whiteTexture);
            GUI.color = oldColor;
        }
        if(GUI.Button(new Rect(10, position.height - 26, position.width - 20, 16), "Apply")) {
            string[] vals = text.Split(new string[] { " ", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            if(byteTag != null) {
                try {
                    byteTag.Value = new byte[vals.Length];
                    for(int i = 0; i < vals.Length; i++) {
                        byteTag.Value[i] = byte.Parse(vals[i]);
                    }
                    Close();
                } catch(System.FormatException) {
                    ShowNotification(new GUIContent("Cannot parse text to a byte array.  Please don't use non-numeric characters."));
                } catch(System.OverflowException) {
                    ShowNotification(new GUIContent("Cannot parse text to a byte array.  Valid bytes range from 0 to 255."));
                }
            } else if(intTag != null) {
                try {
                    intTag.Value = new int[vals.Length];
                    for(int i = 0; i < vals.Length; i++) {
                        intTag.Value[i] = int.Parse(vals[i]);
                    }
                    Close();
                } catch(System.FormatException) {
                    ShowNotification(new GUIContent("Cannot parse text to an integer array.  Please don't use non-numeric characters."));
                } catch(System.OverflowException) {
                    ShowNotification(new GUIContent("Cannot parse text to an integer array.  Some numbers are too large to fit in an int."));
                }
            } else {
                throw new System.ArgumentNullException("Window does not have a tag to assign value to!");
            }
        }
        if(byteTag != null || intTag != null) EditorGUI.LabelField(new Rect(10, 10, position.width - 20, 16), "Editing " + (byteTag == null ? intTag.Name : byteTag.Name));
        EditorStyles.textField.wordWrap = EditorStyles.textArea.wordWrap = true;
        text = EditorGUI.TextArea(new Rect(10, 26, position.width - 20, position.height - 52), text);
    }
}