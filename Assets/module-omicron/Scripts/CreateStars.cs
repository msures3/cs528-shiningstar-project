using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStars : MonoBehaviour
{
    public TextAsset starDataFile;
    public TextAsset constellationDataFile;
    public GameObject starPrefab;
    public List<StarData> stars = new List<StarData>();
    private Dictionary<int, StarData> starDictionary = new Dictionary<int, StarData>();

    // Start is called before the first frame update
    void Start()
    {
        ParseStarData();
        CreateStarObjects();
        ParseConstellationData();
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
        }
    }


    void ParseConstellationData()
    {
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

            if(count >= 20){break;}
            else
            {
                count++;
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
            // Ensure that LineRenderer is already attached to the star objects
            LineRenderer lineRenderer = constellationName.AddComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("LineRenderer component not found on star object.");
                return;
            }

            // Set positions of the line renderer
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

}

