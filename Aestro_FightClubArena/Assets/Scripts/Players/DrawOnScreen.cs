using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Threading.Tasks;

public class DrawOnScreen : MonoBehaviour
{
    public static DrawOnScreen instance;

    public Material lineMaterial;
    public float lineWidth = 0.1f;
    public Camera strokesCamera;
    public float comparisonThreshold = 0.1f; // Adjust this to change sensitivity of comparison

    private LineRenderer currentLineRenderer;
    private List<Vector3> drawnPositions = new List<Vector3>();
    private bool isDrawing = false;


    public List<Texture2D> predefinedPattern;

    public TextMeshProUGUI text1;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    void Update()
    {
        // Check for mouse button down
        /*if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        // Check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }*/

        // If the mouse is being dragged, draw
        if (isDrawing)
        {
            Draw();
        }
    }

    public void StartDrawing()
    {
        isDrawing = true;

        // Create a new GameObject to hold the LineRenderer
        GameObject lineObj = new GameObject("Line");

        // Add a LineRenderer component and set its properties
        currentLineRenderer = lineObj.AddComponent<LineRenderer>();
        currentLineRenderer.gameObject.layer = 6;
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.positionCount = 0;

        drawnPositions.Clear();
    }

    public KeyValuePair<string,Vector3> StopDrawing()
    {
        isDrawing = false;
        Vector2 centroid = CalculateCentroid2D(drawnPositions);
        
        string s = "Comparison\n";
        float HighestSimilarity = 0;
        int HighestIndex = 0;
        List<string> letters = new List<string> { "Firebolt", "Twin Firebolt", "Fire Pillar", "Extinguish", "Blink" };

        for (int i = 0; i < predefinedPattern.Count; i++) {
            float similarity = CompareWithPredefinedPattern(predefinedPattern[i]);
            s += i + ":   " + similarity.ToString() + "\n";
            if (similarity > HighestSimilarity)
            {
                HighestSimilarity = similarity;
                HighestIndex = i;
            }
        }
        s += "Matching: " + letters[HighestIndex] + " with similarity " + HighestSimilarity;
        Debug.Log(s);
        if (text1 != null)
        {
            text1.text = s;
        }
        ClearDrawing();
        Vector3 v = MapTo3DGround(centroid);
        if (v != new Vector3(0, -100, 0))
        {
            return new KeyValuePair<string, Vector3>(letters[HighestIndex], v);
        }
        return new KeyValuePair<string, Vector3>();
    }

    void Draw()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1f; // Set a fixed distance from the camera

