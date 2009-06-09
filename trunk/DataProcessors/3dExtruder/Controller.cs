using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using ActionDictionary.Interfaces;
using AppControlInterfaces._3dExtruder;
using Utility.Core;

namespace DataProcessors._3dExtruder
{
    [Launchable("3d box viewer")]
    public class Controller2 : I3dObjectData, IFullNavigation
    {
        private double _zplane = 0;
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
//            extrusion = extrusion.Extrude(0, 10, 0.5).Extrude(2, 2, 1.2);
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

            return extrusion.Model;
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
/*            var pos = Updater.Camera.Position;
            var look = Updater.Camera.LookDirection;

            var newX = (pos.X - look.X)*0.9 + look.X;
            var newY = (pos.Y - look.Y)*0.9 + look.Y;
            var newZ = (pos.Z - look.Z)*0.9 + look.Z;

            Updater.Camera.Position = new Point3D(newX, newY, newZ);*/
            _zplane += 0.1;
            Updater.Redraw();
        }

        public void MoveDown()
        {
            _zplane -= 0.1;
            Updater.Redraw();
/*            var pos = Updater.Camera.Position;
            var look = Updater.Camera.LookDirection;

            var newX = (pos.X - look.X)*1.1 + look.X;
            var newY = (pos.Y - look.Y)*1.1 + look.Y;
            var newZ = (pos.Z - look.Z)*1.1 + look.Z;

            Updater.Camera.Position = new Point3D(newX, newY, newZ);*/
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

    [Launchable("3d Extruder")]
    public class Controller : I3dObjectData, IFullNavigation
    {
        private Material _material;
        private Material _floorMaterial;
        private double _size = 5;

        private readonly ExtrusionPlane _basePlane = new ExtrusionPlane(new[]
                                                                   {
                                                                       new Triangle3D(new[]
                                                                                          {
                                                                                              new Point3D(-5, -5, 0),
                                                                                              new Point3D(5, -5, 0),
                                                                                              new Point3D(5, 5, 0)
                                                                                          }),
                                                                       new Triangle3D(new[]
                                                                                          {
                                                                                              new Point3D(-5, -5, 0),
                                                                                              new Point3D(5, 5, 0),
                                                                                              new Point3D(-5, 5, 0)
                                                                                          })
                                                                   }, new Point3D(0, 0, 0));

        public ExtrusionPlane FromSize(double size)
        {
            return new ExtrusionPlane(new[]
                                          {
                                              new Triangle3D(new[]
                                                                 {
                                                                     new Point3D(-_size, -_size, 0),
                                                                     new Point3D(_size, -_size, 0),
                                                                     new Point3D(_size, _size, 0)
                                                                 }),
                                              new Triangle3D(new[]
                                                                 {
                                                                     new Point3D(-_size, -_size, 0),
                                                                     new Point3D(_size, _size, 0),
                                                                     new Point3D(-_size, _size, 0)
                                                                 }),
                                          }, new Point3D(0, 0, 0));
        }


        public Model3DGroup GetModel()
        {
            var extrusion = new ExtrusionSet(_basePlane);
            extrusion = extrusion
                .Extrude(0, 5, 1)
                .Extrude(3, 3, 0.5)
                .Extrude(7, 5, 0.5);

            return extrusion.Model;
        }

        public I3dObjectUpdate Updater { get; set; }

        private Material FloorMaterial
        {
            get
            {
                if (_floorMaterial == null)
                    _floorMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Green));
                return _floorMaterial;
            }
        }

        private Material DefaultMaterial
        {
            get
            {
                if (_material == null)
                {
                    var brush = new SolidColorBrush(Colors.Blue);
                    var colorAnim = new ColorAnimation(Colors.Blue, Colors.Red, new Duration(TimeSpan.FromSeconds(1)))
                                        {
                                            AutoReverse = true,
                                            RepeatBehavior = RepeatBehavior.Forever
                                        };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

                    _material = new DiffuseMaterial(brush);
                }
                return _material;
            }
        }

        public void MoveRight()
        {
        }

        public void MoveLeft()
        {
        }

        public void MoveUp()
        {
            _size++;
            Updater.Redraw();
        }

        public void MoveDown()
        {
            _size--;
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
        }

