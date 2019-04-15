using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

using Kitware.VTK;


namespace ActivizLearn
{
    public partial class Form1 : Form
    {
        private RenderWindowControl myRenderWindowControl;
        public Form1()
        {
            InitializeComponent();
            myRenderWindowControl = new RenderWindowControl();
            myRenderWindowControl.SetBounds(0, 0, panel1.Width, panel1.Height);
            myRenderWindowControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(this.myRenderWindowControl);

            myRenderWindowControl.Load += RenderWindowControl1_Load;
        }

        private void RenderWindowControl1_Load(object sender, System.EventArgs e)
        {
            try
            {
                //DrawPoint();
                //DrawTriangle();
                //DrawCylinder();
                //DrawSphere(0.5);
                //ReadStlFileDraw();
                //DrawAssembly();
                //DrawTexturePlane();
                //DrawRainBow();
                //DrawVisQuad();
                DrawBuildUGrid();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
            }
        }
        private void DrawPoint()
        {
            // Create the geometry of the points (the coordinate)
            vtkPoints points = vtkPoints.New();
            double[,] p = new double[,] 
            {
                {1.0, 2.0, 3.0},
                {3.0, 1.0, 2.0},
                {2.0, 3.0, 1.0},
                {1.0, 3.0, 3.0}
            };

            // Create topology of the points (a vertex per point)
            vtkCellArray vertices = vtkCellArray.New();
            int nPts = 4;

            int[] ids = new int[nPts];
             for(int i = 0; i<nPts; i++)
                ids[i] = (int)points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

             int size = Marshal.SizeOf(typeof(int)) * nPts;
            IntPtr pIds = Marshal.AllocHGlobal(size);
            Marshal.Copy(ids, 0, pIds, nPts);
             vertices.InsertNextCell(nPts, pIds);
             Marshal.FreeHGlobal(pIds);

             // Create a polydata object
             vtkPolyData pointPoly = vtkPolyData.New();

            // Set the points and vertices we created as the geometry and topology of the polydata
            pointPoly.SetPoints(points);
             pointPoly.SetVerts(vertices);

             // Visualize
             vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
             mapper.SetInputData(pointPoly);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(10);
            vtkRenderWindow renderWindow = myRenderWindowControl.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);
        }

        private void DrawTriangle()
        {
            //创建点数据
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(1.0, 0.0, 0.0);
            points.InsertNextPoint(0.0, 1.0, 0.0);
            points.InsertNextPoint(0.0, 0.0, 0.0);

            //每两个坐标之间分别创建一条线
            //SetId()的第一个参数是线段的端点ID，第二参数是连接的的点的ID
            vtkLine line0 = vtkLine.New();
            line0.GetPointIds().SetId(0, 0);
            line0.GetPointIds().SetId(1, 1);

            vtkLine line1 = vtkLine.New();
            line1.GetPointIds().SetId(0, 1);
            line1.GetPointIds().SetId(1, 2);

            vtkLine line2 = vtkLine.New();
            line2.GetPointIds().SetId(0, 2);
            line2.GetPointIds().SetId(1, 0);

            //创建单元数组，用于存储以上创建的线段
            vtkCellArray lines = vtkCellArray.New();
            lines.InsertNextCell(line0);
            lines.InsertNextCell(line1);
            lines.InsertNextCell(line2);

            //将点和线加入数据集中，前者定义数据集的几何结构，后者定义拓扑结构
            //创建vtkPolyData类型的数据，是一种数据集
            vtkPolyData polyData = vtkPolyData.New();

            //将创建的点数据加入vtkPolyData数据里
            polyData.SetPoints(points);  //点数据定义了polydata数据集的几何结构。
            polyData.SetLines(lines);   //定义拓扑结构

            //显示数据
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputData(polyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetColor(1.0, 0.0, 0.0);           
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            vtkRenderer renderer = renWin.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(1.0, 1.0, 1.0);
            renderer.AddActor(actor);


        }

        private void DrawCylinder()
        {
            vtkCylinderSource cylinderSource = vtkCylinderSource.New();
            //cylinderSource.SetHeight(3.0);
            //cylinderSource.SetRadius(0.1);
            cylinderSource.SetResolution(8);
            vtkPolyDataMapper cylinderMapper = vtkPolyDataMapper.New();
            cylinderMapper.SetInputConnection(cylinderSource.GetOutputPort());
            vtkActor cylinderActor = vtkActor.New();
            cylinderActor.SetMapper(cylinderMapper);
            //cylinderActor.GetProperty().SetColor(1.0, 0.0, 0.0);
            cylinderActor.GetProperty().SetColor((float)Color.Tomato.R/256, (float)Color.Tomato.G/256, (float)Color.Tomato.B/256);
            cylinderActor.RotateX(30.0);
            cylinderActor.RotateY(-45.0);
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            vtkRenderer renderer = renWin.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(1.0, 1.0, 1.0);
            renderer.AddActor(cylinderActor);
        }

        private void DrawSphere(double radius)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetThetaResolution(8);
            sphereSource.SetPhiResolution(16);
            sphereSource.SetRadius(radius);

            vtkShrinkPolyData shrink = vtkShrinkPolyData.New();
            shrink.SetInputConnection(sphereSource.GetOutputPort());
            shrink.SetShrinkFactor(0.9);

            vtkPolyDataMapper sphereMapper = vtkPolyDataMapper.New();
            //sphereMapper.SetInputConnection(sphereSource.GetOutputPort());
            sphereMapper.SetInputConnection(shrink.GetOutputPort());

            vtkActor sphereActor = vtkActor.New();
            sphereActor.SetMapper(sphereMapper);
            sphereActor.GetProperty().SetColor(1, 0, 0);

            vtkRenderer sphereRender = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            renWin.AddRenderer(sphereRender);

            sphereRender.AddActor(sphereActor);
            sphereRender.SetBackground(0.0, 0.0, 1.0);
        }

