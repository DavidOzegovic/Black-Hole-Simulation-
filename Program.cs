using Raylib_cs;
using System.Numerics;

class CrnaRupa {
    public Vector2 pozicija;
    public int radius = 50;
    public float masa = 800f;
    public float gravitacionaKonstanta = 900f;
    
    public void crtanje()
    {
        float x = pozicija.X;
        float y = pozicija.Y;
        int pozicijaX = (int)x;
        int pozicijaY = (int)y;
        Raylib.DrawCircle(pozicijaX, pozicijaY, radius, Color.Black);
    }
}
class SunceveZrake {
    public Vector2 brzina = new Vector2(100, 0);
    public Vector2 pozicija;
    public Vector2 pozicijaMisa;
    public float zivot = 100f;
    public bool zraka_stvorena =  false;
    public Vector2[] tacke = new Vector2[4];
    
    //tragovi koji se stvaraju iza sunceve zrake
    public List<Vector2> trag = new List<Vector2>();
    const int maxTragova = 40; 
    const float razmak = 12f;
    
    public void crtanje()
    {
        float x = pozicija.X;
        float y = pozicija.Y;
        int brojTacaka = 4;
        float debljina = 20f;
        
        Raylib.DrawSplineBezierCubic(tacke, brojTacaka, debljina, Color.Red);
    }
    public void dodajTrag(Vector2 p) {
        if (trag.Count == 0 || Vector2.Distance(trag[trag.Count - 1], p) >= razmak) {
            trag.Add(p);
            if (trag.Count > maxTragova) trag.RemoveAt(0);
        }
    }  
    public void crtanjeTraga() {
        for (int i = 0; i < trag.Count; i++) {
            float t = (float)(i + 1) / trag.Count;   
            Color boja = Raylib.Fade(Color.Orange, t);
            Raylib.DrawCircleV(trag[i], 2f + 4f * t, boja);
        }
    }
}

class Program {
    static void Main() {
        
        //Inicilizacija
        const int screenWidth = 800;
        const int screenHeight = 450; 
        
        SunceveZrake zr = new SunceveZrake();
        CrnaRupa cr = new CrnaRupa();
        const int cellSize = 50;
        
        Texture2D image = Raylib.LoadTexture("black-hole.png");
        Raylib.InitWindow(screenWidth, screenHeight, "Simulacija Crne Rupe");
        
        Raylib.UnloadTexture(image);
        Raylib.SetTargetFPS(60);
        //glavni krug simulacije
        while (!Raylib.WindowShouldClose()) {
            //Update
            float deltaTime = Raylib.GetFrameTime();
            
            //Crtanje na ekranu
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);
            
            //Nacin koji ce nacrtati kockasti patern na x, y osi
            for (int i = 0; i < screenWidth; i += cellSize) {
                for (int j = 0; j < screenHeight; j += cellSize) {
                    Raylib.DrawRectangleLines(i, j, cellSize, cellSize, Color.Gray );
                }
            }
            
            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                 zr.zraka_stvorena = true;
                 zr.pozicija = Raylib.GetMousePosition();
                 zr.pozicijaMisa = Raylib.GetMousePosition();
                 
                 zr.zivot = 100;
                 zr.tacke[0] = zr.pozicijaMisa;
                 zr.tacke[1] = zr.pozicijaMisa;
                 zr.tacke[2] = zr.pozicijaMisa;
                 zr.tacke[3] = zr.pozicijaMisa;
                 
                 zr.brzina = new Vector2(50, 0);
                 zr.trag.Clear();
            }
            //Crtanje Crne Rupe
            cr.pozicija.X = 600;
            cr.pozicija.Y = 200;
            cr.crtanje();
            
            //stvaranje zrake i racunanje vrednosti za privlacenje ka crnoj rupi
            if (zr.zraka_stvorena == true) {
                    //mjeanjamo poziciju zraka na ekranu
                    zr.zivot -= deltaTime;
                    Vector2 vektorksaUdaljenost = cr.pozicija - zr.pozicija;
                    float rastojanje =  MathF.Sqrt(vektorksaUdaljenost.X * vektorksaUdaljenost.X + vektorksaUdaljenost.Y * vektorksaUdaljenost.Y);
                    if (rastojanje <= cr.radius) {
                        zr.zraka_stvorena = false;
                    }
                    else {
                        Vector2 normalizovaniVektor = vektorksaUdaljenost / rastojanje;
                        float akceleracija = cr.gravitacionaKonstanta * cr.masa / (rastojanje * rastojanje);
                        Vector2 vektorskaAkceleracija = normalizovaniVektor * akceleracija;
                        Console.WriteLine(vektorskaAkceleracija);
                        
                        zr.brzina = zr.brzina + vektorskaAkceleracija * deltaTime;
                        Vector2 novaPozicija =  zr.pozicija + zr.brzina * deltaTime;
                        zr.pozicija = novaPozicija;
                        zr.dodajTrag(novaPozicija);
                        
                        Array.Copy(zr.tacke, 1, zr.tacke, 0, 3);
                        zr.tacke[3] = new(novaPozicija.X, novaPozicija.Y);
                        zr.crtanje();
                        zr.crtanjeTraga();
                        
                    }
            }
            //Provjera da li je veme isteklo || linija izasla van ekrana
            if (zr.zivot <= 0 || zr.tacke[3].X > screenWidth + 50) {
                    zr.zraka_stvorena = false;
                    zr.pozicija = new Vector2(0, 0);
                    if (zr.zraka_stvorena == false) {
                        Console.WriteLine("Suncevi zraci su izbrisani");
                    }else { 
                        Console.WriteLine("Suncevi zraci nisu izbrisani");
                    }
            }
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }
        //deinicilizacija
        Raylib.CloseWindow();
    }
}