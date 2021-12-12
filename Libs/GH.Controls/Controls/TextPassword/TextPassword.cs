using GH.Forms.ComponentModel;
using GH.Forms.Facades;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Timers;
using System.Windows.Forms;

namespace GH.Forms
{
    [DefaultProperty("PasswordCharDelay")]
    [Description("Позволяет пользователю вводить пароль, мгновенно отображая каждый введенный символ.")]
    [ToolboxBitmap(typeof(TextPassword), "toolbox.bmp")]
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class TextPassword : TextBox
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<bool> IsFontSmoothingEnabled = (Func<bool>)(() => SystemInformation.IsFontSmoothingEnabled);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<Control, IGraphics> NewGraphics = (Func<Control, IGraphics>)(self => (IGraphics)new Facades.Graphics(self));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<Color, ISolidBrush> NewSolidBrush = (Func<Color, ISolidBrush>)(color => (ISolidBrush)new Facades.SolidBrush(color));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<bool, double, ISynchronizeInvoke, ITimer> NewTimer = (Func<bool, double, ISynchronizeInvoke, ITimer>)((autoReset, interval, synchronizingObject) => (ITimer)new Facades.Timer(autoReset, interval, synchronizingObject));
        public const char DefaultPasswordChar = '\0';
        public const int DefaultPasswordCharDelay = 1000;
        public const bool DefaultUseSystemPasswordChar = true;
        private char passwordChar;
        private int passwordCharDelay;
        private string textPrevious;
        private ITimer timer;

        public TextPassword()
        {
            this.passwordChar = char.MinValue;
            this.passwordCharDelay = 1000;
            base.UseSystemPasswordChar = true;
            this.SetUpTimer(true);
        }

        public event EventHandler<CancelChangeEventArgs<char>> PasswordCharChanging;

        public event EventHandler<ChangeEventArgs<char>> PasswordCharChanged;

        public event EventHandler<CancelChangeEventArgs<int>> PasswordCharDelayChanging;

        public event EventHandler<ChangeEventArgs<int>> PasswordCharDelayChanged;

        public event EventHandler<CancelChangeEventArgs<bool>> UseSystemPasswordCharChanging;

