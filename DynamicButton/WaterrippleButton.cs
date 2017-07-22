using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicButton
{
    public class WaterrippleButton : Button
    {
        private Timer m_Timer;//计时器  
        private const int TotalProgress = 1;
        private double Progress = 0;
        private SizeF _textSize;
        private Point mouseClickPoint;  
        //图标
        private Image _icon;
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                if (AutoSize)
                    Size = GetPreferredSize();
                Invalidate();
            }
        }

        public WaterrippleButton()
        {
            AutoSize = true;


            m_Timer = new Timer();
            m_Timer.Interval = 100;
            m_Timer.Enabled = false;
            m_Timer.Tick += new EventHandler(aTimer_Tick);
        }

        private void aTimer_Tick(object sender, EventArgs e)
        {
            if (Progress > TotalProgress)
            {
                m_Timer.Enabled = false;
                m_Timer.Stop();
                Progress = 1;
                return;
            }
            Progress += 0.1;
            //if (Progress > TotalProgress)
            //    Progress = 1;
            Invalidate();
        }

        #region 重载事件
         
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                _textSize = CreateGraphics().MeasureString(value.ToUpper(), HelpClass.ROBOTO_MEDIUM_12);
                if (AutoSize)
                    Size = GetPreferredSize();
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Clear(Parent.BackColor);


            Color c = HelpClass.Background;
            using (Brush b = new SolidBrush(c))
                g.FillRectangle(b, ClientRectangle);

            if (Progress<=TotalProgress)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(101 - (Progress * 100)), Color.Green)))
                {
                    var rippleSize = (int)(Progress * Width * 2);
                    g.FillEllipse(rippleBrush, new Rectangle(mouseClickPoint.X - rippleSize / 2, mouseClickPoint.Y - rippleSize / 2, rippleSize, rippleSize));
                }
                g.SmoothingMode = SmoothingMode.None;
            }

            //Text
            var textRect = ClientRectangle;
            //text
            using (Brush brush = new SolidBrush(HelpClass.ForeColor))
            {
                g.DrawString(Text,
                    HelpClass.ROBOTO_MEDIUM_12,
                    brush,
                    textRect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                    );
            }

                

        }



        #endregion

        private Size GetPreferredSize()
        {
            return GetPreferredSize(new Size(0, 0));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var extra = 16;

            if (Icon != null)
                extra += 28;

            return new Size((int)Math.Ceiling(_textSize.Width) + extra, 36);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode) return;
           
            MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    Progress = 0;
                    mouseClickPoint = args.Location;
                    m_Timer.Enabled = true;
                    m_Timer.Start();
                }
            };
        }
    }
}
