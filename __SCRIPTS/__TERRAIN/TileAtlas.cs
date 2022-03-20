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


[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    //Basic
    public TileClass grass;
    public TileClass dirt;
    public TileClass stone;
    public TileClass log;
    public TileClass leaf;

    //Miners
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
