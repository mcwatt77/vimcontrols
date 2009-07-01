using System;
using System.Windows.Media.Media3D;
using ActionDictionary.Interfaces;
using AppControlInterfaces._3dExtruder;

namespace DataProcessors._3dExtruder
{
    [Launchable("3d box viewer")]
    public class Controller2 : I3dObjectData, IFullNavigation
    {
        private double _zplane;
        private readonly ExtrusionPlane _basePlane = new ExtrusionPlane(new[]
                                                                            {
                                                                                new Triangle3D(new[]
                                                                                                   {
                                                                                                       new Point3D(-2.5, -2.5, -2.5),
                                                                                                       new Point3D(2.5, -2.5, -2.5),
                                                                                                       new Point3D(2.5, 2.5, -2.5)
                                                                                                   }),
                                                                                new Triangle3D(new[]
                                                                                                   {
                                                                                                       new Point3D(-2.5, -2.5, -2.5),
                                                                                                       new Point3D(2.5, 2.5, -2.5),
                                                                                                       new Point3D(-2.5, 2.5, -2.5)
                                                                                                   })
                                                                            }, new Point3D(0, 0, 0));
        public Model3DGroup GetModel()
        {
            var extrusion = new ExtrusionSet(_basePlane);
            extrusion = extrusion.Extrude(0, 5, 1);

            var group = new Model3DGroup();
            group.Children.Add(extrusion.Model);
            var model = _basePlane.Model;
            var tgroup = new Transform3DGroup();
            tgroup.Children.Add(new TranslateTransform3D(0, 0, 2.5 + _zplane));
            tgroup.Children.Add(new ScaleTransform3D(1.1, 1.1, 1, 0, 0, 0));
            model.Transform = tgroup;

            group.Children.Add(model);
            return group;
        }

        public I3dObjectUpdate Updater { get; set; }

        public void MoveRight()
        {
            RepositionCamera(Math.PI/180.0);
        }

        private void RepositionCamera(double rotationAmount)
        {
            var pos = Updater.Camera.Position;
            var lookDir = Updater.Camera.LookDirection;

            var lookPoint = new Point3D(pos.X + lookDir.X, pos.Y + lookDir.Y, pos.Z + lookDir.Z);

            var oldAngle = Math.Atan(pos.Y/pos.X);
            var length = Math.Sqrt(pos.X*pos.X + pos.Y*pos.Y);
            var newAngle = oldAngle + rotationAmount;

            var newY = Math.Sin(newAngle)*length;
            var newX = Math.Cos(newAngle)*length;

            Updater.Camera.Position = new Point3D(newX, newY, pos.Z);
            pos = Updater.Camera.Position;

            Updater.Camera.LookDirection = new Vector3D(lookPoint.X - pos.X, lookPoint.Y - pos.Y, lookPoint.Z - pos.Z);
        }

        public void MoveLeft()
        {
            RepositionCamera(-Math.PI/180.0);
        }

        public void MoveUp()
        {
            _zplane += 0.1;
            Updater.Redraw();
        }

        public void MoveDown()
        {
            _zplane -= 0.1;
            Updater.Redraw();
        }

        public void Beginning()
        {
        }

        public void End()
        {
        }

        public void PageUp()
        {
            var pos = Updater.Camera.Position;
            var look = Updater.Camera.LookDirection;

            Updater.Camera.Position = new Point3D(pos.X, pos.Y, pos.Z + 1);
            Updater.Camera.LookDirection = new Vector3D(look.X, look.Y, look.Z - 1);
        }

        public void PageDown()
        {
            var pos = Updater.Camera.Position;
            var look = Updater.Camera.LookDirection;

            Updater.Camera.Position = new Point3D(pos.X, pos.Y, pos.Z - 1);
            Updater.Camera.LookDirection = new Vector3D(look.X, look.Y, look.Z + 1);
        }
    }
}