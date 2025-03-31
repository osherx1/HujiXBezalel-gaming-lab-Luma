using System.Collections.Generic;
using UnityEngine;

namespace Sky
{
    public class TheSkyOldVersion:MonoBehaviour
    {
    
        [SerializeField] private GameObject skyObject;
        [SerializeField] private GameObject[] basePrefabs;
        [SerializeField] private Point pointsPrefabs;
        [SerializeField] private CloudComponent[] cloudsPrefabs;
        [SerializeField] private Trigger triggersPrefabs;
   
        private List<CloudComponent> _player1Clouds = new(); 
        private List<CloudComponent> _player2Clouds = new(); 
        private List<Point> _points = new(); 
    
        private List<Vector2> _startPositionsPlayer1 = new();// Store initial positions
        private List<Vector2> _startPositionsPlayer2 = new();// Store initial positions
        private  List<Vector2> _pointPosition  = new();
   
        //all the important positions.
        private void SetCloudLocations()
        {
            _startPositionsPlayer1 = new List<Vector2>
            {
                new Vector2(-7.4f,3.45f), new Vector2(-7.4f,0.65f), new Vector2(-7.4f,-1.93f),//col 1.
                new Vector2(-6.06f,3.6f),new Vector2(-6.06f,2.3f),new Vector2(-6.06f,1f),
                new Vector2(-6.06f,-0.27f),new Vector2(-6.06f,-1.6f), //col 2.
                new Vector2(-4.78f,2.92f),new Vector2(-4.78f,0.63f),new Vector2(-4.78f,-1.6f), //col 3.
                new Vector2(-3.31f,3.48f),new Vector2(-3.31f,2.18f),new Vector2(-3.31f,0.8799f),
                new Vector2(-3.31f,-0.390f),new Vector2(-3.31f,-1.72f), //col 4.
                new Vector2(-1.72f,3.45f), new Vector2(-1.72f,0.82f), new Vector2(-1.72f,-1.93f), //col 5.
            
            };
            _startPositionsPlayer2 = new List<Vector2>
            {
                new Vector2(1.720001f,3.45f), new Vector2(1.720001f,0.65f), new Vector2(1.720001f,-1.93f),//col 1.
                new Vector2(3.309762f,3.6f),new Vector2(3.309762f,2.3f),new Vector2(3.309762f,1f),
                new Vector2(3.309762f,-0.27f),new Vector2(3.309762f,-1.6f), //col 2.
                new Vector2(4.780004f,2.92f),new Vector2(4.780004f,0.63f),new Vector2(4.780004f,-1.6f), //col 3.
                new Vector2(6.059764f,3.48f),new Vector2(6.059764f,2.18f),new Vector2(6.059764f,0.8799f),
                new Vector2(6.059764f,-0.390f),new Vector2(6.059764f,-1.72f), //col 4.
                new Vector2(7.400005f,3.45f), new Vector2(7.400005f,0.82f), new Vector2(7.400005f,-1.93f), //col 5.
            };
            _pointPosition = new List<Vector2>
            {
                new Vector2(0.04f,2.69f), new Vector2(0.04f,1.5f), new Vector2(0.04f,0.24f),
                new Vector2(0.04f,-1.11f),new Vector2(0.04f,-2.38f)
            };
        }
        private void Awake()
        {
            SetCloudLocations();
            Creat();
            CreatClouds();
            CreatTriggers();
        }

