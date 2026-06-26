using System;
using System.Globalization;
using System.IO.Ports;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using ModernWpf;
using System.IO;

namespace modernuitest
{
    public partial class MainWindow : Window
    {
        float L = 150F;
        float l = 300F;
        float wb = 103.32F;
        float a = 58.32F;
        double b = ((Math.Sqrt(3)) / 2) * (45 - 103.32);
        double c = (45 - 103.32) / 2;
        double ue = 45;
        double we = 45 / 2;
        double sb = 2 * (Math.Sqrt(3)) * 103.32F;
        double se = 2 * (Math.Sqrt(3)) * (45 / 2);
        double theta1;
        double theta2;
        double theta3;
        double prevtheta1;
        double prevtheta2;
        double prevtheta3;
        double x0 = 0,
               y0 = 0,
               z0 = -300;

        private bool isModelVisible = true;
        private bool isWFVisible = true;
        private bool isvectorVisible = false;

        SerialPort sp = new SerialPort();


        ModelVisual3D visual;
        Model3DGroup RA = new Model3DGroup(); //RoboticArm 3d group
        Model3DGroup WF = new Model3DGroup(); //RoboticArm 3d group

        ModelVisual3D RoboticArm = new ModelVisual3D();
        ModelVisual3D WireFrame = new ModelVisual3D();
        ModelVisual3D BaseDesc = new ModelVisual3D();
        ModelVisual3D EndEffDesc = new ModelVisual3D();
        ModelVisual3D invpln = new ModelVisual3D();

        ModelVisual3D Vectors = new ModelVisual3D();
        ModelVisual3D axes = new ModelVisual3D();

        Model3D geom = null;