        private void ReadStlFileDraw()
        {
            vtkSTLReader stlReader = vtkSTLReader.New();
            //stlReader.SetFileName(@"..\..\Data\anchor_hook.stl");
            stlReader.SetFileName(@"..\..\Data\42400-IDGH.stl");
            vtkShrinkPolyData shrink = vtkShrinkPolyData.New();
            shrink.SetInputConnection(stlReader.GetOutputPort());
            shrink.SetShrinkFactor(0.85);

            vtkPolyDataMapper stlMapper = vtkPolyDataMapper.New();
            //stlMapper.SetInputConnection(stlReader.GetOutputPort());
            stlMapper.SetInputConnection(shrink.GetOutputPort());

            vtkLODActor stlActor = vtkLODActor.New();
            stlActor.SetMapper(stlMapper);
            stlActor.GetProperty().SetColor((float)Color.Orange.R/256, (float)Color.Orange.G / 256,(float)Color.Orange.B / 256);  //设置actor的颜色

            vtkRenderer stlRender = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            renWin.AddRenderer(stlRender);

            stlRender.AddActor(stlActor);
            stlRender.SetBackground(1.0, 1.0, 1.0);

        }

        private void DrawAssembly()
        {
            //Create four parts: a top level assembly and three primitives

            vtkSphereSource sphereSource = vtkSphereSource.New();
            vtkPolyDataMapper sphereMapper = vtkPolyDataMapper.New();
            sphereMapper.SetInputConnection(sphereSource.GetOutputPort());
            vtkActor sphereActor = vtkActor.New();
            sphereActor.SetMapper(sphereMapper);
            sphereActor.SetOrigin(2, 1, 3);
            sphereActor.RotateY(6);
            sphereActor.SetPosition(2.25, 0, 0);
            sphereActor.GetProperty().SetColor(1, 0, 1);

            vtkCubeSource cubeSource = vtkCubeSource.New();
            vtkPolyDataMapper cubeMapper = vtkPolyDataMapper.New();
            cubeMapper.SetInputConnection(cubeSource.GetOutputPort());
            vtkActor cubeActor = vtkActor.New();
            cubeActor.SetMapper(cubeMapper);
            cubeActor.SetPosition(0, 2.25, 0);
            cubeActor.GetProperty().SetColor(0, 0, 1);

            vtkConeSource coneSource = vtkConeSource.New();
            vtkPolyDataMapper coneMapper = vtkPolyDataMapper.New();
            coneMapper.SetInputConnection(coneSource.GetOutputPort());
            vtkActor coneActor = vtkActor.New();
            coneActor.SetMapper(coneMapper);
            coneActor.SetPosition(0, 0, 2.25);
            coneActor.GetProperty().SetColor(0, 1, 0);

            vtkCylinderSource cylinderSource = vtkCylinderSource.New();
            vtkPolyDataMapper cylinderMapper = vtkPolyDataMapper.New();
            cylinderMapper.SetInputConnection(cylinderSource.GetOutputPort());
            vtkActor cylinderActor = vtkActor.New();
            cylinderActor.SetMapper(cylinderMapper);
            //cylinderActor.SetPosition(0, 0, 0);
            cylinderActor.GetProperty().SetColor(1, 0, 0);

            vtkAssembly assembly = vtkAssembly.New();
            assembly.AddPart(cylinderActor);
            assembly.AddPart(sphereActor);
            assembly.AddPart(cubeActor);
            assembly.AddPart(coneActor);
            assembly.SetOrigin(5, 10, 5);
            assembly.AddPosition(5, 0, 0);
            assembly.RotateX(15);

            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;

            renWin.AddRenderer(renderer);
            renderer.AddActor(assembly);
            renderer.AddActor(coneActor);

        }

