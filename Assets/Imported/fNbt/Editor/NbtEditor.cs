using UnityEngine;
using UnityEditor;
using fNbt;
using System.Collections.Generic;
using System;

public class NbtEditor : EditorWindow {
    private Texture2D newFile, open, save, saveAs;
    public string filePath = "";
    public NbtFile nbtFile;
    public static NbtTag clipboard;
    public NbtCompoundRender renderRoot;
    public Vector2 fileScroll;

    [MenuItem("Window/NBT Editor")]
    static void ShowWindow() {
        EditorWindow.GetWindow<NbtEditor>("NBT Editor", true);
    }

    /// <summary>
    /// Called by Unity to render the elements inside this EditorWindow's window
    /// </summary>
    void OnGUI() {
        if(newFile == null) {
            newFile = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imported/fNbt/Editor/Sprites/New.png");
            open = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imported/fNbt/Editor/Sprites/Open.png");
            save = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imported/fNbt/Editor/Sprites/Save.png");
            saveAs = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imported/fNbt/Editor/Sprites/SaveAs.png");
        }

        if(GUI.Button(new Rect(10, 8, 28, 28), newFile)) {
            filePath = "New File";
            nbtFile = null;
            renderRoot = null;
        }
        if(GUI.Button(new Rect(48, 8, 28, 28), open)) {
            string newFilePath = EditorUtility.OpenFilePanel("Open NBT File", "Assets/..", "nbt");
            if(newFilePath == "") return;
            filePath = newFilePath;
            nbtFile = null;
            renderRoot = null;
            clipboard = null;
        }
        if(GUI.Button(new Rect(86, 8, 28, 28), save)) {
            if(filePath == "New File") {
                string newFilePath = EditorUtility.SaveFilePanel("Save NBT File", "Assets/..", "file.nbt", "nbt");
                if(newFilePath == "") return;
                filePath = newFilePath;
            }
            nbtFile.SaveToFile(filePath, NbtCompression.None);
        }
        if(GUI.Button(new Rect(114, 8, 28, 28), saveAs)) {
            string newFilePath = EditorUtility.SaveFilePanel("Save NBT File", "Assets/..", "file.nbt", "nbt");
            if(newFilePath == "") return;
            filePath = newFilePath;
            nbtFile.SaveToFile(filePath, NbtCompression.None);
        }

        if(nbtFile == null) {
            if(filePath == "New File") {
                nbtFile = new NbtFile();
                nbtFile.RootTag.Name = "root";
                clipboard = null;
            } else if(filePath != "") {
                nbtFile = new NbtFile(filePath);
                clipboard = null;
            }
            if(nbtFile != null) {
                renderRoot = new NbtCompoundRender(nbtFile.RootTag);
                renderRoot.expanded = true;
                clipboard = null;
            }
        }

        EditorGUI.DropShadowLabel(new Rect(152, 12, 32, 16), "File:");
        EditorGUI.LabelField(new Rect(186, 15, position.width - 196, 16), filePath);

        if(renderRoot != null) {
            fileScroll = GUI.BeginScrollView(new Rect(10, 46, position.width - 20, position.height - 56), fileScroll, new Rect(0, 0, position.width - 35, renderRoot.Height));

            renderRoot.RenderTag(false, 0, 0, position, fileScroll, this);

            GUI.EndScrollView();
        }
    }
}

public class ArrayLengthEditor : EditorWindow {
    private NbtByteArray byteTag;
    private NbtIntArray intTag;
    public int len;

    public static void Edit(Rect rect, NbtByteArray data) {
        ArrayLengthEditor win = ScriptableObject.CreateInstance<ArrayLengthEditor>();
        win.titleContent = new GUIContent("Byte Array");
        win.byteTag = data;
        win.len = data.Value.Length;
        win.ShowAsDropDown(rect, new Vector2(256, 80));
    }

