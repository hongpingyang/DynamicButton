using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseState = DynamicButton.HelpClass.MouseState;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace DynamicButton
{
    public partial class CircleButton : Button
    {
        #region 字段
        private Timer m_Timer;//计时器  
        private int times = 0;//旋转次数
        private HelpClass.MouseState MouseState;
        private HelpClass helpclass =null;
        #endregion

        #region 属性
        private int _Circle;
        [Category("Properties")]
        [DefaultValue(20)]
        [Description("圆行按钮的半径")]
        public int Circle
        {
            get { return _Circle; }
            set
            {
                _Circle = value;
                GetPreferredSize(new Size(0, 0));
                this.Invalidate();
            }
        }

        private Color _BackGroundColor;
        [Category("Properties")]
        [Description("圆行按钮的背景色(默认值)")]
        public Color BackGroundColor
        {
            get { return _BackGroundColor; }
            set
            {
                _BackGroundColor = value;
                this.Invalidate();
            }
        }

        private Color _BackGroundColorH;
        [Category("Properties")]
        [Description("圆行按钮的背景色(Hover)")]
        public Color BackGroundColorH
        {
            get { return _BackGroundColorH; }
            set
            {
                _BackGroundColorH = value;
                this.Invalidate();
            }
        }

        private Color _BackGroundColorD;
        [Category("Properties")]
        [Description("圆行按钮的背景色(Down)")]
        public Color BackGroundColorD
        {
            get { return _BackGroundColorD; }
            set
            {
                _BackGroundColorD = value;
                this.Invalidate();
            }
        }

        private buttonStyle _buttonStyle;
        [Category("Properties")]
        [Description("圆行按钮的填充类型")]
        public buttonStyle ButtonStyle
        {
            get { return _buttonStyle; }
            set
            {
                _buttonStyle = value;
                this.Invalidate();
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        #endregion

        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(int.Parse(Circle.ToString()) + 1, int.Parse(Circle.ToString()) + 1);
        }

        public CircleButton()
        {
            helpclass = new HelpClass();
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            this.BackColor = Color.Transparent;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;

            m_Timer = new Timer();
            m_Timer.Interval = 100;
            m_Timer.Enabled = false;
            m_Timer.Tick += new EventHandler(aTimer_Tick);
        }

        private void aTimer_Tick(object sender, EventArgs e)
        {
            if (times <= 0)
            {
                m_Timer.Enabled = false;
                m_Timer.Stop();
            }

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            //base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            //base.OnMouseMove(mevent);
            if (DesignMode) return;

            MouseState oldMouseState = MouseState;

            if (IsCurorInCircle())
            {
                if (mevent.Button == MouseButtons.Left)
                    MouseState = MouseState.DOWN;
                else
                    MouseState = MouseState.HOVER;
            }
            else
            {
                MouseState = MouseState.OUT;
            }

            if (oldMouseState != MouseState)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            //base.OnMouseDown(mevent);
            if (DesignMode) return;

            MouseState oldmousestate = MouseState;

            if (IsCurorInCircle())
            {
                if (mevent.Button == MouseButtons.Left)
                {
                    times = 10;
                    MouseState = MouseState.DOWN;
                    m_Timer.Enabled = true;
                    m_Timer.Start();
                }
            }
            else
            {
                MouseState = MouseState.OUT;
            }

            if (oldmousestate != MouseState)
            {
                this.Invalidate();
            }

        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
           // base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //g.Clear(Parent.BackColor);


            //背景处理
            Brush b1;
            //if (BackGroundColor != null)
            //{
            //    if (!DesignMode && MouseState == MouseState.HOVER && BackGroundColorH != null)
            //        b1 = new SolidBrush(BackGroundColorH);
            //    else if (!DesignMode && MouseState == MouseState.DOWN && BackGroundColorD != null)
            //        b1 = new SolidBrush(BackGroundColorD);
            //    else
            //        b1 = new SolidBrush(BackGroundColor);
            //}
            //else
            //{
                b1 = new SolidBrush(HelpClass.Background);
            //} 

            g.FillEllipse(b1, ClientRectangle.X, ClientRectangle.Y, Circle, Circle);

            //文字处理 显示画布的的旋转，这里按照中心点旋转，故先要通过TranslateTransform平移，
            //然后用RotateTransform实现旋转，通过计时器来实现动态效果
            if (Text != null && Text.Length > 0)
            {
                double value = Math.Round((Math.Sqrt(Circle * Circle / 2) / 2), 0);

                int v = int.Parse(value.ToString());

                Point circlepoint = new Point(ClientRectangle.X + Circle / 2, ClientRectangle.Y + Circle / 2);

                Rectangle _testRectangle = new Rectangle(circlepoint.X - v, circlepoint.Y - v, 2 * v, 2 * v);

                if (times > 0)
                {
                    g.TranslateTransform(
                        ClientRectangle.X + Circle / 2,
                        ClientRectangle.Y + Circle / 2);
                    g.RotateTransform(-(36 * times));
                    times--;
                    g.TranslateTransform(-(ClientRectangle.X + Circle / 2), -(ClientRectangle.Y + Circle / 2));
                }
                using (SolidBrush solidbrush=new SolidBrush(HelpClass.ForeColorwhite))
                {
                    if (_buttonStyle == buttonStyle.Cross)
                    {
                        g.FillRectangle(solidbrush, new Rectangle(circlepoint.X - 2, circlepoint.Y - v, 4, 2 * v));
                        g.FillRectangle(solidbrush, new Rectangle(circlepoint.X - v, circlepoint.Y - 2, 2 * v, 4));
                    }
                    else if (_buttonStyle==buttonStyle.Triangle)
                    {
                        g.FillPolygon(solidbrush, new Point[] {
                            new Point(circlepoint.X, circlepoint.Y - v),
                            new Point(circlepoint.X - v, circlepoint.Y + v-6),
                            new Point(circlepoint.X + v, circlepoint.Y + v-6) },
                            FillMode.Alternate);
                    }

                    if (_buttonStyle == buttonStyle.Text)
                    {
                        g.DrawString(
                           Text.ToUpper(),
                           HelpClass.ROBOTO_MEDIUM_20,
                            solidbrush, //new SolidBrush(HelpClass.ForeColor),
                           _testRectangle,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }
            }
        }

        /// <summary>
        /// 判断鼠标是否是在圆形内
        /// </summary>
        /// <returns></returns>
        private bool IsCurorInCircle()
        {

            Point mousepoint = PointToClient(Cursor.Position);

            Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, Circle, Circle);

            if (!rect.Contains(mousepoint))
                return false;


            Point circlepoint = new Point(ClientRectangle.X + Circle / 2, ClientRectangle.Y + Circle / 2);

            double value = Math.Sqrt(Math.Abs(mousepoint.X - circlepoint.X) * Math.Abs(mousepoint.X - circlepoint.X)
                                     + Math.Abs(mousepoint.Y - circlepoint.Y) * Math.Abs(mousepoint.Y - circlepoint.Y));

            if (value <= Circle / 2)
            {
                return true;

                //MouseState = MouseState.HOVER;
            }
            else
            {
                return false;
                // MouseState = MouseState.OUT;
            }

        }


    }

    public enum buttonStyle
    {
        Cross,
        Text,
        Triangle
    }
}

