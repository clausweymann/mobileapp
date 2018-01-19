﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using CoreMotion;
using Foundation;
using UIKit;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;

namespace Toggl.Daneel.Views
{
    [Register(nameof(SpiderOnARopeView))]
    public class SpiderOnARopeView : UIView
    {
        private const double height = 155.0f;
        private const int chainLength = 8;
        private const double chainLinkHeight = height / chainLength;
        private const double chainWidth = 2f;
        private readonly CGColor RopeColor = Color.Main.SpiderNetColor.ToNativeColor().CGColor;

        private UIDynamicAnimator spiderAnimator;
        private UIGravityBehavior gravity;
        private CMMotionManager motionManager;
        private UIImage spiderImage;
        private UIView spiderView;
        private UIView[] links;
        private CGPoint anchorPoint;

        public SpiderOnARopeView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            spiderImage = UIImage.FromBundle("icJustSpider");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            spiderAnimator?.Dispose();
            motionManager?.Dispose();
            gravity?.Dispose();
            spiderView?.Dispose();

            spiderAnimator = null;
            motionManager = null;
            gravity = null;
            spiderView = null;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var ctx = UIGraphics.GetCurrentContext();
            if (ctx == null) return;

            if (links != null)
            {
                var points = links.Select(links => links.Center).ToArray();
                var path = createCurvedPath(anchorPoint, points);
                ctx.SetStrokeColor(UIColor.Black.CGColor);
                ctx.SetLineWidth((nfloat)chainWidth);
                ctx.AddPath(path);
                ctx.DrawPath(CGPathDrawingMode.Stroke);
            }
        }

        public void Initialize()
        {
            BackgroundColor = UIColor.Clear;
            reset();
            anchorPoint = new CGPoint(Center.X, 0);

            if (spiderView == null)
            {
                spiderView = new UIImageView(spiderImage);
                AddSubview(spiderView);
            }

            spiderView.Center = new CGPoint(Center.X, -height - spiderImage.Size.Height);
            spiderView.Layer.AnchorPoint = new CGPoint(0.5, 0.1);

            spiderAnimator = new UIDynamicAnimator(this);

            var spider = new UIDynamicItemBehavior(spiderView);
            spider.Action = () => SetNeedsDisplay();
            spider.AllowsRotation = true;
            spider.Density = 2.0f;
            spiderAnimator.AddBehavior(spider);

            links = createRope();

            gravity = new UIGravityBehavior(links);
            spiderAnimator.AddBehavior(gravity);

            motionManager?.Dispose();
            motionManager = new CMMotionManager();
            motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {
                var ax = data.Acceleration.X;
                var ay = data.Acceleration.Y;
                var angle = -(nfloat)Math.Atan2(ay, ax);
                var magnitude = 4 * (nfloat)(Math.Sqrt(ax * ax + ay * ay) / height);

                gravity.Angle = angle;

                var force = new UIPushBehavior(UIPushBehaviorMode.Instantaneous, spiderView);
                force.SetAngleAndMagnitude(angle, magnitude);
                spiderAnimator.AddBehavior(force);
            });
        }

        private void reset()
        {
            spiderAnimator?.Dispose();
            motionManager?.Dispose();
            gravity?.Dispose();
            for (var i = 0; i < Subviews.Length; i++)
            {
                Subviews[i].RemoveFromSuperview();
            }
        }

        private UIView[] createRope()
        {
            var chain = new List<UIView>();
            UIView lastLink = null;

            for (int i = 0; i < chainLength; i++)
            {
                var chainLink = createChainLink(i * chainLinkHeight, lastLink);
                chain.Add(chainLink);
                lastLink = chainLink;
            }

            var spiderAttachment = new UIAttachmentBehavior(spiderView, UIOffset.Zero, lastLink, UIOffset.Zero);
            spiderAttachment.Length = 1f;
            spiderAnimator.AddBehavior(spiderAttachment);

            chain.Add(spiderView);

            return chain.ToArray();
        }

        private UIView createChainLink(double y, UIView lastLink)
        {
            var chainLink = new UIView();
            chainLink.BackgroundColor = UIColor.Clear;
            chainLink.Frame = new CGRect(Center.X, -y, chainWidth, chainLinkHeight);

            AddSubview(chainLink);

            var chainDynamics = new UIDynamicItemBehavior(chainLink);
            spiderAnimator.AddBehavior(chainDynamics);

            var attachment = lastLink == null
                ? new UIAttachmentBehavior(chainLink, anchorPoint)
                : new UIAttachmentBehavior(chainLink, lastLink);
            attachment.Length = (nfloat)chainLinkHeight;
            spiderAnimator.AddBehavior(attachment);

            return chainLink;
        }

        private CGPath createCurvedPath(CGPoint anchor, CGPoint[] points)
        {
            var path = new UIBezierPath();

            if (points.Length > 1)
            {
                var a = points[0];
                var b = a;
                path.MoveTo(anchor);

                for (int i = 1; i < points.Length; i++)
                {
                    var c = points[i];
                    var d = i < points.Length - 1 ? points[i + 1] : points[i];

                    var cpb = new CGPoint((-1.0 / 6.0 * a.X) + b.X + (1.0 / 6.0 * c.X), (-1.0 / 6.0 * a.Y) + b.Y + (1.0 / 6.0 * c.Y));
                    var cpc = new CGPoint((1.0 / 6.0 * b.X) + c.X + (-1.0 / 6.0 * d.X), (1.0 / 6.0 * b.Y) + c.Y + (-1.0 / 6.0 * d.Y));

                    path.AddCurveToPoint(c, cpb, cpc);

                    a = b;
                    b = c;
                }
            }

            return path.CGPath;
        }
    }
}
