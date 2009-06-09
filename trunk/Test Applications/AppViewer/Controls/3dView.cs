using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces._3dExtruder;

namespace AppViewer.Controls
{

    public class _3dView<TDataProcessor> : IAppControl, IMissing, I3dObjectUpdate
        where TDataProcessor : I3dObjectData, new()
    {
        private readonly TDataProcessor _processor;
        private UIElement _control;
        private Viewport3D mainViewport;

        private double cameraPositionX = 9;
        private double cameraPositionY = 10;
        private double cameraPositionZ = 6;
        private double lookAtX = -9;
        private double lookAtY = -8;
        private double lookAtZ = -6;

        public _3dView(TDataProcessor processor)
        {
            _processor = processor;
            _processor.Updater = this;
            Camera = new PerspectiveCamera
                         {
                             FarPlaneDistance = 100,
                             LookDirection = new Vector3D(lookAtX, lookAtY, lookAtZ),
                             UpDirection = new Vector3D(0, 0, 1),
                             NearPlaneDistance = 1,
                             Position = new Point3D(cameraPositionX, cameraPositionY, cameraPositionZ),
                             FieldOfView = 70
                         };
        }

        public UIElement GetControl()
        {
            if (_control == null)
            {
                mainViewport = new Viewport3D
                                   {
                                       Camera = Camera
                                   };
                _control = mainViewport;

                var light = new DirectionalLight
                    {
                        Color = Color.FromRgb(255, 255, 255),
                        Direction = new Vector3D(-2, -3, -1)
                    };
                var _3d = new ModelVisual3D {Content = light};
                mainViewport.Children.Add(_3d);

                Redraw();
            }

            return _control;
        }

        private void SetCamera()
        {
            var camera = (PerspectiveCamera) mainViewport.Camera;
            var position = new Point3D(
                cameraPositionX,
                cameraPositionY,
                cameraPositionZ
                );
            var lookDirection = new Vector3D(
                lookAtX,
                lookAtY,
                lookAtZ
                );
            camera.Position = position;
            camera.LookDirection = lookDirection;
        }

        private void ClearViewport()
        {
            ModelVisual3D m;
            for (int i = mainViewport.Children.Count - 1; i >= 0; i--)
            {
                m = (ModelVisual3D) mainViewport.Children[i];
                if (m.Content is DirectionalLight == false)
                    mainViewport.Children.Remove(m);
            }
        }

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_processor);
        }

        public void Redraw()
        {
                SetCamera();
                ClearViewport();

                var model = new ModelVisual3D {Content = _processor.GetModel()};
                mainViewport.Children.Add(model);
        }

        public PerspectiveCamera Camera { get; set; }
    }
}
