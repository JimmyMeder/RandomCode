using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDraw : MonoBehaviour {


    public Camera cam;
    Vector2 pixelUVPaint;

    public Texture2D texture;
    public Texture2D orig;

    //Vector2 pixelUVPaintOld;

    // Use this for initialization
    void Start () {
        pixelUVPaint = new Vector2(0, 0);
        //pixelUVPaintOld = pixelUVPaint;
        cam = GetComponent<Camera>();
        FixImage();
	}

    // Update is called once per frame
    public Vector2 getOldPos()
    {
        return pixelUVPaint;
    }

    public Vector2 GetSpawnPos(Vector3 testRay)
    {
        RaycastHit hit;
        //Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(testRay)), out hit);
        if (!Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(testRay)), out hit))
        {
            return new Vector2();
        }

        //Säde osuu kappaleeseen

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return new Vector2();
        }

        //Kaikki kunnossa, otetaan osutun pixelin alpha arvo ja palautetaan true tai false

        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        return pixelUV;
        //onko pixelin alpha 1.0
        /*if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a == 1.0)
        {
            return true;
        }
        else
        {
            return false;
        }*/

    }
    void Update () {
        
		if (!Input.GetMouseButton(0))
        {
            return;

        }

        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return;
        }

        //Hiiren nappula on pohjassa ja säde osuu kappaleeseen

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return;
        }

        //Kaikki kunnossa, voidaan ryhtyä maalaamaan

        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        Debug.Log("X: " + pixelUV.x + " Y: " + pixelUV.y);

        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
        //Tärkeä taas
        tex.Apply();
        

	}

    public bool DrawRay(Vector3 testRay)
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(testRay)), out hit))
        {
            return false;
        }

        //Säde osuu kappaleeseen

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return false;
        }

        //Kaikki kunnossa, otetaan osutun pixelin alpha arvo ja palautetaan true tai false

        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        //onko pixelin alpha 1.0
        if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a == 1.0)
        {
            return true;
        } else
        {
            return false;
        }

    }

    public bool PaintRay(Vector3 testRay, Vector2 pixelUVPaintOld)
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(testRay)), out hit))
        {
            return false;
        }

        //Säde osuu kappaleeseen

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return false;
        }

        //Kaikki kunnossa, otetaan osutun pixelin alpha arvo ja palautetaan true tai false

        Texture2D tex = rend.material.mainTexture as Texture2D;
        pixelUVPaint = hit.textureCoord;
        pixelUVPaint.x *= tex.width;
        pixelUVPaint.y *= tex.height;

        float m = SlopeMultiplier(pixelUVPaintOld, pixelUVPaint);
        float b = Intercept(pixelUVPaintOld, m);

        List<int> xCoord = new List<int>();
        List<int> yCoord = new List<int>();

        //PixelCheck toimii viivan funktiolla y=m*x+b. Funtiot tarkastavat kaikki pixelit vanhan ja uuden sijainnin väliltä ja palauttavat pixelien koordinaatit.
        //PixelCheckSimple ei toimi kunnolla korkeilla kulmilla. PixelCheckComplex toimii kaikilla kulmilla
        PixelCheckComplex(m, b, pixelUVPaintOld, pixelUVPaint, xCoord, yCoord);
        //PixelCheckSimple(m, b, pixelUVPaintOld, pixelUVPaint, xCoord, yCoord);

        //Käydään läpi PixelCheckin antamat koordinaatit --> suoritus lopetetaan ensimmäiseen näkyvään pixeliin.

        bool foundIt = false;
        for (int i=0; i<xCoord.Count; i++)
        {
            //Debug.Log(xCoord[i] + " ja Y on: " + yCoord[i]);
            //Tarkistetaan onko pixelin alpha 1
            if (tex.GetPixel(xCoord[i], yCoord[i]).a == 1.0)
            {
                Circle(tex, xCoord[i], yCoord[i], 5, Color.clear);
                tex.Apply();
                foundIt = true;
                break;
            }
            else
            {
                foundIt = false;
                
            }
        }
        if (foundIt)
        {
            return true;
        } else
        {
            return false;
        }

    }

    public bool Digging(Vector3 location, int radius)
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(location)), out hit))
        {
            return false;
        }

        //Säde osuu kappaleeseen

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return false;
        }

        //Kaikki kunnossa, otetaan osutun pixelin alpha arvo ja palautetaan true tai false

        Texture2D tex = rend.material.mainTexture as Texture2D;
        //pixelUVPaintOld = pixelUVPaint;
        pixelUVPaint = hit.textureCoord;
        pixelUVPaint.x *= tex.width;
        pixelUVPaint.y *= tex.height;

            //Debug.Log(xCoord[i] + " ja Y on: " + yCoord[i]);
            if (tex.GetPixel((int)pixelUVPaint.x, (int)pixelUVPaint.y).a == 1.0)
            {
                Circle(tex, (int)pixelUVPaint.x, (int)pixelUVPaint.y, radius, Color.clear);
                tex.Apply();
                return true;
            }
            else
            {
                Circle(tex, (int)pixelUVPaint.x, (int)pixelUVPaint.y, radius, Color.clear);
                tex.Apply();
                return false;

            }
        

    }

    private void PixelCheckComplex(float m, float b, Vector2 origin, Vector2 end, List<int> xCoord, List<int> yCoord)
    {

        // Jokaisen pikselin tarkistus liikutun matkan ajalta alkaa tästä
        if (end.x > origin.x)        //For loop alhaalta ylös. eli ammutaan oikealle
        {
            //jos kulmakerroin on 1 ja -1 välillä, eli jos kulma on loiva (45-(-45)), eli for loop x:n mukaan
            if (m < 1 && m > -1)
            {
                for (int x = (int)origin.x; x <= (int)end.x; x++)
                {
                    int y = (int)(m * x + b);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }
            }
            else if (m < -1)
            {
                //Muut kulmat x:n positiiviselta puolelta tästä alas, eli for loop y:n mukaan
                //Jyrkkä alas, eli for loop ylhäältä alas
                for (int y = (int)origin.y; y >= (int)end.y; y--)
                {
                    int x = (int)((y - b) / m);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }

            }
            else if (m > 1)
            {
                //Jyrkkä ylös, eli for loop alhaalta ylös
                for (int y = (int)origin.y; y <= (int)end.y; y++)
                {
                    int x = (int)((y - b) / m);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }
            }
        }
        else if (end.x < origin.x)     //For loop ylhäältä alas. eli ammutaan vasemmalle
        {
            //jos kulmakerroin on 1 ja -1 välillä, eli jos kulma on loiva (45-(-45)), eli for loop x:n mukaan
            if (m < 1 && m > -1)
            {
                //Debug.Log("Matala kulma vasemmalle");
                for (int x = (int)origin.x; x >= (int)end.x; x--)
                {
                    int y = (int)(m * x + b);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }
            }
            else if (m < -1)
            {
                //Muut kulmat x:n Negatiiviselta puolelta tästä alas, eli for loop y:n mukaan
                //Jyrkkä ylös, eli for loop alhaalta ylös
                for (int y = (int)origin.y; y <= (int)end.y; y++)
                {
                    //Debug.Log("DING");
                    int x = (int)((y - b) / m);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }
            }
            else if (m > 1)
            {
                //Jyrkkä alas, eli for loop ylhäältä alas
                for (int y = (int)origin.y; y >= (int)end.y; y--)
                {
                    //Debug.Log("DING");
                    int x = (int)((y - b) / m);
                    xCoord.Add(x);
                    yCoord.Add(y);
                }
            }
        }
        //Tarkastus loppuu tässä
    }

    private void PixelCheckSimple(float m, float b, Vector2 origin, Vector2 end, List<int> xCoord, List<int> yCoord)
    {
        //Yksinkertaisempi tarkastelu, ei yhtä tarkka jyrkissä kulmissa
        //PAAAAAAALJON helpompi tehdä ja sisäistää. myös paljon nopeampi tehdä
        if (end.x > origin.x) //For loop alhaalta ylös
        {
            for (int x = (int)origin.x; x <= (int)end.x; x++)
            {
                int y = (int)(m * x + b);
                xCoord.Add(x);
                yCoord.Add(y);
            }
        }
        else if (end.x < origin.x)   //For loop ylhäältä alas
        {
            for (int x = (int)origin.x; x >= (int)end.x; x--)
            {
                int y = (int)(m * x + b);
                xCoord.Add(x);
                yCoord.Add(y);
            }
        }
        //Yksinkertaisempi tarkastelu loppuu tässä
    }

    private float SlopeMultiplier(Vector2 origin, Vector2 end)
    {
        if (origin.x == end.x) //Ehkä lisätään jos tulee ongelmia
        {
            return 0;
        }
        float slope = ((end.y - origin.y) / (end.x - origin.x));
        return slope;
    }

    private float Intercept(Vector2 origin, float slope)
    {
        if (slope == 0)
        {
            // vertical line
            return origin.x;
        }

        return origin.y - slope * origin.x;
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));

            for (y = 0; y <= d; y++)
            {

                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);
            }
        }
    }


    void FixImage()
    {
        if (texture.width == orig.width && texture.height == orig.height)
        {
            texture.filterMode = FilterMode.Point;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, orig.GetPixel(x, y));
                }
            }
            texture.Apply();
        }
    }


}
