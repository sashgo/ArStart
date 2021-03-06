using System;
using System.Linq;
using ARKit;
using ArStart.iOS.Delegates;
using ArStart.iOS.Renderers;
using ArStart.Views;
using CoreGraphics;
using Foundation;
using OpenTK;
using SceneKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ArPage), typeof(ArPageRenderer))]
namespace ArStart.iOS.Renderers
{
    public class ArPageRenderer : PageRenderer, IARSCNViewDelegate
    {
        private ARSCNView _sceneView;

        public ArPageRenderer()
        {
            _sceneView = new ARSCNView();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _sceneView = new ARSCNView
            {
                Frame = this.View.Frame,
                UserInteractionEnabled = true,
                Delegate = new PlaceModelDelegate(),
                Session =
                {
                    Delegate = new SessionDelegate()
                },
            };
            this.View.AddSubview(_sceneView);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _sceneView.Session.Run(new ARWorldTrackingConfiguration
            {
                AutoFocusEnabled = true,
                PlaneDetection = ARPlaneDetection.Horizontal,
                LightEstimationEnabled = true,
                WorldAlignment = ARWorldAlignment.GravityAndHeading
            }, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _sceneView.Session.Pause();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                var loc = touch.LocationInView(_sceneView);
                var worldPos = WorldPositionFromHitTest(loc);
                if (worldPos.Item1.HasValue)
                {
                    PlaceModel(worldPos.Item1.Value);
                }
            }
        }

        private SCNVector3 PositionFromTransform(NMatrix4 xform)
        {
            return new SCNVector3(xform.M14, xform.M24, xform.M34);
        }

        Tuple<SCNVector3?, ARAnchor> WorldPositionFromHitTest(CGPoint pt)
        {
            //Hit test against existing anchors
            var hits = _sceneView.HitTest(pt, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hits != null && hits.Length > 0)
            {
                var anchors = hits.Where(r => r.Anchor is ARPlaneAnchor);
                if (anchors.Count() > 0)
                {
                    var first = anchors.First();
                    var pos = PositionFromTransform(first.WorldTransform);
                    return new Tuple<SCNVector3?, ARAnchor>(pos, (ARPlaneAnchor)first.Anchor);
                }
            }
            return new Tuple<SCNVector3?, ARAnchor>(null, null);
        }

        private void PlaceModel(SCNVector3 pos)
        {
            var asset = $"Assets.scnassets/bottle.dae";

            var modelNode = CreateModelFromFile(asset, pos);
            if (modelNode == null) return;
                _sceneView.Scene.RootNode.AddChildNode(modelNode);
        }

        private SCNNode CreateModelFromFile(string asset, SCNVector3 vector)
        {
            try
            {
                var scene = SCNScene.FromFile(asset);
                var modelNodes = scene.RootNode.ChildNodes;

                var modelNode = new SCNNode
                {
                    Position = vector,
                    Scale = new SCNVector3(0.5f, 0.5f, 0.5f)
                };
                modelNode.AddNodes(modelNodes);
                return modelNode;
            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }

            return null;

        }
    }
}
