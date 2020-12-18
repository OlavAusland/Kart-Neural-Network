using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Layout))]
public class LayoutEditor : Editor
{
    private int index;
    private Vector3 position;

    public Object empty { get { return Resources.Load<Object>("Empty"); } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Layout layout = (Layout)target;

        if (GUILayout.Button("Add")){layout.Add(index, position);}
        if (GUILayout.Button("New Layout")){layout.NewLayout();}
        if(GUILayout.Button("Save Layout")){ layout.SaveLayout(); }

        DisplayPreview(layout);
        position = layout.UpdatePosition();
    }

    private Texture2D LoadPreview(Layout layout, int index){ return AssetPreview.GetAssetPreview(layout.pieces[index - 1]); }

    private int DisplayPreview(Layout layout) 
    {
        if(layout.pieces.Count <= 0) { return 0; }
        index = (int)EditorGUILayout.Slider("Piece", index, 1, layout.pieces.Count);
        int piecePreview = index;
        GUILayout.BeginHorizontal();
        GUILayout.Label(LoadPreview(layout, index));
        if (++piecePreview > layout.pieces.Count) { GUILayout.Label(AssetPreview.GetAssetPreview(empty)); }
        else { GUILayout.Label(LoadPreview(layout, piecePreview)); }
        if (++piecePreview > layout.pieces.Count) { GUILayout.Label(AssetPreview.GetAssetPreview(empty)); }
        else { GUILayout.Label(LoadPreview(layout, piecePreview)); }

        GUILayout.EndHorizontal();

        return piecePreview;
    }
}
