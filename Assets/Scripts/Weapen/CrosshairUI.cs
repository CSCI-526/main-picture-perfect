using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    public bool visible = true;
    public Color color = new Color(1f, 1f, 1f, 0.9f);

    [Range(1, 12)]   public int dotRadius = 3;      //Center dot radius in px
    [Range(4, 256)]  public int ringRadius = 28;     //Circle radius in px
    [Range(1, 16)]   public int ringThickness = 2;   //Circle thickness in px

    public bool scaleWithResolution = true;
    public float referenceHeight = 1080f;            //Reference for scaling

    //Resizing dynamically, user can do it but mostly for debugging and testing purposes
    public KeyCode biggerKey = KeyCode.RightBracket;  // ]
    public KeyCode smallerKey = KeyCode.LeftBracket;    // [
    public int step = 2;

    Texture2D dotTex, ringTex;
    int cDot, cRing, cThick;
    Color cCol;

    public bool onlyWhenPlaying = true;

    void Update()
    {
        if (Input.GetKeyDown(biggerKey))  ringRadius += step;
        if (Input.GetKeyDown(smallerKey)) ringRadius = Mathf.Max(1, ringRadius - step);
    }

    void OnGUI()
    {
        if (!visible) return;
        if (onlyWhenPlaying && (Time.timeScale <= 0.001f || Cursor.lockState != CursorLockMode.Locked || Cursor.visible)) return;

        float scale = scaleWithResolution ? (Screen.height / referenceHeight) : 1f;
        int dr = Mathf.Max(1, Mathf.RoundToInt(dotRadius * scale));
        int rr = Mathf.Max(2, Mathf.RoundToInt(ringRadius * scale));
        int th = Mathf.Max(1, Mathf.RoundToInt(ringThickness * scale));

        EnsureTextures(dr, rr, th, color);

        float cx = Screen.width * 0.5f;
        float cy = Screen.height * 0.5f;

        //Dot
        float dSize = dr * 2;
        GUI.DrawTexture(new Rect(cx - dr, cy - dr, dSize, dSize), dotTex);

        //Ring
        float rSize = rr * 2;
        GUI.DrawTexture(new Rect(cx - rr, cy - rr, rSize, rSize), ringTex);
    }

    void EnsureTextures(int dr, int rr, int th, Color col)
    {
        if (dotTex == null || ringTex == null ||
            cDot != dr || cRing != rr || cThick != th || cCol != col)
        {
            cDot = dr; cRing = rr; cThick = th; cCol = col;
            CreateDot(ref dotTex, dr, col);
            CreateRing(ref ringTex, rr, th, col);
        }
    }

    static void CreateDot(ref Texture2D tex, int radius, Color col)
    {
        int size = radius * 2;
        if (tex == null || tex.width != size || tex.height != size)
        {
            if (tex != null) Object.Destroy(tex);
            tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.hideFlags = HideFlags.HideAndDontSave;
        }
        Color clear = new Color(0,0,0,0);
        var pixels = new Color[size * size];
        float r2 = radius * radius;
        float cx = radius - 0.5f, cy = radius - 0.5f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - cx, dy = y - cy, d2 = dx*dx + dy*dy;
            pixels[y*size + x] = (d2 <= r2) ? col : clear;
        }
        tex.SetPixels(pixels);
        tex.Apply(false, true);
    }

    static void CreateRing(ref Texture2D tex, int radius, int thickness, Color col)
    {
        int size = radius * 2;
        if (tex == null || tex.width != size || tex.height != size)
        {
            if (tex != null) Object.Destroy(tex);
            tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.hideFlags = HideFlags.HideAndDontSave;
        }
        Color clear = new Color(0,0,0,0);
        var pixels = new Color[size * size];
        float rOuter = radius;
        float rInner = Mathf.Max(0, radius - thickness);
        float o2 = rOuter * rOuter;
        float i2 = rInner * rInner;
        float cx = radius - 0.5f, cy = radius - 0.5f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - cx, dy = y - cy, d2 = dx*dx + dy*dy;
            bool inRing = (d2 <= o2) && (d2 >= i2);
            pixels[y*size + x] = inRing ? col : clear;
        }
        tex.SetPixels(pixels);
        tex.Apply(false, true);
    }

    void OnDestroy()
    {
        if (dotTex) Destroy(dotTex);
        if (ringTex) Destroy(ringTex);
    }
}

