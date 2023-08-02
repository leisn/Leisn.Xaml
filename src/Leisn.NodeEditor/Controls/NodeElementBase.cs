// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

namespace Leisn.NodeEditor.Controls
{
    public abstract class NodeElementBase
    {
        public NodeElementBase? Parent { get; set; }
        public List<NodeElementBase> Children { get; } = new();
        public SKRect Bounds { get; set; }
        public SKPoint Location
        {
            get => Bounds.Location;
            set => Bounds = Bounds.With(value);
        }
        public SKSize Size
        {
            get => Bounds.Size;
            set => Bounds = Bounds.With(value);
        }
        public float Scale { get; set; } = 1f;
        public Rotation Rotation { get; set; }
        public SKMatrix LocalTransform
        {
            get
            {
                var scale = SKMatrix.CreateScale(Scale, Scale);
                var rotaion = SKMatrix.CreateRotationDegrees(Rotation.Angle,
                    Size.Width * Rotation.CenterX, Size.Height * Rotation.CenterY);
                var translation = SKMatrix.CreateTranslation(Location.X, Location.Y);
                return scale.PostConcat(rotaion).PostConcat(translation);
            }
        }
        public SKMatrix WorldTransform
        {
            get
            {
                if (Parent == null)
                    return LocalTransform;
                return LocalTransform.PreConcat(Parent.WorldTransform);
            }
        }

        protected abstract void DrawSelf(SKCanvas canvas);

        public virtual void Draw(SKCanvas canvas)
        {
            var matrix = canvas.TotalMatrix;
            try
            {
                canvas.SetMatrix(matrix.PreConcat(LocalTransform));
                DrawSelf(canvas);
                foreach (var item in Children)
                    item.Draw(canvas);
            }
            finally
            {
                canvas.SetMatrix(matrix);
            }
        }

    }
}