        private void DrawTexturePlane()
        {
            //load in the texture map
            vtkBMPReader bmpReader = vtkBMPReader.New();           
            bmpReader.SetFileName(@"..\..\Data\masonry.bmp");
            vtkTexture atext = vtkTexture.New();
            atext.SetInputConnection(bmpReader.GetOutputPort());
            atext.InterpolateOn();

            //create a plane source and actor
            vtkPlaneSource plane = vtkPlaneSource.New();
            plane.SetPoint1(0, 0, 0);
            vtkPolyDataMapper planeMapper = vtkPolyDataMapper.New();
            planeMapper.SetInputConnection(plane.GetOutputPort());
            vtkActor planeActor = vtkActor.New();
            planeActor.SetMapper(planeMapper);
            planeActor.SetTexture(atext);

            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;

            renWin.AddRenderer(renderer);
            renderer.AddActor(planeActor);
        }

        private void DrawRainBow()
        {
            //# First create pipeline a simple pipeline that reads a structure grid
            //# and then extracts a plane from the grid. The plane will be colored
            //# differently by using different lookup tables.
            //#
            //# Note: the Update method is manually invoked because it causes the
            //# reader to read; later on we use the output of the reader to set
            //# a range for the scalar values.
            vtkMultiBlockPLOT3DReader pl3d = vtkMultiBlockPLOT3DReader.New();
            pl3d.SetXYZFileName(@"..\..\Data\combxyz.bin");
            pl3d.SetQFileName(@"..\..\Data\combq.bin");
            pl3d.SetScalarFunctionNumber(100);
            pl3d.SetVectorFunctionNumber(202);
            pl3d.Update();
            vtkDataObject pl3d_output = pl3d.GetOutput().GetBlock(0);

            vtkStructuredGridGeometryFilter planeFilter = vtkStructuredGridGeometryFilter.New();
            planeFilter.SetInputData(pl3d_output);
            planeFilter.SetExtent(1, 100, 1, 100, 7, 7);
            vtkLookupTable lut = vtkLookupTable.New();
            vtkPolyDataMapper planeMapper = vtkPolyDataMapper.New();
            planeMapper.SetLookupTable(lut);
            planeMapper.SetInputConnection(planeFilter.GetOutputPort());
            //planeMapper.SetScalarRange(pl3d_output.)
            vtkActor planeActor = vtkActor.New();
            planeActor.SetMapper(planeMapper);

            //this creates an outline around the data
            vtkStructuredGridOutlineFilter outlineFilter = vtkStructuredGridOutlineFilter.New();
            outlineFilter.SetInputData(pl3d_output);
            vtkPolyDataMapper outlineMapper = vtkPolyDataMapper.New();
            outlineMapper.SetInputConnection(outlineFilter.GetOutputPort());
            vtkActor outlineActor = vtkActor.New();
            outlineActor.SetMapper(outlineMapper);

            //Much of the following is commented out. To try different lookup tables.
            //This create a black to white lut
            //lut.SetHueRange(0, 0);
            //lut.SetSaturationRange(0, 0);
            //lut.SetValueRange(0.2, 1.0);

            //This creates a red to blue lut
            //lut.SetHueRange(0.0, 0.677);

            //This creates a blue to red lue
            lut.SetHueRange(0.667, 0.0);

            //This creates a weird effect. the Build() method cause lookup
            //table to allocate memory and create a table based on the correct
            //hue, saturatioin, value, and alpha range. Here we then 
            //manully overwrite the value generated by the Build() method.
            lut.SetNumberOfColors(256);
            lut.Build();
            for(int i=0;i<16;i++)
            {
                lut.SetTableValue(i * 16, (float)Color.Red.R / 256, (float)Color.Red.G / 256, (float)Color.Red.B / 256, 1);
                lut.SetTableValue(i * 16+1, (float)Color.Green.R / 256, (float)Color.Green.G / 256, (float)Color.Green.B / 256, 1);
                lut.SetTableValue(i * 16+2, (float)Color.Blue.R / 256, (float)Color.Blue.G / 256, (float)Color.Blue.B / 256, 1);
                lut.SetTableValue(i * 16+3, (float)Color.Black.R / 256, (float)Color.Black.G / 256, (float)Color.Black.B / 256, 1);
            }


            //Create the renderwindow, the render and both actors
            vtkRenderer ren = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            renWin.AddRenderer(ren);

            //Add the actors to the renderer, set the backgroud
            ren.AddActor(outlineActor);
            ren.AddActor(planeActor);

            ren.SetBackground(0.1, 0.2, 0.4);
            ren.TwoSidedLightingOff();
        }
        
