using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//3D model b1
using Milkshape;
//3D model e
namespace OpenGL
{

    class cOGL
    {
        public Milkshape.Character ch;
        public Milkshape.Character ch1;

       public Control p;
        int Width;
        int Height;
        public float angle=0;
        public float angleG = 0;
        public int intOptionA = 1;

      public  GLUquadric obj;

        public cOGL(Control pb)
        {
            p = pb;
            Width = p.Width;
            Height = p.Height;
            InitializeGL();
            obj = GLU.gluNewQuadric(); //!!!
            //3D model b1
            ch = new Character("ninja.ms3d");
            ch1 = new Character("ninja.ms3d");
            isAnimate = false;
            isInside = false;
            isBounds = false;
            viewAngle = 45;
            Xangl = 0;
            Yangl = 0;
            Zangl = 0;

            intOptionA = 1;

            ground[0, 0] = 1;
            ground[0, 1] = 1;
            ground[0, 2] = 0;

            ground[1, 0] = 0;
            ground[1, 1] = 1;
            ground[1, 2] = 0;

            ground[2, 0] = 1;
            ground[2, 1] = 0;
            ground[2, 2] = 0;


            isSolid = true;


        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); //!!!
            WGL.wglDeleteContext(m_uint_RC);

        }

        float[] planeCoeff = { 1, 1, 1, 1 };
        float[,] ground = { { 1, 1, -0.5f }, { 0, 1, -0.5f }, { 1, 0, -0.5f } };
        public bool isSolid = true;
        public float[] pos = new float[4];

        public int viewAngle;
        public bool isAnimate;
        public bool isInside;
        public bool isBounds;
        public float Xangl;
        public float Yangl;
        public float Zangl;

        float d;
        float fYRot; // amount to rotate around the y axis
        float fZ = -6.0f; // amount to move into the screen
        int iDirection = 0;  // current direction (0 - down the -z axis, 1 - twords the viewer)  




        uint m_uint_HWND = 0;

        public uint HWND
        {
            get { return m_uint_HWND; }
        }

        uint m_uint_DC = 0;

        public uint DC
        {
            get { return m_uint_DC; }
        }
        uint m_uint_RC = 0;

        public uint RC
        {
            get { return m_uint_RC; }
        }



        //ZBuffer TRANSPARENCY

        int angleSph = 0;
        public bool bDepthTest = false;
        public bool bZbufferShow = false;



        public float[] ScrollValue = new float[14];
        public float zShift = 0.0f;
        public float yShift = 0.0f;
        public float xShift = 0.0f;
        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;
        double[] AccumulatedRotationsTraslations = new double[16];