        public void PageDown()
        {
        }
    }

    public class Triangle3D
    {
        private bool _twoSided = true;
        private Point3D[] _points;
        private readonly Material _floorMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 0, 255)));

        public Point3D[] Points
        {
            get
            {
                return _points;
            }
        }

        public Triangle3D Translate(Vector3D vector)
        {
            var newPoints = _points.Select(point => new Point3D(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z));
            return new Triangle3D(newPoints.ToArray());
        }

        public Triangle3D Scale(Point3D point, double amount)
        {
            var newPoints = _points.Select(eachPoint => point.Scale(eachPoint, amount));
            return new Triangle3D(newPoints.ToArray());
        }

        public Vector3D Normal()
        {
            var normal = _points.Normal();
            normal.Normalize();
            return normal;
        }

        public Triangle3D(Point3D[] points)
        {
            _points = points;
        }

        public Model3DGroup Model
        {
            get
            {
                var triangleModel = _points.Triangle(_floorMaterial);

                if (_twoSided)
                {
                    var group = new Model3DGroup();
                    group.Children.Add(triangleModel);
                    var alternateTriangle = Flip();
                    alternateTriangle._twoSided = false;
                    group.Children.Add(alternateTriangle.Model);
                    return group;
                }

                return triangleModel;
            }
        }

        public Triangle3D Flip()
        {
            _points = new [] {_points[1], _points[0], _points[2]};
            return this;
        }
    }

    public static class ExtensionMethods
    {
        public static TResult Aggregate<TSource, TResult>(this IEnumerable<TSource> eVals, TResult list,
                                                          Func<TSource, object> fn)
        {
            //can I do this as a delegate?
            MethodInfo method = typeof (TResult).GetMethod("Add");
            eVals.Aggregate(list, (subList, item) =>
                                      {
                                          method.Invoke(subList, new[] {fn(item)});
                                          return subList;
                                      });
            return list;
        }

        public static Point3D Middle(Point3D p0, Point3D p1)
        {
            return new Point3D(p0.X + (p1.X - p0.X)/2.0,
                               p0.Y + (p1.Y - p0.Y)/2.0,
                               p0.Z + (p1.Z - p0.Z)/2.0);
        }

        public static Point3D Scale(this Point3D center, Point3D pointToScale, double amount)
        {
            return new Point3D(center.X + (pointToScale.X - center.X)*amount,
                               center.Y + (pointToScale.Y - center.Y)*amount,
                               center.Z + (pointToScale.Z - center.Z)*amount);
            ;
        }

        public static Vector3D Normal(this IEnumerable<Point3D> points)
        {
            List<Point3D> p = points.ToList();
            if (p.Count < 3)
                throw new ArgumentException("Must be at least 3 points to calculate a normal.", "points");

            var v0 = new Vector3D(
                p[1].X - p[0].X, p[1].Y - p[0].Y, p[1].Z - p[0].Z);
            var v1 = new Vector3D(
                p[2].X - p[1].X, p[2].Y - p[1].Y, p[2].Z - p[1].Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        public static Model3DGroup Triangle(this IEnumerable<Point3D> points, Material material)
        {
            List<Point3D> p = points.ToList();
            if (p.Count != 3)
                throw new ArgumentException("Must be exactly 3 points to build a triangle.", "points");

            var triangleMesh = new MeshGeometry3D();
            points.Aggregate(triangleMesh, (mesh, pt) =>
                                               {
                                                   mesh.Positions.Add(pt);
                                                   return mesh;
                                               });
            Enumerable.Range(0, 3).Aggregate(triangleMesh, (mesh, i) =>
                                                               {
                                                                   mesh.TriangleIndices.Add(i);
                                                                   return mesh;
                                                               });
            var triangleModel = new GeometryModel3D(triangleMesh, material);
            var group = new Model3DGroup();
            group.Children.Add(triangleModel);
            return group;
        }

        public static Model3DGroup Cube(this IEnumerable<Point3D> points, Material material)
        {
            List<Point3D> p = points.ToList();
            if (p.Count != 3)
                throw new ArgumentException("Must be exactly 3 points to build a cube.", "points");

            var cube = new Model3DGroup();
            var cubePoints = new Point3D[8];
            var vectors = new[] {p.Vector(), new[] {p[0], p[2]}.Vector(), default(Vector3D)};
            double mag = Math.Sqrt(Vector3D.DotProduct(vectors[0], vectors[0]));
            vectors[2] = Vector3D.CrossProduct(vectors[0], vectors[1])/(-mag);

            cubePoints[0] = p[0];
            cubePoints[1] = Point3D.Add(p[0], vectors[0]);
            cubePoints[2] = Point3D.Add(Point3D.Add(p[0], vectors[0]), vectors[1]);
            cubePoints[3] = Point3D.Add(p[0], vectors[1]);
            cubePoints[4] = Point3D.Add(p[0], vectors[2]);
            cubePoints[5] = Point3D.Add(Point3D.Add(p[0], vectors[0]), vectors[2]);
            cubePoints[6] = Point3D.Add(Point3D.Add(Point3D.Add(p[0], vectors[0]), vectors[1]), vectors[2]);
            cubePoints[7] = Point3D.Add(Point3D.Add(p[0], vectors[1]), vectors[2]);
            var triangleAdds = new[]
                                   {
                                       new[] {3, 2, 6}, new[] {3, 6, 7}, new[] {2, 1, 5}, new[] {2, 5, 6},
                                       new[] {1, 0, 4}, new[] {1, 4, 5}, new[] {0, 3, 7}, new[] {0, 7, 4},
                                       new[] {7, 6, 5}, new[] {7, 5, 4},
                                       new[] {2, 3, 0}, new[] {2, 0, 1}
                                   };

            triangleAdds.Aggregate(cube, (model, pIndexes) =>
                                             {
                                                 cube.Children.Add(
                                                     new[]
                                                         {
                                                             cubePoints[pIndexes[0]], cubePoints[pIndexes[1]],
                                                             cubePoints[pIndexes[2]]
                                                         }.Triangle(material));
                                                 return model;
                                             });

            return cube;
        }

        public static Vector3D Vector(this IEnumerable<Point3D> points)
        {
            List<Point3D> p = points.ToList();
            if (p.Count < 2)
                throw new ArgumentException("Must have at least 2 points to form a vector.", "points");

            return new Vector3D(p[1].X - p[0].X, p[1].Y - p[0].Y, p[1].Z - p[0].Z);
        }
    }

    public class ExtrusionPlane
    {
        private Triangle3D[] _triangles;
        private readonly Point3D _center;
        private Vector3D _normal;

        public Vector3D Normal
        {
            get
            {
                return _normal;
            }
        }

        public ExtrusionPlane(Triangle3D[] triangles, Point3D center)
        {
            _triangles = triangles;
            _center = center;
            _normal = _triangles.First().Normal();
/*            if (_triangles.Count(triangle => !triangle.Normal().Equals(_normal)) != 0)
                throw new Exception("Can not create an extrusion plane with triangles of multiple normals");*/
        }

        public Model3DGroup Model
        {
            get
            {
                var obj = new Model3DGroup();
                _triangles.Do(triangle => obj.Children.Add(triangle.Model));
                return obj;
            }
        }

        public IEnumerable<ExtrusionPlane> Extrude(double amount, double scale)
        {
            var triangles = new List<Triangle3D>();
            _normal.Normalize();
            var translateVector = Vector3D.Multiply(amount, _normal);
            _triangles.Do(triangle => triangles.Add(triangle.Translate(translateVector).Scale(_center, scale)));

            var newCenter = new Point3D(_center.X + translateVector.X, _center.Y + translateVector.Y, _center.Z + translateVector.Z);
            var newPerpPlane = new ExtrusionPlane(triangles.ToArray(), newCenter);
            yield return newPerpPlane;
            yield return GetNewFlipPlane(newPerpPlane, 0, 1, 0, 0);
            yield return GetNewFlipPlane(newPerpPlane, 0, 2, 0, 1);
            yield return GetNewFlipPlane(newPerpPlane, 1, 2, 1, 1);
            yield return GetNewFlipPlane(newPerpPlane, 1, 0, 1, 2);
            _triangles = _triangles.Select(triangle => triangle.Flip()).ToArray();
        }

        private ExtrusionPlane GetNewFlipPlane(ExtrusionPlane newPerpPlane, int triangleIndex0, int pointIndex0, int triangleIndex1, int pointIndex1)
        {
            var points = new List<Point3D>
                             {
                                 _triangles[triangleIndex0].Points[pointIndex0],
                                 _triangles[triangleIndex1].Points[pointIndex1],
                                 newPerpPlane._triangles[triangleIndex0].Points[pointIndex0],
                                 newPerpPlane._triangles[triangleIndex1].Points[pointIndex1]
                             };
            newPerpPlane = FromPoints(points.ToArray());
            return newPerpPlane;
        }

        public static ExtrusionPlane FromPoints(Point3D[] fourPoints)
        {
            var center = ExtensionMethods.Middle(
                ExtensionMethods.Middle(fourPoints[2], fourPoints[3]),
                ExtensionMethods.Middle(fourPoints[0], fourPoints[1]));

            var triangles = new List<Triangle3D>
                                {
                                    new Triangle3D(fourPoints.Take(3).ToArray()).Flip(),
                                    new Triangle3D(fourPoints.Skip(1).Take(3).ToArray())
                                };
            return new ExtrusionPlane(triangles.ToArray(), center);
        }
    }

    public class ExtrusionSet
    {
        private readonly ExtrusionPlane[] _planes;

        public ExtrusionSet(params ExtrusionPlane[] plane)
        {
            _planes = plane;
        }

        public ExtrusionSet Extrude(int planeIndex, double amount, double scale)
        {
            var newPlanes = new List<ExtrusionPlane>(_planes);
            var extrusionPlane = _planes[planeIndex];
            extrusionPlane.Extrude(amount, scale).Do(newPlanes.Add);
            return new ExtrusionSet(newPlanes.ToArray());
        }

        public Model3DGroup Model
        {
            get
            {
                var obj = new Model3DGroup();
                _planes.Do(triangle => obj.Children.Add(triangle.Model));
                return obj;
            }
        }
    }

    public class Extrusion
    {
        public int PlaneIndex { get; set; }
        public double Amount { get; set; }
        public double Scale { get; set; }
    }

}