        private void DrawVisQuad()
        {
            //# This example demonstrates the use of the contour filter, and the use of
            //# the vtkSampleFunction to generate a volume of data samples from an
            //# implicit function.

            //# VTK supports implicit functions of the form f(x,y,z)=constant. These
            //# functions can represent things spheres, cones, etc. Here we use a
            //# general form for a quadric to create an elliptical data field.

            vtkQuadric quadricFunction = vtkQuadric.New();
            quadricFunction.SetCoefficients(0.5, 1, 0.2, 0, 0.1, 0, 0, 0.2, 0, 0);

            //# vtkSampleFunction samples an implicit function over the x-y-z range
            //# specified (here it defaults to -1,1 in the x,y,z directions).
            vtkSampleFunction sample = vtkSampleFunction.New();
            sample.SetSampleDimensions(30, 30, 30);
            sample.SetImplicitFunction(quadricFunction);

            //# Create five surfaces F(x,y,z) = constant between range specified. The
            //# GenerateValues() method creates n isocontour values between the range
            //# specified.
            vtkContourFilter contourFilter = vtkContourFilter.New();
            contourFilter.SetInputConnection(sample.GetOutputPort());
            contourFilter.GenerateValues(10, 0.0, 1.2);

            vtkPolyDataMapper contMapper = vtkPolyDataMapper.New();
            contMapper.SetInputConnection(contourFilter.GetOutputPort());
            contMapper.SetScalarRange(0.0, 1.2);

            vtkActor conActor = vtkActor.New();
            conActor.SetMapper(contMapper);

            //We'll put a simple outline around the data
            vtkOutlineFilter outline = vtkOutlineFilter.New();
            outline.SetInputConnection(sample.GetOutputPort());

            vtkPolyDataMapper outlineMapper = vtkPolyDataMapper.New();
            outlineMapper.SetInputConnection(outline.GetOutputPort());

            vtkActor outlineActor = vtkActor.New();
            outlineActor.SetMapper(outlineMapper);
            outlineActor.GetProperty().SetColor(0, 0, 0);

            //The usual rendering stuff
            vtkRenderer ren = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            //vtkRenderWindow renWin = vtkRenderWindow.New();
            renWin.AddRenderer(ren);
            //vtkRenderWindowInteractor iren = vtkRenderWindowInteractor.New();
            //iren.SetRenderWindow(renWin);

            ren.SetBackground(1, 1, 1);
            ren.AddActor(conActor);
            ren.AddActor(outlineActor);

            //iren.Initialize();
            //renWin.Render();
            //iren.Start();

        }

