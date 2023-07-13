using System.ComponentModel;

namespace Chain
{
    public partial class AddableCombo : System.Windows.Forms.ComboBox
    {
        public AddableCombo()
        {
            InitializeComponent();
            KeyDown += AddableCombo_KeyDown;
            LostFocus += AddableCombo_LostFocus;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible && !Disposing)
            {
                DataSource = internalChoices;
            }
        }

        //BindingList<string> links = new();
        public List<string> Choices {
            set 
            {
                internalChoices = new BindingList<string>(value);
                DataSource = internalChoices;
            }
        }
        public BindingList<string> internalChoices { get; set; } = new();

        private void AddableCombo_LostFocus(object? sender, EventArgs e)
        {
            string s = base.Text;
            // if item exists, select it. if it does not exist, add it.
            if (!string.IsNullOrEmpty(s) && (!base.Items.Contains(s)))
            {
                internalChoices.Add(s);
                //((List<string>)DataSource).Add(s);
                //base.Items.Add(s);
                DataSource = internalChoices;
                SelectedItem = s;
                foreach(Binding v in DataBindings)
                {
                    v.WriteValue();
                }
            }
        }

        private void AddableCombo_KeyDown(object? sender, KeyEventArgs e)
        {
            string s = base.Text;
            if (e.KeyCode == Keys.Enter)
            {
                internalChoices.Add(s);
                //((List<string>)DataSource).Add(s);
                //base.Items.Add(s);
                DataSource = internalChoices;
                SelectedItem = s;
                foreach (Binding v in DataBindings)
                {
                    v.WriteValue();
                }
                e.Handled = true;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
