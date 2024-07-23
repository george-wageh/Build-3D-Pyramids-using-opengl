using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
//include GLM library

using System.IO;
using static Tao.Platform.Windows.Gdi;
using System.Xml.Linq;
using System.Drawing;
using System.Diagnostics;

class  Element{
    public List<float> points;
    public int primitive;
    public int id = 0;
    public Element(int id , List<float> points, int primitive) {
        this.points = points;
        this.primitive = primitive;
        this.id = id;
    }

}

namespace Graphics
{
    class Renderer
    {
        struct pyramid
        {
            public vec3 center;
            public float size;
            public vec3 color;
            public pyramid(vec3 center, float size, vec3 color)
            {
                this.center = center;
                this.size = size;
                this.color = color;
            }
        }
        Element[] CreatePyramid(int id ,pyramid pp0 )
        {
            vec3 pos = pp0.center; float size = pp0.size; vec3 color = pp0.color;
            List<float> pointsBase = new List<float> {
                pos.x , pos.y,pos.z,
                color.x+0.1f , color.y+0.1f,color.z+0.1f,
                0,0,
                pos.x+size , pos.y,pos.z,
                color.x+0.1f , color.y+0.1f,color.z+0.1f,
                1,0,
                pos.x+size , pos.y,pos.z-size,
                color.x+0.1f , color.y+0.1f,color.z+0.1f,
                1,1,
                pos.x , pos.y,pos.z-size,
                color.x+0.1f , color.y+0.1f,color.z+0.1f,
                0,1
            };


            Element base0 = new Element(id, pointsBase, Gl.GL_POLYGON);


            List<float> pointsSides = new List<float> { 
                pos.x , pos.y,pos.z,
                color.x+0.2f , color.y+0.2f,color.z+0.2f,
                0,0,
                pos.x+(float)size/2 , pos.y+size,pos.z - (float)(size/2),
                color.x+0.2f , color.y+0.2f,color.z+0.2f,
                1,0,
                pos.x+size , pos.y,pos.z,
                color.x+0.2f , color.y+0.2f,color.z+0.2f,
                1,1,


                pos.x , pos.y,pos.z-size,
                color.x+0.3f , color.y+0.3f,color.z+0.3f,
                0,1,
                pos.x+(float)size/2 , pos.y+size,pos.z - (float)(size/2),
                color.x+0.3f , color.y+0.3f,color.z+0.3f,
                1,0,
                pos.x+size , pos.y,pos.z-size,
                color.x+0.3f , color.y+0.3f,color.z+0.3f,
                1,1,


                pos.x , pos.y,pos.z,
                color.x+0.4f , color.y+0.4f,color.z+0.4f,
                0,1,
                pos.x , pos.y,pos.z-size,
                color.x+0.4f , color.y+0.4f,color.z+0.4f,
                1,0,
                pos.x+(float)size/2 , pos.y+size,pos.z - (float)(size/2),
                color.x+0.4f , color.y+0.4f,color.z+0.4f,
                1,1,


                pos.x+size , pos.y,pos.z,
                color.x+0.5f , color.y+0.5f,color.z+0.5f,
                0,1,
                pos.x+(float)size/2 , pos.y+size,pos.z - (float)(size/2),
                color.x+0.5f , color.y+0.5f,color.z+0.5f,
                1,0,
                pos.x+size , pos.y,pos.z-size,
                color.x+0.5f , color.y+0.5f,color.z+0.5f,
                1,1

          };

            Element Sides = new Element(id, pointsSides, Gl.GL_TRIANGLES);
            return new Element[] { base0, Sides };
        }

        Shader sh;
        uint vertexBufferID;
        //3D Drawing

        mat4 ModelMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;

        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        int caseID =0;

        public List<Element> elements;
        public int selected_id = 1;
        List<pyramid> pyramids;

