using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace Chain
{
    public partial class Form1 : Form
    {
        IChainLinkDataService dataService;

        //List<ChainLink> jobs = new();
        BindingList<ChainLink> links = new();

        BindingSource bs = new BindingSource();

        List<KeyValuePair<string, string>> Statuses = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Looking", "01. Looking"),
            new KeyValuePair<string, string>("Saved", "02. Saved"),
            new KeyValuePair<string, string>("Applied", "03. Applied"),
            new KeyValuePair<string, string>("They Viewed", "04. They Viewed"),
            new KeyValuePair<string, string>("They Reached Out", "05. They Reached Out"),
            new KeyValuePair<string, string>("First Round Interview", "06. First Round Interview"),
            new KeyValuePair<string, string>("Second Round Interview", "07. Second Round Interview"),
            new KeyValuePair<string, string>("Not Moving Forward", "08. Not Moving Forward"),
            new KeyValuePair<string, string>("Offered", "09. Offered"),
            new KeyValuePair<string, string>("Cancelled", "10. Cancelled")
        };

        public ChainLink Item { get; set; }

        public Form1(IChainLinkDataService dataService)
        {
            InitializeComponent();

            this.dataService = dataService;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //await dataService.CreateChainLink(new ChainLink() { Company = "Brads Co.", Applied = DateTime.Now, LinkOne = "https://www.cnn.com", Status = "Applied", Comment = "This is a sample listing" });

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Chain", DataPropertyName = "Chain", ReadOnly = true, Visible = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Description", DataPropertyName = "DisplayText", ReadOnly = true, Visible = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Link", DataPropertyName = nameof(ChainLink.Url), ReadOnly = true, Visible = true });

            dataGridView1.Columns[0].SortMode =
                        DataGridViewColumnSortMode.Programmatic;
            dataGridView1.Columns[1].SortMode =
                DataGridViewColumnSortMode.Programmatic;
            dataGridView1.Columns[2].SortMode =
                DataGridViewColumnSortMode.Programmatic;

            //comboBox1.DataSource = Statuses;
            //comboBox1.DisplayMember = "Value";
            //comboBox1.ValueMember = "Key";

            Item = new();
            bs.DataSource = Item;
            //            bs.ListChanged += Bs_ListChanged;


            textBox1.DataBindings.Add(nameof(textBox1.Text), bs, nameof(ChainLink.Id), false, DataSourceUpdateMode.OnPropertyChanged);
            addableCombo1.DataBindings.Add(nameof(addableCombo1.SelectedItem), bs, nameof(ChainLink.Chain), false, DataSourceUpdateMode.OnPropertyChanged);
            textBox2.DataBindings.Add(nameof(textBox2.Text), bs, nameof(ChainLink.Url), false, DataSourceUpdateMode.OnPropertyChanged);
            textBox6.DataBindings.Add(nameof(textBox6.Text), bs, nameof(ChainLink.DisplayText), false, DataSourceUpdateMode.OnPropertyChanged);

            var v = await dataService.Select(null);

            foreach (ChainLink item in v)
            {
                links.Add(item);
            }
            dataGridView1.DataSource = links;

            addableCombo1.Choices = UniqueChains();
        }

        private List<string> UniqueChains()
        {
            List<string> rtnVal = new();

            rtnVal.Add(string.Empty);
            foreach (ChainLink item in links)
            {
                if (!rtnVal.Contains(item.Chain))
                {
                    rtnVal.Add(item.Chain);
                }
            }

            return rtnVal;
        }

        int idxRowBeingEdited = -1;

        private async Task SaveEdits()
        {
            if (idxRowBeingEdited != -1)
            {
                await dataService.UpdateChainLink(Item);
                idxRowBeingEdited = -1;
            }
        }

        private async void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            await SaveEdits();

            if (dataGridView1.CurrentRow != null)
            {
                Item = (ChainLink)dataGridView1.CurrentRow.DataBoundItem;
                bs.DataSource = Item;
                idxRowBeingEdited = dataGridView1.CurrentRow.Index;
            }

            //if (dataGridView1.SelectedRows.Count > 0)
            //{
            //    Item = (ChainLink)dataGridView1.SelectedRows[0].DataBoundItem;
            //    bs.DataSource = Item;
            //    idxRowBeingEdited = dataGridView1.SelectedRows[0].Index;
            //    //textBox1.DataBindings[0].ReadValue();

            //    //textBox6.DataBindings.Clear();
            //    //textBox6.DataBindings.Add(nameof(textBox6.Text), bs, nameof(ChainLink.Company), false, DataSourceUpdateMode.OnPropertyChanged);
            //}
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            await SaveEdits();
        }

        string prevOrderBy = "";
        bool descending = false;

        private async void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            await SaveEdits();
            string name = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            if (name != prevOrderBy)
            {

                prevOrderBy = name;
                descending = false;
            }
            else
            {
                descending = !descending;
            }

            var v = await dataService.Select(name, descending);
            links.Clear();
            foreach (ChainLink item in v)
            {
                links.Add(item);
            }
            dataGridView1.DataSource = links;
        }

        // https://stackoverflow.com/questions/8396331/glyph-not-displayed-on-datagridview#8412704
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Put each of the columns into programmatic sort mode.
            foreach (DataGridViewColumn column in ((DataGridView)sender).Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Programmatic;

                if (column.DataPropertyName != prevOrderBy)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                else
                    column.HeaderCell.SortGlyphDirection = descending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(textBox2.Text) { UseShellExecute = true });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //            Process.Start(new ProcessStartInfo(textBox3.Text) { UseShellExecute = true });
        }

        private async void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            await SaveEdits();

            ChainLink newLink = new ChainLink();
            newLink.Chain = string.Empty;
            links.Add(newLink);
            await dataService.CreateChainLink(newLink);

            //Item = newJob;
            //bs.DataSource = Item;

            dataGridView1.CurrentCell = dataGridView1.Rows[idxNewRow].Cells[0];
            dataGridView1.CurrentRow.Selected = true;

            dataGridView1_SelectionChanged(sender, EventArgs.Empty);
        }

        int idxNewRow = -1;

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            idxNewRow = e.RowIndex;
            //            dataGridView1.Rows[e.RowIndex].Selected = true;

            //dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
            //dataGridView1.CurrentRow.Selected = true;

        }

        private async void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            ChainLink job = (ChainLink)(dataGridView1.SelectedRows[0].DataBoundItem);
            var v = await dataService.DeleteChainLink(job.Id);
            links.Remove(job);
            //Item = new();
            //bs.DataSource = Item;


            ////int idx = dataGridView1.CurrentRow.Index;
            //if (idx < dataGridView1.Rows.Count)
            //{
            //}
            //else if (idx > 1)
            //{
            //    idx--;
            //}

            ////dataGridView1.CurrentRow = idx;

            //dataGridView1.CurrentCell = dataGridView1.Rows[idx].Cells[0];


        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            int idx = e.RowIndex;
            if (idx < dataGridView1.Rows.Count)
            {
            }
            else if (idx > 1)
            {
                idx--;
            }

            if (dataGridView1.Rows.Count == 0)
                return;

            // https://social.msdn.microsoft.com/Forums/azure/en-US/556f528b-4583-4be2-bb42-487ad2c96532/how-to-set-the-currentrow-of-a-datagridview?forum=winformsdatacontrols
            // You can NOT set the current row, BUT you can set the current cell
            dataGridView1.CurrentCell = dataGridView1.Rows[idx].Cells[0];
            dataGridView1.CurrentRow.Selected = true;
            Item = (ChainLink)dataGridView1.CurrentRow.DataBoundItem;
            bs.DataSource = Item;
            idxRowBeingEdited = idx;
        }

        private async void textBox2_Leave(object sender, EventArgs e)
        {
            await SetDescription();
        }

        private async Task SetDescription()
        {
            if (string.IsNullOrEmpty(Item.DisplayText) && !string.IsNullOrEmpty(Item.Url))
            {
                using HttpClient client = new()
                {
                    BaseAddress = new Uri(Item.Url),
                };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "web api client");

                using HttpResponseMessage response = await client.GetAsync("");

                var stringResponse = await response.Content.ReadAsStringAsync();

                int titleStart = stringResponse.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
                if (titleStart != -1)
                {
                    titleStart += 7;
                    int titleEnd = stringResponse.IndexOf("</title>", titleStart, StringComparison.OrdinalIgnoreCase);

                    Item.DisplayText = stringResponse.Substring(titleStart, (titleEnd - titleStart));
                    textBox6.DataBindings[0].ReadValue();
                }
            }
        }

        private async Task<string> GetDescription(string url)
        {
            string rtnVal = "";

            using HttpClient client = new()
            {
                BaseAddress = new Uri(url),
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "web api client");

            using HttpResponseMessage response = await client.GetAsync("");

            var stringResponse = await response.Content.ReadAsStringAsync();

            int titleStart = stringResponse.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
            if (titleStart != -1)
            {
                titleStart += 7;
                int titleEnd = stringResponse.IndexOf("</title>", titleStart, StringComparison.OrdinalIgnoreCase);

                rtnVal = stringResponse.Substring(titleStart, (titleEnd - titleStart));
                textBox6.DataBindings[0].ReadValue();
            }

            return rtnVal;
        }


        private async void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                if (dataString.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {

                }
                else
                {
                    // because Chrome removes this part, by default
                    // If you drag the lock icon, instead of the text it will include the full https://
                    dataString = "https://" + dataString;
                }

                await SaveEdits();

                ChainLink newLink = new ChainLink();
                newLink.Chain = string.Empty;
                newLink.Url = dataString;
                newLink.DisplayText = await GetDescription(dataString);

                links.Add(newLink);
                await dataService.CreateChainLink(newLink);

                //Item = newJob;
                //bs.DataSource = Item;

                dataGridView1.CurrentCell = dataGridView1.Rows[idxNewRow].Cells[0];
                dataGridView1.CurrentRow.Selected = true;

                dataGridView1_SelectionChanged(sender, EventArgs.Empty);
            }
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {

        }
    }
}