        private void Creat()
        {
            if (skyObject != null)
            {
                GameObject sky = Instantiate(skyObject);
                sky.SetActive(true);
            }

            if (basePrefabs != null)
            {
                GameObject base1 = Instantiate(basePrefabs[0]);
                GameObject base2 = Instantiate(basePrefabs[1]);
                GameObject base3 = Instantiate(basePrefabs[2]);
                base1.SetActive(true);
                base2.SetActive(true);
                base3.SetActive(true);
            }
            if (pointsPrefabs != null) {
                for (int i = 0; i < 5; i++)
                {
                    Point point = Instantiate(pointsPrefabs,_pointPosition[i],Quaternion.identity);
                    _points.Add(point);
                }
            }
        }
        private void CreatClouds()
        {
            System.Random random = new System.Random(); // Random generator
            // We give each cloud its position, type, and a random color.
            for (int i = 0; i < 19; i++)
            {
                ColorType randomColorType = (ColorType)random.Next(0, 3); // Generate a random enum value (0, 1, or 2)
                // Instantiate a cloud at each position
                if (i < 3 || i >= 16)
                {
                    var cloud1 = Instantiate(cloudsPrefabs[0], _startPositionsPlayer1[i], Quaternion.identity);
                    _player1Clouds.Add(cloud1); 
                    cloud1.SetColor(randomColorType); // Set random colorType for cloud1

                    var cloud2 = Instantiate(cloudsPrefabs[0], _startPositionsPlayer2[i], Quaternion.identity);
                    _player2Clouds.Add(cloud2);
                    cloud2.SetColor(randomColorType); // Set random colorType for cloud2
                }
                if ((i >= 3 && i < 8) || (i >= 11 && i < 16))
                {
                    var cloud1 = Instantiate(cloudsPrefabs[1], _startPositionsPlayer1[i], Quaternion.identity);
                    _player1Clouds.Add(cloud1); 
                    cloud1.SetColor(randomColorType);

                    var cloud2 = Instantiate(cloudsPrefabs[1], _startPositionsPlayer2[i], Quaternion.identity);
                    _player2Clouds.Add(cloud2);
                    cloud2.SetColor(randomColorType);
                }

                if (i > 7 && i < 11)
                {
                    var cloud1 = Instantiate(cloudsPrefabs[2], _startPositionsPlayer1[i], Quaternion.identity);
                    _player1Clouds.Add(cloud1);
                    cloud1.SetColor(randomColorType);

                    var cloud2 = Instantiate(cloudsPrefabs[2], _startPositionsPlayer2[i], Quaternion.identity);
                    _player2Clouds.Add(cloud2);
                    cloud2.SetColor(randomColorType);
                }
            }
        }
        private void CreatTriggers()
        {
            System.Random random = new System.Random(); // Use a random generator
            ColorType[] colors = { ColorType.Blue, ColorType.Gold, ColorType.White }; 
        
            for (int i = 0; i < 3; i++)
            {
                // Pick a random cloud from _player1Clouds
                int randomIndex = random.Next(3, _player1Clouds.Count);
                CloudComponent selectedCloud = _player1Clouds[randomIndex];
                // Remove the selected cloud from the list to avoid duplication
                _player1Clouds.RemoveAt(randomIndex);
                // Instantiate a new Trigger at the position of the selected cloud
                Trigger newTrigger = Instantiate(triggersPrefabs);
                newTrigger.SetupCloud(selectedCloud.gameObject.transform);
                newTrigger.SetColor(colors[i]);
                // Link the trigger to a random selection of 4 clouds from Player 2
                GetRandomClouds(_player2Clouds, 4, random,colors[i]);
            }
        
            // Create Triggers for Player 2 that affect Player 1's clouds
            for (int i = 0; i < 3; i++)
            {
                // Pick a random cloud from _player2Clouds
                int randomIndex = random.Next(3, _player2Clouds.Count);
                CloudComponent selectedCloud = _player2Clouds[randomIndex];
                // Remove the selected cloud from the list to avoid duplication
                _player2Clouds.RemoveAt(randomIndex);
                // Instantiate a new Trigger at the position of the selected cloud
                Trigger newTrigger = Instantiate(triggersPrefabs);
                newTrigger.SetupCloud(selectedCloud.gameObject.transform);
                newTrigger.SetColor(colors[i]);
                // Link the trigger to a random selection of 4 clouds from Player 2
                GetRandomClouds(_player1Clouds, 4, random,colors[i]);
            }
        }

// This method selects a random subset of clouds
        private void GetRandomClouds(List<CloudComponent> cloudList, int count, System.Random random, ColorType colorType)
        {
            List<CloudComponent> selectedClouds = new List<CloudComponent>();

            // Create a filtered list of clouds that match the desired colorType
            List<CloudComponent> matchingClouds = cloudList.FindAll(cloud => cloud.GetColor() == colorType);

            // If there are not enough clouds of the desired colorType, limit the count
            if (matchingClouds.Count < count)
            {
                count = matchingClouds.Count;
            }

            // Shuffle the list to get random clouds
            for (int i = 0; i < matchingClouds.Count; i++)
            {
                int swapIndex = random.Next(i, matchingClouds.Count);
                (matchingClouds[i], matchingClouds[swapIndex]) = (matchingClouds[swapIndex], matchingClouds[i]);
            }

            // Add the first 'count' clouds to the selected list
            for (int i = 0; i < count; i++)
            {
                CloudComponent selectedCloud = matchingClouds[i];
                selectedClouds.Add(selectedCloud);
                selectedCloud.InTrigger();
                cloudList.Remove(selectedCloud); // Remove from the original list
            }
        }


    
    }
}
