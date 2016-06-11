﻿using UnityEngine;
using System.Collections;

public class ProceduralPlacement : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] bool m_showDebugSpheres = false;
    [SerializeField] int m_seed = 1;
    [SerializeField] int m_maxPlacementAttempts = 100;

    [Header("Main target")]
    [SerializeField] PlaceableObject m_mainTargetPrefab;
    [SerializeField] float m_minDistanceFromPlayer = 800f;
    [SerializeField] float m_maxDistanceFromPlayer = 1200f;

    [Header("Enemy aircraft")]
    [SerializeField] PlaceableObject[] m_aircraftTypePrefabss;
    [SerializeField] int m_numberOfAircraft = 10;

    [Header("Enemy ground defences")]
    [SerializeField] PlaceableObject[] m_groundDefenceTypePrefabs;
    [SerializeField] int m_numberOfGroundDefences = 20;

    private MapGenerator m_mapGenerator;
    private Vector3 m_player;
    private PlaceableObject m_mainTarget;


    void Awake()
    {
        var mapGeneratorObject = GameObject.FindGameObjectWithTag(Tags.MapGenerator);

        if (mapGeneratorObject != null)
            m_mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
    }


    void Start()
    {
        if (m_mapGenerator == null)
            return;

        Random.seed = m_seed;

        var playerObject = GameObject.FindGameObjectWithTag(Tags.Player);

        if (playerObject != null)
            m_player = playerObject.transform.position;

        PlaceMainTarget();
        PlaceEnemyAircraft();
    }


    private void PlaceMainTarget()
    {
        if (m_mainTargetPrefab == null)
        {
            print("No main target prefab defined");
            return;
        }

        var mainTargetObject = (GameObject) Instantiate(m_mainTargetPrefab.gameObject, Vector3.zero, Quaternion.identity);
        m_mainTarget = mainTargetObject.GetComponent<PlaceableObject>();
        m_mainTarget.gameObject.transform.parent = transform;

        bool success = false;
        int attempts = 1;

        while (!success && attempts <= m_maxPlacementAttempts)
        {
            //print(string.Format("Attempt {0}", attempts));
            success = TestPosition(m_mainTarget);
            attempts++;
        }

        if (attempts > m_maxPlacementAttempts)
        {
            print("Failed to place main target");
            Destroy(m_mainTarget);
        }
        else
            print(string.Format("Main target took {0} attempts to place", --attempts));
    }


    private void PlaceEnemyAircraft()
    {
        if (m_aircraftTypePrefabss.Length == 0)
        {
            print("No enemy aircraft prefabs defined");
            return;
        }
    }


    private void PlaceEnemyGroundDefences()
    {
        if (m_groundDefenceTypePrefabs == null)
        {
            print("No enemy ground defence prefabs defined");
            return;
        }
    }


    private bool TestPosition(PlaceableObject testPlaceableObject)
    {
        var testObject = testPlaceableObject.gameObject;
        float distance = Random.Range(m_minDistanceFromPlayer, m_maxDistanceFromPlayer);
        float rotationY = Random.Range(0f, 360f);
        
        var trialPosition = m_player + Vector3.forward * distance;
        var trialRotation = Quaternion.Euler(0, rotationY, 0);

        testObject.transform.position = trialPosition;
 
        //var rigidbody = testObject.GetComponent<Rigidbody>();

        var bounds = BoundsUtilities.OverallBounds(testObject);

        bool success = true;

        if (bounds != null)
        {
            float minY = bounds.Value.min.y;
            float minX = bounds.Value.min.x;
            float minZ = bounds.Value.min.z;
            float maxX = bounds.Value.max.x;
            float maxZ = bounds.Value.max.z;

            var corner1 = new Vector3(minX, minY, minZ);
            var corner2 = new Vector3(minX, minY, maxZ);
            var corner3 = new Vector3(maxX, minY, minZ);
            var corner4 = new Vector3(maxX, minY, maxZ);

            var originToCorner1 = corner1 - trialPosition;
            var originToCorner2 = corner2 - trialPosition;
            var originToCorner3 = corner3 - trialPosition;
            var originToCorner4 = corner4 - trialPosition;

            float originAboveBase = trialPosition.y - bounds.Value.min.y;

            originToCorner1 = trialRotation * originToCorner1;
            originToCorner2 = trialRotation * originToCorner2;
            originToCorner3 = trialRotation * originToCorner3;
            originToCorner4 = trialRotation * originToCorner4;

            corner1 = trialPosition + originToCorner1;
            corner2 = trialPosition + originToCorner2;
            corner3 = trialPosition + originToCorner3;
            corner4 = trialPosition + originToCorner4;

            float terrainHeightCorner1 = m_mapGenerator.GetTerrainHeight(corner1.x, corner1.z);
            float terrainHeightCorner2 = m_mapGenerator.GetTerrainHeight(corner2.x, corner2.z);
            float terrainHeightCorner3 = m_mapGenerator.GetTerrainHeight(corner3.x, corner3.z);
            float terrainHeightCorner4 = m_mapGenerator.GetTerrainHeight(corner4.x, corner4.z);

            float minHeight = Mathf.Min(terrainHeightCorner1, terrainHeightCorner2, terrainHeightCorner3, terrainHeightCorner4);
            float maxHeight = Mathf.Max(terrainHeightCorner1, terrainHeightCorner2, terrainHeightCorner3, terrainHeightCorner4);

            success = maxHeight < -originAboveBase;

            if (success)
            {
                testObject.transform.rotation = trialRotation;

                trialPosition.y = 0;
                testObject.transform.position = trialPosition;

                if (m_showDebugSpheres)
                    AddDebugSpheres(corner1, corner2, corner3, corner4, originAboveBase, terrainHeightCorner1,
                        terrainHeightCorner2, terrainHeightCorner3, terrainHeightCorner4, testObject.transform);
            }

            //float y = rigidbody == null
            //    ? Mathf.Min(terrainHeightCorner1, terrainHeightCorner2, terrainHeightCorner3, terrainHeightCorner4)
            //    : Mathf.Max(terrainHeightCorner1, terrainHeightCorner2, terrainHeightCorner3, terrainHeightCorner4);

            //if (y < 0f)
            //    Destroy(testObject);
            //else
            //{
            //    y += originAboveBase;

            
        }

        return success;
    }


    private void AddDebugSpheres(Vector3 corner1, Vector3 corner2, Vector3 corner3, Vector3 corner4,
        float originAboveBase, float terrainHeightCorner1, float terrainHeightCorner2,
        float terrainHeightCorner3, float terrainHeightCorner4, Transform parent)
    {
        corner1.y = -originAboveBase;
        corner2.y = -originAboveBase;
        corner3.y = -originAboveBase;
        corner4.y = -originAboveBase;

        var sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere1.transform.position = corner1;
        sphere2.transform.position = corner2;
        sphere3.transform.position = corner3;
        sphere4.transform.position = corner4;

        //print(string.Format("Pos: {0}, height: {1}", corner1, terrainHeightCorner1));
        //print(string.Format("Pos: {0}, height: {1}", corner2, terrainHeightCorner2));
        //print(string.Format("Pos: {0}, height: {1}", corner3, terrainHeightCorner3));
        //print(string.Format("Pos: {0}, height: {1}", corner4, terrainHeightCorner4));

        var sphere5 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere6 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere7 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var sphere8 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        corner1.y = terrainHeightCorner1;
        corner2.y = terrainHeightCorner2;
        corner3.y = terrainHeightCorner3;
        corner4.y = terrainHeightCorner4;

        sphere5.transform.position = corner1;
        sphere6.transform.position = corner2;
        sphere7.transform.position = corner3;
        sphere8.transform.position = corner4;

        sphere1.transform.parent = parent;
        sphere2.transform.parent = parent;
        sphere3.transform.parent = parent;
        sphere4.transform.parent = parent;
        sphere5.transform.parent = parent;
        sphere6.transform.parent = parent;
        sphere7.transform.parent = parent;
        sphere8.transform.parent = parent;
    }
}
