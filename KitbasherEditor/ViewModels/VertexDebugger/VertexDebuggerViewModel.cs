﻿using CommonControls.MathViews;
using KitbasherEditor.Views.EditorViews.VertexDebugger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using View3D.Components;
using View3D.Components.Component.Selection;
using View3D.Components.Rendering;
using View3D.Rendering;
using View3D.Rendering.Geometry;
using View3D.Rendering.RenderItems;
using View3D.SceneNodes;
using View3D.Utility;

namespace KitbasherEditor.ViewModels.VertexDebugger
{
    class VertexDebuggerViewModel : BaseComponent, IDisposable
    {
        public class VertexInstance
        {
            public int Id { get; set; }
            public Vector4 AnimIndecies { get; set; }
            public Vector4 AnimWeights { get; set; }
            public float TotalWeight { get; set; }

            public Vector3 Normal { get; set; }
            public float NormalLength { get; set; }
            public Vector3 BiNormal { get; set; }
            public float BiNormalLength { get; set; }
            public Vector3 Tangent { get; set; }
            public float TangentLength { get; set; }

            public Vector4 Position { get; set; }

        }

        public ObservableCollection<VertexInstance> VertexList { get; set; } = new ObservableCollection<VertexInstance>();
        VertexInstance _selectedVertex;
        public VertexInstance SelectedVertex
        {
            get { return _selectedVertex; }
            set { SetAndNotify(ref _selectedVertex, value); }
        }

        DoubleViewModel _debugScale = new DoubleViewModel(0.03f);
        public DoubleViewModel DebugScale
        {
            get { return _debugScale; }
            set { SetAndNotify(ref _debugScale, value); }
        }

        LineMeshRender _lineRenderer;
        Effect _lineShader;

        public VertexDebuggerViewModel(IComponentManager componentManager) : base(componentManager)
        {

        }

        public override void Initialize()
        {
            var resourceLib = ComponentManager.GetComponent<ResourceLibary>();
            _lineShader = resourceLib.GetStaticEffect(ShaderTypes.Line);
            _lineRenderer = new LineMeshRender(resourceLib);

            var selectionMgr = ComponentManager.GetComponent<SelectionManager>();
            selectionMgr.SelectionChanged += SelectionMgr_SelectionChanged;
            Refresh();

            base.Initialize();
        }


        private void SelectionMgr_SelectionChanged(ISelectionState state)
        {
            Refresh();
        }

        public void Refresh()
        {
            VertexList.Clear();
            SelectedVertex = null;
            var selectionMgr = ComponentManager.GetComponent<SelectionManager>();

            if (selectionMgr.GetState() is VertexSelectionState selection)
            {
                var mesh = selection.GetSingleSelectedObject() as Rmv2MeshNode;
                var vertexList = selection.SelectedVertices;
                foreach (var vertexIndex in vertexList)
                {
                    var vertexInfo = (mesh.Geometry as MeshObject).GetVertexExtented(vertexIndex);

                    VertexList.Add(new VertexInstance()
                    {
                        Id = vertexIndex,
                        AnimWeights = vertexInfo.BlendWeights,
                        AnimIndecies = vertexInfo.BlendIndices,
                        TotalWeight = vertexInfo.BlendWeights.X + vertexInfo.BlendWeights.Y + vertexInfo.BlendWeights.Z + vertexInfo.BlendWeights.W,

                        Normal = vertexInfo.Normal,
                        NormalLength = vertexInfo.Normal.Length(),

                        BiNormal = vertexInfo.BiNormal,
                        BiNormalLength = vertexInfo.BiNormal.Length(),

                        Tangent = vertexInfo.Tangent,
                        TangentLength = vertexInfo.Tangent.Length(),

                        Position = vertexInfo.Position
                    });
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _lineRenderer.Clear();

            var selection = ComponentManager.GetComponent<SelectionManager>().GetState<VertexSelectionState>();
            if (selection != null)
            {
                var renderEngine = ComponentManager.GetComponent<RenderEngineComponent>();
                var mesh = selection.GetSingleSelectedObject() as Rmv2MeshNode;

                if (SelectedVertex != null)
                {
                    var bb = BoundingBox.CreateFromSphere(new BoundingSphere(mesh.Geometry.GetVertexById(SelectedVertex.Id), 0.05f));
                    renderEngine.AddRenderItem(RenderBuckedId.Normal, new BoundingBoxRenderItem(_lineShader, bb, Color.White));
                }

                var vertexList = selection.SelectedVertices;
                foreach (var vertexIndex in vertexList)
                {
                    var vertexInfo = mesh.Geometry.GetVertexExtented(vertexIndex);
                    var scale = (float)DebugScale.Value;
                    var pos = vertexInfo.Position3();
                    _lineRenderer.AddLine(pos, pos + vertexInfo.Normal * scale, Color.Red);
                    _lineRenderer.AddLine(pos, pos + vertexInfo.BiNormal * scale, Color.Green);
                    _lineRenderer.AddLine(pos, pos + vertexInfo.Tangent * scale, Color.Blue);
                }

                renderEngine.AddRenderItem(RenderBuckedId.Normal, new LineRenderItem() { LineMesh = _lineRenderer, ModelMatrix = mesh.ModelMatrix * Matrix.CreateTranslation(mesh.Material.PivotPoint) });
            }
        }

        public static void Create(IComponentManager componentManager)
        {
            var renderComp = componentManager.GetComponent<RenderEngineComponent>();

            var viewModel = new VertexDebuggerViewModel(componentManager);
            componentManager.AddComponent(viewModel);

            var containingWindow = new Window();
            containingWindow.Title = "Vertex debuger";
            containingWindow.Width = 1200;
            containingWindow.Height = 1100;
            containingWindow.DataContext = viewModel;
            containingWindow.Content = new VertexDebuggerView();
            containingWindow.Closed += (x, y) => { componentManager.RemoveComponent(viewModel); viewModel.Dispose(); };
            containingWindow.Show();
        }

        public void Dispose()
        {
            var selectionMgr = ComponentManager.GetComponent<SelectionManager>();
            if(selectionMgr != null)
                selectionMgr.SelectionChanged -= SelectionMgr_SelectionChanged;
            _lineRenderer.Dispose();
        }
    }
}
