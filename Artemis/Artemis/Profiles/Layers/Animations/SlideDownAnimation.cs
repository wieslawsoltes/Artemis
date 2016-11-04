﻿using System.Windows;
using System.Windows.Media;
using Artemis.Profiles.Layers.Interfaces;
using Artemis.Profiles.Layers.Models;

namespace Artemis.Profiles.Layers.Animations
{
    public class SlideDownAnimation : ILayerAnimation
    {
        public string Name { get; } = "Slide down";

        public void Update(LayerModel layerModel, bool updateAnimations)
        {
            var progress = layerModel.Properties.AnimationProgress;
            if (MustExpire(layerModel))
                progress = 0;
            progress = progress + layerModel.Properties.AnimationSpeed*2;

            // If not previewing, store the animation progress in the actual model for the next frame
            if (updateAnimations)
                layerModel.Properties.AnimationProgress = progress;
        }

        public void Draw(LayerPropertiesModel props, LayerPropertiesModel applied, DrawingContext c)
        {
            if (applied.Brush == null)
                return;

            const int scale = 4;
            // Set up variables for this frame
            var rect = applied.Contain
                ? new Rect(applied.X*scale, applied.Y*scale, applied.Width*scale, applied.Height*scale)
                : new Rect(props.X*scale, props.Y*scale, props.Width*scale, props.Height*scale);

            var s1 = new Rect(new Point(rect.X, rect.Y + props.AnimationProgress), new Size(rect.Width, rect.Height));
            var s2 = new Rect(new Point(s1.X, s1.Y - rect.Height), new Size(rect.Width, rect.Height + .5));

            var clip = new Rect(applied.X*scale, applied.Y*scale, applied.Width*scale, applied.Height*scale);

            c.PushClip(new RectangleGeometry(clip));
            c.DrawRectangle(applied.Brush, null, s1);
            c.DrawRectangle(applied.Brush, null, s2);
            c.Pop();
        }

        public bool MustExpire(LayerModel layer)
        {
            return layer.Properties.AnimationProgress + layer.Properties.AnimationSpeed*2 >= layer.Properties.Height*4;
        }
    }
}