        Stopwatch timer = Stopwatch.StartNew();
        Texture tex1;
        public void Initialize()
        {
            timer.Start();
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            tex1 = new Texture(projectPath + "\\imgs\\pyramid_texture.png", 1);
            Gl.glClearColor(0.17f, 0.17f, 0.17f, 1);

            elements = new List<Element>() ;
          


            Element road = new Element(-1,new List<float> {
                -20,0,3.1f, 1,0,1 , 0,0, 
                20,0,3.1f, 1,1,0  , 0,0,
                -20,0,4.8f, 1,0,1 , 0,0,
                20,0,4.8f, 1,0,1  , 0,0
            }, Gl.GL_LINES);
            elements.Add(road);


            List<float> sun_points = new List<float>();
            float centerX = -4f;
            float centerY = 3.1f;
            float radius = 0.5f;
            for (int i = 0; i <= 360; i++)
            {
                float degInRad = (float)(i * Math.PI/180);
                sun_points.Add((float)(Math.Cos(degInRad) * radius + centerX));
                sun_points.Add((float)(Math.Sin(degInRad) * radius + centerY));
                sun_points.Add(0);

                sun_points.Add(0.96f);
                sun_points.Add(0.91f);
                sun_points.Add(0.282f);

                sun_points.Add(0f);
                sun_points.Add(0f);
            }
            Element sun = new Element(-1,sun_points, Gl.GL_TRIANGLE_FAN);
            elements.Add(sun);

            pyramids = new List<pyramid> {
                new pyramid(new vec3(-4.4f, 0, 2f),1.7f, new vec3(0.4f, 0.1f, 0.3f)),
                new pyramid(new vec3(-2.2f, 0, 2f),2.7f, new vec3(0f, 0.5f, 0.6f)),
                new pyramid(new vec3(1f, 0, 2f),3.7f, new vec3(0.1f, 0.2f, 0f)),
                };
            for (int i = 0; i < pyramids.Count; i++)
            {
                Element[] p = CreatePyramid(i,pyramids[i]);
                elements.Add(p[0]);
                elements.Add(p[1]);
            }
            List<float> lines_points = new List<float>();
            for (float i = -20; i <= 20; i += 0.3f) {
                for (float j = 3.1f; j <= 4.8f; j += 0.3f) {
                    lines_points.Add(i);  lines_points.Add(0); lines_points.Add(j);
                    lines_points.Add(1f); lines_points.Add(0.4f); lines_points.Add(1f);
                    lines_points.Add(0f); lines_points.Add(0f);
                }
            }
            Element linesE = new Element(-1, lines_points, Gl.GL_POINTS);
            elements.Add(linesE);


            //create verts array
            int max_size = 0;
            foreach (Element e in elements)
            {
                max_size += e.points.Count;
            }
            float[] verts = new float[max_size];
            int h = 0;
            foreach (Element e in elements)
            {
                foreach (float f in e.points)
                {
                    verts[h] = f;
                    h++;
                }
            }


            vertexBufferID = GPU.GenerateBuffer(verts);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            // View matrix 
            ViewMatrix = glm.lookAt(
                new vec3(0,4f,10),// eye
                new vec3(0,0,0), // center
                new vec3(0,1,0)); // up
            ModelMatrix = new mat4(1);

            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            sh.UseShader();


            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");
            caseID = Gl.glGetUniformLocation(sh.ID, "case1");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

        }

        public void Draw()
        {
          
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            
            Gl.glEnableVertexAttribArray(0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glEnableVertexAttribArray(2);

            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            int h = 0;
            foreach (Element e in elements)
            {
                int size = e.points.Count / 8;
                if (selected_id == e.id)
                {

                    Gl.glUniform1i(caseID, 1);
                    tex1.Bind();

                    Gl.glUniformMatrix4fv(this.ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());
                    Gl.glDrawArrays(e.primitive, h, size);
                }
                else
                {
                    Gl.glUniform1i(caseID, 0);
                    Gl.glUniformMatrix4fv(this.ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array());
                    Gl.glDrawArrays(e.primitive, h, size);
                }
                h += size;

            }
         

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
        }
        public float angle = 0;
        public float rate = 0.0008f;

        public float translationX = 0,
                     translationY = 0,
                     translationZ = 0;
        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;

            angle += rate* deltaTime;
            if (selected_id >= 0) {

                List<mat4> transformations = new List<mat4>();
                transformations.Add(glm.translate(new mat4(1), -1 * (pyramids[selected_id].center + new vec3(pyramids[selected_id].size / 2, 0, -pyramids[selected_id].size / 2))));
                transformations.Add(glm.rotate(angle, new vec3(0, 1, 0)));
                transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));
                transformations.Add(glm.translate(new mat4(1), (pyramids[selected_id].center + new vec3(pyramids[selected_id].size / 2, 0, -pyramids[selected_id].size / 2))));
                ModelMatrix = MathHelper.MultiplyMatrices(transformations);
            }
          
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