        public event EventHandler<ChangeEventArgs<bool>> UseSystemPasswordCharChanged;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                if (value)
                    throw new ArgumentException(string.Format("{0} не поддерживает Multiline.", (object)this.GetType().FullName));
            }
        }

        [DefaultValue('\0')]
        [Description("Указывает символ, который будет отображаться для ввода пароля в элементе управления.")]
        public new char PasswordChar
        {
            get
            {
                return this.passwordChar;
            }
            set
            {
                if ((int)this.passwordChar == (int)value && (int)base.PasswordChar == (int)value)
                    return;
                CancelChangeEventArgs<char> cancelChangeEventArgs = CancelChangeEventArgs<char>.DoIf(this, this.PasswordCharChanging, new Action<CancelChangeEventArgs<char>>(this.OnPasswordCharChanging), this.PasswordChar, value);
                if (((object)cancelChangeEventArgs != null ? (cancelChangeEventArgs.Cancel ? 1 : 0) : 0) != 0)
                    return;
                char passwordChar = this.PasswordChar;
                this.passwordChar = base.PasswordChar = value;
                ChangeEventArgs<char>.DoIf(this, this.PasswordCharChanged, new Action<ChangeEventArgs<char>>(this.OnPasswordCharChanged), passwordChar, this.PasswordChar);
            }
        }

        [Category("Behavior")]
        [DefaultValue(1000)]
        [Description("Указывает время в миллисекундах, в течение которого ввод пароля является разборчивым, прежде чем появится в качестве символа пароля.")]
        public int PasswordCharDelay
        {
            get
            {
                return this.passwordCharDelay;
            }
            set
            {
                ITimer timer = this.timer;
                if (timer == null || this.passwordCharDelay == value && (int)timer.Interval == value)
                    return;
                CancelChangeEventArgs<int> cancelChangeEventArgs = CancelChangeEventArgs<int>.DoIf(this, this.PasswordCharDelayChanging, new Action<CancelChangeEventArgs<int>>(this.OnPasswordCharDelayChanging), this.PasswordCharDelay, value);
                if (((object)cancelChangeEventArgs != null ? (cancelChangeEventArgs.Cancel ? 1 : 0) : 0) != 0)
                    return;
                try
                {
                    timer.Interval = (double)value;
                }
                catch
                {
                    throw new ArgumentOutOfRangeException((string)null, (object)value, "Must be greater than zero.");
                }
                int passwordCharDelay = this.PasswordCharDelay;
                this.passwordCharDelay = value;
                ChangeEventArgs<int>.DoIf(this, this.PasswordCharDelayChanged, new Action<ChangeEventArgs<int>>(this.OnPasswordCharDelayChanged), passwordCharDelay, this.PasswordCharDelay);
            }
        }

        [Browsable(false)]
        public char PasswordCharEffective
        {
            get
            {
                return !this.UseSystemPasswordChar ? this.passwordChar : base.PasswordChar;
            }
        }

        [DefaultValue(true)]
        [Description("Указывает, должен ли текст в элементе управления Текстовое поле Пароля отображаться в качестве символа пароля по умолчанию.")]
        public new bool UseSystemPasswordChar
        {
            get
            {
                return base.UseSystemPasswordChar;
            }
            set
            {
                if (base.UseSystemPasswordChar == value)
                    return;
                CancelChangeEventArgs<bool> cancelChangeEventArgs = CancelChangeEventArgs<bool>.DoIf(this, this.UseSystemPasswordCharChanging, new Action<CancelChangeEventArgs<bool>>(this.OnUseSystemPasswordCharChanging), this.UseSystemPasswordChar, value);
                if (((object)cancelChangeEventArgs != null ? (cancelChangeEventArgs.Cancel ? 1 : 0) : 0) != 0)
                    return;
                bool systemPasswordChar = this.UseSystemPasswordChar;
                base.UseSystemPasswordChar = value;
                ChangeEventArgs<bool>.DoIf(this, this.UseSystemPasswordCharChanged, new Action<ChangeEventArgs<bool>>(this.OnUseSystemPasswordCharChanged), systemPasswordChar, this.UseSystemPasswordChar);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.SetUpTimer(false);
            base.Dispose(disposing);
        }

        protected virtual void OnPasswordCharChanged(ChangeEventArgs<char> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.PasswordCharChanged == null)
                return;
            this.PasswordCharChanged((object)this, e);
        }

        protected virtual void OnPasswordCharChanging(CancelChangeEventArgs<char> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.PasswordCharChanging == null)
                return;
            this.PasswordCharChanging((object)this, e);
        }

        protected virtual void OnPasswordCharDelayChanged(ChangeEventArgs<int> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.PasswordCharDelayChanged == null)
                return;
            this.PasswordCharDelayChanged((object)this, e);
        }

        protected virtual void OnPasswordCharDelayChanging(CancelChangeEventArgs<int> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.PasswordCharDelayChanging == null)
                return;
            this.PasswordCharDelayChanging((object)this, e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            this.timer?.Stop();
            this.timer?.Start();
            string str = this.Text ?? "";
            if (0 < str.Length && str.Length <= this.SelectionStart && (this.textPrevious ?? "").Length < str.Length)
                this.PaintUnobscured(str.Substring(str.Length - 1, 1), str.Length - 1);
            this.textPrevious = str;
            base.OnTextChanged(e);
        }

        protected virtual void OnUseSystemPasswordCharChanged(ChangeEventArgs<bool> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.UseSystemPasswordCharChanged == null)
                return;
            this.UseSystemPasswordCharChanged((object)this, e);
        }

        protected virtual void OnUseSystemPasswordCharChanging(CancelChangeEventArgs<bool> e)
        {
            if ((object)e == null)
                throw new ArgumentNullException(nameof(e));
            if (this.UseSystemPasswordCharChanging == null)
                return;
            this.UseSystemPasswordCharChanging((object)this, e);
        }

        protected void PaintUnobscured(string @string, int position)
        {
            if (position < 0 || this.TextLength <= position)
                throw new ArgumentOutOfRangeException(nameof(position), (object)position, "Must be greater than or equal to 0 and less than the value of TextLength.");
            if (this.PasswordCharEffective == char.MinValue || string.IsNullOrEmpty(@string))
                return;
            using (IGraphics graphics = TextPassword.NewGraphics((Control)this))
            {
                if (TextPassword.IsFontSmoothingEnabled())
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                using (ISolidBrush solidBrush1 = TextPassword.NewSolidBrush(this.ForeColor))
                {
                    using (ISolidBrush solidBrush2 = TextPassword.NewSolidBrush(this.BackColor))
                    {
                        string string1 = new string(this.PasswordCharEffective, @string.Length);
                        SizeF size = graphics.MeasureString(string1, this.Font);
                        Point positionFromCharIndex = this.GetPositionFromCharIndex(position);
                        positionFromCharIndex.Offset(-(int)Math.Round(Math.Pow((double)this.Font.SizeInPoints, 0.15)), (int)Math.Round(Math.Pow((double)this.Font.SizeInPoints, 0.05)));
                        graphics.FillRectangle(solidBrush2, new RectangleF((PointF)positionFromCharIndex, size));
                        graphics.DrawString(@string, this.Font, solidBrush1, (PointF)positionFromCharIndex);
                    }
                }
            }
        }

        private void SetUpTimer(bool settingUp)
        {
            if (settingUp)
            {
                this.timer = TextPassword.NewTimer(false, (double)this.PasswordCharDelay, (ISynchronizeInvoke)this);
                this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
            }
            else
            {
                this.timer?.Dispose();
                this.timer = (ITimer)null;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invalidate();
        }
    }
}