    public static void Edit(Rect rect, NbtIntArray data) {
        ArrayLengthEditor win = ScriptableObject.CreateInstance<ArrayLengthEditor>();
        win.titleContent = new GUIContent("Int Array");
        win.intTag = data;
        win.len = data.Value.Length;
        win.ShowAsDropDown(rect, new Vector2(256, 80));
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
        if(byteTag != null || intTag != null) EditorGUI.LabelField(new Rect(10, 10, position.width - 20, 16), "Editing " + (byteTag == null ? intTag.Name : byteTag.Name));

        len = EditorGUI.IntField(new Rect(10, 26, position.width - 20, 16), "Length:", len);

        if(GUI.Button(new Rect(10, position.height - 26, position.width - 20, 16), "Apply")) {
            if(byteTag != null) {
                byte[] array = new byte[len];
                Array.Copy(byteTag.Value, array, (len < byteTag.Value.Length ? len : byteTag.Value.Length));
                byteTag.Value = array;
                Close();
            } else if(intTag != null) {
                int[] array = new int[len];
                Array.Copy(intTag.Value, array, (len < intTag.Value.Length ? len : intTag.Value.Length));
                intTag.Value = array;
                Close();
            } else {
                throw new ArgumentNullException("Window does not have a tag to assign value to!");
            }
        }
    }
}

public abstract class NbtRenderer {
    public NbtRenderer parent;

    public abstract void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win);

    public abstract float Height { get; }

    public static NbtRenderer MakeRenderer(NbtTag tag) {
        switch(tag.TagType) {
            case NbtTagType.Byte:
                return new NbtByteRender(tag as NbtByte);
            case NbtTagType.ByteArray:
                return new NbtByteArrayRender(tag as NbtByteArray);
            case NbtTagType.Compound:
                return new NbtCompoundRender(tag as NbtCompound);
            case NbtTagType.Double:
                return new NbtDoubleRender(tag as NbtDouble);
            case NbtTagType.Float:
                return new NbtFloatRender(tag as NbtFloat);
            case NbtTagType.Int:
                return new NbtIntRender(tag as NbtInt);
            case NbtTagType.IntArray:
                return new NbtIntArrayRender(tag as NbtIntArray);
            case NbtTagType.List:
                return new NbtListRender(tag as NbtList);
            case NbtTagType.Long:
                return new NbtLongRender(tag as NbtLong);
            case NbtTagType.Short:
                return new NbtShortRender(tag as NbtShort);
            case NbtTagType.String:
                return new NbtStringRender(tag as NbtString);
            default:
                throw new ArgumentException("Tag not a legit tag.");
        }
    }

    public abstract void Delete();
}

public class NbtByteRender : NbtRenderer {
    NbtByte data;

