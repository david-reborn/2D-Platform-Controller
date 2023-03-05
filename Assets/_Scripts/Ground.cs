using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ground : MonoBehaviour
{
    public Color GroundColor;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Debug.Log(Input.mousePosition);
            Tilemap tilemap = this.GetComponent<Tilemap>();
            Vector3Int cell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            TileBase tile = tilemap.GetTile(cell);
            Debug.Log(tile.name);
        }
    }

    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);

        // Set the colour.
        tilemap.SetColor(position, colour);
    }
}
