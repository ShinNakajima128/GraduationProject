using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableGenerator : ObstacleGenerator
{

    protected override void Generate(int index)
    {
        int randomPos = Random.Range(0, _generateTrans.Length);

        _obstacleList[index].Use(_generateTrans[randomPos].position, c => 
        {
            c.GeneratePos = (TableGeneratePosition)randomPos + 1;
        });
    }
}
