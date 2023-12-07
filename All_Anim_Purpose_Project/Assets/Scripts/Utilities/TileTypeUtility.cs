using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileTypeUtility {
    private static readonly Dictionary<TileType.Type, Color> _TypeColorEncoding = new(){
        {TileType.Type.Standard, Color.red},
        {TileType.Type.Droppable, Color.green},
        {TileType.Type.Rotating, Color.blue},
        {TileType.Type.Bouncy, Color.cyan},
        {TileType.Type.Empty, Color.white},
    };

    public static Color GetTypeColor(TileType.Type type) => _TypeColorEncoding[type];
}
