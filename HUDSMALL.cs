using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Collections;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL;
using SvgNet.SvgGdi;

//AUTOR ORIGINAL:MICHAEL OBORNE DO TIME ARDUPILOT

namespace JCFLIGHTGCS
{
    public partial class HUDSMALL : UserControl
    {

        private object streamlock = new object();
        private MemoryStream _streamjpg = new MemoryStream();
        public MemoryStream streamjpg
        {
            get
            {
                lock (streamlock)
                {
                    return _streamjpg;
                }
            }
            set
            {
                lock (streamlock)
                {
                    _streamjpg = value;
                }
            }
        }

        public bool streamjpgenable = false;
        public bool HoldInvalidation = false;
        public bool Russian { get; set; }

        private class character
        {
            public GraphicsPath pth;
            public Bitmap bitmap;
            public int gltextureid;
            public int width;
            public int size;
        }

        private Dictionary<int, character> charDict = new Dictionary<int, character>();

        public int huddrawtime = 0;

        [DefaultValue(true)] public bool opengl { get; set; }

        [Browsable(false)] public bool npotSupported { get; private set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool bgon { get; set; }

        private static ImageCodecInfo ici = GetImageCodec("image/jpeg");
        private static EncoderParameters eps = new EncoderParameters(1);

        private bool started = false;

        public HUDSMALL()
        {
            InitializeComponent();

            opengl = bgon = true;

            this.Name = "Hudsmall";

            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);

            objBitmap.MakeTransparent();

            graphicsObject = this;
            graphicsObjectGDIP = new GdiGraphics(Graphics.FromImage(objBitmap));
        }