        private void DrawBuildUGrid()
        {
            //# This example shows how to manually construct unstructured grids
            //# using C#.  Unstructured grids require explicit point and cell
            //# representations, so every point and cell must be created, and then
            //# added to the vtkUnstructuredGrid instance.

            //# Create several unstructured grids each containing a cell of a
            //# different type.

            //create voxel
            vtkPoints voxelPoints = vtkPoints.New();
            voxelPoints.SetNumberOfPoints(8);
            voxelPoints.InsertPoint(0, 0, 0, 0);
            voxelPoints.InsertPoint(1, 1, 0, 0);
            voxelPoints.InsertPoint(2, 0, 1, 0);
            voxelPoints.InsertPoint(3, 1, 1, 0);
            voxelPoints.InsertPoint(4, 0, 0, 1);
            voxelPoints.InsertPoint(5, 1, 0, 1);
            voxelPoints.InsertPoint(6, 0, 1, 1);
            voxelPoints.InsertPoint(7, 1, 1, 1);
            vtkVoxel aVoxel = vtkVoxel.New();
            aVoxel.GetPointIds().SetId(0, 0);
            aVoxel.GetPointIds().SetId(1, 1);
            aVoxel.GetPointIds().SetId(2, 2);
            aVoxel.GetPointIds().SetId(3, 3);
            aVoxel.GetPointIds().SetId(4, 4);
            aVoxel.GetPointIds().SetId(5, 5);
            aVoxel.GetPointIds().SetId(6, 6);
            aVoxel.GetPointIds().SetId(7, 7);
            vtkUnstructuredGrid aVoxelGrid = vtkUnstructuredGrid.New();
            aVoxelGrid.Allocate(1, 1);
            aVoxelGrid.InsertNextCell(aVoxel.GetCellType(), aVoxel.GetPointIds());
            aVoxelGrid.SetPoints(voxelPoints);
            vtkDataSetMapper aVoxelMapper = vtkDataSetMapper.New();
            aVoxelMapper.SetInputData(aVoxelGrid);
            vtkActor aVoxelActor = vtkActor.New();
            aVoxelActor.SetMapper(aVoxelMapper);
            aVoxelActor.GetProperty().SetDiffuseColor(1, 0, 0);

            //create Hexahedron
            vtkPoints hexahedronPoints = new vtkPoints();
            hexahedronPoints.SetNumberOfPoints(8);
            hexahedronPoints.InsertPoint(0, 0, 0, 0);
            hexahedronPoints.InsertPoint(1, 1, 0, 0);
            hexahedronPoints.InsertPoint(2, 1, 1, 0);
            hexahedronPoints.InsertPoint(3, 0, 1, 0);
            hexahedronPoints.InsertPoint(4, 0, 0, 1);
            hexahedronPoints.InsertPoint(5, 1, 0, 1);
            hexahedronPoints.InsertPoint(6, 1, 1, 1);
            hexahedronPoints.InsertPoint(7, 0, 1, 1);
            vtkHexahedron aHexahedron = new vtkHexahedron();
            aHexahedron.GetPointIds().SetId(0, 0);
            aHexahedron.GetPointIds().SetId(1, 1);
            aHexahedron.GetPointIds().SetId(2, 2);
            aHexahedron.GetPointIds().SetId(3, 3);
            aHexahedron.GetPointIds().SetId(4, 4);
            aHexahedron.GetPointIds().SetId(5, 5);
            aHexahedron.GetPointIds().SetId(6, 6);
            aHexahedron.GetPointIds().SetId(7, 7);
            vtkUnstructuredGrid aHexahedronGrid = new vtkUnstructuredGrid();
            aHexahedronGrid.Allocate(1, 1);
            aHexahedronGrid.InsertNextCell(aHexahedron.GetCellType(), aHexahedron.GetPointIds());
            aHexahedronGrid.SetPoints(hexahedronPoints);

            vtkDataSetMapper aHexahedronMapper = new vtkDataSetMapper();
            aHexahedronMapper.SetInputData(aHexahedronGrid);
            vtkActor aHexahedronActor = new vtkActor();
            aHexahedronActor.SetMapper(aHexahedronMapper);
            aHexahedronActor.AddPosition(2, 0, 0);
            aHexahedronActor.GetProperty().SetDiffuseColor(1, 1, 0);


            vtkRenderer render = vtkRenderer.New();
            vtkRenderWindow renWin = myRenderWindowControl.RenderWindow;
            renWin.AddRenderer(render);

            render.SetBackground(0, 0, 1);
            render.AddActor(aVoxelActor);
            render.AddActor(aHexahedronActor);

        }

