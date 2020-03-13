using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMaker : MonoBehaviour
{
    [SerializeField] int planetCountPerCluster;
    [SerializeField] GameObject[] planetPrefabs;
    [SerializeField] Vector2 clusterDimensions;
    Vector2Int currentCluster = Vector2Int.zero;
    PlanetCluster[,] clusters = new PlanetCluster[3,3];
    PlayerMovement player;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();

        //set up cluster variables
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                clusters[x, y] = new PlanetCluster();
                clusters[x, y].planets = new GameObject[planetCountPerCluster];
                for (int j = 0; j < planetCountPerCluster; j++)
                {
                    clusters[x, y].planets[j] = Instantiate(planetPrefabs[Random.Range(0, planetPrefabs.Length)]);
                    clusters[x, y].planets[j].transform.position = Vector3.zero;
                }
            }
        }


        //rng clusters
        Vector2 clustersBottomLeft = -clusterDimensions * 1.5f;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var origin = new Vector2(clustersBottomLeft.x + clusterDimensions.x * x, clustersBottomLeft.y + clusterDimensions.y * y);
                clusters[x, y].origin = origin;
                clusters[x, y].index = new Vector2Int(x-1, y-1);
                clusters[x, y].GenerateWithBlueNoise(clusterDimensions, planetCountPerCluster);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 clustersBottomLeft = -clusterDimensions * 1.5f;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var origin = new Vector2(clustersBottomLeft.x + clusterDimensions.x * x, clustersBottomLeft.y + clusterDimensions.y * y);
                Gizmos.DrawWireCube(origin+clusterDimensions*.5f, clusterDimensions);
            }
        }
    }

    private void Update()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        //check if still within current cluster boundary (with currentCluster index)
        //if not still within current cluster, generate new clusters where the player is going
        if(playerPos.x > (currentCluster.x) * clusterDimensions.x + clusterDimensions.x * .5f) 
        {
            OnXChanged(1);
        }
        else if (playerPos.x < currentCluster.x * clusterDimensions.x - clusterDimensions.x*.5f)
        {
            OnXChanged(-1);
        }

        if (playerPos.y > (currentCluster.y) * clusterDimensions.y + clusterDimensions.y * .5f)
        {
            onYChanged(1);
        }
        else if (playerPos.y < currentCluster.y * clusterDimensions.y - clusterDimensions.y * .5f)
        {
            onYChanged(-1);
        }
    }

    void OnXChanged(int increment) 
    {
        print("X changed : " + increment.ToString());
        int changedX = currentCluster.x - increment;
        currentCluster.x+= increment;
        for (int y = -1; y < 2; y++)
        {
            ShiftCluster(new Vector2Int(changedX, currentCluster.y + y), new Vector2Int(currentCluster.x + increment, currentCluster.y + y));
        }
    }

    void onYChanged(int increment) 
    {

        print("Y changed : " + increment.ToString());
        int changedY = currentCluster.y - increment;
        currentCluster.y+= increment;
        for (int x = -1; x < 2; x++)
        {
            ShiftCluster(new Vector2Int(currentCluster.x + x, changedY), new Vector2Int(currentCluster.x + x, currentCluster.y + increment));
        }
    }

    void ShiftCluster(Vector2Int from, Vector2Int to) 
    {
        print("Cluster shifted from : " + from.ToString() + " to : " + to.ToString());
        PlanetCluster currentCluster = null;
        for(int y = 0; y < 3; y++) 
        {
            for(int x = 0; x < 3; x++) 
            {
                if (clusters[x, y].index == from) currentCluster = clusters[x, y];
            }
        }

        Debug.Assert(currentCluster != null);

        currentCluster.index = to;
        Vector2 clustersBottomLeft = -clusterDimensions * .5f;

        currentCluster.origin = new Vector2(clustersBottomLeft.x + clusterDimensions.x * to.x, clustersBottomLeft.y + clusterDimensions.y * to.y);
        currentCluster.GenerateWithBlueNoise(clusterDimensions, planetCountPerCluster);
    }
}

public class PlanetCluster 
{
    const int candidateCount = 3;
    public Vector2 origin;
    public Vector2Int index;
    public GameObject[] planets;

    //taken from https://twitter.com/jazzmickle/status/1237371977482014721/photo/2
    public void GenerateWithBlueNoise(Vector2 dimensions, int count)
    {
        Random.InitState(index.GetHashCode());
        List<Vector3> outputPlanets = new List<Vector3>();
        outputPlanets.Add(new Vector3(origin.x + Random.value * dimensions.x, origin.y + Random.value * dimensions.y));
        

        Vector3[] candidates = new Vector3[candidateCount];
        float[] distances = new float[candidateCount];
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < candidateCount; j++)
            {
                candidates[j] = new Vector3(origin.x + Random.value * dimensions.x, origin.y + Random.value * dimensions.y);
                float minDist = 10000000.0f;
                foreach (var planet in outputPlanets)
                {
                    float dist = Mathf.Abs(planet.x - candidates[j].x) + Mathf.Abs(planet.y - candidates[j].y);
                    if (dist < minDist) minDist = dist;
                }
                distances[j] = minDist;
            }
            int bestCandidateIndex = 0;
            for (int j = 1; j < candidateCount; j++)
            {
                if (distances[bestCandidateIndex] < distances[j]) bestCandidateIndex = j;
            }
            outputPlanets.Add(candidates[bestCandidateIndex]);
        }

        Vector3 playerPos = Object.FindObjectOfType<PlayerMovement>().transform.position;
        for (int i = 0; i < count; i++)
        {
            if (Vector3.Distance(playerPos, outputPlanets[i]) > 2.0f) planets[i].transform.position = outputPlanets[i];
            else planets[i].transform.position = new Vector3(100000.0f, 100000.0f);
        }
    }
}