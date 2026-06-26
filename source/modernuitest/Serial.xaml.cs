using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using ModernWpf;

namespace modernuitest
{
    public partial class Serial : Window
    {
        Model3DGroup Base = new Model3DGroup();
        Model3DGroup link0 = new Model3DGroup();
        Model3DGroup link1= new Model3DGroup();
        Model3DGroup link2 = new Model3DGroup();
        Model3DGroup link3 = new Model3DGroup();

        AxisAngleRotation3D J0Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
        AxisAngleRotation3D J1Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
        AxisAngleRotation3D J2Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);


        Transform3DGroup BaseTransform = new Transform3DGroup();
        Transform3DGroup Link0Transform = new Transform3DGroup();
        Transform3DGroup Link1Transform = new Transform3DGroup();
        Transform3DGroup Link2Transform = new Transform3DGroup();




        Model3DGroup RA = new Model3DGroup(); //RoboticArm 3d group

        ModelVisual3D RoboticArm = new ModelVisual3D();
        ModelVisual3D visual;
        Model3D geom = null;

        public Serial()
        {
            InitializeComponent();
            ModernWpf.ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light; // or Light, or null for system

            RoboticArm.Content = Load_Model();
            viewPort3d.Children.Add(RoboticArm);
            //viewPort3d.Children.Add(visual);

            viewPort3d.Camera.LookDirection = new Vector3D(-541.280,575.956,-191.214);
            viewPort3d.Camera.UpDirection = new Vector3D(-0.162,0.171,0.972);
            viewPort3d.Camera.Position = new Point3D(401.013,-508.035,229.438);


        }
        private Model3DGroup Load_Model()
        {

            var modelimp = new ModelImporter();
            //Base = modelimp.Load("C:\\Users\\V\\Desktop\\New folder (3)\\Assembly - Base-1.STL");

            Base = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\SERIAL\\MB1.STL");
            link0 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\SERIAL\\B1.STL");
            link1 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\SERIAL\\M1.STL");
            link2 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\SERIAL\\F1.STL");
            link3 = modelimp.Load("C:\\Users\\V\\Desktop\\Models\\SERIAL\\G1.STL");


            //link0 = modelimp.Load("C:\\Users\\V\\Desktop\\Serial Manipulator\\link0.stl");
            //link1_plate = modelimp.Load("C:\\Users\\V\\Desktop\\Serial Manipulator\\link1_1.stl");
            //link1_cap = modelimp.Load("C:\\Users\\V\\Desktop\\Serial Manipulator\\link1_2.stl");
            //link2 = modelimp.Load("C:\\Users\\V\\Desktop\\Serial Manipulator\\link2.stl");


            ApplyMaterial(Base, Colors.DarkGray);
            ApplyMaterial(link0, Colors.DarkOrange);
            ApplyMaterial(link1, Colors.DarkCyan);
            ApplyMaterial(link2, Colors.DarkOrange);
            ApplyMaterial(link3, Colors.DarkCyan);


            var frame = new HelixToolkit.Wpf.CoordinateSystemVisual3D
            {
                Transform = Base.Transform,
                ArrowLengths = 20
            };
            viewPort3d.Children.Add(frame);


            Link0Transform.Children.Add(new RotateTransform3D(J0Rotation));
            link0.Transform = Link0Transform;




            Link1Transform.Children.Add(new RotateTransform3D(J1Rotation, new Point3D(0,0,75.50)));
            link1.Transform = Link1Transform;
            link0.Children.Add(link1);


            Link2Transform.Children.Add(new RotateTransform3D(J2Rotation, new Point3D(90, 0, 75.50)));
            link2.Transform = Link2Transform;
            link1.Children.Add(link2);
            link2.Children.Add(link3);


            //var builder = new MeshBuilder(true, true);
            //var position = new Point3D(0, 0, 0);
            //builder.AddSphere(position, 10, 15, 15);
            //geom = new GeometryModel3D(builder.ToMesh(), Materials.Brown);
            //visual = new ModelVisual3D();
            //visual.Content = geom;


            RA.Children.Add(Base);
            RA.Children.Add(link0);

            return RA;
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

        private void Dev_sel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = Dev_sel.SelectedItem as ComboBoxItem;
            string text = selectedItem.Content.ToString();

            if (text == "Delta Robot")
            {
                var Window = new MainWindow();
                Window.Show();
                this.Close();
            }
        }

        private void RobC_Btn_Click(object sender, RoutedEventArgs e)
        {
            RobotControlWindow.Visibility = Visibility.Visible;
            AnimateCameraTo(new Point3D(357.096,-514.292,355.561), new Vector3D(-512.611,553.353,-342.165), new Vector3D(-0.282,0.303,0.911));

        }
        public void AnimateCameraTo(Point3D position, Vector3D lookDirection, Vector3D upDirection)
        {
            var camera = viewPort3d.Camera as ProjectionCamera;
            if (camera != null)
            {
                CameraHelper.AnimateTo(camera, position, lookDirection, upDirection, 800);
            }
        }

        private void J0_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            J0Rotation.Angle = J0_Sldr.Value;
        }

        private void J1_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            J1Rotation.Angle = -J1_Sldr.Value;

        }

        private void J2_Sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            J2Rotation.Angle = J2_Sldr.Value;

        }
    }
}
