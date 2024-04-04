using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveStars : MonoBehaviour
{
    private CreateStars createStars;
    // private Dictionary<int, StarData> starDictionary = new Dictionary<int, StarData>();
    public List<StarData> stars = new List<StarData>();


    // Start is called before the first frame update
    void Start()
    {
        createStars = FindObjectOfType<CreateStars>();
    }

    // Update is called once per frame
    void Update()
    {
        if(stars.Count == 0){
            stars = createStars.stars;
        }
        
        foreach (StarData starData in stars)
        {   
            starData.position += starData.velocity * 10000 * Time.deltaTime;
            Debug.Log("Moved: " + starData.position);
            starData.starObject.transform.position = starData.position;
        }
    }
}
