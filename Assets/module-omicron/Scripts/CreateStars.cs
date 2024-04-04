using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ExoplanetData
{
    public string name;
    public int numPlanets;
    public string spectralClass;

    public ExoplanetData(string name, int numPlanets, string spectralClass)
    {
        this.name = name;
        this.numPlanets = numPlanets;
        this.spectralClass = spectralClass;
    }
}

public class CreateStars : MonoBehaviour
{
    public TextAsset starDataFile;
    public TextAsset constellationDataFile;
    public TextAsset exoplanetDataFile;
    // public TextAsset selectedFile;
    public GameObject starPrefab;
    public List<StarData> stars = new List<StarData>();
    private Dictionary<int, StarData> starDictionary = new Dictionary<int, StarData>();
    private Dictionary<int, int> exoplanetCounts = new Dictionary<int, int>();
    private Dictionary<int, Color> originalStarColors = new Dictionary<int, Color>();
    public bool colorChanged = false;
    public GameObject camera_CAVE;
    public static Vector3 initialUserPosition;
    public static Quaternion initialUserRotation;
    public TextMeshProUGUI distanceText;
    public static GameObject constellationSet;
    public float maxVisibleDistance = 20;
    public Vector3 lastRendererPosition;
    // public float moveSpeed = 100; // Adjust the speed of movement
    // public Vector3 moveDirection = Vector3.forward; // Adjust the direction of movement
    // private List<GameObject> activeStars = new List<GameObject>();
    // public float maxDistanceLightYears = 100;

    void Start()
    {
        constellationSet = new GameObject("constellations");
        lastRendererPosition = camera_CAVE.transform.position;
        //Debug.Log(lastRendererPosition);
        ParseStarData();
        Debug.Log(stars.Count);
        // UpdateStarVisibility();
        CreateStarObjects();
        ParseConstellationData();
        ParseExoplanetData();
        initialUserPosition = camera_CAVE.transform.position;
        initialUserRotation = camera_CAVE.transform.rotation;
        // RecolorStarsBasedOnExoplanets();
    }
    void Update()
    {
        // Debug.Log("Update method called.");

        // Distance
        float userDistance = Vector3.Distance(camera_CAVE.transform.position, initialUserPosition);
        distanceText.text = "Distance from Sol: " + userDistance.ToString("F2") + " parsecs";

        if (Vector3.Distance(camera_CAVE.transform.position, lastRendererPosition) >= 6)
        {
            lastRendererPosition = camera_CAVE.transform.position;
            // Debug.Log("Entered if statement " + lastRendererPosition);

            foreach (StarData starData in stars)
            {
                float starDistance = Vector3.Distance(starData.starObject.transform.position, camera_CAVE.transform.position);
                starData.distance = starDistance;
                if (starData.distance < maxVisibleDistance)
                {
                    starData.starObject.SetActive(true);
                }
                else
                {
                    starData.starObject.SetActive(false);
                }
                // starData.starObject.GetComponent<MeshRenderer>().enabled = starDistance <= maxVisibleDistance;
            }

        }


        // foreach (StarData starData in stars)
        // {
        //     Vector3 moveVector = starData.velocity * 10000 * Time.deltaTime;
        //     Debug.Log("Move vector: " + moveVector);
        //     starData.position += moveVector;
        //     starData.starObject.transform.position = starData.position;
        // }

        //     if(Vector3.Distance(camera_CAVE.transform.position, lastRendererPosition) >= 6){
        //         lastRendererPosition = camera_CAVE.transform.position;
        //         float starDistance = Vector3.Distance(starData.starObject.transform.position, lastRendererPosition);
        //         starData.distance = starDistance;
        //         if (starData.distance < maxVisibleDistance){
        //             starData.starObject.SetActive(true);
        //         }
        //         else{
        //             starData.starObject.SetActive(false);
        //         }
        //     }
        // }

        // }
    }