        LinesVisual3D vec_M1 = CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, 0), Colors.White, 4);
        LinesVisual3D vec_L1 = CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, 0), Colors.Blue, 4);
        LinesVisual3D vec_l1 = CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, 0), Colors.LimeGreen, 4);
        LinesVisual3D vec_P = CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, 0), Colors.Red, 4);
        LinesVisual3D vec_P1 = CreateLine(new Point3D(0, 0, 0), new Point3D(0, 0, 0), Colors.Yellow, 4);

        Model3DGroup Base = new Model3DGroup();
        Model3DGroup cover = new Model3DGroup();

        Model3DGroup l0 = new Model3DGroup();
        Model3DGroup l1_1 = new Model3DGroup();
        Model3DGroup l1_2 = new Model3DGroup();
        Model3DGroup l2 = new Model3DGroup();


        Model3DGroup link1 = new Model3DGroup();
        Model3DGroup link2 = new Model3DGroup();
        Model3DGroup link3 = new Model3DGroup();
        Model3DGroup pl1_1 = new Model3DGroup();
        Model3DGroup pl1_2 = new Model3DGroup();

        Model3DGroup pl2_1 = new Model3DGroup();
        Model3DGroup pl2_2 = new Model3DGroup();

        Model3DGroup pl3_1 = new Model3DGroup();
        Model3DGroup pl3_2 = new Model3DGroup();

        Model3DGroup endf = new Model3DGroup();

        Model3DGroup Basewf = new Model3DGroup();
        Model3DGroup L1WF = new Model3DGroup();
        Model3DGroup L2WF = new Model3DGroup();
        Model3DGroup L3WF = new Model3DGroup();
        Model3DGroup EndeffWF = new Model3DGroup();

        //BillboardTextVisual3D joint1 = new BillboardTextVisual3D();
        //BillboardTextVisual3D joint2 = new BillboardTextVisual3D();
        //BillboardTextVisual3D joint3 = new BillboardTextVisual3D();



        private MenuItem _currentSelectedMenuItem;
        public MainWindow()
        {
            ModernWpf.ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light; // or Light, or null for system
            InitializeComponent();
            RoboticArm.Content = Load_Model();
            WireFrame.Content = Load_WF();
            Base_Desc();
            EndEff_Desc();
            inv_planes();
            viewPort3d.Children.Add(RoboticArm);
            //viewPort3d.Children.Add(visual);


            viewPort3d.Camera.LookDirection = new Vector3D(-19.038, 1334.446, -187.563);
            viewPort3d.Camera.UpDirection = new Vector3D(-0.002, 0.140, 0.990);
            viewPort3d.Camera.Position = new Point3D(-15.915, -1359.817, 92.786);
            //viewPort3d.MouseMove += Viewport_MouseMove;

            vec_M1.Points = new Point3DCollection
                                {
                                     new Point3D(0, 0, 0)
                                    ,new Point3D(0,-wb,0)  // start
                                                   
                                };
            //Sim(x0, y0, z0);
            UpdatePl1Transform(x0, y0, z0);
            UpdatePl2Transform(x0, y0, z0);
            UpdatePl3Transform(x0, y0, z0);

            prevtheta1 = theta1;
            prevtheta2 = theta2;
            prevtheta3 = theta3;

            Xtb.Text = x0.ToString("00");
            Ytb.Text = y0.ToString("00");
            Ztb.Text = z0.ToString("00");

            Vectors.Children.Add(vec_L1);
            Vectors.Children.Add(vec_P);
            Vectors.Children.Add(vec_P1);
            Vectors.Children.Add(vec_l1);
            Vectors.Children.Add(vec_M1);

            //int f = 20;

            //joint3 = new BillboardTextVisual3D
            //{
            //    Position = new Point3D(-89.482, 51.663, 50),
            //    Foreground = Brushes.Yellow,
            //    Background = Brushes.Transparent,
            //    FontSize = f
            //};
            // joint2 = new BillboardTextVisual3D
            //{
            //    Position = new Point3D(89.201, 51.50, 50),
            //    Foreground = Brushes.Yellow,
            //    Background = Brushes.Transparent,
            //    FontSize = f
            //};
            // joint1 = new BillboardTextVisual3D
            //{
            //    Position = new Point3D(0, -89.201, 50),
            //    Foreground = Brushes.Yellow,
            //    Background = Brushes.Transparent,
            //    FontSize = f

            //};



        }
        private Model3DGroup Load_Model()
        {

            var modelimp = new ModelImporter();

            //Base = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\Base.STL");
            //cover = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\cover.STL");
            //link1 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\link1.STL");
            //link2 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\link2.STL");
            //link3 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\link3.STL");

            //pl1_1 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl1_1.STL");
            //pl1_2 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl1_2.STL");
            //pl2_1 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl2_1.STL");
            //pl2_2 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl2_2.STL");
            //pl3_1 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl3_1.STL");
            //pl3_2 = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\pl3_2.STL");

            //endf = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (2)\\endf.STL");




            Base = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\Base.STL");
            link1 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\Alink1.STL");
            link2 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\ALink2.STL");
            link3 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\ALink3.STL");
            endf = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\EndEff.STL");

            pl1_1 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink1-1.STL");
            pl1_2 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink1-2.STL");

            pl2_1 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink2_1.STL");
            pl2_2 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink2_2.STL");

            pl3_1 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink3_1.STL");
            pl3_2 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\PLink3_2.STL");


            ApplyMaterial(Base, Colors.DimGray);
            ApplyMaterial(cover, Colors.WhiteSmoke);

            ApplyMaterial(l0, Colors.White);
            ApplyMaterial(l1_1, Colors.Gray);
            ApplyMaterial(l1_2, Colors.White);
            ApplyMaterial(l2, Colors.White);


            ApplyMaterial(link1, Colors.MediumTurquoise);
            ApplyMaterial(link2, Colors.MediumTurquoise);
            ApplyMaterial(link3, Colors.MediumTurquoise);

            ApplyMaterial(pl1_1, Colors.Orange);
            ApplyMaterial(pl1_2, Colors.Orange);
            ApplyMaterial(pl2_1, Colors.Orange);
            ApplyMaterial(pl2_2, Colors.Orange);
            ApplyMaterial(pl3_1, Colors.Orange);
            ApplyMaterial(pl3_2, Colors.Orange);

            ApplyMaterial(endf, Colors.White);

            //var builder = new MeshBuilder(true, true);
            //var position = new Point3D(-89.482, 51.663, 80);
            //builder.AddSphere(position, 10, 15, 15);
            //geom = new GeometryModel3D(builder.ToMesh(), Materials.Brown);
            //visual = new ModelVisual3D();
            //visual.Content = geom;

            //RA.Children.Add(l0);
            //RA.Children.Add(l1_1);
            //RA.Children.Add(l1_2);
            //RA.Children.Add(l2);


            RA.Children.Add(endf);
            RA.Children.Add(Base);
            RA.Children.Add(cover);
            RA.Children.Add(link1);
            RA.Children.Add(link2);
            RA.Children.Add(link3);

            RA.Children.Add(pl1_1);
            RA.Children.Add(pl1_2);
            RA.Children.Add(pl2_1);
            RA.Children.Add(pl2_2);
            RA.Children.Add(pl3_1);
            RA.Children.Add(pl3_2);

            return RA;
        }
        private Model3DGroup Load_WF()
        {


            var modelimp = new ModelImporter();
            Basewf = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\basewireframe.STL");
            L1WF = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\L1WF.STL");
            L2WF = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\L2WF.STL");
            L3WF = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\L3WF.STL");
            EndeffWF = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\DELTA\\EndeffWF.STL");


            ApplyMaterial(Basewf, Colors.White);
            ApplyMaterial(L1WF, Colors.White);
            ApplyMaterial(L2WF, Colors.White);
            ApplyMaterial(L3WF, Colors.White);
            ApplyMaterial(EndeffWF, Colors.White);


            WF.Children.Add(Basewf);
            WF.Children.Add(EndeffWF);


            WF.Children.Add(L1WF);
            WF.Children.Add(L2WF);
            WF.Children.Add(L3WF);
            return WF;
        }
        private void ApplyMaterial(Model3DGroup modelGroup, Color color)
        {
            var material = new DiffuseMaterial(new SolidColorBrush(color));

            foreach (var model in modelGroup.Children)
            {
                if (model is GeometryModel3D geometryModel)
                {
                    geometryModel.Material = material;
                    geometryModel.BackMaterial = material; // Apply to both sides
                }
            }
        }
        private void UpdatePl3Transform(double x, double y, double z)
        {
            theta3 = angle_3(x, y, z);

            var link3Transform = new Transform3DGroup();
            var pl3_1Transform = new Transform3DGroup();
            var pl3_2Transform = new Transform3DGroup();

            var Servo = new Point3D(-89.482, 51.663, 0);
            var defdir = new Vector3D(-0.866, 0.5, 0);
            var pldef = new Vector3D(0, 0, -1);
            var linkaxis = new Vector3D(0.5, 0.866, 0);
            var ploffset = 35;


            link3Transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(-0.5, -0.866, 0), theta3), new Point3D(-89.482, 51.663, 0)));
            link3.Transform = link3Transform;


            var L_end = new Point3D(Servo.X + Math.Sin((90 + theta3) * Math.PI / 180) * (defdir.X * L),
                                      Servo.Y + Math.Sin((90 + theta3) * Math.PI / 180) * defdir.Y * L,
                                       Math.Cos((90 + theta3) * Math.PI / 180) * L);

            var AL2_2end = new Point3D(Servo.X + Math.Sin((90 + theta3) * Math.PI / 180) * (defdir.X * L) + (linkaxis.X * ploffset),
                                      Servo.Y + Math.Sin((90 + theta3) * Math.PI / 180) * (defdir.Y * L) + (linkaxis.Y * ploffset),
                                       Math.Cos((90 + theta3) * Math.PI / 180) * L);

            var AL2_1end = new Point3D(Servo.X + Math.Sin((90 + theta3) * Math.PI / 180) * (defdir.X * L) + (linkaxis.X * -ploffset),
                                      Servo.Y + Math.Sin((90 + theta3) * Math.PI / 180) * (defdir.Y * L) + (linkaxis.Y * -ploffset),
                                       Math.Cos((90 + theta3) * Math.PI / 180) * L);

            var P2_2 = new Point3D(x + defdir.X * 40 + linkaxis.X * ploffset, y + defdir.Y * 40 + linkaxis.Y * ploffset, z);
            var P2_1 = new Point3D(x + defdir.X * 40 + linkaxis.X * -ploffset, y + defdir.Y * 40 + linkaxis.Y * -ploffset, z);

            Vector3D direction2_2 = P2_2 - AL2_2end;
            direction2_2.Normalize();

            Vector3D direction2_1 = P2_1 - AL2_1end;
            direction2_1.Normalize();


            pl3_2Transform.Children.Add(new TranslateTransform3D(L_end.X + 219.386, L_end.Y - 126.663, L_end.Z));
            pl3_1Transform.Children.Add(new TranslateTransform3D(L_end.X + 219.386, L_end.Y - 126.663, L_end.Z)); // translating the PL to that point


            var rotation2_1 = new AxisAngleRotation3D(Vector3D.CrossProduct(pldef, direction2_1), //gives the axis of rotation
                                                   Vector3D.AngleBetween(pldef, direction2_1));// gives the angle of rotation

            var rotation2_2 = new AxisAngleRotation3D(Vector3D.CrossProduct(pldef, direction2_2), //gives the axis of rotation
                                                   Vector3D.AngleBetween(pldef, direction2_2));// gives the angle of rotation
            rotation2_1.Axis.Normalize();
            rotation2_2.Axis.Normalize();

            pl3_1Transform.Children.Add(new RotateTransform3D(rotation2_1, AL2_1end)); // rotate 

            pl3_2Transform.Children.Add(new RotateTransform3D(rotation2_2, AL2_2end)); // rotate 



            // translating the PL to that point

            pl3_1.Transform = pl3_1Transform;
            pl3_2.Transform = pl3_2Transform;


        }
        private void UpdatePl2Transform(double x, double y, double z)
        {
            theta2 = angle_2(x, y, z);

            var link2Transform = new Transform3DGroup();
            var pl2_1Transform = new Transform3DGroup();
            var pl2_2Transform = new Transform3DGroup();

            var Servo = new Point3D(89.201, 51.50, 0);
            var defdir = new Vector3D(0.866, 0.5, 0);
            var pldef = new Vector3D(0, 0, -1);
            var linkaxis = new Vector3D(-0.5, 0.866, 0);
            var ploffset = 35;

            theta2 = angle_2(x, y, z);

            link2Transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(-0.5, 0.866, 0), theta2), new Point3D(89.201, 51.50, 0)));
            link2.Transform = link2Transform;


            var L_end = new Point3D(Servo.X + Math.Sin((90 + theta2) * Math.PI / 180) * (defdir.X * L),
                                      Servo.Y + Math.Sin((90 + theta2) * Math.PI / 180) * defdir.Y * L,
                                       Math.Cos((90 + theta2) * Math.PI / 180) * L);

            var AL2_2end = new Point3D(Servo.X + Math.Sin((90 + theta2) * Math.PI / 180) * (defdir.X * L) + (linkaxis.X * ploffset),
                                      Servo.Y + Math.Sin((90 + theta2) * Math.PI / 180) * (defdir.Y * L) + (linkaxis.Y * ploffset),
                                       Math.Cos((90 + theta2) * Math.PI / 180) * L);

            var AL2_1end = new Point3D(Servo.X + Math.Sin((90 + theta2) * Math.PI / 180) * (defdir.X * L) + (linkaxis.X * -ploffset),
                                      Servo.Y + Math.Sin((90 + theta2) * Math.PI / 180) * (defdir.Y * L) + (linkaxis.Y * -ploffset),
                                       Math.Cos((90 + theta2) * Math.PI / 180) * L);

            var P2_2 = new Point3D(x + defdir.X * 40 + linkaxis.X * ploffset, y + defdir.Y * 40 + linkaxis.Y * ploffset, z);
            var P2_1 = new Point3D(x + defdir.X * 40 + linkaxis.X * -ploffset, y + defdir.Y * 40 + linkaxis.Y * -ploffset, z);

            Vector3D direction2_2 = P2_2 - AL2_2end;
            direction2_2.Normalize();

            Vector3D direction2_1 = P2_1 - AL2_1end;
            direction2_1.Normalize();


            pl2_2Transform.Children.Add(new TranslateTransform3D(L_end.X - 219.104, L_end.Y - 126.60, L_end.Z));
            pl2_1Transform.Children.Add(new TranslateTransform3D(L_end.X - 219.104, L_end.Y - 126.60, L_end.Z)); // translating the PL to that point


            var rotation2_1 = new AxisAngleRotation3D(Vector3D.CrossProduct(pldef, direction2_1), //gives the axis of rotation
                                                   Vector3D.AngleBetween(pldef, direction2_1));// gives the angle of rotation

            var rotation2_2 = new AxisAngleRotation3D(Vector3D.CrossProduct(pldef, direction2_2), //gives the axis of rotation
                                                   Vector3D.AngleBetween(pldef, direction2_2));// gives the angle of rotation
            rotation2_1.Axis.Normalize();
            rotation2_2.Axis.Normalize();

            pl2_1Transform.Children.Add(new RotateTransform3D(rotation2_1, AL2_1end)); // rotate 

            pl2_2Transform.Children.Add(new RotateTransform3D(rotation2_2, AL2_2end)); // rotate 




            pl2_1.Transform = pl2_1Transform;
            pl2_2.Transform = pl2_2Transform;


        }
        private void UpdatePl1Transform(double x, double y, double z)
        {
            theta1 = angle_1(x, y, z);
            theta2 = angle_2(x, y, z);
            theta3 = angle_3(x, y, z);

            J1_Sldr.Value = theta1;
            J2_Sldr.Value = theta2;
            J3_Sldr.Value = theta3;
            prevtheta1 = theta1;
            prevtheta2 = theta2;
            prevtheta3 = theta3;



            var link1Transform = new Transform3DGroup();
            var link3Transform = new Transform3DGroup();
            var pl1_1Transform = new Transform3DGroup();
            var pl1_2Transform = new Transform3DGroup();
            var endfTransform = new Transform3DGroup();

            var Al1_1end = new Point3D(35, -wb - L * Math.Cos(theta1 * Math.PI / 180), -L * Math.Sin(theta1 * Math.PI / 180)); // calculate the joining point of AL and PL after applying IK
            var Al1_2end = new Point3D(-35, -wb - L * Math.Cos(theta1 * Math.PI / 180), -L * Math.Sin(theta1 * Math.PI / 180)); // calculate the joining point of AL and PL after applying IK

            pl1_1Transform.Children.Add(new TranslateTransform3D(Al1_1end.X - 35, Al1_1end.Y + 253.32, Al1_1end.Z)); // translating the PL to that point
            pl1_2Transform.Children.Add(new TranslateTransform3D(Al1_2end.X + 35, Al1_2end.Y + 253.32, Al1_2end.Z)); // translating the PL to that point

            var P1_1 = new Point3D(x + 35, y - 40, z); // point joining endeffector and passive link 
            var P1_2 = new Point3D(x - 35, y - 40, z); // point joining endeffector and passive link 

            Vector3D direction1_1 = P1_1 - Al1_1end; // vector along the two points (B-A!!)
            Vector3D direction1_2 = P1_2 - Al1_2end; // vector along the two points (B-A!!)

            direction1_1.Normalize();
            direction1_2.Normalize();


            Vector3D defaultDirection = new Vector3D(0, 0, -1); // default direction vector of the PL 

            var rotation1_1 = new AxisAngleRotation3D(Vector3D.CrossProduct(defaultDirection, direction1_1), //gives the axis of rotation
                                                   Vector3D.AngleBetween(defaultDirection, direction1_1));// gives the angle of rotation
            var rotation1_2 = new AxisAngleRotation3D(Vector3D.CrossProduct(defaultDirection, direction1_2), //gives the axis of rotation
                                       Vector3D.AngleBetween(defaultDirection, direction1_2));// gives the angle of rotation
            rotation1_1.Axis.Normalize();
            rotation1_2.Axis.Normalize();

            pl1_1Transform.Children.Add(new RotateTransform3D(rotation1_1, Al1_1end)); // rotate 
            pl1_2Transform.Children.Add(new RotateTransform3D(rotation1_2, Al1_2end)); // rotate 


            link1Transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), theta1), new Point3D(0, -103.32, 0)));


            endfTransform.Children.Add(new TranslateTransform3D(x, y, z + 200));



            link1.Transform = link1Transform;
            link3.Transform = link3Transform;
            pl1_1.Transform = pl1_1Transform;
            pl1_2.Transform = pl1_2Transform;

            endf.Transform = endfTransform;
            EndeffWF.Transform = endfTransform;

            vec_P1.Points = new Point3DCollection
                                {
                                    new Point3D(x, y, z),  // start
                                    new Point3D(x, y-40, z)                 // updated end
                                };

            vec_P.Points = new Point3DCollection
                                {
                                    new Point3D(0, 0, 0),  // start
                                    new Point3D(x, y, z)                 // updated end
                                };

            vec_L1.Points = new Point3DCollection
                                {
                                     new Point3D(0, -wb, 0)
                                    ,new Point3D(Al1_1end.X-35, Al1_1end.Y, Al1_1end.Z)  // start

                                };
            vec_l1.Points = new Point3DCollection
                                {
                                     new Point3D(Al1_1end.X-35, Al1_1end.Y, Al1_1end.Z)
                                    ,new Point3D(x, y-40, z)  // start

                                };

        }

        private void Sim(double targetX, double targetY, double targetZ)
        {
            Point3D start = new Point3D(x0, y0, z0);
            Point3D target = new Point3D(targetX, targetY, targetZ);

            Vector3D delta = target - start;
            double totalDistance = delta.Length;
            delta.Normalize(); // Get direction

            double stepSize = 2.0; // mm per frame (controls speed)
            int steps = (int)(totalDistance / stepSize);


            DispatcherTimer timer = new DispatcherTimer();
            Point3D lastMarkedPoint = new Point3D(double.NaN, double.NaN, double.NaN);

            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS

            int currentStep = 0;

            timer.Tick += (s, e) =>
            {
                if (currentStep >= steps)
                {
                    timer.Stop();
                    return;
                }

                Point3D current = start + delta * (stepSize * currentStep);
                UpdatePl1Transform(current.X, current.Y, current.Z);
                UpdatePl2Transform(current.X, current.Y, current.Z);
                UpdatePl3Transform(current.X, current.Y, current.Z);
                currentStep++;
            };

            timer.Start();
        }

        //private void Viewport_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var pos = e.GetPosition(viewPort3d);
        //    var hit = viewPort3d.Viewport.FindHits(pos).FirstOrDefault();

        //    if (hit != null && hit.Visual == plane2)
        //    {
        //        // Mouse is over the plane - make it more opaque
        //        plane1.Fill = new SolidColorBrush(Color.FromArgb(230, 255, 165, 0)); // ~90% opacity
        //    }
        //    else
        //    {
        //        // Mouse not over the plane - keep it semi-transparent
        //        plane1.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 165, 0)); // 50% opacity
        //    }
        //}


        public static LinesVisual3D CreateLine(Point3D start, Point3D end, Color color, double thickness = 2)
        {
            return new LinesVisual3D
            {
                Color = color,
                Thickness = thickness,
                Points = new Point3DCollection { start, end }
            };
        }
        private void RoboDesc_Btn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedMenuItem(RoboDesc_Btn);
            RobotControlWindow.Visibility = Visibility.Collapsed;
            InvKinWin.Visibility = Visibility.Collapsed;
            Traj_Win.Visibility = Visibility.Collapsed;
            Robo_Desc.Visibility = Visibility.Visible;

            Sim(0, 0, -300);
            AnimateCameraTo(new Point3D(-564.407, -631.402, 615.515), new Vector3D(644.130, 676.403, -668.704), new Vector3D(0.401, 0.422, 0.813));
           
            viewPort3d.Children.Remove(invpln);

            if (isModelVisible == true)
            {
                viewPort3d.Children.Remove(RoboticArm);
                viewPort3d.Children.Add(WireFrame);
                isModelVisible = false;
            }
            //viewPort3d.Children.Add(CreateOriginAxesWithArrows());

            viewPort3d.Children.Add(BaseDesc);

        }

        private void RobC_Win(object sender, RoutedEventArgs e)
        {
            Robo_Desc.Visibility = Visibility.Collapsed;
            RobotControlWindow.Visibility = Visibility.Visible;
            InvKinWin.Visibility = Visibility.Collapsed;
            Traj_Win.Visibility = Visibility.Collapsed;

            viewPort3d.Children.Remove(BaseDesc);
            viewPort3d.Children.Remove(EndEffDesc);
            viewPort3d.Children.Remove(invpln);
            SetSelectedMenuItem(RobC_Btn);

            AnimateCameraTo(new Point3D(1046.441, -648.676, 374.433), new Vector3D(-794.656, 458.758, -341.242), new Vector3D(-0.302, 0.175, 0.937));
            //viewPort3d.Children.Add(joint1);
            //viewPort3d.Children.Add(joint2);
            //viewPort3d.Children.Add(joint3);
            if (isModelVisible == false)
            {
                viewPort3d.Children.Remove(WireFrame);
                viewPort3d.Children.Add(RoboticArm);
                isModelVisible = true;
            }

        }
        private void InvKin_Btn_Click(object sender, RoutedEventArgs e)
        {
            Robo_Desc.Visibility = Visibility.Collapsed;
            RobotControlWindow.Visibility = Visibility.Collapsed;
            Traj_Win.Visibility = Visibility.Collapsed;
            InvKinWin.Visibility = Visibility.Visible;
            SetSelectedMenuItem(InvKin_Btn);
            AnimateCameraTo(new Point3D(-333.272, -812.599, 468.620), new Vector3D(436.934, 928.647, -527.448), new Vector3D(0.195, 0.414, 0.890));
            viewPort3d.Children.Remove(BaseDesc);
            viewPort3d.Children.Remove(EndEffDesc);

            if (isModelVisible == true)
            {
                viewPort3d.Children.Remove(RoboticArm);
                viewPort3d.Children.Add(WireFrame);
                isModelVisible = false;
            }

            //RoboDesc.Text = "The joint variables:\r\n θ₁ \r\n θ₂ \r\n θ₃\r\n \r\nJoint Rot Axis Vec:";
            //viewPort3d.Children.Add(CreateOriginAxesWithArrows());
            viewPort3d.Children.Add(invpln);

        }
        private void SetSelectedMenuItem(MenuItem selectedItem)
        {
            if (_currentSelectedMenuItem != null)
                _currentSelectedMenuItem.Background = Brushes.Transparent;

            // Select new
            selectedItem.Background = Brushes.LightBlue;
            _currentSelectedMenuItem = selectedItem;
        }

        public void AnimateCameraTo(Point3D position, Vector3D lookDirection, Vector3D upDirection)
        {
            var camera = viewPort3d.Camera as ProjectionCamera;
            if (camera != null)
            {
                CameraHelper.AnimateTo(camera, position, lookDirection, upDirection, 800);
            }
        }

        private void HideModel_Click(object sender, RoutedEventArgs e)
        {
            if (isModelVisible)
            {
                viewPort3d.Children.Remove(RoboticArm);
                isModelVisible = false;
            }
            else
            {
                viewPort3d.Children.Add(RoboticArm);
                isModelVisible = true;
            }
        }

        private void X_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Xtb.Text = X_Sldr.Value.ToString();
        }

        private void Y_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Ytb.Text = Y_Sldr.Value.ToString();
        }

        private void Z_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Ztb != null)
            {
                Ztb.Text = Z_Sldr.Value.ToString("F1");
            }
        }

        private void J1_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (J1tb != null)
            {
                J1tb.Text = J1_Sldr.Value.ToString("F1");
                //joint1.Text = J1_Sldr.Value.ToString("0.0");
                
            }
        }

        private void J2_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (J2tb != null)
            {
                J2tb.Text = J2_Sldr.Value.ToString("F1");
                //joint2.Text = J2_Sldr.Value.ToString("0.0");

            }
        }

        private void J3_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (J3tb != null)
            {
                J3tb.Text = J3_Sldr.Value.ToString("F1");
                //joint3.Text = J3_Sldr.Value.ToString("0.0");

            }
        }

        private void SimBtn_Click(object sender, RoutedEventArgs e)
        {
            theta1 = J1_Sldr.Value;
            theta2 = J2_Sldr.Value;
            theta3 = J3_Sldr.Value;

            //if (theta1 == prevtheta1 && theta2 == prevtheta2 && theta3 == prevtheta3)
            //{
                float x = float.Parse(Xtb.Text);
                float y = float.Parse(Ytb.Text);
                float z = float.Parse(Ztb.Text);

                if (Double.IsNaN(theta1) == true && Double.IsNaN(theta2) == true && Double.IsNaN(theta3) == true)
                {
                    MessageBox.Show("Invalid Coordinates!");
                }
                else
                {
                    Sim(x, y, z);
                    x0 = x;
                    y0 = y;
                    z0 = z;
                }

            //}
            //else
            //{
            //    double x, y, z;

            //    Forw_Kin(theta1, theta2, theta3, out x, out y, out z);
            //    Sim(x, y, z);
            //    x0 = x;
            //    y0 = y;
            //    z0 = z;
            //}

        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {

            Sim(0, 0, -215.9);
            x0 = 0;
            y0 = 0;
            z0 = -215.9;
            try
            {
                sp.Write("H");
                MessageBox.Show("DATA SENT!!!");

            }
            catch (Exception)
            {
                MessageBox.Show("Device not Connected");
            }

        }

        private void ShowVector_Click(object sender, RoutedEventArgs e)
        {
            if (isvectorVisible)
            {
                viewPort3d.Children.Remove(Vectors);
                isvectorVisible = false;
            }
            else
            {
                viewPort3d.Children.Add(Vectors);
                isvectorVisible = true;
            }

        }

        public ModelVisual3D CreateOriginAxesWithArrows()
        {
            double axisLength = 50;
            var group = new ModelVisual3D();
            double arrowSize = axisLength * 0.05; // size of arrowhead


            var xArrow = new ArrowVisual3D
            {
                Direction = new Vector3D(1, 0, 0),
                Point2 = new Point3D(axisLength - arrowSize, 0, 0),
                Diameter = arrowSize,
                Fill = Brushes.Red
            };

            var yArrow = new ArrowVisual3D
            {
                Direction = new Vector3D(0, 1, 0),
                Point2 = new Point3D(0, axisLength - arrowSize, 0),
                Diameter = arrowSize,
                Fill = Brushes.Green
            };

            var zArrow = new ArrowVisual3D
            {
                Direction = new Vector3D(0, 0, 1),
                Point2 = new Point3D(0, 0, axisLength - arrowSize),
                Diameter = arrowSize,
                Fill = Brushes.Blue,

            };

            // Optional labels
            var xLabel = new TextVisual3D { Text = "X", Position = new Point3D(axisLength + 5, 0, 0), Foreground = Brushes.Red };
            var yLabel = new TextVisual3D { Text = "Y", Position = new Point3D(0, axisLength + 5, 0), Foreground = Brushes.Green };
            var zLabel = new TextVisual3D { Text = "Z", Position = new Point3D(0, 0, axisLength + 5), Foreground = Brushes.Blue };

            // Add all visuals to the group
            group.Children.Add(xArrow); group.Children.Add(xLabel);
            group.Children.Add(yArrow); group.Children.Add(yLabel);
            group.Children.Add(zArrow); group.Children.Add(zLabel);

            return group;
        }
        private double angle_1(double x, double y, double z)
        {
            double u1 = 2 * 150 * (y + a);
            double v1 = 2 * L * z;
            double w1 = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(a, 2) + Math.Pow(L, 2) + (2 * y * a) - Math.Pow(l, 2);
            double t1 = (-v1 - Math.Sqrt(Math.Pow(u1, 2) + Math.Pow(v1, 2) - Math.Pow(w1, 2))) / (w1 - u1);
            double a1 = 2 * (Math.Atan(t1) * (180 / Math.PI));
            return a1;
        }
        private double angle_2(double x, double y, double z)
        {
            double u2 = -150 * (Math.Sqrt(3) * (x + b) + y + c);
            double v2 = 2 * L * z;
            double w2 = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(b, 2) + Math.Pow(c, 2) + Math.Pow(L, 2) + (2 * x * b) + (2 * y * c) - Math.Pow(l, 2);
            double t2 = (-v2 - Math.Sqrt(Math.Pow(u2, 2) + Math.Pow(v2, 2) - Math.Pow(w2, 2))) / (w2 - u2);
            double a2 = 2 * (Math.Atan(t2) * (180 / Math.PI));
            return a2;
        }
        private double angle_3(double x, double y, double z)
        {
            double u3 = 150 * (Math.Sqrt(3) * (x - b) - y - c);
            double v3 = 2 * L * z;
            double w3 = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(b, 2) + Math.Pow(c, 2) + Math.Pow(L, 2) - (2 * x * b) + (2 * y * c) - Math.Pow(l, 2);
            double t3 = (-v3 - Math.Sqrt(Math.Pow(u3, 2) + Math.Pow(v3, 2) - Math.Pow(w3, 2))) / (w3 - u3);
            double a3 = 2 * (Math.Atan(t3) * (180 / Math.PI));
            return a3;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            AnimateCameraTo(new Point3D(-169.640, -172.340, -112.364), new Vector3D(130.849, 137.405, -135.841), new Vector3D(0.401, 0.422, 0.813));
            viewPort3d.Children.Remove(BaseDesc);
            viewPort3d.Children.Add(EndEffDesc);

            dim.Text = "EndEff Plate Dimensions:";
            SL.Text = "Side Length(se)";
            t1.Text = "Wₑ₁";
            t2.Text = "Wₑ₂";
            t3.Text = "Wₑ₃";
            w1.Text = "45";
            w2.Text = "45";
            w3.Text = "45";

            Next.Visibility = Visibility.Collapsed;
            Prev.Visibility = Visibility.Visible;

        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            AnimateCameraTo(new Point3D(-564.407, -631.402, 615.515), new Vector3D(644.130, 676.403, -668.704), new Vector3D(0.401, 0.422, 0.813));
            viewPort3d.Children.Add(BaseDesc);
            viewPort3d.Children.Remove(EndEffDesc);

            Next.Visibility = Visibility.Visible;
            Prev.Visibility = Visibility.Collapsed;
            dim.Text = "BASE Dimensions:";
            SL.Text = "Side Length(sb)";
            t1.Text = "Wₘ₁";
            t2.Text = "Wₘ₂";
            t3.Text = "Wₘ₃";
            w1.Text = "103.32";
            w2.Text = "103.32";
            w3.Text = "103.32";

        }

        private void Base_Desc()
        {
            var M1 = new TextVisual3D
            {
                Text = "M1",
                Position = new Point3D(0, -122.32, 0),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 30,
                FontWeight = FontWeights.Bold,
            };

            var M2 = new TextVisual3D
            {
                Text = "M2",
                Position = new Point3D(Math.Sqrt(3) / 2 * 103.32 + 20, 103.32 / 2 + 10, 0),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 30,
                FontWeight = FontWeights.Bold,
            };
            var M3 = new TextVisual3D
            {
                Text = "M3",
                Position = new Point3D(-Math.Sqrt(3) / 2 * 103.32 - 20, 103.32 / 2 + 10, 0),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 30,
                FontWeight = FontWeights.Bold,
            };

            LinesVisual3D axis1 = CreateLine(new Point3D(-178.955, -103.32, 0), new Point3D(178.955, -103.32, 0), Colors.Red, 3);
            LinesVisual3D axis2 = CreateLine(new Point3D(-178.955, -103.32, 0), new Point3D(0, 206.639, 0), Colors.Red, 3);
            LinesVisual3D axis3 = CreateLine(new Point3D(178.955, -103.32, 0), new Point3D(0, 206.639, 0), Colors.Red, 3);


            var Wm1 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(0, 0, 0),
                Point2 = new Point3D(0, -103.32, 0),
                Diameter = 2.5,
                Fill = Brushes.Yellow
            };

            var Wm2 = new ArrowVisual3D
            {
                Direction = new Vector3D(Math.Sqrt(3) / 2, 1 / 2, 0),
                Point1 = new Point3D(0, 0, 0),
                Point2 = new Point3D(Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Diameter = 2.5,
                Fill = Brushes.Red
            };

            var Wm3 = new ArrowVisual3D
            {
                Direction = new Vector3D(-Math.Sqrt(3) / 2, 1 / 2, 0),
                Point1 = new Point3D(0, 0, 0),
                Point2 = new Point3D(-Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Diameter = 2.5,
                Fill = Brushes.Purple
            };

            BaseDesc.Children.Add(M1);
            BaseDesc.Children.Add(M2);
            BaseDesc.Children.Add(M3);

            BaseDesc.Children.Add(Wm1);
            BaseDesc.Children.Add(Wm2);
            BaseDesc.Children.Add(Wm3);

            BaseDesc.Children.Add(axis1);
            BaseDesc.Children.Add(axis2);
            BaseDesc.Children.Add(axis3);

        }
        private void EndEff_Desc()
        {
            var E1 = new TextVisual3D
            {
                Text = "E1",
                Position = new Point3D(0, -58, -300),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 20,
                FontWeight = FontWeights.Bold,
            };

            var E2 = new TextVisual3D
            {
                Text = "E2",
                Position = new Point3D(Math.Sqrt(3) / 2 * 45 + 10, 45 / 2, -300),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 20,
                FontWeight = FontWeights.Bold,
            };
            var E3 = new TextVisual3D
            {
                Text = "E3",
                Position = new Point3D(-Math.Sqrt(3) / 2 * 45 - 10, 45 / 2, -300),
                Foreground = Brushes.Orange,
                UpDirection = new Vector3D(0, 1, 0),
                Height = 20,
                FontWeight = FontWeights.Bold,
            };




            var Wm1 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(0, 0, -300),
                Point2 = new Point3D(0, -45, -300),
                Diameter = 1,
                Fill = Brushes.Yellow
            };

            var Wm2 = new ArrowVisual3D
            {
                Direction = new Vector3D(Math.Sqrt(3) / 2, 1 / 2, 0),
                Point1 = new Point3D(0, 0, -300),
                Point2 = new Point3D(Math.Sqrt(3) / 2 * 45, 45 / 2, -300),
                Diameter = 1,
                Fill = Brushes.Red
            };

            var Wm3 = new ArrowVisual3D
            {
                Direction = new Vector3D(-Math.Sqrt(3) / 2, 1 / 2, 0),
                Point1 = new Point3D(0, 0, -300),
                Point2 = new Point3D(-Math.Sqrt(3) / 2 * 45, 45 / 2, -300),
                Diameter = 1,
                Fill = Brushes.Purple
            };

            EndEffDesc.Children.Add(E1);
            EndEffDesc.Children.Add(E2);
            EndEffDesc.Children.Add(E3);

            EndEffDesc.Children.Add(Wm1);
            EndEffDesc.Children.Add(Wm2);
            EndEffDesc.Children.Add(Wm3);



        }

        private void inv_planes()
        {
            var plane1 = new RectangleVisual3D
            {
                Origin = new Point3D(0, -103.32, 0),
                LengthDirection = new Vector3D(0, 0, 1),
                Normal = new Vector3D(-1, 0, 0),
                Width = 150,
                Length = 150,
                Fill = new SolidColorBrush(Color.FromArgb(128, 255, 165, 0))
            };
            var plane2 = new RectangleVisual3D
            {
                Origin = new Point3D(Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                LengthDirection = new Vector3D(Math.Sqrt(3) / 2, 1 / 2, 0),
                Normal = new Vector3D(0.5, -Math.Sqrt(3) / 2, 0),
                Width = 150,
                Length = 150,
                Fill = new SolidColorBrush(Color.FromArgb(128, 255, 165, 0))
            };
            var plane3 = new RectangleVisual3D
            {
                Origin = new Point3D(-Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                LengthDirection = new Vector3D(0, 0, 1),
                Normal = new Vector3D(-0.5, -Math.Sqrt(3) / 2, 0),
                Width = 150,
                Length = 150,
                Fill = new SolidColorBrush(Color.FromArgb(128, 255, 165, 0))
            };

            var p1 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(0, -103.32, 0),
                Point2 = new Point3D(0, -133.32, 0),
                Diameter = 2.5,
                Fill = Brushes.Red
            };

            var p2 = new ArrowVisual3D
            {
                Direction = new Vector3D(Math.Sqrt(3) / 2, 1 / 2, 0),
                Point1 = new Point3D(0, -103.32, 0),
                Point2 = new Point3D(0, -103.32, -30),
                Diameter = 2.5,
                Fill = Brushes.Green
            };

            var p3 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Point2 = new Point3D(Math.Sqrt(3) / 2 * 103.32 * 1.3, 103.32 / 2 * 1.3, 0),
                Diameter = 2.5,
                Fill = Brushes.Red
            };

            var p4 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Point2 = new Point3D(Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, -30),
                Diameter = 2.5,
                Fill = Brushes.Green
            };


            var p5 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(-Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Point2 = new Point3D(-Math.Sqrt(3) / 2 * 103.32 * 1.3, 103.32 / 2 * 1.3, 0),
                Diameter = 2.5,
                Fill = Brushes.Red
            };

            var p6 = new ArrowVisual3D
            {
                Direction = new Vector3D(0, -1, 0),
                Point1 = new Point3D(-Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, 0),
                Point2 = new Point3D(-Math.Sqrt(3) / 2 * 103.32, 103.32 / 2, -30),
                Diameter = 2.5,
                Fill = Brushes.Green
            };

            invpln.Children.Add(p1);
            invpln.Children.Add(p2);
            invpln.Children.Add(p3);
            invpln.Children.Add(p4);
            invpln.Children.Add(p5);
            invpln.Children.Add(p6);


            invpln.Children.Add(plane1);
            invpln.Children.Add(plane2);
            invpln.Children.Add(plane3);

        }

        private void RUN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Write("K" + "," + theta1.ToString("0.00") + "," + theta2.ToString("0.00") + "," + theta3.ToString("0.00"));
                MessageBox.Show("DATA SENT!!!");

            }
            catch (Exception)
            {
                MessageBox.Show("Device not Connected");
            }
        }

        private void com_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCom = sender as ComboBox;
            string name = selectedCom.SelectedItem as string;
            sp.PortName = name;
            sp.BaudRate = 9600;
            try
            {
                sp.Open();
                MessageBox.Show("Successfully Connected");
            }
            catch (Exception)
            {
                MessageBox.Show("No device detected in this Port!");

            }
        }

        private void Traj_Click(object sender, RoutedEventArgs e)
        {
            Robo_Desc.Visibility = Visibility.Collapsed;
            RobotControlWindow.Visibility = Visibility.Collapsed;
            InvKinWin.Visibility = Visibility.Collapsed;
            Traj_Win.Visibility = Visibility.Visible;
            SetSelectedMenuItem(Traj_Btn);
        }

        private void circ_run_Click(object sender, RoutedEventArgs e)
        {

            float k, h, r, z, l;
            double[] x = new double[70];
            double[] y = new double[70];
            int j = 0;
            int v = 0;

            k = float.Parse(c_x.Text);
            h = float.Parse(c_y.Text);
            r = float.Parse(radius.Text);
            l = float.Parse(cnol.Text);
            z = -350;

           
            for (int i = 0; i < 30; i++)
            {
                x[i] = k + (r * Math.Cos(j * (Math.PI / 180)));
                y[i] = h + (r * Math.Sin(j * (Math.PI / 180)));
                if (checkworkvol_C(x[i], y[i]) == false) { v = 1; }
                j = j + 12;
            }

            if (v == 0)
            {
                Sim(x[0], y[0], z);
                for (int n = 0; n < l; n++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Sim(x[i], y[i], z);
                    }
                }

            }
            else { MessageBox.Show("Coordinates out of Work Volume !"); }

        }
        bool checkworkvol_C(double x, double y)
        {
            if (Math.Abs(x) > 100 || Math.Abs(y) > 100) { return false; }
            else { return true; }
        }
        private void Dev_sel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = Dev_sel.SelectedItem as ComboBoxItem;
            string text = selectedItem.Content.ToString();

            if (text == "4Dof Serial")
            {
                var Window = new Serial();
                Window.Show();
                this.Close();
            }
        }

        private void Forw_Kin(double theta1, double theta2, double theta3,out double x0, out double y0, out double z0)
        {
            var dtr = Math.PI / 180.0;
            theta1 *= dtr;
            theta2 *= dtr;
            theta3 *= dtr;

   var x1 = 0;
            var y1 = -(wb + L * Math.Cos(theta1)) + ue;
            var z1 = -L * Math.Sin(theta1);

            var y2 = 0.5 * (wb + L * Math.Cos(theta2)) - we;
    var x2 = (Math.Sqrt(3) / 2) * (wb + L * Math.Cos(theta2)) - se / 2;
    var z2 = -L * Math.Sin(theta2);

            var y3 = 0.5 * (wb + L * Math.Cos(theta3)) - we;
            var x3 = -(Math.Sqrt(3) / 2) * (wb + L * Math.Cos(theta3)) + se / 2;
    var z3 = -L * Math.Sin(theta3);

            var dnm = (y2 - y1) * x3 - (y3 - y1) * x2;

           var w1 = Math.Pow(y1,2) + Math.Pow(z1, 2);
            var w2 = Math.Pow(x2, 2) + Math.Pow(y2, 2) + Math.Pow(z2, 2);
            var w3 = Math.Pow(x3, 2) + Math.Pow(y3, 2) + Math.Pow(z3, 2);

            var a1 = (z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1);
           var  b1 = -((w2 - w1) * (y3 - y1) - (w3 - w1) * (y2 - y1)) / 2.0;

           var a2 = -(z2 - z1) * x3 + (z3 - z1) * x2;
            var b2 = ((w2 - w1) * x3 - (w3 - w1) * x2) / 2.0;

           var a = Math.Pow(a1, 2) + Math.Pow(a2, 2) + Math.Pow(dnm, 2);
           var b = 2 * (a1 * b1 + a2 * (b2 - y1 * dnm) - z1 * Math.Pow(dnm, 2));
            var c = Math.Pow((b2 - y1 * dnm),2) + Math.Pow(b1, 2) + Math.Pow(dnm, 2) * (Math.Pow(z1, 2) - Math.Pow(l, 2));

           var  d = Math.Pow(b, 2) - 4 * a * c;
    if (d < 0)
        {}

            z0 = -0.5 * (b + Math.Sqrt(d)) / a;
            x0 = (a1 * z0 + b1) / dnm;
            y0 = (a2 * z0 + b2) / dnm;

        }

    }
}