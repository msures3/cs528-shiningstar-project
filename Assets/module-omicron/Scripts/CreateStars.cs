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
    public TMP_Text distanceText;

    // Start is called before the first frame update
    void Start()
    {
        ParseStarData();
        CreateStarObjects();
        // constellationDataFile = toggleFiles();
        ParseConstellationData();
        ParseExoplanetData();
        initialUserPosition = camera_CAVE.transform.position;
        initialUserRotation = camera_CAVE.transform.rotation; 
        // RecolorStarsBasedOnExoplanets();
    }
    void Update()
    {
        // Calculate the distance between the user (camera) and Sol
        float distance = Vector3.Distance(camera_CAVE.transform.position, initialUserPosition);
        float distanceInParsecs = camera_CAVE.transform.position.magnitude; // / 3.0856776e16f; // Convert distance to parsecs

        // Debug.Log("Distance: " + distanceInParsecs);
        // Display the distance in parsecs
        distanceText.text = "Distance from Sol: " + distanceInParsecs.ToString("F2") + " parsecs";
    }
    public void ResetLocationAndOrientation(){
        camera_CAVE.transform.position = initialUserPosition; 
        camera_CAVE.transform.rotation = initialUserRotation; 
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

                        // Add star to list and dictionary
                        stars.Add(star);
                        starDictionary.Add(star.hip, star);
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
            // Instantiate star prefab
            GameObject starObject = Instantiate(starPrefab, starData.position, Quaternion.LookRotation(starData.position));
            AttachLineRenderer(starObject);
            starData.starObject = starObject;
            starObject.GetComponent<StarDataMonoBehavior>().data = starData;
            

            // Set star properties (position, size, color, etc.)
            starObject.transform.localScale *= 0.3f * starData.absoluteMagnitude;
            // starObject.transform.localScale = Vector3.one * Mathf.Pow(10, starData.absoluteMagnitude / 2);
            // Vector3.one * Mathf.Pow(10, -starData.absoluteMagnitude / 2); // Size based on absolute magnitude

            // Adjust position based on declination
            starObject.transform.position = new Vector3(starData.position.x, starData.position.y, starData.position.z);

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
            starObject.GetComponent<Renderer>().material.color = starColor;
            originalStarColors.Add(starData.hip, starColor);
        }
    }
    void ParseConstellationData()
    {
        // constellationDataFile = selectedFile;
        string[] lines = constellationDataFile.text.Split('\n');
        int count = 0;

        foreach (string line in lines)
        {
            string[] values = line.Trim().Split(' ');
            string constellationName = values[0];
            
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
                        DrawLineBetweenStars(star1, star2);
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
    void DrawLineBetweenStars(GameObject star1, GameObject star2)
    {
        StarDataMonoBehavior starMono1 = star1.GetComponent<StarDataMonoBehavior>();
        StarDataMonoBehavior starMono2 = star2.GetComponent<StarDataMonoBehavior>();

        if (starMono1 != null && starMono2 != null)
        {
            GameObject constellationName = new GameObject();

            LineRenderer lineRenderer = constellationName.AddComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("LineRenderer component not found on star object.");
                return;
            }

            lineRenderer.positionCount = 2;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = Color.white;
            // lineRenderer.material = new Material(Shader.Find("Standard"));

            // Set line width and color
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.SetPosition(0, starMono1.transform.position);
            lineRenderer.SetPosition(1, starMono2.transform.position);
            Debug.Log("drawing line between" + starMono1.transform.position + " " + starMono2.transform.position);

            // lineRenderer.material.color = Color.magenta;
        }
        else
        {
            Debug.LogError($"Error in drawing lines for {starMono1} and {starMono2}");
        }
    }
    void AttachLineRenderer(GameObject starObject)
    {
        // Check if LineRenderer is already attached
        LineRenderer lineRenderer = starObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            // If LineRenderer is not attached, add it
            lineRenderer = starObject.AddComponent<LineRenderer>();
        }

        // Set LineRenderer properties
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.white;
    }
    void ParseExoplanetData()
    {
        string[] lines = exoplanetDataFile.text.Split('\n');
        bool headingsParsed = false;

        foreach (string line in lines)
        {
            if(headingsParsed){
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
            if(!colorChanged){
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

            else{
                if(originalStarColors.ContainsKey(starData.hip)){
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

}