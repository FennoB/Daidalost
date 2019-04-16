using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FieldType
{
    FT_StaticWall = 0,
    FT_DynamicWall,
    FT_Air
}

public class LabField
{
    public int x = 0, y = 0;
    public FieldType type = FieldType.FT_Air;
    public GameObject obj = null;

    public LabField(int x, int y, FieldType type, GameObject obj)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.obj = obj;
    }
}

public class Labyrinth
{
    List<List<LabField>> field = null;
    GameObject wallPrefab;
    GameObject world;

    public void StartGenerate(int startSizeX, int startSizeY, GameObject world)
    {
        this.world = world;
        wallPrefab = world.GetComponent<LabGenerator>().wallPrefab;

        // Grundwelt erzeugen
        field = new List<List<LabField>>();
        for (int x = 0; x < startSizeX; ++x)
        {
            field.Add(new List<LabField>());
            for (int y = 0; y < startSizeY; ++y)
            {
                FieldType type = FieldType.FT_DynamicWall;

                if (x % 2 == 1 && y % 2 == 1)
                {
                    type = FieldType.FT_StaticWall;
                }
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    type = FieldType.FT_Air;
                }

                GameObject obj = null;

                if (type == FieldType.FT_StaticWall || type == FieldType.FT_DynamicWall)
                {
                    obj = GameObject.Instantiate(wallPrefab);
                    obj.transform.SetParent(world.transform);
                    obj.transform.position = new Vector3(x * 4, 1, y * 4);
                    obj.SetActive(type == FieldType.FT_StaticWall);
                }

                field[x].Add(new LabField(x, y, type, obj));
            }
        }

        for(int x = 0; x < startSizeX / 20; ++x)
        {
            for(int y = 0; y < startSizeY / 20; ++y)
            {
                GenerateChunk(x * 20, y * 20, 20);
            }
        }
    }
        
    bool GetChunkState(int chunkX, int chunkY, int chunkSize)
    {
        // Enthält false, falls ein abgetrennter Raum gefunden wurde
        bool result = true;

        List<List<int>> bufferstate = new List<List<int>>();

        for(int x = 0; x < chunkSize / 2; ++x)
        {
            bufferstate.Add(new List<int>());
            for(int y = 0; y < chunkSize / 2; ++y)
            {
                // Alles kommt erstmal auf 10000
                bufferstate[x].Add(10000);

                // Äußer die Eingänge
                if(x == chunkSize / 4 && y == chunkSize / 4)
                {
                    bufferstate[x][y] = 0;
                }
                
            }
        }
        
        // Oft genug iterieren
        for(int i = 0; i < chunkSize / 2 * 5; ++i)
        {
            // Wir nehmen zu Beginn der Iteration einen fehlerlosen Zustand an und suchen dann nach Fehlern
            result = true;

            for (int x = 0; x < chunkSize / 2; ++x)
            {
                for (int y = 0; y < chunkSize / 2; ++y)
                {
                    if (bufferstate[x][y] == 10000)
                    {
                        result = false;
                    }

                    if (x > 0 && bufferstate[x - 1][y] + 1 < bufferstate[x][y])
                    {
                        // Links
                        if (!field[chunkX + 2 * x - 1][chunkY + 2 * y].obj.activeSelf)
                        {
                            bufferstate[x][y] = bufferstate[x - 1][y] + 1;
                        }
                    }
                    
                    if (y > 0 && bufferstate[x][y - 1] + 1 < bufferstate[x][y])
                    {
                        // Hinten
                        if (!field[chunkX + 2 * x][chunkY + 2 * y - 1].obj.activeSelf)
                        {
                            bufferstate[x][y] = bufferstate[x][y - 1] + 1;
                        }
                    }

                    if (x < chunkSize / 2 - 1 && bufferstate[x + 1][y] + 1 < bufferstate[x][y])
                    {
                        // Rechts
                        if (!field[chunkX + 2 * x + 1][chunkY + 2 * y].obj.activeSelf)
                        {
                            bufferstate[x][y] = bufferstate[x + 1][y] + 1;
                        }
                    }

                    if (y < chunkSize / 2 - 1 && bufferstate[x][y + 1] + 1 < bufferstate[x][y])
                    {
                        // Vorne
                        if (!field[chunkX + 2 * x][chunkY + 2 * y + 1].obj.activeSelf)
                        {
                            bufferstate[x][y] = bufferstate[x][y + 1] + 1;
                        }
                    }
                }
            }
        }
        return result;
    }

    void GenerateChunk(int chunkX, int chunkY, int chunkSize)
    {
        GameObject last = null;
        
        int counter = 0;

        while(counter < 100)
        {
            int x = Random.Range(0, chunkSize / 2) * 2;
            int y = Random.Range(0, chunkSize / 2) * 2;

            if(Random.Range(0f, 1f) > 0.5f)
            {
                x += 1;
            }
            else
            {
                y += 1;
            }

            last = field[chunkX + x][chunkY + y].obj;
            last.SetActive(true);

            if (!GetChunkState(chunkX, chunkY, chunkSize))
            {
                counter++;
                last.SetActive(false);
            }
        }

        last.SetActive(false);

        //for (int x = 0; x < chunkSize / 2; ++x)
        //{
        //    for (int y = 0; y < chunkSize / 2; ++y)
        //    {
        //        last = field[chunkX + x * 2 + 1][chunkY + y * 2].obj;
        //        last.SetActive(true);
        //        if (!GetChunkState(chunkX, chunkY, chunkSize))
        //        {
        //            last.SetActive(false);
        //        }

        //        last = field[chunkX + x * 2][chunkY + y * 2 + 1].obj;
        //        last.SetActive(true);
        //        if (!GetChunkState(chunkX, chunkY, chunkSize))
        //        {
        //            last.SetActive(false);
        //        }
        //    }
        //}
    }
}

public class LabGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject enemyPrefab;
    public Labyrinth lab;

    // Start the generation
    void Start()
    {
        lab = new Labyrinth();
        lab.StartGenerate(100, 100, gameObject);

        GetComponent<NavMeshSurface>().BuildNavMesh();

        for(int i = 0; i < 50; ++i)
        {
            GameObject.Instantiate(enemyPrefab).transform.position = new Vector3(Random.value * 400, 0, Random.value * 400);
        }
    }
    
}