        void drawground()
        {
           
           
            {

               
                GL.glBindTexture(GL.GL_TEXTURE_2D, TextureGround[0]);
                GL.glNormal3f(0.0f, -1.0f, 0.0f);
                GL.glBegin(GL.GL_QUADS);

                GL.glTexCoord2f(0.0f, 0.0f);
                GL.glVertex3d(-20, -20, ground[0, 2] - 0.05);
                GL.glTexCoord2f(1.0f, 0.0f);
                GL.glVertex3d(-20, 20, ground[0, 2] - 0.05);
                GL.glTexCoord2f(1.0f, 1.0f);
                GL.glVertex3d(20, 20, ground[0, 2] - 0.05);
                GL.glTexCoord2f(0.0f, 1.0f);
                GL.glVertex3d(20, -20, ground[0, 2] - 0.05);
               
                GL.glColor3f(0, 0, 0);

                GL.glEnd();
            }
        }
        void DrawFigures()
        {

            //!!!!!!!!!!!!!
            GL.glPushMatrix();
            //!!!!!!!!!!!!!
            GL.glEnable(GL.GL_TEXTURE_2D);
            //plane grids
            if (!rotate)
            {
              
                GL.glPushMatrix();
               
                drawground();
              

                GL.glDisable(GL.GL_TEXTURE_2D);
                GL.glDisable(GL.GL_LIGHTING);

                GL.glEnable(GL.GL_COLOR);
                GL.glTranslatef(pos[0], pos[1], 10);
               
                GL.glColor3f(1, 1, 0);

                GLU.gluSphere(obj, 1, 16, 16);
               
                GL.glTranslatef(-pos[0], -pos[1], -10);
               
                GL.glEnd();
                GL.glEnable(GL.GL_TEXTURE_2D);
                GL.glPopMatrix();
            }
            else
            {
              
                GL.glPushMatrix();
                angleG -= 0.2f;
                GL.glRotatef(angleG, 0, 0, 0.1f);
                drawground();
              
                GL.glDisable(GL.GL_TEXTURE_2D);
                GL.glDisable(GL.GL_LIGHTING);

              
                GL.glEnable(GL.GL_COLOR);
                GL.glTranslatef(pos[0], pos[1], 10);
               
                GL.glColor3f(1, 1, 0);

                GL.glRotatef(angleG, 0, 2, 0);
                GLU.gluSphere(obj, 1, 16, 16);
              
                GL.glTranslatef(-pos[0], -pos[1], -10);
            
                GL.glEnd();
                GL.glEnable(GL.GL_TEXTURE_2D);
                GL.glPopMatrix();
            }

          
            DrawObjects(false, 1);


            //end of regular showFfron
            //!!!!!!!!!!!!!
            GL.glPopMatrix();
            //!!!!!!!!!!!!!

            //SHADING begin
            //we'll define cubeXform matrix in MakeShadowMatrix Sub
            // Disable lighting, we'll just draw the shadow
            //else instead of shadow we'll see stange projection of the same objects
            GL.glDisable(GL.GL_LIGHTING);

            // floor shadow
            //!!!!!!!!!!!!!
            GL.glPushMatrix();
            //!!!!!!!!!!!!    
            MakeShadowMatrix(ground);
            GL.glMultMatrixf(cubeXform);
            DrawObjects(true, 1);
            //!!!!!!!!!!!!!

            GL.glPopMatrix();
            
        }


        void DrawObjects(bool isForShades, int c)
        {

            if (!isForShades)
                GL.glColor3d(1, 0, 0);
            else
                if (c == 1)
                GL.glColor3d(0.5, 0.5, 0.5);
            else
                GL.glColor3d(0.8, 0.8, 0.8);


            GL.glRotated(90, 1, 0, 0);
            if (!isForShades)
                GL.glColor3d(1, 1, 1);
            else
                if (c == 1)
                GL.glColor3d(0.5, 0.5, 0.5);
            else
                GL.glColor3d(0.8, 0.8, 0.8);
            if ((c == 1) && (isForShades))
            {
               
                GL.glTranslated(0, 0, 0);
                GL.glScaled(0.027, 0.027, 0.027);
                GL.glColor3d(0, 0, 0);
                ch1.DrawModel2();
                GL.glTranslated(0, 0, -200);
                GL.glScaled(0.6, 0.6, 0.6);
                ch.DrawModel2();
            }
            else if ((c == 1) && (!isForShades))
            {
                GL.glTranslated(0, 0, 0);
                GL.glScaled(0.027, 0.027, 0.027);
                ch1.DrawModel();
                GL.glTranslated(0, 0, -200);
                GL.glScaled(0.6, 0.6, 0.6);
                ch.DrawModel();
            }

           
        }