    // void GenerateStarDynamic(StarData starData)
    //     {
    //         GameObject instance = Instantiate(starPrefab, starData.position, Quaternion.LookRotation(starData.position));
    //         MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>();
    //         LineRenderer lineRenderer = instance.GetComponent<LineRenderer>() ?? instance.AddComponent<LineRenderer>();

    //         Color starColor;
    //                 switch (starData.spect[0]) 
    //                 {
    //                     case 'O':
    //                         starColor = Color.blue;
    //                         break;
    //                     case 'B':
    //                         starColor = new Color(0.67f, 0.89f, 1f);  // Bluish white
    //                         break;
    //                     case 'A':
    //                         starColor = Color.white;
    //                         break;
    //                     case 'F':
    //                         starColor = new Color(1f, 1f, 0.75f);  // Yellowish white
    //                         break;
    //                     case 'G':
    //                         starColor = Color.yellow;
    //                         break;
    //                     case 'K':
    //                         starColor = new Color(1f, 0.65f, 0.35f);  // Light orange
    //                         break;
    //                     case 'M':
    //                         starColor = new Color(1f, 0.55f, 0.41f);  // Orangish red
    //                         break;
    //                     default:
    //                         starColor = Color.white;
    //                         break;
    //                 }
    //                 instance.GetComponent<MeshRenderer>().material.color = starColor;
    //                 initialStarColors[starData.hip] = starColor;

    //         starData.starObject = instance;
    //         StarDataMonobehaviour starMonobehaviour = instance.GetComponent<StarDataMonobehaviour>();
    //         starMonobehaviour.data = starData;
    //     }


    void ParseStarData()
    {
        // Parsing star data
        string[] lines = starDataFile.text.Split('\n');
        bool headingsParsed = false;

        foreach (string line in lines)
        {
            if (headingsParsed)
            {
                string[] values = line.Trim().Split(',');


                if (values.Length >= 11)
                {
                    StarData star = new StarData();
                    try
                    {
                        // float newDistance = float.Parse(values[1]);
                        // if (newDistance <= maxVisibleDistance){
                        // Parse the hip value as a float then convert to int to handle decimal numbers
                        star.hip = (int)float.Parse(values[0]);
                        star.distance = float.Parse(values[1]);
                        star.position = new Vector3(
                            float.Parse(values[2]),
                            float.Parse(values[3]),
                            float.Parse(values[4]));
                        star.absoluteMagnitude = float.Parse(values[6]);
                        star.relativeMagnitude = float.Parse(values[5]);
                        star.velocity = new Vector3(
                            float.Parse(values[7]),
                            float.Parse(values[8]),
                            float.Parse(values[9]));
                        star.spect = values[10];
                        
                        Debug.Log(star.velocity);
                        stars.Add(star);
                        starDictionary[star.hip] = star;
                        // if(star.distance <= maxVisibleDistance){
                        // CreateStarObjects();
                        // }
                    }
                    catch (System.FormatException e)
                    {
                        Debug.LogError($"Error parsing line: {line}\nError message: {e.Message}");
                        continue;
                    }
                }
            }
            else
            {
                headingsParsed = true;
            }
        }
    }

