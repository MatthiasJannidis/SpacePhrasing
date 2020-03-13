using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMaker : MonoBehaviour
{
    GameObject[,] planets;
    [SerializeField] int planetCountPerCluster;
    [SerializeField] GameObject[] planetPrefabs;
    [SerializeField] Vector2 clusterDimensions;


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

    // Start is called before the first frame update
    void Start()
    {
        planets = new GameObject[9, planetCountPerCluster];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < planetCountPerCluster; j++)
            {
                planets[i, j] = Instantiate(planetPrefabs[Random.Range(0, planetPrefabs.Length)]);
                planets[i, j].transform.position = Vector3.zero;
            }
        }

        Vector2 clustersBottomLeft = -clusterDimensions * 1.5f;

        for (int y = 0; y < 3; y++) 
        {
            for(int x = 0; x < 3; x++) 
            {
                var origin = new Vector2(clustersBottomLeft.x + clusterDimensions.x * x, clustersBottomLeft.y + clusterDimensions.y * y);
                GenerateWithBlueNoise(origin, clusterDimensions, y*3+x);
            }
        }
    }

    //taken from https://twitter.com/jazzmickle/status/1237371977482014721/photo/2
    void GenerateWithBlueNoise(Vector2 origin, Vector2 dimensions, int clusterIndex) 
    {
        Random.InitState(origin.GetHashCode());
        List<Vector3> outputPlanets = new List<Vector3>();
        outputPlanets.Add(new Vector3(origin.x + Random.value * dimensions.x, origin.y + Random.value * dimensions.y));
        const int candidateCount = 20;

        Vector3[] candidates = new Vector3[candidateCount];
        float[] distances = new float[candidateCount];
        for (int i = 0; i < planetCountPerCluster; i++)
        {
            for (int j = 0; j < candidateCount; j++)
            {
                candidates[j] = new Vector3(origin.x + Random.value * dimensions.x, origin.y + Random.value * dimensions.y);
                float minDist = 10000000.0f;
                foreach (var planet in outputPlanets)
                {
                    float dist = Mathf.Abs(planet.x - candidates[j].x) + Mathf.Abs(planet.y- candidates[j].y);
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

        Vector3 playerPos = GameObject.FindObjectOfType<PlayerMovement>().transform.position;
        for (int i = 0; i < planetCountPerCluster; i++) 
        {
            if (Vector3.Distance(playerPos, outputPlanets[i]) > 2.0f) planets[clusterIndex, i].transform.position = outputPlanets[i];
            else planets[clusterIndex, i].transform.position = new Vector3(100000.0f,100000.0f);
        }
    }

}