        private void DrawTest()
        {
            vtkProp3D prop3D;
            vtkActor actor = vtkActor.New();
            vtkActor2D actor2D = vtkActor2D.New();
            vtkLODActor lODActor = vtkLODActor.New();
            vtkLODProp3D lodProp3d = vtkLODProp3D.New();
            vtkCamera camera = vtkCamera.New();
            vtkCameraActor cameraActor = vtkCameraActor.New();
            vtkLight light = vtkLight.New();
            vtkLightActor lightActor = vtkLightActor.New();
            vtkPicker picker = vtkPicker.New();
            vtkPointPicker pointPicker = vtkPointPicker.New();
            vtkCellPicker cellPicker = vtkCellPicker.New();
            vtkAreaPicker areaPicker = vtkAreaPicker.New();

            vtkAssembly assembly = vtkAssembly.New();
            vtkConeSource coneSource = vtkConeSource.New();
            vtkCone cone = vtkCone.New();

            vtkArcSource arcSource = vtkArcSource.New();
            vtkLineSource lineSource = vtkLineSource.New();
            vtkPointSource pointSource = vtkPointSource.New();

            vtkPolyData polyData = vtkPolyData.New();
            vtkArrayReader arrayReader = vtkArrayReader.New();
            vtkArrayDataReader arrayDataReader = vtkArrayDataReader.New();
            vtkArrayWriter arrayWriter = vtkArrayWriter.New();
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            vtkRenderWindowInteractor3D renderWindowInteractor3D = vtkRenderWindowInteractor3D.New();
            vtkInteractorStyle interactorStyle = vtkInteractorStyle.New();
            vtkInteractorStyle3D interactorStyle3D = vtkInteractorStyle3D.New();
            vtkInteractorStyleFlight interactorStyleFlight = vtkInteractorStyleFlight.New();
            vtkInteractorStyleTrackball interactorStyleTrackball = vtkInteractorStyleTrackball.New();

            vtkVolume volume = vtkVolume.New();
            vtkVolumeMapper volumeMapper;
            vtkSmartVolumeMapper smartVolumeMapper = vtkSmartVolumeMapper.New();
            vtkUnstructuredGridVolumeMapper unstructuredGridVolumeMapper;
            vtkUnstructuredGridVolumeRayCastMapper unstructuredGridVolumeRayCastMapper = vtkUnstructuredGridVolumeRayCastMapper.New();
            vtkGPUVolumeRayCastMapper gPUVolumeRayCastMapper = vtkGPUVolumeRayCastMapper.New();
            vtkVolumeRayCastMapper volumeRayCastMapper = vtkVolumeRayCastMapper.New();
            vtkFixedPointVolumeRayCastMapper pointVolumeRayCastMapper = vtkFixedPointVolumeRayCastMapper.New();
            vtkOpenGLGPUVolumeRayCastMapper openGLGPUVolumeRayCastMapper = vtkOpenGLGPUVolumeRayCastMapper.New();
            vtkVolumeProperty volumeProperty = vtkVolumeProperty.New();

            vtkTexture texture = vtkTexture.New();
            vtkCoordinate coordinate = vtkCoordinate.New();
            vtkImageData vtkImage = vtkImageData.New();

            vtkBMPReader bMPReader = vtkBMPReader.New();
            vtkJPEGReader jPEGReader = vtkJPEGReader.New();
            vtkPNGReader pNGReader = vtkPNGReader.New();
            vtkTIFFReader tIFFReader = vtkTIFFReader.New();
            vtkOBJReader oBJReader = vtkOBJReader.New();


            vtkContourFilter contourFilter = vtkContourFilter.New();
            vtkSynchronizedTemplates2D synchronizedTemplates2D = vtkSynchronizedTemplates2D.New();
            vtkSynchronizedTemplates3D synchronizedTemplates3D = vtkSynchronizedTemplates3D.New();
            vtkSynchronizedTemplatesCutter3D synchronizedTemplatesCutter3D = vtkSynchronizedTemplatesCutter3D.New();

            vtkImageMapper imageMapper = vtkImageMapper.New();
            vtkImageSliceMapper imageSliceMapper = vtkImageSliceMapper.New();
            vtkImageResliceMapper imageResliceMapper = vtkImageResliceMapper.New();

            vtkStructuredGridReader structuredGridReader = vtkStructuredGridReader.New(); 
            vtkRungeKutta4 integ = vtkRungeKutta4.New();
            vtkStreamTracer streamer = vtkStreamTracer.New();
            vtkTubeFilter streamTube = vtkTubeFilter.New();
            vtkRuledSurfaceFilter ruledSurfaceFilter = vtkRuledSurfaceFilter.New();
            vtkPlane plane = vtkPlane.New();
            vtkCutter cutter = new vtkCutter();
            vtkMergeFilter mergeFilter = vtkMergeFilter.New();
            vtkImageLuminance imageLuminance = vtkImageLuminance.New();
            vtkImageDataGeometryFilter imageDataGeometryFilter = vtkImageDataGeometryFilter.New();
            vtkWarpScalar warpScalar = vtkWarpScalar.New();
            vtkWarpVector warpVector = vtkWarpVector.New();

        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            myRenderWindowControl.SetBounds(0, 0, panel1.Width, panel1.Height);
            
        }
    }
}