    void CreateStarObjects()
    {
        foreach (StarData starData in stars)
        {
            // if (Vector3.Distance(starData.position, camera_CAVE.transform.position) <= 100)
            // {
            // Instantiate star prefab
            GameObject starObject = Instantiate(starPrefab, starData.position, Quaternion.LookRotation(starData.position));
            // GameObject starObject = Instantiate(starPrefab, starData.position, Quaternion.identity);

            // AttachLineRenderer(starObject);
            starData.starObject = starObject;
            // starObject.GetComponent<StarDataMonoBehavior>().data = starData;


            // Set star properties (position, size, color, etc.)
            starData.starObject.transform.localScale *= 0.3f * starData.absoluteMagnitude;

            // Adjust position based on declination
            starData.starObject.transform.position = new Vector3(starData.position.x, starData.position.y, starData.position.z);

            // Set star color based on spectral class (for simplicity, using a basic color mapping)
            Color starColor = Color.white;
            switch (starData.spect[0])
            {
                case 'O':
                    starColor = Color.blue;
                    break;
                case 'B':
                    starColor = new Color(170f / 255f, 191f / 255f, 255f / 255f, 1f);
                    break;
                case 'A':
                    starColor = Color.white;
                    break;
                case 'F':
                    starColor = new Color(248f / 255f, 247f / 255f, 255f / 255f, 1f);
                    break;
                case 'G':
                    starColor = new Color(255f / 255f, 244f / 255f, 234f / 255f, 1f);
                    break;
                case 'K':
                    starColor = new Color(255f / 255f, 210f / 255f, 161f / 255f, 1f);
                    break;
                case 'M':
                    starColor = new Color(255f / 255f, 204f / 255f, 111f / 255f, 1f);
                    break;
            }
            starData.starObject.GetComponent<Renderer>().material.color = starColor;
            originalStarColors.Add(starData.hip, starColor);

            if (starData.distance <= maxVisibleDistance)
            {
                starData.starObject.SetActive(true);
            }
            else
            {
                starData.starObject.SetActive(false);
            }
            // }

            // to get rid of billboard effect, but it needs to be updated based on camera movement for each star
            // starObject.transform.LookAt(camera_CAVE.transform.position, Vector3.up);
        }
    }
    void ParseConstellationData()
    {
        if (constellationDataFile == null)
        {
            Debug.LogError("Constellation data file is not assigned.");
            return;
        }

        string[] lines = constellationDataFile.text.Split('\n');
        int count = 0;

        foreach (string line in lines)
        {
            string[] values = line.Trim().Split(' ');

            GameObject constellationName = new GameObject();
            constellationName.name = values[0];

            constellationName.transform.parent = constellationSet.transform;

            // Store pairs of values starting from index 2
            List<int> hipPairs = new List<int>();
            for (int i = 2; i < values.Length; i++)
            {
                hipPairs.Add(int.Parse(values[i]));
            }
            try
            {
                // Process pairs of values as needed
                for (int i = 0; i < hipPairs.Count; i += 2)
                {
                    int hip1 = hipPairs[i];
                    int hip2 = hipPairs[i + 1];

                    if (starDictionary.ContainsKey(hip1) && starDictionary.ContainsKey(hip2))
                    {
                        GameObject star1 = starDictionary[hip1].starObject;
                        GameObject star2 = starDictionary[hip2].starObject;
                        // Debug.Log()
                        // StarDataMonoBehavior starMono1 = star1.GetComponent<StarDataMonoBehavior>();
                        // StarDataMonoBehavior starMono2 = star2.GetComponent<StarDataMonoBehavior>();

                        if (star1 != null && star2 != null)
                        {
                            // if (star1.activeSelf && star2.activeSelf)
                            // {
                                GameObject constellationLine = new GameObject();
                                LineRenderer lineRenderer = constellationLine.AddComponent<LineRenderer>();

                                constellationLine.transform.parent = constellationName.transform;

                                GameObject starLines = new GameObject();

                                if (lineRenderer == null)
                                {
                                    Debug.LogError("LineRenderer component not found on star object.");
                                    return;
                                }

                                lineRenderer.positionCount = 10;
                                lineRenderer.numCapVertices = 0;
                                lineRenderer.numCornerVertices = 0;
                                lineRenderer.useWorldSpace = true;

                                lineRenderer.positionCount = 2;
                                lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
                                lineRenderer.material.color = Color.white;
                                // lineRenderer.material = new Material(Shader.Find("Standard"));

                                // Set line width and color
                                lineRenderer.startWidth = 0.1f;
                                lineRenderer.endWidth = 0.1f;
                                lineRenderer.SetPosition(0, star1.transform.position);
                                lineRenderer.SetPosition(1, star2.transform.position);
                                Debug.Log("drawing line between" + star1.transform.position + " " + star2.transform.position);
                            // }
                            // lineRenderer.material.color = Color.magenta;
                        }
                        else
                        {
                            Debug.LogError($"Error in drawing lines for {star1} and {star2}");
                        }
                    }
                }
            }
            catch (System.FormatException e)
            {
                Debug.LogError($"Error parsing line: {line}\nError message: {e.Message}");
                continue;
            }
        }
    }
    // void AttachLineRenderer(GameObject starObject)
    // {
    //     // Check if LineRenderer is already attached
    //     LineRenderer lineRenderer = starObject.GetComponent<LineRenderer>();
    //     if (lineRenderer == null)
    //     {
    //         // If LineRenderer is not attached, add it
    //         lineRenderer = starObject.AddComponent<LineRenderer>();
    //     }