        private float _roll = 0;
        private float _pitch = 0;
        private float _linkqualitygcs = 0;
        private DateTime _datetime;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float roll
        {
            get { return _roll; }
            set
            {
                if (_roll != value)
                {
                    _roll = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float pitch
        {
            get { return _pitch; }
            set
            {
                if (_pitch != value)
                {
                    _pitch = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float linkqualitygcs
        {
            get { return _linkqualitygcs; }
            set
            {
                if (_linkqualitygcs != value)
                {
                    _linkqualitygcs = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime datetime
        {
            get { return _datetime; }
            set
            {
                if (_datetime.Hour == value.Hour && _datetime.Minute == value.Minute &&
                    _datetime.Second == value.Second)
                    return;
                if (_datetime != value)
                {
                    _datetime = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool imuhealty { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool failsafe { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public int status { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime messagetime { get; set; }

        private int statuslast = 0;
        private DateTime armedtimer = DateTime.MinValue;

        public struct Custom
        {
            public string Header;

            public System.Reflection.PropertyInfo Item;

            public double GetValue
            {
                get
                {
                    return Convert.ToDouble(Item.GetValue(src, null));
                }
            }

            public static object src { get; set; }
        }

        public Hashtable CustomItems = new Hashtable();

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color hudcolor
        {
            get { return this._whitePen.Color; }
            set
            {
                _hudcolor = value;
                this._whitePen = new Pen(value, 2);
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color skyColor1
        {
            get { return _skyColor1; }
            set { _skyColor1 = value; }
        }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color skyColor2
        {
            get { return _skyColor2; }
            set { _skyColor2 = value; }
        }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values"), DefaultValue(typeof(Color), "0x9bb824")]
        public Color groundColor1
        {
            get { return _groundColor1; }
            set { _groundColor1 = value; }
        }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values"), DefaultValue(typeof(Color), "0x414f07")]
        public Color groundColor2
        {
            get { return _groundColor2; }
            set { _groundColor2 = value; }
        }

        private Color _skyColor1 = Color.Blue;
        private Color _skyColor2 = Color.LightBlue;
        private Color _groundColor1 = Color.FromArgb(0x9b, 0xb8, 0x24);
        private Color _groundColor2 = Color.FromArgb(0x41, 0x4f, 0x07);
        private Color _hudcolor = Color.White;
        private Pen _whitePen = new Pen(Color.White, 2);
        private readonly SolidBrush _whiteBrush = new SolidBrush(Color.White);
        private static readonly SolidBrush SolidBrush = new SolidBrush(Color.FromArgb(0x55, 0xff, 0xff, 0xff));
        private static readonly SolidBrush SlightlyTransparentWhiteBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255));
        private static readonly SolidBrush AltGroundBrush = new SolidBrush(Color.FromArgb(100, Color.BurlyWood));
        private readonly object _bgimagelock = new object();

        public Image bgimage
        {
            set
            {
                lock (this._bgimagelock)
                {
                    try
                    {
                        _bgimage = (Image)value;
                    }
                    catch
                    {
                        _bgimage = null;
                    }

                    this.Invalidate();
                }
            }
            get { return _bgimage; }
        }

        private Image _bgimage;
        private Font font = new Font("Font", 10);
        public Bitmap objBitmap = new Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        private int count = 0;
        private DateTime countdate = DateTime.Now;
        private HUDSMALL graphicsObject;
        private IGraphics graphicsObjectGDIP;
        private DateTime starttime = DateTime.MinValue;

        private System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HUD));

        public override void Refresh()
        {
            if (!ThisReallyVisible())
            {
            }
            using (Graphics gg = this.CreateGraphics())
            {
                OnPaint(new PaintEventArgs(gg, this.ClientRectangle));
            }
        }

        public new void Invalidate()
        {
            if (HoldInvalidation)
                return;

            if (!ThisReallyVisible())
            {
            }

            base.Invalidate();
        }

        public bool ThisReallyVisible()
        {
            return this.Visible;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (opengl && !DesignMode)
            {
                try
                {

                    int[] viewPort = new int[4];

                    GL.GetInteger(GetPName.Viewport, viewPort);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, Width, Height, 0, -1, 1);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();
                    GL.PushAttrib(AttribMask.DepthBufferBit);
                    GL.Disable(EnableCap.DepthTest);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Blend);

                    string versionString = GL.GetString(StringName.Version);
                    string majorString = versionString.Split(' ')[0];
                    var v = new Version(majorString);
                    npotSupported = v.Major >= 2;
                }
                catch (Exception)
                {
                }

                try
                {
                    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                    GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
                    GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
                    GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
                    GL.Hint(HintTarget.TextureCompressionHint, HintMode.Nicest);
                }
                catch (Exception)
                {
                }

                try
                {
                    GL.Enable(EnableCap.LineSmooth);
                    GL.Enable(EnableCap.PointSmooth);
                    GL.Disable(EnableCap.PolygonSmooth);

                }
                catch (Exception)
                {
                }
            }
            started = true;
        }

        bool inOnPaint = false;

        protected override void OnPaint(PaintEventArgs e)
        {

            if (!started)
                return;

            if (this.DesignMode)
            {
                e.Graphics.Clear(this.BackColor);
                e.Graphics.Flush();
                opengl = false;
                doPaint();
                e.Graphics.DrawImageUnscaled(objBitmap, 0, 0);
                opengl = true;
                return;
            }

            if ((DateTime.Now - starttime).TotalMilliseconds < 30 && (_bgimage == null))
            {
                return;
            }

            lock (this)
            {
                if (inOnPaint)
                {
                    return;
                }
                inOnPaint = true;
            }

            starttime = DateTime.Now;

            try
            {

                if (opengl)
                {
                    if (DateTime.Now.Second % 5 == 0) glControl1.MakeCurrent();
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                }

                doPaint();

                if (!opengl)
                {
                    e.Graphics.DrawImageUnscaled(objBitmap, 0, 0);
                }
                else if (opengl)
                {
                    glControl1.SwapBuffers();
                    glControl1.MakeCurrent();
                }

            }
            catch (Exception)
            {
            }

            count++;

            huddrawtime += (int)(DateTime.Now - starttime).TotalMilliseconds;

            if (DateTime.Now.Second != countdate.Second)
            {
                countdate = DateTime.Now;
                if ((huddrawtime / count) > 1000) opengl = false;
                count = 0;
                huddrawtime = 0;
            }

            lock (this)
            {
                inOnPaint = false;
            }
        }

        void Clear(Color color)
        {
            if (opengl)
            {
                GL.ClearColor(color);

            }
            else
            {
                graphicsObjectGDIP.Clear(color);
            }
        }

        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        public void DrawArc(Pen penn, RectangleF rect, float start, float degrees)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineStrip);

                start = 360 - start;
                start -= 30;

                float x = 0, y = 0;
                for (float i = start; i <= start + degrees; i++)
                {
                    x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                    y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                    x = x + rect.X + rect.Width / 2;
                    y = y + rect.Y + rect.Height / 2;
                    GL.Vertex2(x, y);
                }

                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawArc(penn, rect, start, degrees);
            }
        }

        public void DrawEllipse(Pen penn, Rectangle rect)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineLoop);
                float x, y;
                for (float i = 0; i < 360; i += 1)
                {
                    x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                    y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                    x = x + rect.X + rect.Width / 2;
                    y = y + rect.Y + rect.Height / 2;
                    GL.Vertex2(x, y);
                }

                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawEllipse(penn, rect);
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.ClearOutputChannelColorProfile();
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private character[] _texture = new character[2];

        public void DrawImage(Image img, int x, int y, int width, int height, int textureno = 0)
        {
            if (opengl)
            {
                if (img == null)
                    return;

                if (_texture[textureno] == null)
                    _texture[textureno] = new character();

                if (npotSupported && img is Bitmap)
                {
                    _texture[textureno].bitmap = (Bitmap)img;
                }
                else
                {
                    _texture[textureno].bitmap = ResizeImage(img, 512, 512);
                }

                if (_texture[textureno].gltextureid == 0)
                {
                    GL.GenTextures(1, out _texture[textureno].gltextureid);
                }

                GL.BindTexture(TextureTarget.Texture2D, _texture[textureno].gltextureid);

                BitmapData data = _texture[textureno].bitmap.LockBits(
                    new Rectangle(0, 0, _texture[textureno].bitmap.Width, _texture[textureno].bitmap.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                if (_texture[textureno].width != _texture[textureno].bitmap.Width)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    _texture[textureno].width = data.Width;
                }
                else
                {
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, data.Width, data.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                }

                _texture[textureno].bitmap.UnlockBits(data);

                bool polySmoothEnabled = GL.IsEnabled(EnableCap.PolygonSmooth);
                if (polySmoothEnabled) GL.Disable(EnableCap.PolygonSmooth);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, _texture[textureno].gltextureid);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.Begin(PrimitiveType.TriangleStrip);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(x, y);
                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(x, y + height);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(x + width, y);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(x + width, y + height);
                GL.End();
                GL.Disable(EnableCap.Texture2D);
                if (polySmoothEnabled) GL.Enable(EnableCap.PolygonSmooth);
            }
            else
            {
                graphicsObjectGDIP.DrawImage(img, x, y, width, height);
            }
        }

        public void DrawPath(Pen penn, GraphicsPath gp)
        {
            try
            {
                List<PointF> list = new List<PointF>();
                for (int i = 0; i < gp.PointCount; i++)
                {
                    var pnt = gp.PathPoints[i];
                    var type = gp.PathTypes[i];

                    if (type == 0)
                    {
                        if (list.Count != 0)
                            DrawPolygon(penn, list.ToArray());
                        list.Clear();
                        list.Add(pnt);
                    }

                    if (type <= 3)
                        list.Add(pnt);

                    if ((type & 0x80) > 0)
                    {
                        list.Add(pnt);
                        list.Add(list[0]);
                        DrawPolygon(penn, list.ToArray());
                        list.Clear();
                    }
                }
            }
            catch
            {
            }
        }

        public void FillPath(Brush brushh, GraphicsPath gp)
        {
            try
            {
                if (opengl)
                {
                    var bounds = gp.GetBounds();

                    var list = gp.PathPoints;

                    GL.Enable(EnableCap.StencilTest);
                    GL.Disable(EnableCap.CullFace);
                    GL.ClearStencil(0);

                    GL.ColorMask(false, false, false, false);
                    GL.Clear(ClearBufferMask.StencilBufferBit);
                    GL.DepthMask(false);
                    GL.StencilFunc(StencilFunction.Always, 0, 0xff);
                    GL.StencilOp(StencilOp.Invert, StencilOp.Invert, StencilOp.Invert);

                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Color4(((SolidBrush)brushh).Color);
                    GL.Vertex2(0, 0);
                    foreach (var pnt in list)
                    {
                        GL.Vertex2(pnt.X, pnt.Y);
                    }

                    GL.End();

                    GL.ColorMask(true, true, true, true);
                    GL.DepthMask(true);

                    GL.StencilFunc(StencilFunction.Equal, 1, 1);
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Color4(((SolidBrush)brushh).Color);
                    GL.Vertex2(0, 0);
                    foreach (var pnt in list)
                    {
                        GL.Vertex2(pnt.X, pnt.Y);
                    }

                    GL.End();

                    GL.Disable(EnableCap.StencilTest);
                }
                else
                {
                    graphicsObjectGDIP.FillPath(brushh, gp);
                }
            }
            catch
            {
            }
        }

        public void ResetTransform()
        {
            if (opengl)
            {
                GL.LoadIdentity();
            }
            else
            {
                graphicsObjectGDIP.ResetTransform();
            }
        }

        public void RotateTransform(float angle)
        {
            if (opengl)
            {
                GL.Rotate(angle, 0, 0, 1);
            }
            else
            {
                graphicsObjectGDIP.RotateTransform(angle);
            }
        }

        public void TranslateTransform(float x, float y)
        {
            if (opengl)
            {
                GL.Translate(x, y, 0f);
            }
            else
            {
                graphicsObjectGDIP.TranslateTransform(x, y);
            }
        }

        public void FillPolygon(Brush brushh, Point[] list)
        {
            if (opengl)
            {
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Color4(((SolidBrush)brushh).Color);
                foreach (Point pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }

                GL.Vertex2(list[list.Length - 1].X, list[list.Length - 1].Y);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillPolygon(brushh, list);
            }
        }

        public void FillPolygon(Brush brushh, PointF[] list)
        {
            if (opengl)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(((SolidBrush)brushh).Color);
                foreach (PointF pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }

                GL.Vertex2(list[0].X, list[0].Y);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillPolygon(brushh, list);
            }
        }

        public void DrawPolygon(Pen penn, Point[] list)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineLoop);
                foreach (Point pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }

                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawPolygon(penn, list);
            }
        }

        public void DrawPolygon(Pen penn, PointF[] list)
        {
            if (opengl)
            {
                GL.LineWidth(penn.Width);
                GL.Color4(penn.Color);

                GL.Begin(PrimitiveType.LineLoop);
                foreach (PointF pnt in list)
                {
                    GL.Vertex2(pnt.X, pnt.Y);
                }

                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawPolygon(penn, list);
            }
        }


        public void FillRectangle(Brush brushh, RectangleF rectf)
        {
            if (opengl)
            {
                float x1 = rectf.X;
                float y1 = rectf.Y;

                float width = rectf.Width;
                float height = rectf.Height;

                GL.Begin(PrimitiveType.TriangleFan);

                GL.LineWidth(0);

                if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
                {
                    LinearGradientBrush temp = (LinearGradientBrush)brushh;
                    GL.Color4(temp.LinearColors[0]);
                }
                else
                {
                    GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f,
                        ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
                }

                GL.Vertex2(x1, y1);
                GL.Vertex2(x1 + width, y1);

                if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
                {
                    LinearGradientBrush temp = (LinearGradientBrush)brushh;
                    GL.Color4(temp.LinearColors[1]);
                }
                else
                {
                    GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f,
                        ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
                }

                GL.Vertex2(x1 + width, y1 + height);
                GL.Vertex2(x1, y1 + height);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.FillRectangle(brushh, rectf);
            }
        }


        public void DrawLine(Pen penn, double x1, double y1, double x2, double y2)
        {

            if (opengl)
            {
                GL.Color4(penn.Color);
                GL.LineWidth(penn.Width);

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(x1, y1);
                GL.Vertex2(x2, y2);
                GL.End();
            }
            else
            {
                graphicsObjectGDIP.DrawLine(penn, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        private readonly Pen _blackPen = new Pen(Color.Black, 2);
        private readonly Pen _greenPen = new Pen(Color.Green, 2);
        private readonly Pen _redPen = new Pen(Color.Red, 2);

        void doPaint()
        {
            try
            {
                if (graphicsObjectGDIP == null || !opengl &&
                    (objBitmap.Width != this.Width || objBitmap.Height != this.Height))
                {
                    objBitmap = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    objBitmap.MakeTransparent();
                    graphicsObjectGDIP = new GdiGraphics(Graphics.FromImage(objBitmap));

                    graphicsObjectGDIP.SmoothingMode = SmoothingMode.HighSpeed;
                    graphicsObjectGDIP.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphicsObjectGDIP.CompositingMode = CompositingMode.SourceOver;
                    graphicsObjectGDIP.CompositingQuality = CompositingQuality.HighSpeed;
                    graphicsObjectGDIP.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphicsObjectGDIP.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                }

                graphicsObjectGDIP.InterpolationMode = InterpolationMode.Bilinear;

                try
                {
                    graphicsObject.Clear(Color.Transparent);
                }
                catch
                {
                    opengl = false;
                }

                if (_bgimage != null)
                {
                    bgon = false;
                    lock (this._bgimagelock)
                        lock (_bgimage)
                        {
                            try
                            {
                                graphicsObject.DrawImage(_bgimage, 0, 0, this.Width, this.Height);
                            }
                            catch (Exception)
                            {
                                _bgimage = null;
                            }
                        }
                }
                else
                {
                    bgon = true;
                }

                float _roll = this._roll;

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                graphicsObject.RotateTransform(-_roll);

                int fontsize = this.Height / 30;
                int fontoffset = fontsize - 10;

                float every5deg = -this.Height / 65;

                float pitchoffset = -_pitch * every5deg;

                int halfwidth = this.Width / 2;
                int halfheight = this.Height / 2;

                this._whiteBrush.Color = this._whitePen.Color;

                this._blackPen.Width = 2;
                this._greenPen.Width = 2;
                this._redPen.Width = 2;

                this._whitePen.Color = _hudcolor;

                if (bgon == true)
                {
                    RectangleF bg = new RectangleF(-halfwidth * 2, -halfheight * 2, this.Width * 2, halfheight * 2 + pitchoffset);

                    if (bg.Height != 0)
                    {
                        using (LinearGradientBrush linearBrush = new LinearGradientBrush(
                            bg, _skyColor1, _skyColor2, LinearGradientMode.Vertical))
                        {
                            graphicsObject.FillRectangle(linearBrush, bg);
                        }
                    }

                    bg = new RectangleF(-halfwidth * 2, pitchoffset, this.Width * 2, halfheight * 2 - pitchoffset);

                    if (bg.Height != 0)
                    {
                        using (
                            LinearGradientBrush linearBrush = new LinearGradientBrush(
                                bg, _groundColor1, _groundColor2,
                                LinearGradientMode.Vertical))
                        {
                            graphicsObject.FillRectangle(linearBrush, bg);
                        }
                    }
                    graphicsObject.DrawLine(this._whitePen, -halfwidth * 2, pitchoffset + 0, halfwidth * 2, pitchoffset + 0);
                }

                graphicsObject.ResetTransform();

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                graphicsObject.RotateTransform(-_roll);

                int lengthshort = this.Width / 14;
                int lengthlong = this.Width / 10;

                for (int a = -90; a <= 90; a += 5)
                {
                    if (a >= _pitch - 29 && a <= _pitch + 20)
                    {
                        if (a % 10 == 0)
                        {
                            if (a == 0)
                            {
                                graphicsObject.DrawLine(this._greenPen, this.Width / 2 - lengthlong - halfwidth,
                                    pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth,
                                    pitchoffset + a * every5deg);
                            }
                            else
                            {
                                graphicsObject.DrawLine(this._whitePen, this.Width / 2 - lengthlong - halfwidth,
                                    pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth,
                                    pitchoffset + a * every5deg);
                            }

                            drawstring(a.ToString(), font, fontsize + 2, _whiteBrush,
                                this.Width / 2 - lengthlong - 30 - halfwidth - (int)(fontoffset * 1.7),
                                pitchoffset + a * every5deg - 8 - fontoffset);
                        }
                        else
                        {
                            graphicsObject.DrawLine(this._whitePen, this.Width / 2 - lengthshort - halfwidth,
                                pitchoffset + a * every5deg, this.Width / 2 + lengthshort - halfwidth,
                                pitchoffset + a * every5deg);
                        }
                    }
                }

                graphicsObject.ResetTransform();

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                /////////////////////////

                lengthlong = this.Height / 50;

                int extra = (int)(this.Height / 17.0 * 4.9f);

                int lengthlongex = lengthlong + 2;

                Point[] pointlist = new Point[3];
                pointlist[0] = new Point(0, -lengthlongex * 2 - extra + 4);
                pointlist[1] = new Point(-lengthlongex, -lengthlongex - extra + 4);
                pointlist[2] = new Point(lengthlongex, -lengthlongex - extra + 4);

                this._redPen.Width = 2;

                graphicsObject.DrawPolygon(this._redPen, pointlist);

                this._redPen.Width = 2;

                int[] array = new int[] { -60, -45, -30, -20, -10, 0, 10, 20, 30, 45, 60 };

                foreach (int a in array)
                {
                    graphicsObject.ResetTransform();
                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);
                    graphicsObject.RotateTransform(a - _roll);
                    drawstring(String.Format("{0,2}", Math.Abs(a)), font, fontsize, _whiteBrush,
                        0 - 6 - fontoffset, -lengthlong * 8 - extra);
                    graphicsObject.DrawLine(this._whitePen, 0, -lengthlong * 3 - extra, 0,
                        -lengthlong * 3 - extra - lengthlong);
                }

                graphicsObject.ResetTransform();
                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                RectangleF arcrect = new RectangleF(-lengthlong * 3 - extra, -lengthlong * 3 - extra,
                    (extra + lengthlong * 3) * 2f, (extra + lengthlong * 3) * 2f);

                graphicsObject.DrawArc(this._whitePen, arcrect, 180 + 30 + -_roll, 120);

                graphicsObject.ResetTransform();

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                ///////////////////////

                Rectangle centercircle = new Rectangle(-halfwidth / 2, -halfwidth / 2, halfwidth, halfwidth);

                using (Pen redtemp = new Pen(Color.FromArgb(200, this._redPen.Color.R, this._redPen.Color.G, this._redPen.Color.B), 4.0f))
                {
                    graphicsObject.DrawLine(redtemp, centercircle.Left - halfwidth / 5, 0, centercircle.Left, 0);
                    graphicsObject.DrawLine(redtemp, centercircle.Right, 0, centercircle.Right + halfwidth / 5, 0);
                    graphicsObject.DrawLine(redtemp, 0 - 1, 0, centercircle.Right - halfwidth / 3,
                        0 + halfheight / 10);
                    graphicsObject.DrawLine(redtemp, 0 + 1, 0, centercircle.Left + halfwidth / 3,
                        0 + halfheight / 10);
                }

                Rectangle scrollbg = new Rectangle(0, halfheight - halfheight / 2, this.Width / 10, this.Height / 2);

                float speed = 0;

                if (GetValues.AirSpeedEnabled > 0)
                {
                    speed = GetValues.ReadAirSpeed;
                }
                else
                {
                    speed = GetValues.ReadGroundSpeed;
                }

                speed /= 27.778f;

                graphicsObject.ResetTransform();
                drawstring("FuselagemVel:" + speed.ToString("0.0") + "KM/h", font, fontsize + 1, _whiteBrush, 1, scrollbg.Bottom + 25);

                if (float.IsNaN(_linkqualitygcs)) _linkqualitygcs = 0;
                if (_linkqualitygcs >= 10 && _linkqualitygcs < 100)
                {
                    graphicsObject.DrawLine(this._greenPen, 145, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 15, 145, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    graphicsObject.DrawLine(this._greenPen, 150, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 10, 150, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    drawstring(_linkqualitygcs.ToString("0") + "%", font, fontsize, _whiteBrush, 115, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                }
                else if (_linkqualitygcs < 10)
                {
                    graphicsObject.DrawLine(this._greenPen, 150, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 10, 150, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    drawstring(_linkqualitygcs.ToString("0") + "%", font, fontsize, _whiteBrush, 120, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                }
                else
                {
                    graphicsObject.DrawLine(this._greenPen, 140, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20, 140, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    graphicsObject.DrawLine(this._greenPen, 145, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 15, 145, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    graphicsObject.DrawLine(this._greenPen, 150, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 10, 150, scrollbg.Top - (int)(fontsize) - 2 - 15);
                    drawstring(_linkqualitygcs.ToString("0") + "%", font, fontsize, _whiteBrush, 105, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                }

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                if (status != statuslast)
                {
                    armedtimer = DateTime.Now;
                }

                if (imuhealty == true)
                {
                    drawstring("IMU não calibrada", font, fontsize + 1, (SolidBrush)Brushes.Red, -75, halfheight / -1);
                    statuslast = status;
                }
                else
                {
                    if (this._roll > 35 || this._roll < (-35))
                    {
                        drawstring("Bank-Angle", font, fontsize + 5, (SolidBrush)Brushes.Red, -75, halfheight / -1);
                        statuslast = status;
                    }
                    else
                    {
                        if (failsafe == true)
                        {
                            drawstring("Fail-Safe", font, fontsize + 5, (SolidBrush)Brushes.Red, -75, halfheight / -1);
                            statuslast = status;
                        }
                        else
                        {
                            if (status == 0)
                            {
                                drawstring("Desarmado", font, fontsize + 5, (SolidBrush)Brushes.Red, -75, halfheight / -1);
                                statuslast = status;
                            }
                            else if (status == 1)
                            {
                                if ((armedtimer.AddSeconds(8) > DateTime.Now))
                                {
                                    drawstring("Armado", font, fontsize + 5, (SolidBrush)Brushes.Red, -75, halfheight / -1);
                                    statuslast = status;
                                }
                            }
                        }
                    }
                }

                graphicsObject.ResetTransform();

                if (DesignMode)
                {
                    return;
                }

                lock (streamlock)
                {
                    if (streamjpgenable || streamjpg == null)
                    {
                        if (opengl)
                        {
                            objBitmap = GrabScreenshot();
                        }

                        streamjpg = new MemoryStream();
                        objBitmap.Save(streamjpg, ici, eps);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        static ImageCodecInfo GetImageCodec(string mimetype)
        {
            foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
            {
                if (ici.MimeType == mimetype) return ici;
            }

            return null;
        }

        public Bitmap GrabScreenshot()
        {
            if (OpenTK.Graphics.GraphicsContext.CurrentContext == null)
                throw new OpenTK.Graphics.GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        private readonly Pen _p = new Pen(Color.FromArgb(0x26, 0x27, 0x28), 2f);

        void drawstring(string text, Font font, float fontsize, SolidBrush brush, float x, float y)
        {
            if (!opengl)
            {
                drawstringGDI(text, font, fontsize, brush, x, y);
                return;
            }

            if (text == null || text == "")
                return;

            float maxy = 1;

            foreach (char cha in text)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character()
                    {
                        bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb),
                        size = (int)fontsize
                    };
                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);
                    float maxx = this.Width / 150;
                    using (var gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        var pth = new GraphicsPath();
                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 5, new Point((int)0, (int)0),
                                StringFormat.GenericTypographic);
                        charDict[charid].pth = pth;
                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        gfx.DrawPath(this._p, pth);
                        gfx.FillPath(brush, pth);
                        if (pth.PointCount > 0)
                        {
                            foreach (PointF pnt in pth.PathPoints)
                            {
                                if (pnt.X > maxx)
                                    maxx = pnt.X;

                                if (pnt.Y > maxy)
                                    maxy = pnt.Y;
                            }
                        }
                    }
                    charDict[charid].width = (int)(maxx + 2);
                    int textureId;
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                        (float)TextureEnvModeCombine.Replace);
                    Bitmap bitmap = charDict[charid].bitmap;
                    GL.GenTextures(1, out textureId);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                        (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Linear);
                    GL.Flush();
                    bitmap.UnlockBits(data);

                    charDict[charid].gltextureid = textureId;
                }
                float scale = 1.0f;
                if (cha != ' ')
                {
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, charDict[charid].gltextureid);

                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + charDict[charid].bitmap.Width * scale, y + charDict[charid].bitmap.Height * scale);
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x + 0, y + charDict[charid].bitmap.Height * scale);
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                }
                x += charDict[charid].width * scale;
            }
        }

        void drawstringGDI(string text, Font font, float fontsize, SolidBrush brush, float x, float y)
        {
            if (text == null || text == "")
                return;

            float maxy = 0;

            foreach (char cha in text)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character()
                    {
                        bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb),
                        size = (int)fontsize
                    };

                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);
                    float maxx = this.Width / 150;
                    using (var gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        var pth = new GraphicsPath();
                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 5, new Point((int)0, (int)0),
                                StringFormat.GenericTypographic);
                        charDict[charid].pth = pth;
                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        gfx.DrawPath(this._p, pth);
                        gfx.FillPath(brush, pth);
                        if (pth.PointCount > 0)
                        {
                            foreach (PointF pnt in pth.PathPoints)
                            {
                                if (pnt.X > maxx)
                                    maxx = pnt.X;

                                if (pnt.Y > maxy)
                                    maxy = pnt.Y;
                            }
                        }
                    }

                    charDict[charid].width = (int)(maxx + 2);
                }
                float scale = 1.0f;
                if (cha != ' ')
                {
                    DrawImage(charDict[charid].bitmap, (int)x, (int)y, charDict[charid].bitmap.Width, charDict[charid].bitmap.Height, charDict[charid].gltextureid);
                }
                else
                {

                }

                x += charDict[charid].width * scale;
            }

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                if (opengl && !DesignMode)
                {
                    base.OnHandleCreated(e);
                }
            }
            catch (Exception)
            {
                opengl = false;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            try
            {
                if (opengl && !DesignMode)
                {
                    base.OnHandleDestroyed(e);
                }
            }
            catch (Exception)
            {
                opengl = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (DesignMode || !IsHandleCreated || !started)
                return;

            base.OnResize(e);

            int ht = (int)(this.Width / 1.333f);
            if (ht >= this.Height + 5 || ht <= this.Height - 5)
            {
                this.Height = ht;
                return;
            }

            graphicsObjectGDIP = new GdiGraphics(Graphics.FromImage(objBitmap));

            try
            {
                foreach (character texid in charDict.Values)
                {
                    try
                    {
                        texid.bitmap.Dispose();
                    }
                    catch
                    {
                    }
                }

                if (opengl)
                {
                    foreach (character texid in _texture)
                    {
                        if (texid != null && texid.gltextureid != 0)
                            GL.DeleteTexture(texid.gltextureid);
                    }

                    this._texture = new character[_texture.Length];

                    foreach (character texid in charDict.Values)
                    {
                        if (texid.gltextureid != 0)
                            GL.DeleteTexture(texid.gltextureid);
                    }
                }

                charDict.Clear();
            }
            catch
            {
            }

            try
            {
                if (opengl)
                {
                    glControl1.MakeCurrent();

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, Width, Height, 0, -1, 1);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    GL.Viewport(0, 0, Width, Height);
                }
            }
            catch
            {
            }

            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}

