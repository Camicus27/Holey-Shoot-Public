using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SaveData : ScriptableObject
{
    // Singleton Creation
    private static SaveData instance_;
    public static SaveData current
    {
        get
        {
            if (instance_ == null)
                instance_ = CreateInstance<SaveData>();
            return instance_;
        }
        set
        {
            instance_ = value;
        }
    }

    // Serializable data classes
    public TutorialData tutorialData = new TutorialData();
    public GameSettings gameSettings = new GameSettings();
    public PlayerData playerData = new PlayerData();
    public Statistics statistics = new Statistics();
    public LevelData levelData = new LevelData();
    public BulletData bulletData = new BulletData();


    [Serializable]
    public class TutorialData
    {
        // Tutorial Information
        // Redacted for privacy
    }


    [Serializable]
    public class GameSettings
    {
        public float cameraSens = 4.69f;
        public bool isZoomedOut;
        public bool isMusicOn = true;
        public bool isSFXOn = true;
        public bool isRightHanded = true;
        public bool oneHandedMode = false;
        public float preferredAutoFireRate = 1f;

        // Crosshairs
        public int crosshairID = 0;
        public float crosshairR = 0.25f;
        public float crosshairG = 0.25f;
        public float crosshairB = 0.25f;
        public float crosshairA = 1;

        /// <summary>
        /// Change the color to the given color.
        /// </summary>
        public void SetColor(Color color)
        {
            crosshairR = color.r;
            crosshairG = color.g;
            crosshairB = color.b;
            crosshairA = color.a;
        }

        /// <summary>
        /// Return the current crosshair color.
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return new Color(crosshairR, crosshairG, crosshairB, crosshairA);
        }
    }


    [Serializable]
    public class PlayerData
    {
        // Player Information
        // Redacted for privacy
    }


    [Serializable]
    public class Statistics
    {
        // Stastics and Metadata Information
        // Redacted for privacy
    }


    [Serializable]
    public class LevelData
    {
        private float RADIUS;
        private float EDGE;
        public List<Spawnpoint> allPoints;
        public List<Spawnpoint> expandablePoints;

        //public HashSet<Spawnpoint> allPoints;
        //public HashSet<Spawnpoint> expandablePoints;

        public LevelData()
        {
            // Constants
            RADIUS = 17.5f;
            EDGE = RADIUS * 0.5f * Mathf.Sqrt(3);

            // Initialize points with 4 locked areas and 4 spawnable areas
            allPoints = new List<Spawnpoint>
            {
                new Spawnpoint(new Vector2Int(0, 0), new Vector3(0, 0, EDGE)),
                new Spawnpoint(new Vector2Int(0, 1), new Vector3(1.5f * RADIUS, 0, 0)),
                new Spawnpoint(new Vector2Int(0, -1), new Vector3(-1.5f * RADIUS, 0, 0)),
                new Spawnpoint(new Vector2Int(-1, 0), new Vector3(0, 0, -EDGE)),
                new Spawnpoint(new Vector2Int(1, 1), new Vector3(1.5f * RADIUS, 0, 2 * EDGE)),
                new Spawnpoint(new Vector2Int(-1, 1), new Vector3(1.5f * RADIUS, 0, -2 * EDGE)),
                new Spawnpoint(new Vector2Int(-1, -1), new Vector3(-1.5f * RADIUS, 0, -2 * EDGE)),
                new Spawnpoint(new Vector2Int(1, -1), new Vector3(-1.5f * RADIUS, 0, 2 * EDGE))
            };
            expandablePoints = new List<Spawnpoint>(allPoints);
        }


        /// <summary>
        /// Add a spawnpoint to the possible spawn positions.
        /// </summary>
        /// <param name="point">Point to add</param>
        public void AddSpawnpoint(Spawnpoint point)
        {
            if (!allPoints.Contains(point))
                allPoints.Add(point);
            if (!expandablePoints.Contains(point))
                expandablePoints.Add(point);
        }


        /// <summary>
        /// Get all of the possible world position spawnpoints.
        /// </summary>
        /// <returns>List of all world positions</returns>
        public List<Vector3> GetAllSpawnpointPositions()
        {
            List<Vector3> result = new List<Vector3>();
            List<Vector2Int> forbiddenSpawns = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0)
            };

            foreach (Spawnpoint point in allPoints)
            {
                if (!forbiddenSpawns.Contains(point.coords))
                    result.Add(point.pos);
            }

            return result;
        }


        /// <summary>
        /// Get all of the possible spawnpoints.
        /// </summary>
        /// <param name="includeForbidden">Whether or not to include the locked spawnpoints</param>
        /// <returns>List of all spawnpoints</returns>
        public List<Spawnpoint> GetAllSpawnpoints(bool includeForbidden)
        {
            List<Spawnpoint> result = new List<Spawnpoint>();
            
            if (includeForbidden)
            {
                foreach (Spawnpoint point in allPoints)
                {
                    result.Add(point);
                }
            }
            else
            {
                List<Vector2Int> forbiddenSpawns = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, 0)
                };

                foreach (Spawnpoint point in allPoints)
                {
                    if (!forbiddenSpawns.Contains(point.coords))
                        result.Add(point);
                }
            }

            return result;
        }
    }


    [Serializable]
    public class BulletData
    {
        // Bullet Information
        // Redacted for privacy
    }


    /// <summary>
    /// Attempts to write a save file to persistent data path.
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Save()
    {
        string saveDataPath = Application.persistentDataPath + "/game.sg";

        try
        {
            string data = JsonUtility.ToJson(current);
            File.WriteAllText(saveDataPath, data.Normalize());
            return true;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Attempts to load a save file from persistent data path.
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Load()
    {
        string saveDataPath = Application.persistentDataPath + "/game.sg";

        if (File.Exists(saveDataPath))
        {
            string data = File.ReadAllText(saveDataPath);
            JsonUtility.FromJsonOverwrite(data, current);
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Attempts to delete a save file from persistent data path.
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Delete()
    {
        string saveDataPath = Application.persistentDataPath + "/game.sg";

        if (!File.Exists(saveDataPath))
            return false;
        else
        {
            File.Delete(saveDataPath);
            current = null;
            return true;
        }
    }
}