        // Convert the screen space mouse position to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Add the world position as a point in the LineRenderer
        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, worldPosition);

        drawnPositions.Add(worldPosition);
    }

    void ClearDrawing()
    {
        // Destroy the line object
        if (currentLineRenderer != null)
        {
            Destroy(currentLineRenderer.gameObject);
        }

        drawnPositions.Clear();
    }

    float CompareWithPredefinedPattern(Texture2D texture)
    {
        if (predefinedPattern == null)
        {
            Debug.LogWarning("No predefined pattern assigned.");
            return 0;
        }

        // Enable the strokes camera
        strokesCamera.enabled = true;

        // Create a render texture and set it as the active render texture for the strokes camera
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        strokesCamera.targetTexture = renderTexture;
        strokesCamera.Render();

        // Create a new texture and read the render texture into it
        Texture2D drawnPattern = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        drawnPattern.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        drawnPattern.Apply();

        // Reset the camera and render texture
        strokesCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Disable the strokes camera
        strokesCamera.enabled = false;

        // Extract the bounding box of the drawn pattern
        Rect boundingBox = GetBoundingBox(drawnPositions);

        // Get the extracted pattern texture
        Texture2D extractedPattern = ExtractPattern(drawnPattern, boundingBox);

        // Compare the extracted pattern with the predefined pattern
        return ComparePatterns(extractedPattern, texture);
    }

    Rect GetBoundingBox(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            return new Rect();
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector3 pos in positions)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            minX = Mathf.Min(minX, screenPos.x);
            minY = Mathf.Min(minY, screenPos.y);
            maxX = Mathf.Max(maxX, screenPos.x);
            maxY = Mathf.Max(maxY, screenPos.y);
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    Texture2D ExtractPattern(Texture2D source, Rect boundingBox)
    {
        int x = Mathf.FloorToInt(Mathf.Clamp(boundingBox.xMin, 0, source.width));
        int y = Mathf.FloorToInt(Mathf.Clamp(boundingBox.yMin, 0, source.height));
        int width = Mathf.FloorToInt(Mathf.Clamp(boundingBox.width, 0, source.width - x));
        int height = Mathf.FloorToInt(Mathf.Clamp(boundingBox.height, 0, source.height - y));

        // Extract the pixels from the source texture
        Color[] pixels = source.GetPixels(x, y, width, height);

        // Create a new texture and set the extracted pixels
        Texture2D extractedPattern = new Texture2D(width, height, TextureFormat.RGBA32, false);
        extractedPattern.SetPixels(pixels);
        extractedPattern.Apply();

        return extractedPattern;
    }

    float ComparePatterns(Texture2D drawnPatternOriginal, Texture2D predefinedPattern)
    {
        /*byte[] bytes = drawnPatternOriginal.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, "test.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Saved drawn pattern to: " + filePath);*/

        //Debug.Log("Drawn Ratio"+ drawnPatternOriginal.width+","+ drawnPatternOriginal.height);
        //Debug.Log("Compared Ratio" + predefinedPattern.width + "," + predefinedPattern.height);

        Texture2D drawnPattern = ResizePatterns(drawnPatternOriginal, predefinedPattern);

        if (drawnPattern.width != predefinedPattern.width || drawnPattern.height != predefinedPattern.height)
        {
            Debug.LogWarning("Pattern sizes do not match.");
            return 0;
        }

        int matchingPixels = 0;
        int totalPixels = drawnPattern.width * drawnPattern.height;

        for (int y = 0; y < drawnPattern.height; y++)
        {
            for (int x = 0; x < drawnPattern.width; x++)
            {
                Color drawnPixel = drawnPattern.GetPixel(x, y);
                Color predefinedPixel = predefinedPattern.GetPixel(x, y);

                if (Vector4.Distance(drawnPixel, predefinedPixel) < comparisonThreshold)
                {
                    matchingPixels++;
                }
            }
        }

        float similarity = (float)matchingPixels /* 4*/ / totalPixels;
        return similarity;


        Debug.Log("Similarity: " + (similarity * 100) + "%");

        // Optionally, you can do something based on the similarity
        if (similarity > 0.9f) // 90% similarity threshold
        {
            Debug.Log("Patterns match closely!");
        }
    }

    public static Texture2D ResizePatterns(Texture2D drawnPattern, Texture2D targetPattern)
    {
        // Create a new Texture2D with the dimensions of the target pattern
        Texture2D resizedPattern = new Texture2D(targetPattern.width, targetPattern.height, drawnPattern.format, false);

        // Calculate scale factors
        float scaleX = (float)targetPattern.width / drawnPattern.width;
        float scaleY = (float)targetPattern.height / drawnPattern.height;

        // Iterate through each pixel of the target pattern
        for (int y = 0; y < targetPattern.height; y++)
        {
            for (int x = 0; x < targetPattern.width; x++)
            {
                // Calculate the corresponding pixel position in the drawn pattern
                int sourceX = Mathf.FloorToInt(x / scaleX);
                int sourceY = Mathf.FloorToInt(y / scaleY);

                // Copy the pixel color from the drawn pattern to the resized pattern
                Color pixelColor = drawnPattern.GetPixel(sourceX, sourceY);
                resizedPattern.SetPixel(x, y, pixelColor);
            }
        }

        // Apply changes to the resized texture
        resizedPattern.Apply();

        return resizedPattern;
    }
    Vector2 CalculateCentroid2D(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 sum = Vector2.zero;
        foreach (Vector3 pos in positions)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            sum += new Vector2(screenPos.x, screenPos.y);
        }

        return sum / positions.Count;
    }

    Vector3 MapTo3DGround(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Assuming the ground is at a specific layer
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 groundPosition = hit.point;
                Debug.Log("Ground Position: " + groundPosition);
                return groundPosition;
                //GameObject g= GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.transform.position = groundPosition;
                // Perform further actions with the ground position
            }
        }
        return new Vector3(0,-100,0);
    }

}