    //     // Set LineRenderer properties
    //     lineRenderer.startWidth = 0.1f;
    //     lineRenderer.endWidth = 0.1f;
    //     lineRenderer.material.color = Color.white;
    // }
    void ParseExoplanetData()
    {
        string[] lines = exoplanetDataFile.text.Split('\n');
        bool headingsParsed = false;

        foreach (string line in lines)
        {
            if (headingsParsed)
            {
                string[] values = line.Trim().Split(',');

                if (values.Length >= 2)
                {
                    try
                    {
                        int hip = int.Parse(values[0]);
                        int numExoplanets = int.Parse(values[1]);

                        // Debug.Log("Issue is here: " + hip);

                        if (starDictionary.ContainsKey(hip))
                        {
                            if (exoplanetCounts.ContainsKey(hip))
                            {
                                exoplanetCounts[hip] += numExoplanets;
                            }
                            else
                            {
                                exoplanetCounts.Add(hip, numExoplanets);
                            }
                        }
                    }
                    catch (System.FormatException e)
                    {
                        Debug.LogError($"Error parsing line: {line}\nError message: {e.Message}");
                        continue;
                    }
                }
            }
            else
            {
                headingsParsed = true;
            }
        }
    }
    public void ChangeStarColors()
    {
        foreach (StarData starData in stars)
        {
            if (!colorChanged)
            {
                Color starColor = Color.white;

                // Modify star color based on the number of exoplanets
                if (exoplanetCounts.ContainsKey(starData.hip))
                {
                    int numExoplanets = exoplanetCounts[starData.hip];
                    starColor = GetColorForNumExoplanets(numExoplanets);
                }

                // Set the color of the star's material
                starData.starObject.GetComponent<Renderer>().material.color = starColor;
                // colorChanged = true;
            }

            else
            {
                if (originalStarColors.ContainsKey(starData.hip))
                {
                    starData.starObject.GetComponent<MeshRenderer>().material.color = originalStarColors[starData.hip];
                }
            }
            // Renderer starRenderer = starData.starObject.GetComponent<Renderer>();
            // if (starRenderer != null)
            // {
            //     starRenderer.material.color = starColor;
            // }
            // else
            // {
            //     Debug.LogError("Star object renderer not found.");
            // }
        }
        colorChanged = !colorChanged;
    }
    private Color GetColorForNumExoplanets(int numExoplanets)
    {
        // Define a color scheme for different numbers of exoplanets
        switch (numExoplanets)
        {
            case 1:
                return Color.blue; // Example color for 1 exoplanet
            case 2:
                return Color.green; // Example color for 2 exoplanets
            case 3:
                return Color.yellow; // Example color for 3 exoplanets
            case 4:
                return Color.red; // Example color for 4 exoplanets
            default:
                return Color.magenta; // Example color for 5+ exoplanets
        }
    }

    public void ResetLocationAndOrientation()
    {
        camera_CAVE.transform.position = initialUserPosition;
        camera_CAVE.transform.rotation = initialUserRotation;
    }


}