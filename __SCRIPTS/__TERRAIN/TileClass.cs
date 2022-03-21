using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vitor Felipe Ramos Mello
/// Studant of Computer Engenering in University Of Vale Of Itajai UNIVALI
/// Santa Catarina - BRAZIL - Itajai
/// Codname VDEV
/// GanjGameSudio
/// GanjGameStudio, GGS and VDEV
/// 
/// They are authentic brands, their use for commercial and distribution purposes is only allowed if the proper attribution
/// is made to the creator Vitor Felipe Ramos Mello. Failure to comply with the terms of reference will result in lawsuits!
/// 
/// </summary>

[CreateAssetMenu(fileName = "NewTileClass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    //public Sprite tileSprite;

    public Sprite[] tileSprites;
}
