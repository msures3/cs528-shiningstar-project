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

public class ConstellationLine
{
    public int hip1;
    public int hip2;
    public LineRenderer lineRenderer;

    public ConstellationLine(int hip1, int hip2, LineRenderer lineRenderer)
    {
        this.hip1 = hip1;
        this.hip2 = hip2;
        this.lineRenderer = lineRenderer;
    }
}

public class CreateStars : MonoBehaviour
{
    // TextAssets - Files that are being loaded 
    public TextAsset starDataFile; // Star Data 
    public TextAsset ModernFile;  // Constellation Data
    public TextAsset IndianFile;
    public TextAsset NorseFile;
    public TextAsset InuitFile;
    public TextAsset EgyptianFile;
    public TextAsset SamoanFile;

    // public TextAsset selectedFile;

    public List<TextAsset> constellationFiles = new List<TextAsset>();
    public TextAsset exoplanetDataFile; // Exoplanet Data
    
    public GameObject starPrefab;
    public static GameObject constellationSet;
    public List<ConstellationLine> constellationLines = new List<ConstellationLine>();
    public GameObject constellationToggle;


    // Storing Values
    public List<StarData> stars = new List<StarData>(); // Parsed Star Data stored in List
    private Dictionary<int, StarData> starDictionary = new Dictionary<int, StarData>(); // Dictionary with HIP and star info for constellations
    private Dictionary<int, int> exoplanetCounts = new Dictionary<int, int>(); // Exoplanet Count, HIP and number of planets
    private Dictionary<int, Color> originalStarColors = new Dictionary<int, Color>();  // Star colors, original HIP with color
    public bool colorChanged = false;
    public GameObject camera_CAVE;
    public static Vector3 initialUserPosition;
    public static Quaternion initialUserRotation;
    public int movementDir = 1;

    // Text UI
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI elapsedTimeText;
    public Slider movementSpeedSlider;
    public Slider distanceScalingSlider;
    public float maxVisibleDistance = 10;
    public Vector3 lastRendererPosition;
    public bool starsMoving = false;
    private float yearsPassed = 0;
    public int speed;
    // public float moveSpeed = 100; // Adjust the speed of movement
    // public Vector3 moveDirection = Vector3.forward; // Adjust the direction of movement
    // private List<GameObject> activeStars = new List<GameObject>();
    // public float maxDistanceLightYears = 100;

    void Start()
    {
        constellationSet = new GameObject("constellations");
        lastRendererPosition = camera_CAVE.transform.position;
        constellationFiles.Add(ModernFile);
        constellationFiles.Add(IndianFile);
        constellationFiles.Add(NorseFile);
        constellationFiles.Add(InuitFile);
        constellationFiles.Add(EgyptianFile);
        constellationFiles.Add(SamoanFile);
        // startTime = Time.time;
        ParseStarData();
        // UpdateStarVisibility();
        CreateStarObjects();
        ParseConstellationData(0);
        ParseExoplanetData();
        initialUserPosition = camera_CAVE.transform.position;
        initialUserRotation = camera_CAVE.transform.rotation;
        // RecolorStarsBasedOnExoplanets();
    }
    void Update()
    {
        // Update Distance text on screen 
        float userDistance = Vector3.Distance(camera_CAVE.transform.position, initialUserPosition);
        distanceText.text = "Distance from Sol: " + userDistance.ToString("F2") + " parsecs";

        if (Vector3.Distance(camera_CAVE.transform.position, lastRendererPosition) >= 10)
        {
            lastRendererPosition = camera_CAVE.transform.position;
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
                // to get rid of billboard effect, but it needs to be updated based on camera movement for each star
                // starData.starObject.transform.LookAt(camera_CAVE.transform.position);
            }
            
        }

