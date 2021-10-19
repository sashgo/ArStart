using System;
using Android.App;
using Android.Content;
using Android.Views;
using ArStart.Droid.Renderers;
using ArStart.Views;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using Java.Util.Functions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ArPage), typeof(ArPageRenderer))]
namespace ArStart.Droid.Renderers
{
    public class ArPageRenderer : PageRenderer, IConsumer
    {
        private Android.Views.View _view;
        private ArFragment _arFragment;
        private Renderable _renderable;

        private static string Asset =
   "https://github.com/KhronosGroup/glTF-Sample-Models/raw/master/2.0/Duck/glTF/Duck.gltf";

        public ArPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            var activity = this.Context as Activity;

            //this.viewModel = this.Element.BindingContext as ARViewModel;

            _view = activity.LayoutInflater.Inflate(Resource.Layout.ARLayout, this, false);
            AddView(_view);

            _arFragment = activity.GetFragmentManager().FindFragmentById(Resource.Id.ar_fragment) as ArFragment;
            if (_arFragment != null)
            {
                //var a = Google.AR.Sceneform.Assets.RenderableSource.InvokeBuilder()
                //    .SetSource(Context, Android.Net.Uri.Parse(Asset), Google.AR.Sceneform.Assets.RenderableSource.SourceType.Glb)
                //    .Build();

                ModelRenderable.InvokeBuilder().SetSource(Context, Resource.Raw.andy).Build()
                    .ThenAccept(this);

                //ModelRenderable.InvokeBuilder()
                //    .SetSource(Context, Android.Net.Uri.Parse("bottle.glb"))
                //    .Build()
                //    .ThenAccept(this);



                _arFragment.TapArPlane += OnTapArPlane;
            }
        }

        /// <summary>
        /// Fix the layout measures to fill the whole view
        /// </summary>        
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
            this._view.Measure(msw, msh);
            this._view.Layout(0, 0, r - l, b - t);
        }


        /// <summary>
        /// Remoev the AR fragment when the pages closes otherwise will throw an error when returning
        /// </summary>
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            var activity = this.Context as Activity;
            activity.GetFragmentManager().BeginTransaction().Remove(this._arFragment).Commit();
        }

        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (_renderable == null) return;

            // Create the Anchor.
            var anchor = e.HitResult.CreateAnchor();
            var anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(_arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            var andy = new TransformableNode(_arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = _renderable;

            andy.Select();
        }

        public void Accept(Java.Lang.Object t)
        {
            if (t is Renderable view)
            {
                _renderable = view;
            }
        }
    }
}