    public NbtByteRender(NbtByte tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Byte");
        int newVal = 0;
        if(suppressName) {
            newVal = EditorGUI.IntField(new Rect(indent + 48, top, 32, 16), data.ByteValue);
            EditorGUI.HelpBox(new Rect(indent + 88, top, 110, 16), "Value: 0 to 255", MessageType.None);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = EditorGUI.IntField(new Rect(indent + 180, top, 32, 16), data.ByteValue);
            EditorGUI.HelpBox(new Rect(indent + 220, top, 110, 16), "Value: 0 to 255", MessageType.None);
        }
        try {
            data.Value = (byte)newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtByteArrayRender : NbtRenderer {
    NbtByteArray data;
    public bool expanded;

    public NbtByteArrayRender(NbtByteArray tag) {
        data = tag;
        expanded = false;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 64, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                float t = top;

                menu.AddItem(new GUIContent("Mass Edit"), false, delegate() {
                    NbtMassArrayEdit.EditArray(new Rect(position.x + indent + 20, position.y - fileScroll.y + 48 + t, 64, 16), data);
                });

                menu.AddItem(new GUIContent("Edit Size"), false, delegate() {
                    ArrayLengthEditor.Edit(new Rect(position.x + indent + 20, position.y - fileScroll.y + 48 + t, 64, 16), data);
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        expanded = EditorGUI.Foldout(new Rect(indent, top, 64, 16), expanded, "ByteArr");
        if(!suppressName) {
            string newName = EditorGUI.TextField(new Rect(indent + 64, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            EditorGUI.HelpBox(new Rect(indent + 200, top, 110, 16), "Value: 0 to 255", MessageType.None);
        } else {
            EditorGUI.HelpBox(new Rect(indent + 60, top, 110, 16), "Value: 0 to 255", MessageType.None);
        }
        if(expanded) {
            byte[] value = data.Value;
            for(int i = 0; i < value.Length; i++) {
                if(i % 16 == 0) {
                    top += 16;
                    EditorGUI.LabelField(new Rect(indent + 20, top, 44, 16), i.ToString("X4"));
                }
                value[i] = (byte)EditorGUI.IntField(new Rect(indent + 64 + (32 * (i % 16)), top, 32, 16), value[i]);
            }
        }
    }

    public override float Height {
        get {
            if(!expanded) {
                return 16;
            } else {
                return 16 + (Mathf.CeilToInt(data.Value.Length / 16.0f) * 16);
            }
        }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtCompoundRender : NbtRenderer {
    NbtCompound data;
    public bool expanded;
    public List<NbtRenderer> children;

    public NbtCompoundRender(NbtCompound tag) {
        data = tag;
        expanded = false;
        children = new List<NbtRenderer>();

        foreach(NbtTag alpha in tag) {
            NbtRenderer renderer = NbtRenderer.MakeRenderer(alpha);
            renderer.parent = this;
            children.Add(renderer);
        }
    }

    public static string GenerateUniqueName(NbtCompound src)
    {
        if(!src.Contains("newtag")) return "newtag";
        for(int i = 0; ;i++)
            if(!src.Contains("newtag" + i)) return "newtag" + i;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Add New/Byte Tag"), false, delegate() {
                    var newb = new NbtByte(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtByteRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Byte Array Tag"), false, delegate() {
                    var newb = new NbtByteArray(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtByteArrayRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Compound Tag"), false, delegate() {
                    var newb = new NbtCompound(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtCompoundRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Double Tag"), false, delegate() {
                    var newb = new NbtDouble(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtDoubleRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Float Tag"), false, delegate() {
                    var newb = new NbtFloat(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtFloatRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Int Tag"), false, delegate() {
                    var newb = new NbtInt(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtIntRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Int Array Tag"), false, delegate() {
                    var newb = new NbtIntArray(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtIntArrayRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/List Tag"), false, delegate() {
                    var newb = new NbtList(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtListRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Long Tag"), false, delegate() {
                    var newb = new NbtLong(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtLongRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/Short Tag"), false, delegate() {
                    var newb = new NbtShort(GenerateUniqueName(data)); data.Add(newb);
                    children.Add(new NbtShortRender(newb) { parent = this });
                });
                menu.AddItem(new GUIContent("Add New/String Tag"), false, delegate() {
                    var newb = new NbtString(GenerateUniqueName(data), ""); data.Add(newb);
                    children.Add(new NbtStringRender(newb) { parent = this });
                });

                if(NbtEditor.clipboard != null) {
                    menu.AddItem(new GUIContent("Paste Tag " + (string.IsNullOrEmpty(NbtEditor.clipboard.Name) ? "-unnamed-" : NbtEditor.clipboard.Name)), false, delegate() {
                        var newb = NbtEditor.clipboard.Clone();
                        if(string.IsNullOrEmpty(newb.Name)) newb.Name = "pasted tag";
                        data.Add(newb);
                        NbtRenderer renderer = NbtRenderer.MakeRenderer(newb);
                        renderer.parent = this;
                        children.Add(renderer);
                    });
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Expand All Children"), false, RecursiveExpand);

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                if(parent != null) {
                    menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);
                }

                menu.ShowAsContext();
                evt.Use();
            }
        }

        expanded = EditorGUI.Foldout(new Rect(indent, top, 48, 16), expanded, "Cmpd");
        if(!suppressName) {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
        }
        if(expanded) {
            float runningTop = 16f;
            for(int i = 0; i < children.Count; i++) {
                children[i].RenderTag(false, indent + 20, top + runningTop, position, fileScroll, win);
                runningTop += children[i].Height;
            }
        } else {
            RecursiveCollapse();
        }
    }

    public override float Height {
        get {
            if(!expanded) {
                return 16;
            } else {
                float rtn = 16;
                foreach(NbtRenderer r in children) {
                    rtn += r.Height;
                }
                return rtn;
            }
        }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }

    public void RecursiveExpand() {
        expanded = true;
        foreach(NbtRenderer child in children) {
            if(child is NbtCompoundRender) ((NbtCompoundRender)child).RecursiveExpand();
            else if(child is NbtListRender) ((NbtListRender)child).RecursiveExpand();
        }
    }
    public void RecursiveCollapse() {
        expanded = false;
        foreach(NbtRenderer child in children) {
            if(child is NbtCompoundRender) ((NbtCompoundRender)child).RecursiveCollapse();
            else if(child is NbtListRender) ((NbtListRender)child).RecursiveCollapse();
            else if(child is NbtByteArrayRender) ((NbtByteArrayRender)child).expanded = false;
            else if(child is NbtIntArrayRender) ((NbtIntArrayRender)child).expanded = false;
        }
    }
}

public class NbtDoubleRender : NbtRenderer {
    NbtDouble data;

    public NbtDoubleRender(NbtDouble tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Double");
        double newVal = 0;
        if(suppressName) {
            newVal = EditorGUI.DoubleField(new Rect(indent + 48, top, 128, 16), data.DoubleValue);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = EditorGUI.DoubleField(new Rect(indent + 180, top, 128, 16), data.DoubleValue);
        }
        try {
            data.Value = newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtFloatRender : NbtRenderer {
    NbtFloat data;

    public NbtFloatRender(NbtFloat tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Float");
        float newVal = 0;
        if(suppressName) {
            newVal = EditorGUI.FloatField(new Rect(indent + 48, top, 128, 16), data.FloatValue);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = EditorGUI.FloatField(new Rect(indent + 180, top, 128, 16), data.FloatValue);
        }
        try {
            data.Value = newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtIntRender : NbtRenderer {
    NbtInt data;

    public NbtIntRender(NbtInt tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Int");
        uint newVal = 0;
        if(suppressName) {
            newVal = (uint)EditorGUI.LongField(new Rect(indent + 48, top, 128, 16), data.IntValue);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = (uint)EditorGUI.LongField(new Rect(indent + 180, top, 128, 16), data.IntValue);
        }
        try {
            data.Value = (int)newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtIntArrayRender : NbtRenderer {
    NbtIntArray data;
    public bool expanded;

    public NbtIntArrayRender(NbtIntArray tag) {
        data = tag;
        expanded = false;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 64, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                float t = top;

                menu.AddItem(new GUIContent("Mass Edit"), false, delegate() {
                    NbtMassArrayEdit.EditArray(new Rect(position.x + indent + 20, position.y - fileScroll.y + 48 + t, 64, 16), data);
                });

                menu.AddItem(new GUIContent("Edit Size"), false, delegate() {
                    ArrayLengthEditor.Edit(new Rect(position.x + indent + 20, position.y - fileScroll.y + 48 + t, 64, 16), data);
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        expanded = EditorGUI.Foldout(new Rect(indent, top, 64, 16), expanded, "IntArr");
        if(!suppressName) {
            string newName = EditorGUI.TextField(new Rect(indent + 64, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
        }
        if(expanded) {
            int[] value = data.Value;
            for(int i = 0; i < value.Length; i++) {
                if(i % 4 == 0) {
                    top += 16;
                    EditorGUI.LabelField(new Rect(indent + 20, top, 44, 16), i.ToString("X4"));
                }
                value[i] = EditorGUI.IntField(new Rect(indent + 64 + (128 * (i % 4)), top, 128, 16), value[i]);
            }
        }
    }

    public override float Height {
        get {
            if(!expanded) {
                return 16;
            } else {
                return 16 + (Mathf.CeilToInt(data.Value.Length / 4.0f) * 16);
            }
        }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtListRender : NbtRenderer {
    NbtList data;
    public bool expanded;
    public List<NbtRenderer> children;

    public NbtListRender(NbtList tag) {
        data = tag;
        expanded = false;
        children = new List<NbtRenderer>();

        foreach(NbtTag alpha in tag) {
            NbtRenderer renderer = NbtRenderer.MakeRenderer(alpha);
            renderer.parent = this;
            children.Add(renderer);
        }
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Set Type/Byte"), data.ListType == NbtTagType.Byte, delegate() {
                    if(data.ListType != NbtTagType.Byte && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Byte", "This List contains items.  Changing the type to Byte will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Byte;
                });
                menu.AddItem(new GUIContent("Set Type/Byte Array"), data.ListType == NbtTagType.ByteArray, delegate() {
                    if(data.ListType != NbtTagType.ByteArray && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Byte Array", "This List contains items.  Changing the type to Byte Array will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.ByteArray;
                });
                menu.AddItem(new GUIContent("Set Type/Compound"), data.ListType == NbtTagType.Compound, delegate() {
                    if(data.ListType != NbtTagType.Compound && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Compound", "This List contains items.  Changing the type to Compound will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Compound;
                });
                menu.AddItem(new GUIContent("Set Type/Double"), data.ListType == NbtTagType.Double, delegate() {
                    if(data.ListType != NbtTagType.Double && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Double", "This List contains items.  Changing the type to Double will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Double;
                });
                menu.AddItem(new GUIContent("Set Type/Float"), data.ListType == NbtTagType.Float, delegate() {
                    if(data.ListType != NbtTagType.Float && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Float", "This List contains items.  Changing the type to Float will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Float;
                });
                menu.AddItem(new GUIContent("Set Type/Int"), data.ListType == NbtTagType.Int, delegate() {
                    if(data.ListType != NbtTagType.Int && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Int", "This List contains items.  Changing the type to Int will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Int;
                });
                menu.AddItem(new GUIContent("Set Type/Int Array"), data.ListType == NbtTagType.IntArray, delegate() {
                    if(data.ListType != NbtTagType.IntArray && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Int Array", "This List contains items.  Changing the type to Int Array will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.IntArray;
                });
                menu.AddItem(new GUIContent("Set Type/List"), data.ListType == NbtTagType.List, delegate() {
                    if(data.ListType != NbtTagType.List && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> List", "This List contains items.  Changing the type to List will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.List;
                });
                menu.AddItem(new GUIContent("Set Type/Long"), data.ListType == NbtTagType.Long, delegate() {
                    if(data.ListType != NbtTagType.Long && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Long", "This List contains items.  Changing the type to Long will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Long;
                });
                menu.AddItem(new GUIContent("Set Type/Short"), data.ListType == NbtTagType.Short, delegate() {
                    if(data.ListType != NbtTagType.Short && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> Short", "This List contains items.  Changing the type to Short will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.Short;
                });
                menu.AddItem(new GUIContent("Set Type/String"), data.ListType == NbtTagType.String, delegate() {
                    if(data.ListType != NbtTagType.String && data.Count > 0) {
                        if(EditorUtility.DisplayDialog("Set Type -> String", "This List contains items.  Changing the type to String will require emptying the list.  Is this okay?", "No", "Yes")) return;
                    }
                    data.Clear();
                    children.Clear();
                    data.ListType = NbtTagType.String;
                });

                if(data.ListType != NbtTagType.Unknown) {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Add New Tag to List"), false, delegate() {
                        NbtTag alpha = null;
                        switch(data.ListType) {
                            case NbtTagType.Byte:
                                alpha = new NbtByte();
                                break;
                            case NbtTagType.ByteArray:
                                alpha = new NbtByteArray();
                                break;
                            case NbtTagType.Compound:
                                alpha = new NbtCompound();
                                break;
                            case NbtTagType.Double:
                                alpha = new NbtDouble();
                                break;
                            case NbtTagType.Float:
                                alpha = new NbtFloat();
                                break;
                            case NbtTagType.Int:
                                alpha = new NbtInt();
                                break;
                            case NbtTagType.IntArray:
                                alpha = new NbtIntArray();
                                break;
                            case NbtTagType.List:
                                alpha = new NbtList();
                                break;
                            case NbtTagType.Long:
                                alpha = new NbtLong();
                                break;
                            case NbtTagType.Short:
                                alpha = new NbtShort();
                                break;
                            case NbtTagType.String:
                                alpha = new NbtString();
                                break;
                            default:
                                return;
                        }
                        data.Add(alpha);
                        NbtRenderer newRenderer = NbtRenderer.MakeRenderer(alpha);
                        newRenderer.parent = this;
                        children.Add(newRenderer);
                    });

                    if(NbtEditor.clipboard != null && NbtEditor.clipboard.TagType == data.ListType) {
                        menu.AddItem(new GUIContent("Paste Tag " + (string.IsNullOrEmpty(NbtEditor.clipboard.Name) ? "-unnamed-" : NbtEditor.clipboard.Name)), false, delegate() {
                            var newb = NbtEditor.clipboard.Clone();
                            data.Add(newb);
                            NbtRenderer renderer = NbtRenderer.MakeRenderer(newb);
                            renderer.parent = this;
                            children.Add(renderer);
                        });
                    }
                }

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Expand All Children"), false, RecursiveExpand);

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        expanded = EditorGUI.Foldout(new Rect(indent, top, 48, 16), expanded, "List");
        if(!suppressName) {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            EditorGUI.LabelField(new Rect(indent + 180, top, 128, 16), "Type <" + data.ListType + ">");
        } else {
            EditorGUI.LabelField(new Rect(indent + 48, top, 128, 16), "Type <" + data.ListType + ">");
        }
        if(expanded) {
            float runningTop = 16f;
            for(int i = 0; i < children.Count; i++) {
                children[i].RenderTag(true, indent + 20, top + runningTop, position, fileScroll, win);
                runningTop += children[i].Height;
            }
        } else {
            RecursiveCollapse();
        }
    }

    public override float Height {
        get {
            if(!expanded) {
                return 16;
            } else {
                float rtn = 16;
                foreach(NbtRenderer r in children) {
                    rtn += r.Height;
                }
                return rtn;
            }
        }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }

    public void RecursiveExpand() {
        expanded = true;
        if(data.ListType == NbtTagType.Compound) {
            foreach(NbtRenderer child in children) {
                ((NbtCompoundRender)child).RecursiveExpand();
            }
        } else if(data.ListType == NbtTagType.List) {
            foreach(NbtRenderer child in children) {
                ((NbtListRender)child).RecursiveExpand();
            }
        }
    }

    public void RecursiveCollapse() {
        expanded = false;
        if(data.ListType == NbtTagType.Compound) {
            foreach(NbtRenderer child in children) {
                ((NbtCompoundRender)child).RecursiveCollapse();
            }
        } else if(data.ListType == NbtTagType.List) {
            foreach(NbtRenderer child in children) {
                ((NbtListRender)child).RecursiveCollapse();
            }
        } else if(data.ListType == NbtTagType.IntArray) {
            foreach(NbtRenderer child in children) {
                ((NbtIntArrayRender)child).expanded = false;
            }
        } else if(data.ListType == NbtTagType.ByteArray) {
            foreach(NbtRenderer child in children) {
                ((NbtByteArrayRender)child).expanded = false;
            }
        }

    }
}

public class NbtLongRender : NbtRenderer {
    NbtLong data;

    public NbtLongRender(NbtLong tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Long");
        long newVal = 0;
        if(suppressName) {
            newVal = EditorGUI.LongField(new Rect(indent + 48, top, 128, 16), data.LongValue);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = EditorGUI.LongField(new Rect(indent + 180, top, 128, 16), data.LongValue);
        }
        try {
            data.Value = newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtShortRender : NbtRenderer {
    NbtShort data;

    public NbtShortRender(NbtShort tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "Short");
        int newVal = 0;
        if(suppressName) {
            newVal = EditorGUI.IntField(new Rect(indent + 48, top, 64, 16), data.ShortValue);
            EditorGUI.HelpBox(new Rect(indent + 120, top, 160, 16), "Value: -32768 to 32767", MessageType.None);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            newVal = EditorGUI.IntField(new Rect(indent + 180, top, 64, 16), data.ShortValue);
            EditorGUI.HelpBox(new Rect(indent + 252, top, 160, 16), "Value: -32768 to 32767", MessageType.None);
        }
        try {
            data.Value = (short)newVal;
        } catch(Exception) { }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}

public class NbtStringRender : NbtRenderer {
    NbtString data;

    public NbtStringRender(NbtString tag) {
        data = tag;
    }

    public override void RenderTag(bool suppressName, float indent, float top, Rect position, Vector2 fileScroll, EditorWindow win) {
        Event evt = Event.current;

        if(evt.type == EventType.ContextClick) {
            if(new Rect(indent, top, 48, 16).Contains(evt.mousePosition)) {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy this Tag"), false, delegate() {
                    NbtEditor.clipboard = data;
                });
                menu.AddItem(new GUIContent("Delete This Tag"), false, Delete);

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUI.LabelField(new Rect(indent, top, 48, 16), "String");
        if(suppressName) {
            data.Value = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.StringValue);
        } else {
            string newName = EditorGUI.TextField(new Rect(indent + 48, top, 128, 16), data.Name);
            if((data.Parent != null) && ((NbtCompound)data.Parent).Contains(newName) && ((NbtCompound)data.Parent)[newName] != data) {
                win.ShowNotification(new GUIContent("Cannot rename tag: parent Compound already contains that name"));
            } else {
                data.Name = newName;
            }
            data.Value = EditorGUI.TextField(new Rect(indent + 180, top, 128, 16), data.StringValue);
        }
    }

    public override float Height {
        get { return 16; }
    }

    public override void Delete() {
        if(parent is NbtCompoundRender) {
            ((NbtCompoundRender)parent).children.Remove(this);
        } else if(parent is NbtListRender) {
            ((NbtListRender)parent).children.Remove(this);
        }
        if(data.Parent is NbtCompound) {
            ((NbtCompound)data.Parent).Remove(data);
        } else if(data.Parent is NbtList) {
            ((NbtList)data.Parent).Remove(data);
        }
    }
}