        float[] cubeXform = new float[16];
        const int x = 0;
        const int y = 1;
        const int z = 2;
        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }


        void DrawBounds()
        {
            if (isBounds)
            {
                GL.glScalef(0.99f, 0.99f, 0.99f);
                GL.glLineWidth(2);
                GL.glColor3f(1.0f, 0.0f, 0.0f);
                GL.glDisable(GL.GL_LIGHTING);
                GL.glBegin(GL.GL_LINE_LOOP);
                GL.glVertex3f(-1, -1, -1);
                GL.glVertex3f(1, -1, -1);
                GL.glVertex3f(1, -1, 1);
                GL.glVertex3f(-1, -1, 1);
                GL.glEnd();
                GL.glBegin(GL.GL_LINE_LOOP);
                GL.glVertex3f(-1, 1, -1);
                GL.glVertex3f(1, 1, -1);
                GL.glVertex3f(1, 1, 1);
                GL.glVertex3f(-1, 1, 1);
                GL.glEnd();
                GL.glBegin(GL.GL_LINES);
                GL.glVertex3f(-1, -1, -1);
                GL.glVertex3f(-1, 1, -1);

                GL.glVertex3f(1, -1, -1);
                GL.glVertex3f(1, 1, -1);

                GL.glVertex3f(1, -1, 1);
                GL.glVertex3f(1, 1, 1);

                GL.glVertex3f(-1, -1, 1);
                GL.glVertex3f(-1, 1, 1);
                GL.glEnd();
                GL.glScalef(1.0f / 0.99f, 1.0f / 0.99f, 1.0f / 0.99f);
            }

            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glTranslatef(0.1f, 0.2f, -0.7f);
            GL.glColor3f(0, 1, 0);
            GLU.gluSphere(obj, 0.05, 16, 16);
            GL.glTranslatef(-0.1f, -0.2f, 0.7f);
            GL.glDisable(GL.GL_LIGHTING);

        }
        public uint[] Textures = new uint[6];
        public uint[] TextureGround=new uint[1];
        float oldViewAngle = 0.0f;
      public  void DrawTexturedCube()
        {
            GL.glScaled(55, 55, 55);
        

            {   

               

                // front
             
                 GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(0.0f, 0.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 0.0f);
                GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                 GL.glTexCoord2f(1.0f, 0.0f);
                GL.glVertex3f(1.0f, -1.0f, 1.0f);
                 GL.glTexCoord2f(1.0f, 1.0f);
                GL.glVertex3f(1.0f, 1.0f, 1.0f);
                 GL.glTexCoord2f(0.0f, 1.0f);
                GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glEnd();
                // back
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(0.0f, 0.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // left
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(-1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // right
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glEnd();
                // top
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(0.0f, 1.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, 1.0f, -1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, 1.0f, -1.0f);
                GL.glEnd();
                // bottom
                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(0.0f, -1.0f, 0.0f);
                GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(1.0f, -1.0f, -1.0f);
                GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(1.0f, -1.0f, 1.0f);
                GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-1.0f, -1.0f, 1.0f);
                GL.glEnd();
            }
        }
        //! TEXTURE CUBE e

        void GenerateTextures()
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glGenTextures(6, Textures);
            GL.glGenTextures(1, TextureGround);
            string[] imgGroundName = { "Sand.bmp" };
            string[] imagesName ={ "front.bmp","back.bmp",
                                    "left.bmp","right.bmp","top.bmp","bottom.bmp",};
            for (int i = 0; i < 6; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
            {
                Bitmap imageG = new Bitmap(imgGroundName[0]);
                imageG.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdataG;
                Rectangle rect = new Rectangle(0, 0, imageG.Width, imageG.Height);

                bitmapdataG = imageG.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, TextureGround[0]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, imageG.Width, imageG.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdataG.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                imageG.UnlockBits(bitmapdataG);
                imageG.Dispose();
               
            }
        }
        void DrawFloor()
        {
            GL.glEnable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            //!!! for blended REFLECTION
            GL.glColor4d(0, 0, 1, 0.5);
            GL.glVertex3d(-1, -1, 0);
            GL.glVertex3d(-1, 1, 0);
            GL.glVertex3d(1, 1, 0);
            GL.glVertex3d(1, -1, 0);
            GL.glEnd();
        }

        void DrawFiguresRef()
        {
            GL.glPushMatrix();

            // must be in scene to be reflected too
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, pos);

            //Draw Light Source
            GL.glDisable(GL.GL_LIGHTING);
            GL.glTranslatef(pos[0], pos[1], pos[2]);
            //Yellow Light source
            GL.glColor3f(1, 1, 0);
            GL.glTranslatef(-pos[0], -pos[1], -pos[2]);
            //projection line from source to plane
            GL.glBegin(GL.GL_LINES);
            GL.glColor3d(0.5, 0.5, 0);
            GL.glVertex3d(pos[0], pos[1], 0);
            GL.glVertex3d(pos[0], pos[1], pos[2]);
            GL.glEnd();

            GL.glEnable(GL.GL_LIGHTING);

            GL.glRotated(intOptionB, 0, 0, 1); 

            GL.glColor3f(1, 0, 0);
            GL.glTranslated(0, -0.5, 1);
            GL.glRotated(intOptionC, 1, 1, 1);
         
            GL.glRotated(-intOptionC, 1, 1, 1);
            GL.glTranslated(0, -0.5, -1);

            GL.glTranslated(1, 2, 1.5);
            GL.glRotated(90, 1, 0, 0);
            GL.glColor3d(0, 1, 1);
            GL.glRotated(intOptionB, 1, 0, 0);
          
            GL.glRotated(-intOptionB, 1, 0, 0); 
            GL.glRotated(-90, 1, 0, 0);    
            GL.glTranslated(-1, -2, -1.5);  

            GL.glRotated(intOptionB, 0, 0, 1); 

            GL.glPopMatrix();
        }

        public bool rotate=false;
        public float[] posRef = new float[4];
        public int intOptionB = 1;
        public void Draw()
        {

            //Shadows
            pos[0] = 0;//ScrollValue[9];
            pos[1] = ScrollValue[8];
            pos[2] = ScrollValue[7];
            pos[3] = ScrollValue[9];
          

            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);

            GL.glLoadIdentity();



            // not trivial
            double[] ModelVievMatrixBeforeSpecificTransforms = new double[16];
            double[] CurrentRotationTraslation = new double[16];

            GLU.gluLookAt(ScrollValue[0], ScrollValue[1], ScrollValue[2],
                  ScrollValue[3], ScrollValue[4], ScrollValue[5],
              ScrollValue[6], ScrollValue[7], ScrollValue[8]);
            GL.glTranslatef(0.0f, 0.0f, -1.0f);


            if (!bPerspective)
                GL.glTranslatef(0.0f, 0.0f, 8.0f);



            //3D model b3
            GL.glTranslatef(0.0f, -5.0f, -15.0f);
            GL.glRotated(180, 0, 1, 0);
            //3D model e



            //save current ModelView Matrix values
            //in ModelVievMatrixBeforeSpecificTransforms array
            //ModelView Matrix ========>>>>>> ModelVievMatrixBeforeSpecificTransforms
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, ModelVievMatrixBeforeSpecificTransforms);
            //ModelView Matrix was saved, so
          //  GL.glLoadIdentity(); // make it identity matrix
            GL.glLoadIdentity(); // make it identity matrix

            //make transformation in accordance to KeyCode
            float delta;
            if (intOptionC != 0)
            {
                delta = 5.0f * Math.Abs(intOptionC) / intOptionC; // signed 5

                switch (Math.Abs(intOptionC))
                {
                    case 1:
                        GL.glRotatef(delta, 1, 0, 0);
                        break;
                    case 2:
                        GL.glRotatef(delta, 0, 1, 0);
                        break;
                    case 3:
                        GL.glRotatef(delta, 0, 0, 1);
                        break;
                    case 4:
                        GL.glTranslatef(delta / 20, 0, 0);
                        break;
                    case 5:
                        GL.glTranslatef(0, delta / 20, 0);
                        break;
                    case 6:
                        GL.glTranslatef(0, 0, delta / 20);
                        break;
                }
            }
            //as result - the ModelView Matrix now is pure representation
            //of KeyCode transform and only it !!!

            //save current ModelView Matrix values
            //in CurrentRotationTraslation array
            //ModelView Matrix =======>>>>>>> CurrentRotationTraslation
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, CurrentRotationTraslation);

            //The GL.glLoadMatrix function replaces the current matrix with
            //the one specified in its argument.
            //The current matrix is the
            //projection matrix, modelview matrix, or texture matrix,
            //determined by the current matrix mode (now is ModelView mode)
            GL.glLoadMatrixd(AccumulatedRotationsTraslations); //Global Matrix

            //The GL.glMultMatrix function multiplies the current matrix by
            //the one specified in its argument.
            //That is, if M is the current matrix and T is the matrix passed to
            //GL.glMultMatrix, then M is replaced with M • T
            GL.glMultMatrixd(CurrentRotationTraslation);

            //save the matrix product in AccumulatedRotationsTraslations
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            //replace ModelViev Matrix with stored ModelVievMatrixBeforeSpecificTransforms
            GL.glLoadMatrixd(ModelVievMatrixBeforeSpecificTransforms);
            //multiply it by KeyCode defined AccumulatedRotationsTraslations matrix
            GL.glMultMatrixd(AccumulatedRotationsTraslations);

            GL.glEnable(GL.GL_TEXTURE_2D);


            if (!rotate)
            {
                GL.glRotatef(0, 0, 20, 0);
                GL.glDepthRange(1, 1);////////if the zebra behind the cube, change to (1,1)
                GL.glPushMatrix();
              
                DrawTexturedCube();
            
                GL.glPopMatrix();
            }
            else {
                GL.glRotatef(0, 0, 20, 0);
                GL.glDepthRange(1, 1);
                GL.glPushMatrix();
                angle -= 0.1f;
                GL.glRotatef(angle, 0, 2, 0);
                DrawTexturedCube();
             
                GL.glPopMatrix();
            }


            GL.glRotated(100, 0, 90, 0);
            GL.glScaled(0.05, 0.05, 0.05);
        

            
            GL.glRotated(-90, 180, 0, 0);
            GL.glScaled(30, 30, 30);
           
            DrawFigures();

       
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);


            //only floor, draw only to STENCIL buffer
            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            GL.glDisable(GL.GL_DEPTH_TEST);

           
            // restore regular settings
            GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

           
            GL.glEnable(GL.GL_STENCIL_TEST);

            // draw reflected scene
            GL.glPushMatrix();
            GL.glScalef(1, 1, -1); //swap on Z axis
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glCullFace(GL.GL_BACK);
            DrawFigures();
            GL.glCullFace(GL.GL_FRONT);
            DrawFigures();
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glPopMatrix();


            // really draw floor
            //( half-transparent ( see its color's alpha byte)))
            // in order to see reflected objects
            GL.glDepthMask((byte)GL.GL_FALSE);
            DrawFloor();
            GL.glDepthMask((byte)GL.GL_TRUE);
            // Disable GL.GL_STENCIL_TEST to show All, else it will be cut on GL.GL_STENCIL
            GL.glDisable(GL.GL_STENCIL_TEST);
            DrawFigures();
            //REFLECTION e    
            GL.glFlush();

            WGL.wglSwapBuffers(m_uint_DC);

        }

        float[] Zbuf;
        protected virtual void InitializeGL()
        {
            m_uint_HWND = (uint)p.Handle.ToInt32();
            m_uint_DC = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
            // result in a failure to subsequently create the RC.
            WGL.wglSwapBuffers(m_uint_DC);

            WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
            WGL.ZeroPixelDescriptor(ref pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_SUPPORT_OPENGL | WGL.PFD_DOUBLEBUFFER);
            pfd.iPixelType = (byte)(WGL.PFD_TYPE_RGBA);
            pfd.cColorBits = 32;
            pfd.cDepthBits = 32;
            pfd.iLayerType = (byte)(WGL.PFD_MAIN_PLANE);

            int pixelFormatIndex = 0;
            pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
            if (pixelFormatIndex == 0)
            {
                MessageBox.Show("Unable to retrieve pixel format");
                return;
            }

            if (WGL.SetPixelFormat(m_uint_DC, pixelFormatIndex, ref pfd) == 0)
            {
                MessageBox.Show("Unable to set pixel format");
                return;
            }
            //Create rendering context
            m_uint_RC = WGL.wglCreateContext(m_uint_DC);
            if (m_uint_RC == 0)
            {
                MessageBox.Show("Unable to get rendering context");
                return;
            }
            if (WGL.wglMakeCurrent(m_uint_DC, m_uint_RC) == 0)
            {
                MessageBox.Show("Unable to make rendering context current");
                return;
            }


            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        public bool bPerspective = true;

        public void initRenderingGL()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;
            if (this.Width == 0 || this.Height == 0)
                return;
            GL.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);

            GL.glViewport(0, 0, this.Width, this.Height);

            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();

            //Z-BUFFER SHOW begin  
            if (!bPerspective)
                GL.glOrtho(-4, 4, -4, 4, -4, 4);
            else
            {
                // - no Grey nuances: differences of our objects Z
                //        are relatively 0 to 100.0f ... 0.45f
                GLU.gluPerspective(45.0f, Width / Height, 0.45f, 500.0f);
               
            }
            GL.glMatrixMode(GL.GL_MODELVIEW);

            GL.glLoadIdentity();

            //save the current MODELVIEW Matrix (now it is Identity)
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            Zbuf = new float[Width * Height];

          
            GenerateTextures();
           
        }


    }

}

