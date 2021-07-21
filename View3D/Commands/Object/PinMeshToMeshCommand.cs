﻿using Microsoft.Xna.Framework;
using MonoGame.Framework.WpfInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using View3D.Animation;
using View3D.Components.Component.Selection;
using View3D.Rendering.Geometry;
using View3D.SceneNodes;

namespace View3D.Commands.Object
{
    public class PinMeshToMeshCommand : CommandBase<CreateAnimatedMeshPoseCommand>
    {
        IGeometry _originalGeo;
        string _originalSkeletonName;
        ISelectionState _oldState;
        SelectionManager _selectionManager;

        Rmv2MeshNode _sourceMesh; 
        Rmv2MeshNode _objectToPin;
        GameSkeleton _skeleton;

        public PinMeshToMeshCommand(Rmv2MeshNode sourceMesh, Rmv2MeshNode objectToPin, GameSkeleton skeleton)
        {
            _sourceMesh = sourceMesh;
            _objectToPin = objectToPin;
            _skeleton = skeleton;
        }

        public override string GetHintText()
        {
            return "Pin mesh to mesh";
        }

        public override void Initialize(IComponentManager componentManager)
        {
            _selectionManager = componentManager.GetComponent<SelectionManager>();
            base.Initialize(componentManager);
        }

        protected override void ExecuteCommand()
        {
            _originalGeo = _objectToPin.Geometry.Clone();
            _originalSkeletonName = _objectToPin.MeshModel.ParentSkeletonName;
            _oldState = _selectionManager.GetStateCopy();

            // Update the skeleton
            _objectToPin.MeshModel.ParentSkeletonName = _sourceMesh.MeshModel.ParentSkeletonName;

            // Use the center of the bb box to find the closest vert
            var bbCorners = _objectToPin.Geometry.BoundingBox.GetCorners();
            var bbCenter = new Vector3(bbCorners.Average(x => x.X), bbCorners.Average(x => x.Y), bbCorners.Average(x => x.Z));
            //Vector3.Transform(bbCenter, _objectToPin.ModelMatrix)


            // Get closest vert
            var sourceGeo = _sourceMesh.Geometry as Rmv2Geometry;
            int closestSourceVert = -1;
            float closestDistSqrt = float.MaxValue;
            for (int i = 0; i < sourceGeo.VertexCount(); i++)
            {
                var vertPos = sourceGeo.GetVertexById(i);
                var distSqrt = (bbCenter - vertPos).Length();
                if (distSqrt < closestDistSqrt)
                {
                    closestDistSqrt = distSqrt;
                    closestSourceVert = i;
                }
            }

            // Update the object to pin
            var sourceVert = sourceGeo.GetVertexExtented(closestSourceVert);
            for (int i = 0; i < _objectToPin.Geometry.VertexCount(); i++)
            {
                _objectToPin.Geometry.SetVertexBlendIndex(i, sourceVert.BlendIndices);
                _objectToPin.Geometry.SetVertexWeights(i, sourceVert.BlendWeights);
            }

            _objectToPin.Geometry.RebuildVertexBuffer();
        }

        protected override void UndoCommand()
        {
            _objectToPin.Geometry = _originalGeo;
            _objectToPin.MeshModel.ParentSkeletonName = _originalSkeletonName;
            _selectionManager.SetState(_oldState);
        }
    }
}
