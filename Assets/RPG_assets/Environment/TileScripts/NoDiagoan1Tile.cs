using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class NoDiagoan1Tile :Tile
{
    
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        AStar.NoDiaonal1Tiles.Add(position);
        
        return base.StartUp(position, tilemap, go);
    }    

   #if UNITY_EDITOR
    [MenuItem("Assets/Create/Tiles/NoDiagoan1Tile")]
    public static void CreateWaterTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save NoDiagoan1Tile", "NoDiagoan1Tile", "asset", "Save NoDiagoan1Tile", "Assets");
        if (path == "")
        {
            return;
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<NoDiagoan1Tile>(), path);
    }

#endif
}