        // Update to move stars
        if(starsMoving){
            // if(!isStarMovementPaused){
            //     float elapsedTime = Time.time - startTime - pausedTime;
            //     UpdateElapsedTimeText(elapsedTime);
            // }
            // speed = setMovementSliderValue();
            setMovementSliderValue();
            foreach (StarData starData in stars)
            {
                starData.position += movementDir * starData.velocity * speed * Time.deltaTime;
                // If stars are visible, move them
                if(starData.distance < maxVisibleDistance)
                    starData.starObject.transform.position = starData.position;

            }

            UpdateConstellationLines();

            yearsPassed += movementDir * speed * Time.deltaTime;
            elapsedTimeText.text = "Time Elapsed: " + (int)yearsPassed + " years";
        }
    }

    public void setMovementSliderValue(){
        if (movementSpeedSlider != null)
        {
        //  Debug.Log("Slider value: " + movementSpeedSlider.value);
            switch(movementSpeedSlider.value){
                case 1:
                    speed = 100;
                    break;
                case 2:
                    speed = 500;
                    break;
                case 3:
                    speed = 1000;
                    break;
                case 4:
                    speed = 1500;
                    break;
                case 5:
                    speed = 2000;
                    break;
                default:
                    speed = 500;
                    break;
                    // return 500;
            }
        }
    }

    public void UpdateStarDistances(float distance)
    {
        float scaleFactor = distance / maxVisibleDistance; // Calculate the scaling factor

        // Loop through stars and update their positions based on the scaling factor
        foreach (StarData starData in stars)
        {
            // Calculate the new position relative to the camera based on the scaling factor
            Vector3 newPosition = (starData.position - camera_CAVE.transform.position) * scaleFactor + camera_CAVE.transform.position;

            // Update the star's position
            starData.position = newPosition;

            // Update the star object's position in the scene
            starData.starObject.transform.position = newPosition;
        }

        maxVisibleDistance = distance; // Update the maximum visible distance
    }

    public void setDistanceScale(){
        if (distanceScalingSlider != null)
        {
        //  Debug.Log("Slider value: " + movementSpeedSlider.value);
            switch(distanceScalingSlider.value){
                case 1:
                    UpdateStarDistances(10);
                    break;
                case 2:
                    UpdateStarDistances(30);
                    break;
                case 3:
                    UpdateStarDistances(50);
                    break;
                case 4:
                    UpdateStarDistances(70);
                    break;
                default:
                    UpdateStarDistances(10);
                    break;
            }
        }
    }

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
                        stars.Add(star);
                        starDictionary[star.hip] = star;
                        // if(star.distance <= maxVisibleDistance){
                        // CreateStarObjects();
                        // }
                    }
                    catch (System.FormatException e)
                    {
                        // Debug.LogError($"Error parsing line: {line}\nError message: {e.Message}");
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
        }
    }
    
    void ParseConstellationData(int index)
    {
        if (constellationFiles == null)
        {
            Debug.LogError("Constellation data file is not assigned.");
            return;
        }

        // if (constellationLines != null)
        // {
        //     // Destroy each ConstellationLine object in the list
        //     foreach (ConstellationLine line in constellationLines)
        //     {
        //         Destroy(line.gameObject);
        //     }

        //     // Clear the list
        //     constellationLines.Clear();
        // }
        
        // Clear the existing constellation lines
        ClearConstellationLines();

        // filename = toggleConstellations();
        
        // private TextAsset selectedFile = constellationFiles[index];
        
        string[] lines = constellationFiles[index].text.Split('\n');
        // string[] lines = constellationFiles[index].text.Split('\n');


        foreach (string line in lines)
        {
            string[] values = line.Trim().Split(' ');

            // GameObject constellationName = new GameObject();
            // constellationName.name = values[0];
            // constellationName.transform.parent = constellationSet.transform;

            // Store pairs of values starting from index 2
            for (int i = 2; i < values.Length; i += 2)
            {
                int hip1 = int.Parse(values[i]);
                int hip2 = int.Parse(values[i + 1]);

                if (starDictionary.ContainsKey(hip1) && starDictionary.ContainsKey(hip2))
                {
                    StarData starData1 = starDictionary[hip1];
                    StarData starData2 = starDictionary[hip2];

                    ConstellationLine constellationLine = CreateConstellationLine(starData1, starData2);
                    constellationLines.Add(constellationLine); // Add ConstellationLine object instead of LineRenderer
                }
            }
        }
    }

    ConstellationLine CreateConstellationLine(StarData star1, StarData star2)
    {
        string lineName = $"ConstellationLine_{star1.hip}-{star2.hip}"; // Naming convention

        GameObject constellationLineObject = new GameObject(lineName);
        LineRenderer lineRenderer = constellationLineObject.AddComponent<LineRenderer>();

        // GameObject constellationLineObject = new GameObject(lineName);
        constellationLineObject.transform.parent = constellationSet.transform;


        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.white;

        // Set the initial positions of the line
        lineRenderer.SetPosition(0, star1.position);
        lineRenderer.SetPosition(1, star2.position);

        ConstellationLine constellationLine = new ConstellationLine(star1.hip, star2.hip, lineRenderer);

        return constellationLine;
    }

    void UpdateConstellationLines()
    {
        foreach (ConstellationLine constellationLine in constellationLines)
        {
            // Check if both stars exist in the starDictionary
            if (starDictionary.ContainsKey(constellationLine.hip1) && starDictionary.ContainsKey(constellationLine.hip2))
            {
                // Get the StarData objects directly from the dictionary
                StarData starData1 = starDictionary[constellationLine.hip1];
                StarData starData2 = starDictionary[constellationLine.hip2];

                // Update line positions
                constellationLine.lineRenderer.SetPosition(0, starData1.position);
                constellationLine.lineRenderer.SetPosition(1, starData2.position);
            }
        }
    }

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
                        // Debug.LogError($"Error parsing line: {line}\nError message: {e.Message}");
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
        // yearsPassed = 0;
        
    }
    public void forwardStarMovement(){
        // movementDir = 1;
        starsMoving = !starsMoving;
        
    }

    // public void backwardStarMovement(){
    //     movementDir = -1; 
    //     starsMoving = !starsMoving;
    // }

    void ClearConstellationLines()
    {
        // Destroy all existing constellation GameObjects and clear the list
        foreach (Transform child in constellationSet.transform)
        {
            Destroy(child.gameObject);
        }
        constellationLines.Clear();
    }

    public void toggleConstellations(GameObject gameObject)
    {
        // Destroy(constellationLines);
        // constellationLines = new List<ConstellationLine>();
        // Debug.Log(gameObject.name);
        // Check if there's an existing constellation set
        // if (constellationSet != null)
        // {
        //     // Destroy each GameObject in the constellation set
        //     foreach (Transform child in constellationSet.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        ClearConstellationLines();

        switch(gameObject.name){
            case "ModernFile":

                ParseConstellationData(0);
                break;
            case "IndianFile":
                ParseConstellationData(1);
                break;
            case "NorseFile":
                ParseConstellationData(2);
                break;
            case "InuitFile":
                ParseConstellationData(3);
                break;
            case "EgyptianFile":
                ParseConstellationData(4);
                break;
            case "SamoanFile":
                ParseConstellationData(5);
                break;
            default:
                break;
        }

    }
}