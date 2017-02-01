using System;
using Tao.FreeGlut;
using OpenGL;
using System.Threading;

class Program
    {
        
        private static int width = 1280, height = 720;
        private static ShaderProgram program;
        private static VBO<Vector3> cube;
        private static VBO<Vector3> cubeNormals;
        private static VBO<Vector2> cubeUV;
        private static VBO<int> cubeElements;
        private static Texture Texture1, Texture2, Texture3, Texture4;
        private static System.Diagnostics.Stopwatch timer;
        private static float xangle, yangle;
        private static float xtran = 0.005f, upperbound = 2f, lowerbound = -2f, pos = 0;
        private static bool lighting = true, autoRotate = true, fullscreen = false;
        private static bool up = false, down = false, left = false, right = false;

        static void Main(string[] args)
        {
            System.Media.SoundPlayer km = new System.Media.SoundPlayer(@"C:\Users\DariusziElżbieta\Documents\3D_Rectangles\DDD\bin\Debug\pil.wav");
            // tworzenie okienka
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("3D Rectangles");

            //callbacki
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutReshapeFunc(OnReshape);

            Gl.Enable(EnableCap.DepthTest);

            //kompilacja shaderów
            program = new ShaderProgram(VertexShader, FragmentShader);

            //ustawienie macierzy widoku i projekcji
            program.Use();
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up));

            //obsługa oświetlenia
            program["light_direction"].SetValue(new Vector3(1, 1, 1));
            program["enable_lighting"].SetValue(lighting);

            //wgrywanie tekstury
            Texture1 = new Texture("wolf.jpg");
            Texture2 = new Texture("wolftex.png");
            Texture3 = new Texture("wood.png");
            Texture4 = new Texture("doom.jpg");

            //tworzenie kostki
            cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), //góra
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1), //dół
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1), //front
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), //tył
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), //lewa
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1), //prawa
            });
            //oteksturowanie kostki
            cubeUV = new VBO<Vector2>(new Vector2[] {
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2 (0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            });
            //wektory normalne do oświetlenia
            cubeNormals = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0),
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0),
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0),
            });
            //wierzchołki
            cubeElements = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);

            timer = System.Diagnostics.Stopwatch.StartNew();

            km.PlayLooping();
            Glut.glutMainLoop();
            
        }

        private static void OnClose()
        {
            System.Media.SoundPlayer sh = new System.Media.SoundPlayer(@"C:\Users\DariusziElżbieta\Documents\3D_Rectangles\DDD\bin\Debug\shot.wav");
            sh.Play();
            Thread.Sleep(600);
            cube.Dispose();
            cubeUV.Dispose();
            cubeElements.Dispose();
            cubeNormals.Dispose();
            Texture1.Dispose();
            Texture2.Dispose();
            Texture3.Dispose();
            Texture4.Dispose();
            program.Dispose();

        }

        private static void OnDisplay()
        {

        }

        private static void OnRenderFrame()
        {
            timer.Stop();
            float deltaTime = timer.ElapsedMilliseconds / 500f;
            timer.Restart();

            if (autoRotate)
            {
                xangle += deltaTime/2;
                yangle += deltaTime/2;
                pos += xtran;
            }
            else
            {
                if (up) xangle += deltaTime;
                if (down) xangle -= deltaTime;
                if (left) yangle += deltaTime;
                if (right) yangle -= deltaTime;
            }

            if (pos > upperbound)
            {
                pos = upperbound;
                xtran = -xtran;
            }
            else if (pos < lowerbound)
            {
                pos = lowerbound;
                xtran = -xtran;
            }
            //ustawianie pozycji sceny
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //użycie shaderów
            Gl.UseProgram(program);

            //załączanie oświetlenia
            program["enable_lighting"].SetValue(lighting);


            #region Rysowanie Kostek
            Gl.BindTexture(Texture1);
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle) * Matrix4.CreateRotationX(xangle) * Matrix4.CreateTranslation(new Vector3(-5, pos, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.BindTexture(Texture2);
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(-yangle) * Matrix4.CreateRotationX(xangle) * Matrix4.CreateTranslation(new Vector3(-1.65f, -pos, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.BindTexture(Texture4);
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle) * Matrix4.CreateRotationX(xangle) * Matrix4.CreateTranslation(new Vector3(1.65f, pos, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.BindTexture(Texture3);
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(-yangle) * Matrix4.CreateRotationX(xangle) * Matrix4.CreateTranslation(new Vector3(5, -pos, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            #endregion

            Glut.glutSwapBuffers();
        }

        //obsługa poiększania/pomniejszania okna
        private static void OnReshape(int width, int height)
        {
            Program.width = width;
            Program.height = height;

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.90f, (float)width / height, 0.5f, 1000f));
        }

        //obsługa klawiatury
        private static void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 27) Glut.glutLeaveMainLoop();
            else if (key == 'w') up = true;
            else if (key == 's') down = true;
            else if (key == 'a') left = true;
            else if (key == 'd') right = true;
        }

        private static void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == ' ') autoRotate = !autoRotate;
            else if (key == 'l') lighting = !lighting;
            else if (key == 'f')
            {
                fullscreen = !fullscreen;

                if (fullscreen) Glut.glutFullScreen();
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
            else if (key == 'w') up = false;
            else if (key == 's') down = false;
            else if (key == 'a') left = false;
            else if (key == 'd') right = false;
        }

        public static string VertexShader = @"

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = normalize((model_matrix * vec4(vertexNormal, 0)).xyz);
    uv = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"

uniform vec3 light_direction;
uniform sampler2D texture;
uniform bool enable_lighting;

in vec3 normal;
in vec2 uv;

out vec4 fragment;

void main(void)
{
    float diffuse = max(dot(normal, light_direction), 0);
    float ambient = 0.1;
    float lighting = (enable_lighting ? max(diffuse, ambient) : 1.0);

    vec4 sample = texture2D(texture, uv);
    fragment = vec4(sample.xyz * lighting, sample.a);
}
";

}
