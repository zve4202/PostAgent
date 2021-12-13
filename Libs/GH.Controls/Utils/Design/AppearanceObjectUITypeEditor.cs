using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GH.Controls.Utils.Design
{
    public class AppearanceObjectUITypeEditor : UITypeEditor
    {
        public override object EditValue(
          ITypeDescriptorContext context,
          System.IServiceProvider provider,
          object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (service != null)
                {
                    if (!(value is AppearanceObject appearanceObject))
                        return (object)appearanceObject;
                    AppearanceObjectEditorForm objectEditorForm = new AppearanceObjectEditorForm();
                    string currentAppearanceName = string.Empty;
                    if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.ComponentType != (System.Type)null)
                        currentAppearanceName = context.PropertyDescriptor.ComponentType.Name + " - " + context.PropertyDescriptor.DisplayName;
                    objectEditorForm.Init(value, provider, currentAppearanceName);
                    if (service.ShowDialog((Form)objectEditorForm) == DialogResult.OK)
                    {
                        context.OnComponentChanging();
                        appearanceObject.Reset();
                        appearanceObject.Assign(objectEditorForm.ResultValue);
                        context.OnComponentChanged();
                    }
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(
          ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }

    public class AppearanceObjectEditorForm : DesignTimeEditorForm
    {
        private Dictionary<AppearanceObject, Bitmap> imagePreviewBitmap = new Dictionary<AppearanceObject, Bitmap>();
        private const string PropertyGridSort = "PropertyGridSort";
        private const string RegistryStoreBasePath = "Software\\Developer Express\\Designer\\";
        private System.IServiceProvider serviceProvider;
        private IContainer components;
        private AppearancesPreview appearancePreview;
        private LayoutControl layoutControl;
        private LayoutControlGroup Root;
        private MVVMContext mvvmContext1;
        private SimpleButton okSimpleButton;
        private BindingSource appearanceObjectEditorModelBindingSource;
        private SimpleButton btnAddToCollection;
        private SimpleButton btnSave;
        private LayoutControlItem lciBtnSave;
        private LayoutControlItem lciBtnAddToCollection;
        private DXPropertyGridEx propertyGrid1;
        private PopupContainerControl popupContainerControl1;
        private PopupContainerEdit popupContainerEdit1;
        private LayoutControlItem lciPopupContainerEdit1;
        private GridControl gridControl1;
        private TileView tileView1;
        private TileViewColumn colTemplateName;
        private LayoutControlItem lciAppearancePreview;
        private LayoutControlItem lciPropertyGrid1;

        public AppearanceObjectEditorForm()
        {
            this.InitializeComponent();
            this.popupContainerEdit1.MinimumSize = this.popupContainerEdit1.MaximumSize = new Size(0, this.btnSave.Height);
        }

        private static Bitmap CreateBitmap(AppearanceObject appearanceObject, Bitmap bitmap)
        {
            if (bitmap == null)
                bitmap = new Bitmap(50, 44);
            Rectangle rectangle = new Rectangle(Point.Empty, bitmap.Size);
            rectangle.Inflate(-1, -1);
            string s = "Aa";
            using (Graphics g = Graphics.FromImage((Image)bitmap))
            {
                g.Clear(Color.Transparent);
                TextRenderingHint textRenderingHint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                using (GraphicsCache cache = new GraphicsCache(g))
                {
                    appearanceObject.FillRectangle(cache, rectangle);
                    cache.DrawRectangle(appearanceObject.GetBorderPen(cache), rectangle);
                    rectangle.Inflate(-2, -2);
                    g.DrawString(s, appearanceObject.GetFont(), appearanceObject.ForeColor == Color.Empty ? Brushes.Black : appearanceObject.GetForeBrush(cache), (RectangleF)rectangle, appearanceObject.GetStringFormat());
                }
                g.TextRenderingHint = textRenderingHint;
            }
            return bitmap;
        }

        private void SetFontStyleByState(AppearanceObjectEditorModel x, AppearanceObject appearance)
        {
            switch (x.ModelState & ~AppearanceViewModelState.AllowDelete)
            {
                case AppearanceViewModelState.AllowSave:
                    appearance.FontStyleDelta = FontStyle.Bold;
                    appearance.ForeColor = Color.Empty;
                    break;
                case AppearanceViewModelState.AllowAddToCollection:
                    appearance.FontStyleDelta = FontStyle.Italic;
                    appearance.ForeColor = CommonSkins.GetSkin((ISkinProvider)this.LookAndFeel).Colors.GetColor((object)CommonColors.DisabledText);
                    break;
                default:
                    appearance.FontStyleDelta = FontStyle.Regular;
                    appearance.ForeColor = Color.Empty;
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
                foreach (KeyValuePair<AppearanceObject, Bitmap> keyValuePair in this.imagePreviewBitmap)
                    keyValuePair.Value.Dispose();
                this.imagePreviewBitmap.Clear();
            }
            base.Dispose(disposing);
        }

        protected override object GetService(System.Type service)
        {
            return this.serviceProvider != null ? this.serviceProvider.GetService(service) : base.GetService(service);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.StoreProperties();
        }

        protected override void RestoreProperties()
        {
            this.Store.Restore();
            this.propertyGrid1.PropertySort = (PropertySort)this.Store.RestoreProperty("PropertyGridSort", typeof(PropertySort), (object)this.propertyGrid1.PropertySort);
            this.Store.RestoreForm((Form)this);
        }

        protected override void StoreProperties()
        {
            this.Store.AddProperty("PropertyGridSort", (object)this.propertyGrid1.PropertySort);
            this.Store.AddForm((Form)this);
            this.Store.Store();
        }

        protected override string RegistryStorePath
        {
            get
            {
                return "Software\\Developer Express\\Designer\\AppearanceObjectEditor\\";
            }
        }

        public void Init(object value, System.IServiceProvider serviceProvider, string currentAppearanceName)
        {
            this.serviceProvider = serviceProvider;
            this.mvvmContext1.SetViewModel(typeof(AppearanceObjectEditorViewModel), (object)POCOSource.Create<AppearanceObjectEditorViewModel>((Expression<Func<AppearanceObjectEditorViewModel>>)(() => new AppearanceObjectEditorViewModel((value as AppearanceObject).Clone() as AppearanceObject, currentAppearanceName))));
            MVVMContextFluentAPI<AppearanceObjectEditorViewModel> fluentAPI = this.mvvmContext1.OfType<AppearanceObjectEditorViewModel>();
            fluentAPI.SetObjectDataSourceBinding<BindingList<AppearanceObjectEditorModel>>(this.appearanceObjectEditorModelBindingSource, (Expression<Func<AppearanceObjectEditorViewModel, BindingList<AppearanceObjectEditorModel>>>)(x => x.TemplateAppearances), (Expression<Action<AppearanceObjectEditorViewModel>>)null);
            fluentAPI.SetBinding<PopupContainerEdit, object, AppearanceObjectEditorModel>(this.popupContainerEdit1, (Expression<Func<PopupContainerEdit, object>>)(x => x.EditValue), (Expression<Func<AppearanceObjectEditorViewModel, AppearanceObjectEditorModel>>)(x => x.SelectedTemplateAppearance), (Func<AppearanceObjectEditorModel, object>)null, (Func<object, AppearanceObjectEditorModel>)null);
            fluentAPI.BindCommand<XtraInputBoxArgs>((ISupportCommandBinding)this.btnAddToCollection, (Expression<Action<AppearanceObjectEditorViewModel, XtraInputBoxArgs>>)((x, p) => x.AddToCollection(p)), (Expression<Func<AppearanceObjectEditorViewModel, XtraInputBoxArgs>>)(x => new XtraInputBoxArgs(this.LookAndFeel, this, "Template name:", "Appearance Name", "New Appearance", default(DialogResult[]), 0, DefaultBoolean.Default, MessageBeepSound.None)));
            fluentAPI.BindCommand((ISupportCommandBinding)this.btnSave, (Expression<Action<AppearanceObjectEditorViewModel>>)(x => x.Save()), (Expression<Func<AppearanceObjectEditorViewModel, object>>)null);
            this.SetFontStyleByState(fluentAPI.ViewModel.SelectedTemplateAppearance, this.popupContainerEdit1.Properties.Appearance);
            fluentAPI.SetBinding<DXPropertyGridEx, object, AppearanceObject>(this.propertyGrid1, (Expression<Func<DXPropertyGridEx, object>>)(x => x.SelectedObject), (Expression<Func<AppearanceObjectEditorViewModel, AppearanceObject>>)(x => x.SelectedTemplateAppearance.AppearanceObject), (Func<AppearanceObject, object>)null, (Func<object, AppearanceObject>)null);
            this.appearancePreview.SetAppearance(new object[1]
            {
        this.propertyGrid1.SelectedObject
            });
            fluentAPI.WithEvent<PropertyValueChangedEventArgs>((object)this.propertyGrid1, "PropertyValueChanged").EventToCommand((Expression<Action<AppearanceObjectEditorViewModel>>)(x => x.SetAllowSave()));
            fluentAPI.SetTrigger<bool>((Expression<Func<AppearanceObjectEditorViewModel, bool>>)(x => x.UpdateTrigger), (Action<bool>)(x =>
            {
                this.appearancePreview.SetAppearance(new object[1]
                {
          (object) fluentAPI.ViewModel.SelectedTemplateAppearance.AppearanceObject
                });
                this.SetFontStyleByState(fluentAPI.ViewModel.SelectedTemplateAppearance, this.popupContainerEdit1.Properties.Appearance);
            }));
            this.RestoreProperties();
        }

        public override void InitLookAndFeel(object targetObject, System.IServiceProvider service = null)
        {
            base.InitLookAndFeel(targetObject, service ?? this.serviceProvider);
            ControlContainerLookAndFeelHelper.UpdateControlChildrenLookAndFeel((Control)this, (UserLookAndFeel)this.LookAndFeel);
            this.tileView1.Appearance.ItemHovered.BackColor = Color.FromArgb(50, CommonSkins.GetSkin((ISkinProvider)this.LookAndFeel).Colors.GetColor((object)CommonColors.Highlight));
            this.tileView1.Appearance.ItemNormal.BackColor = this.popupContainerEdit1.BackColor;
            this.tileView1.Appearance.ItemNormal.ForeColor = CommonSkins.GetSkin((ISkinProvider)this.LookAndFeel).Colors.GetColor((object)CommonColors.ControlText);
        }

        public AppearanceObject ResultValue
        {
            get
            {
                return this.mvvmContext1.OfType<AppearanceObjectEditorViewModel>().ViewModel.SelectedTemplateAppearance.AppearanceObject;
            }
        }

        private void tileView1_ContextButtonClick(object sender, ContextItemClickEventArgs e)
        {
            MVVMContextFluentAPI<AppearanceObjectEditorViewModel> contextFluentApi = this.mvvmContext1.OfType<AppearanceObjectEditorViewModel>();
            if (!(e.DataItem is TileViewItem dataItem) || !(this.tileView1.GetRow(dataItem.RowHandle) is AppearanceObjectEditorModel row))
                return;
            if (e.Item.Name == "removeContextButton")
            {
                contextFluentApi.ViewModel.Delete(row, new XtraMessageBoxArgs((UserLookAndFeel)this.LookAndFeel, (IWin32Window)this, "Are you sure you want to delete this template?", "Delete Appearance Template", new DialogResult[2]
                {
          DialogResult.Yes,
          DialogResult.No
                }, (Icon)null, 0, DefaultBoolean.Default, MessageBeepSound.None));
                this.tileView1.RefreshData();
            }
            if (!(e.Item.Name == "renameContextButton"))
                return;
            contextFluentApi.ViewModel.Rename(row, new XtraInputBoxArgs((UserLookAndFeel)this.LookAndFeel, (IWin32Window)this, "Template name:", "Appearance Name", row.TemplateName, (DialogResult[])null, 0, DefaultBoolean.Default, MessageBeepSound.None));
            this.tileView1.RefreshData();
        }

        private void tileView1_ContextButtonCustomize(
          object sender,
          TileViewContextButtonCustomizeEventArgs e)
        {
            if (!(this.tileView1.GetRow(e.RowHandle) is AppearanceObjectEditorModel row))
                return;
            if (e.Item.Name == "removeContextButton" && (row.ModelState & AppearanceViewModelState.AllowDelete) == AppearanceViewModelState.Normal)
                e.Item.Visibility = ContextItemVisibility.Hidden;
            if (!(e.Item.Name == "renameContextButton") || (row.ModelState & AppearanceViewModelState.AllowAddToCollection) == AppearanceViewModelState.Normal)
                return;
            e.Item.Visibility = ContextItemVisibility.Hidden;
        }

        private void tileView1_ItemClick(object sender, TileViewItemClickEventArgs e)
        {
            MVVMContextFluentAPI<AppearanceObjectEditorViewModel> contextFluentApi = this.mvvmContext1.OfType<AppearanceObjectEditorViewModel>();
            if (!(this.tileView1.GetRow(e.Item.RowHandle) is AppearanceObjectEditorModel row))
                return;
            contextFluentApi.ViewModel.SelectedTemplateAppearance = row;
            this.popupContainerEdit1.ClosePopup();
        }

        private void tileView1_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
        {
            if (!(this.tileView1.GetRow(e.RowHandle) is AppearanceObjectEditorModel row))
                return;
            this.SetFontStyleByState(row, e.Item.AppearanceItem.Normal);
            TileViewItemElement elementByName = e.Item.GetElementByName("PreviewImage");
            Bitmap bitmap = (Bitmap)null;
            if (!this.imagePreviewBitmap.TryGetValue(row.AppearanceObject, out bitmap))
                this.imagePreviewBitmap.Add(row.AppearanceObject, bitmap);
            bitmap = AppearanceObjectEditorForm.CreateBitmap(row.AppearanceObject, bitmap);
            elementByName.Image = (Image)bitmap;
            elementByName.ImageAlignment = TileItemContentAlignment.MiddleLeft;
            elementByName.ImageBorder = TileItemElementImageBorderMode.SingleBorder;
            elementByName.ImageScaleMode = TileItemImageScaleMode.NoScale;
        }

        private void popupContainerEdit1_BeforePopup(object sender, EventArgs e)
        {
            this.popupContainerControl1.Width = this.popupContainerEdit1.Width - 2;
            this.popupContainerControl1.Height = this.appearanceObjectEditorModelBindingSource.Count * (this.tileView1.OptionsTiles.ItemSize.Height + 1);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            ContextButton contextButton1 = new ContextButton();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AppearanceObjectEditorForm));
            ContextButton contextButton2 = new ContextButton();
            TableColumnDefinition columnDefinition1 = new TableColumnDefinition();
            TableColumnDefinition columnDefinition2 = new TableColumnDefinition();
            TableRowDefinition tableRowDefinition = new TableRowDefinition();
            TileViewItemElement tileViewItemElement1 = new TileViewItemElement();
            TileViewItemElement tileViewItemElement2 = new TileViewItemElement();
            this.colTemplateName = new TileViewColumn();
            this.appearancePreview = new AppearancesPreview();
            this.layoutControl = new LayoutControl();
            this.popupContainerEdit1 = new PopupContainerEdit();
            this.popupContainerControl1 = new PopupContainerControl();
            this.gridControl1 = new GridControl();
            this.appearanceObjectEditorModelBindingSource = new BindingSource(this.components);
            this.tileView1 = new TileView();
            this.propertyGrid1 = new DXPropertyGridEx();
            this.btnAddToCollection = new SimpleButton();
            this.btnSave = new SimpleButton();
            this.Root = new LayoutControlGroup();
            this.lciBtnSave = new LayoutControlItem();
            this.lciBtnAddToCollection = new LayoutControlItem();
            this.lciPopupContainerEdit1 = new LayoutControlItem();
            this.lciAppearancePreview = new LayoutControlItem();
            this.lciPropertyGrid1 = new LayoutControlItem();
            this.okSimpleButton = new SimpleButton();
            this.mvvmContext1 = new MVVMContext(this.components);
            this.btnsPanel.BeginInit();
            this.btnsPanel.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlMainForm.SuspendLayout();
            this.pnlFrame.BeginInit();
            this.pnlFrame.SuspendLayout();
            this.appearancePreview.BeginInit();
            this.layoutControl.BeginInit();
            this.layoutControl.SuspendLayout();
            this.popupContainerEdit1.Properties.BeginInit();
            this.popupContainerControl1.BeginInit();
            this.popupContainerControl1.SuspendLayout();
            this.gridControl1.BeginInit();
            ((ISupportInitialize)this.appearanceObjectEditorModelBindingSource).BeginInit();
            this.tileView1.BeginInit();
            this.Root.BeginInit();
            this.lciBtnSave.BeginInit();
            this.lciBtnAddToCollection.BeginInit();
            this.lciPopupContainerEdit1.BeginInit();
            this.lciAppearancePreview.BeginInit();
            this.lciPropertyGrid1.BeginInit();
            ((ISupportInitialize)this.mvvmContext1).BeginInit();
            this.SuspendLayout();
            this.btnsPanel.MinimumSize = new Size(0, 30);
            this.btnsPanel.Size = new Size(560, 30);
            this.btnsPanel.Controls.Add((Control)this.okSimpleButton);
            this.btnsPanel.Controls.SetChildIndex((Control)this.okSimpleButton, 0);
            this.closeButton.Location = new Point(470, 0);
            this.closeButton.Text = "Cancel";
            this.pnlMainForm.Size = new Size(684, 561);
            this.pnlFrame.Controls.Add((Control)this.popupContainerControl1);
            this.pnlFrame.Controls.Add((Control)this.layoutControl);
            this.pnlFrame.Padding = new System.Windows.Forms.Padding(24, 24, 22, 24);
            this.pnlFrame.Size = new Size(684, 506);
            this.colTemplateName.FieldName = "TemplateName";
            this.colTemplateName.Name = "colTemplateName";
            this.colTemplateName.Visible = true;
            this.colTemplateName.VisibleIndex = 0;
            this.appearancePreview.CaptionContentMargins = (Margins)null;
            this.appearancePreview.CaptionTextPadding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.appearancePreview.Location = new Point(2, 376);
            this.appearancePreview.MaximumSize = new Size(0, 80);
            this.appearancePreview.MinimumSize = new Size(0, 80);
            this.appearancePreview.Name = "appearancePreview";
            this.appearancePreview.ShowCaption = false;
            this.appearancePreview.Size = new Size(634, 80);
            this.appearancePreview.TabIndex = 0;
            this.appearancePreview.Text = "Appearances Preview";
            this.layoutControl.AllowCustomization = false;
            this.layoutControl.Controls.Add((Control)this.popupContainerEdit1);
            this.layoutControl.Controls.Add((Control)this.propertyGrid1);
            this.layoutControl.Controls.Add((Control)this.btnAddToCollection);
            this.layoutControl.Controls.Add((Control)this.btnSave);
            this.layoutControl.Controls.Add((Control)this.appearancePreview);
            this.layoutControl.Dock = DockStyle.Fill;
            this.layoutControl.Location = new Point(24, 24);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle?(new Rectangle(-791, 66, 650, 586));
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new Size(638, 458);
            this.layoutControl.TabIndex = 1;
            this.popupContainerEdit1.EditValue = (object)"Editing Appearance";
            this.popupContainerEdit1.Location = new Point(2, 2);
            this.popupContainerEdit1.MaximumSize = new Size(0, 30);
            this.popupContainerEdit1.MinimumSize = new Size(0, 30);
            this.popupContainerEdit1.Name = "popupContainerEdit1";
            this.popupContainerEdit1.Properties.Buttons.AddRange(new EditorButton[1]
            {
        new EditorButton(ButtonPredefines.Combo)
            });
            this.popupContainerEdit1.Properties.PopupControl = this.popupContainerControl1;
            this.popupContainerEdit1.Properties.PopupSizeable = false;
            this.popupContainerEdit1.Properties.ShowPopupCloseButton = false;
            this.popupContainerEdit1.Size = new Size(446, 30);
            this.popupContainerEdit1.StyleController = (IStyleController)this.layoutControl;
            this.popupContainerEdit1.TabIndex = 12;
            this.popupContainerEdit1.BeforePopup += new EventHandler(this.popupContainerEdit1_BeforePopup);
            this.popupContainerControl1.Controls.Add((Control)this.gridControl1);
            this.popupContainerControl1.Location = new Point(64, 12);
            this.popupContainerControl1.Name = "popupContainerControl1";
            this.popupContainerControl1.Size = new Size(389, 180);
            this.popupContainerControl1.TabIndex = 2;
            this.gridControl1.DataSource = (object)this.appearanceObjectEditorModelBindingSource;
            this.gridControl1.Dock = DockStyle.Fill;
            this.gridControl1.Location = new Point(0, 0);
            this.gridControl1.MainView = (BaseView)this.tileView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new Size(389, 180);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new BaseView[1]
            {
        (BaseView) this.tileView1
            });
            this.appearanceObjectEditorModelBindingSource.DataSource = (object)typeof(AppearanceObjectEditorModel);
            this.tileView1.Columns.AddRange(new GridColumn[1]
            {
        (GridColumn) this.colTemplateName
            });
            this.tileView1.ContextButtonOptions.AnimationType = ContextAnimationType.None;
            contextButton1.AlignmentOptions.Panel = ContextItemPanel.Center;
            contextButton1.AlignmentOptions.Position = ContextItemPosition.Far;
            contextButton1.Id = new Guid("cccaf291-e362-49d4-91e1-fd08fc4ef61b");
            contextButton1.ImageOptions.SvgImage = (SvgImage)componentResourceManager.GetObject("resource.SvgImage");
            contextButton1.ImageOptions.SvgImageSize = new Size(24, 24);
            contextButton1.Name = "removeContextButton";
            contextButton1.Padding = new System.Windows.Forms.Padding(4);
            contextButton1.ToolTip = "Remove this template from the collection";
            contextButton2.AlignmentOptions.Panel = ContextItemPanel.Center;
            contextButton2.AlignmentOptions.Position = ContextItemPosition.Far;
            contextButton2.Id = new Guid("e4fefcd4-a8f0-42be-8f17-f99d479e6881");
            contextButton2.ImageOptions.SvgImage = (SvgImage)componentResourceManager.GetObject("resource.SvgImage1");
            contextButton2.ImageOptions.SvgImageSize = new Size(24, 24);
            contextButton2.Name = "renameContextButton";
            contextButton2.Padding = new System.Windows.Forms.Padding(4);
            contextButton2.ToolTip = "Rename this appearance template";
            this.tileView1.ContextButtons.Add((ContextItem)contextButton1);
            this.tileView1.ContextButtons.Add((ContextItem)contextButton2);
            this.tileView1.FocusBorderColor = Color.Transparent;
            this.tileView1.GridControl = this.gridControl1;
            this.tileView1.Name = "tileView1";
            this.tileView1.OptionsBehavior.AllowSmoothScrolling = true;
            this.tileView1.OptionsTiles.AllowItemHover = true;
            this.tileView1.OptionsTiles.AllowPressAnimation = false;
            this.tileView1.OptionsTiles.IndentBetweenGroups = 0;
            this.tileView1.OptionsTiles.IndentBetweenItems = 0;
            this.tileView1.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tileView1.OptionsTiles.ItemSize = new Size(150, 50);
            this.tileView1.OptionsTiles.LayoutMode = TileViewLayoutMode.List;
            this.tileView1.OptionsTiles.Orientation = Orientation.Vertical;
            this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
            this.tileView1.OptionsTiles.RowCount = 0;
            this.tileView1.OptionsTiles.ScrollMode = TileControlScrollMode.TouchScrollBar;
            columnDefinition1.Length.Type = TableDefinitionLengthType.Pixel;
            columnDefinition1.Length.Value = 60.0;
            columnDefinition1.PaddingRight = 10;
            this.tileView1.TileColumns.Add(columnDefinition1);
            this.tileView1.TileColumns.Add(columnDefinition2);
            this.tileView1.TileRows.Add(tableRowDefinition);
            tileViewItemElement1.Column = (GridColumn)this.colTemplateName;
            tileViewItemElement1.ColumnIndex = 1;
            tileViewItemElement1.ImageAlignment = TileItemContentAlignment.MiddleCenter;
            tileViewItemElement1.ImageScaleMode = TileItemImageScaleMode.ZoomInside;
            tileViewItemElement1.Text = "colTemplateName";
            tileViewItemElement1.TextAlignment = TileItemContentAlignment.MiddleLeft;
            tileViewItemElement2.ImageScaleMode = TileItemImageScaleMode.NoScale;
            tileViewItemElement2.Name = "PreviewImage";
            tileViewItemElement2.Text = "";
            tileViewItemElement2.TextAlignment = TileItemContentAlignment.MiddleCenter;
            this.tileView1.TileTemplate.Add((TileItemElement)tileViewItemElement1);
            this.tileView1.TileTemplate.Add((TileItemElement)tileViewItemElement2);
            this.tileView1.ItemClick += new TileViewItemClickEventHandler(this.tileView1_ItemClick);
            this.tileView1.ContextButtonClick += new ContextItemClickEventHandler(this.tileView1_ContextButtonClick);
            this.tileView1.ContextButtonCustomize += new TileViewContextButtonCustomizeEventHandler(this.tileView1_ContextButtonCustomize);
            this.tileView1.ItemCustomize += new TileViewItemCustomizeEventHandler(this.tileView1_ItemCustomize);
            this.propertyGrid1.DrawFlat = false;
            this.propertyGrid1.Location = new Point(2, 36);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.ShowSearchPanel = true;
            this.propertyGrid1.Size = new Size(634, 336);
            this.propertyGrid1.TabIndex = 11;
            this.btnAddToCollection.Location = new Point(546, 2);
            this.btnAddToCollection.MaximumSize = new Size(90, 30);
            this.btnAddToCollection.MinimumSize = new Size(90, 30);
            this.btnAddToCollection.Name = "btnAddToCollection";
            this.btnAddToCollection.Size = new Size(90, 30);
            this.btnAddToCollection.StyleController = (IStyleController)this.layoutControl;
            this.btnAddToCollection.TabIndex = 9;
            this.btnAddToCollection.Text = "Save As...";
            this.btnAddToCollection.ToolTip = "Save current appearance settings as a template";
            this.btnSave.Location = new Point(452, 2);
            this.btnSave.MaximumSize = new Size(90, 30);
            this.btnSave.MinimumSize = new Size(90, 30);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(90, 30);
            this.btnSave.StyleController = (IStyleController)this.layoutControl;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.ToolTip = "Save the modified template";
            this.Root.EnableIndentsWithoutBorders = DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new BaseLayoutItem[5]
            {
        (BaseLayoutItem) this.lciBtnSave,
        (BaseLayoutItem) this.lciBtnAddToCollection,
        (BaseLayoutItem) this.lciPopupContainerEdit1,
        (BaseLayoutItem) this.lciAppearancePreview,
        (BaseLayoutItem) this.lciPropertyGrid1
            });
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new Size(638, 458);
            this.lciBtnSave.Control = (Control)this.btnSave;
            this.lciBtnSave.Location = new Point(450, 0);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new Size(94, 34);
            this.lciBtnSave.TextSize = new Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            this.lciBtnAddToCollection.Control = (Control)this.btnAddToCollection;
            this.lciBtnAddToCollection.Location = new Point(544, 0);
            this.lciBtnAddToCollection.Name = "lciBtnAddToCollection";
            this.lciBtnAddToCollection.Size = new Size(94, 34);
            this.lciBtnAddToCollection.TextSize = new Size(0, 0);
            this.lciBtnAddToCollection.TextVisible = false;
            this.lciPopupContainerEdit1.Control = (Control)this.popupContainerEdit1;
            this.lciPopupContainerEdit1.Location = new Point(0, 0);
            this.lciPopupContainerEdit1.Name = "lciPopupContainerEdit1";
            this.lciPopupContainerEdit1.Size = new Size(450, 34);
            this.lciPopupContainerEdit1.TextSize = new Size(0, 0);
            this.lciPopupContainerEdit1.TextVisible = false;
            this.lciAppearancePreview.Control = (Control)this.appearancePreview;
            this.lciAppearancePreview.Location = new Point(0, 374);
            this.lciAppearancePreview.Name = "lciAppearancePreview";
            this.lciAppearancePreview.Size = new Size(638, 84);
            this.lciAppearancePreview.TextSize = new Size(0, 0);
            this.lciAppearancePreview.TextVisible = false;
            this.lciPropertyGrid1.Control = (Control)this.propertyGrid1;
            this.lciPropertyGrid1.Location = new Point(0, 34);
            this.lciPropertyGrid1.Name = "lciPropertyGrid1";
            this.lciPropertyGrid1.Size = new Size(638, 340);
            this.lciPropertyGrid1.TextSize = new Size(0, 0);
            this.lciPropertyGrid1.TextVisible = false;
            this.okSimpleButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.okSimpleButton.DialogResult = DialogResult.OK;
            this.okSimpleButton.Location = new Point(476, 14);
            this.okSimpleButton.Name = "okSimpleButton";
            this.okSimpleButton.Size = new Size(90, 30);
            this.okSimpleButton.TabIndex = 4;
            this.okSimpleButton.Text = "Apply";
            this.mvvmContext1.ContainerControl = (ContainerControl)this;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.ClientSize = new Size(684, 561);
            this.MinimumSize = new Size(450, 450);
            this.Name = nameof(AppearanceObjectEditorForm);
            this.Text = "Appearance Editor";
            this.Controls.SetChildIndex((Control)this.pnlMainForm, 0);
            this.btnsPanel.EndInit();
            this.btnsPanel.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlMainForm.ResumeLayout(false);
            this.pnlFrame.EndInit();
            this.pnlFrame.ResumeLayout(false);
            this.appearancePreview.EndInit();
            this.layoutControl.EndInit();
            this.layoutControl.ResumeLayout(false);
            this.popupContainerEdit1.Properties.EndInit();
            this.popupContainerControl1.EndInit();
            this.popupContainerControl1.ResumeLayout(false);
            this.gridControl1.EndInit();
            ((ISupportInitialize)this.appearanceObjectEditorModelBindingSource).EndInit();
            this.tileView1.EndInit();
            this.Root.EndInit();
            this.lciBtnSave.EndInit();
            this.lciBtnAddToCollection.EndInit();
            this.lciPopupContainerEdit1.EndInit();
            this.lciAppearancePreview.EndInit();
            this.lciPropertyGrid1.EndInit();
            ((ISupportInitialize)this.mvvmContext1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }

    [ToolboxItem(false)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class MouseWheelContainerForm : Form, IMouseWheelContainer
    {
        Control IMouseWheelContainer.Control
        {
            get
            {
                return (Control)this;
            }
        }

        void IMouseWheelContainer.BaseOnMouseWheel(MouseEventArgs e)
        {
            if (ControlHelper.IsHMouseWheel(e))
                this.OnMouseHWheel(e);
            else
                base.OnMouseWheel(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MouseWheelHelper.DoMouseWheel((IMouseWheelContainer)this, e);
        }

        protected virtual void OnMouseHWheel(MouseEventArgs e)
        {
        }
    }

    public interface IMouseWheelContainer
    {
        void BaseOnMouseWheel(MouseEventArgs e);

        Control Control { get; }
    }

    public class XtraForm : MouseWheelContainerForm, ISupportLookAndFeel, IDXControl, ICustomDrawNonClientArea, IGlassForm
    {
        private bool closeBox = true;
        private string htmlText = string.Empty;
        private string mdiChildCaptionFormatString = "{1} - {0}";
        private FormBorderEffect formBorderEffect = FormBorderEffect.Default;
        protected Rectangle prevFormBounds = Rectangle.Empty;
        protected Rectangle formBounds = Rectangle.Empty;
        private Point minimizedFormLocation = new Point(-32000, -32000);
        private SizeF scaleFactor = new SizeF(1f, 1f);
        private Size? minimumSize = new Size?();
        private Size? maximumSize = new Size?();
        private int acrylicAccentTintOpacity = DCompositionSettings.DCompositionLayerAlpha;
        private bool themeEnabled = true;
        private static readonly object formLayoutChangedEvent = new object();
        private static bool suppressDeactivation;
        private bool allowMdiBar;
        private DevExpress.Utils.ImageCollection htmlImages;
        private ControlHelper helper;
        private bool allowFormSkin;
        private FormPainter painter;
        private Form prevMdiChild;
        private Size minimumClientSize;
        private Size maximumClientSize;
        private string textMdiTab;
        private FormShowStrategyBase showingStrategy;
        private bool showMdiChildCaptionInParentTitle;
        private LayoutEventArgs dummyArgsCore;
        private WeakReference delayedLayoutEventArgsRef;
        private int lockFormLayoutEvent;
        private int isInitializing;
        private bool forceInitialized;
        private Color activeGlowColorCore;
        private Color inactiveGlowColorCore;
        private DevExpress.Utils.FormShadow.FormShadow formShadowCore;
        private PropertyInfo piLayout;
        private FieldInfo fiLayoutSuspendCount;
        private bool shouldUpdateLookAndFeelOnResumeLayout;
        private FieldInfo formStateCoreField;
        private FieldInfo formStateWindowActivated;
        protected bool boundsUpdated;
        protected bool lockDrawingClientBackground;
        protected bool isFormPainted;
        private bool clientSizeSet;
        private bool inScaleControl;
        internal bool creatingHandle;
        private int lockRedraw;
        private bool isWindowActive;
        private DevExpress.XtraEditors.XtraForm suspendRedrawMdiParent;
        private bool delayedFontChanged;
        private int lockTextChanged;
        private object currentPainterType;
        private bool enableAcrylicAccent;
        private bool useBaseFont;
        private AppearanceDefault defaultAppearance;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool SuppressDeactivation
        {
            get
            {
                return DevExpress.XtraEditors.XtraForm.suppressDeactivation;
            }
            set
            {
                if (value)
                {
                    foreach (Form openForm in (ReadOnlyCollectionBase)Application.OpenForms)
                    {
                        if (openForm is DevExpress.XtraEditors.XtraForm xtraForm)
                            xtraForm.IsActiveSaved = xtraForm.IsActive;
                    }
                }
                DevExpress.XtraEditors.XtraForm.suppressDeactivation = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static Form DeactivatedForm { get; set; }

        static XtraForm()
        {
            GHAssemblyResolverEx.Init();
        }

        public XtraForm()
        {
            this.htmlImages = (DevExpress.Utils.ImageCollection)null;
            this.allowFormSkin = true;
            this.CheckRightToLeft();
            this.helper = this.CreateHelper();
            this.OnAppearance_Changed((object)this.helper);
            this.UpdateSkinPainter();
            this.minimumClientSize = Size.Empty;
            this.maximumClientSize = Size.Empty;
            this.showingStrategy = this.CreateShowingStrategy();
        }

        internal bool IsActiveSaved { get; set; }

        protected internal bool IsActive
        {
            get
            {
                return ((BitVector32)this.FormStateCoreField.GetValue((object)this))[(BitVector32.Section)this.FormStateWindowActivatedField.GetValue((object)this)] == 1;
            }
        }

        [DefaultValue(false)]
        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("XtraFormShowMdiChildCaptionInParentTitle")]
        public virtual bool ShowMdiChildCaptionInParentTitle
        {
            get
            {
                return this.showMdiChildCaptionInParentTitle;
            }
            set
            {
                if (this.ShowMdiChildCaptionInParentTitle == value)
                    return;
                this.showMdiChildCaptionInParentTitle = value;
                this.OnShowMdiChildCaptionInParentTitle();
            }
        }

        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("XtraFormMdiChildCaptionFormatString")]
        [DefaultValue("{1} - {0}")]
        public virtual string MdiChildCaptionFormatString
        {
            get
            {
                return this.mdiChildCaptionFormatString;
            }
            set
            {
                if (this.MdiChildCaptionFormatString == value)
                    return;
                this.mdiChildCaptionFormatString = value;
                this.OnShowMdiChildCaptionInParentTitle();
            }
        }

        protected virtual void OnShowMdiChildCaptionInParentTitle()
        {
            if (this.FormPainter != null)
                this.FormPainter.ForceFrameChanged();
            this.Refresh();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public event LayoutEventHandler FormLayoutChanged
        {
            add
            {
                this.Events.AddHandler(DevExpress.XtraEditors.XtraForm.formLayoutChangedEvent, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(DevExpress.XtraEditors.XtraForm.formLayoutChangedEvent, (Delegate)value);
            }
        }

        private LayoutEventArgs GetDummyArgs()
        {
            if (this.dummyArgsCore == null)
                this.dummyArgsCore = new LayoutEventArgs((Control)this, string.Empty);
            return this.dummyArgsCore;
        }

        private LayoutEventArgs GetDelayedLayoutEventArgs()
        {
            return this.delayedLayoutEventArgsRef == null || !(this.delayedLayoutEventArgsRef.Target is LayoutEventArgs target) ? this.GetDummyArgs() : target;
        }

        private void ResumeFormLayoutEvent(bool raiseEvent)
        {
            if (--this.lockFormLayoutEvent != 0 || !raiseEvent)
                return;
            this.RaiseFormLayoutChanged(this.GetDelayedLayoutEventArgs());
        }

        private void SuspendFormLayoutEvent()
        {
            ++this.lockFormLayoutEvent;
        }

        protected void RaiseFormLayoutChanged(LayoutEventArgs e)
        {
            if (this.lockFormLayoutEvent != 0)
            {
                this.delayedLayoutEventArgsRef = new WeakReference((object)e);
            }
            else
            {
                LayoutEventHandler layoutEventHandler = (LayoutEventHandler)this.Events[DevExpress.XtraEditors.XtraForm.formLayoutChangedEvent];
                if (layoutEventHandler == null)
                    return;
                layoutEventHandler((object)this, e);
            }
        }

        protected virtual void CheckRightToLeft()
        {
            this.RightToLeft = WindowsFormsSettings.GetRightToLeft((Control)this);
            this.RightToLeftLayout = WindowsFormsSettings.GetIsRightToLeftLayout((Control)this);
        }

        protected virtual ControlHelper CreateHelper()
        {
            return (ControlHelper)new FormControlHelper((IDXControl)this, true);
        }

        protected FormShowStrategyBase ShowingStrategy
        {
            get
            {
                return this.showingStrategy;
            }
        }

        protected virtual FormShowStrategyBase CreateShowingStrategy()
        {
            return FormShowStrategyBase.Create(this, this.ShowMode);
        }

        protected virtual bool InvisibleDueShowingStrategy
        {
            get
            {
                return this.showingStrategy is AfterInitializationFormShowStrategy && this.ShowingStrategy.AllowRestore;
            }
        }

        public static bool CheckInvisibleDueShowingStrategy(DevExpress.XtraEditors.XtraForm form)
        {
            return form != null && form.InvisibleDueShowingStrategy;
        }

        protected virtual FormShowMode ShowMode
        {
            get
            {
                return FormShowMode.Default;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!this.ShowingStrategy.AllowRestore)
                return;
            this.ShowingStrategy.Restore();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            if (this.painter != null)
                this.painter.Dispose();
            this.painter = (FormPainter)null;
            if (this.helper != null)
                this.helper.Dispose();
            this.helper = (ControlHelper)null;
            this.ReleaseFormShadow();
        }

        protected internal virtual bool ShouldShowMdiBar
        {
            get
            {
                return this.IsMdiContainer && this.ActiveMdiChild != null && (this.ActiveMdiChild.WindowState == FormWindowState.Maximized && this.MainMenuStrip == null) && this.Menu == null && this.AllowMdiBar;
            }
        }

        protected bool IsInitializing
        {
            get
            {
                if (this.forceInitialized)
                    return false;
                return this.isInitializing != 0 || this.IsLayoutSuspendedCore;
            }
        }

        protected bool IsApplyingLocalizableResourcesInDesignTime
        {
            get
            {
                return this.DesignMode && StackTraceHelper.CheckStackFrame("ApplyResources", typeof(ComponentResourceManager));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;
                if (value == null)
                    return;
                if (value.DesignMode)
                    Utility.IsDesignMode = true;
                ((UserLookAndFeelDefault)UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx((IComponent)this);
            }
        }

        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("XtraFormFormBorderEffect")]
        [DefaultValue(FormBorderEffect.Default)]
        public FormBorderEffect FormBorderEffect
        {
            get
            {
                return this.formBorderEffect;
            }
            set
            {
                if (this.formBorderEffect == value)
                    return;
                this.formBorderEffect = value;
                this.OnFormBorderEffectChanged();
            }
        }

        private void OnFormBorderEffectChanged()
        {
            if (this.IsInitializing || this.DesignMode)
                return;
            this.UpdateFormBorderEffect();
        }

        [DevExpressUtilsLocalizedDescription("XtraFormActiveGlowColor")]
        [DXCategory("Appearance")]
        public Color ActiveGlowColor
        {
            get
            {
                return this.activeGlowColorCore;
            }
            set
            {
                if (this.ActiveGlowColor == value)
                    return;
                this.activeGlowColorCore = value;
                this.OnActiveGlowColorChanged();
            }
        }

        private bool ShouldSerializeActiveGlowColor()
        {
            return !this.ActiveGlowColor.IsEmpty;
        }

        private void ResetActiveGlowColor()
        {
            this.ActiveGlowColor = Color.Empty;
        }

        [DevExpressUtilsLocalizedDescription("XtraFormInactiveGlowColor")]
        [DXCategory("Appearance")]
        public Color InactiveGlowColor
        {
            get
            {
                return this.inactiveGlowColorCore;
            }
            set
            {
                if (this.InactiveGlowColor == value)
                    return;
                this.inactiveGlowColorCore = value;
                this.OnInactiveGlowColorChanged();
            }
        }

        private bool ShouldSerializeInactiveGlowColor()
        {
            return !this.InactiveGlowColor.IsEmpty;
        }

        private void ResetInactiveGlowColor()
        {
            this.InactiveGlowColor = Color.Empty;
        }

        protected virtual void OnActiveGlowColorChanged()
        {
            if (this.IsInitializing || this.DesignMode || this.FormShadow == null)
                return;
            this.FormShadow.ActiveGlowColor = this.ActiveGlowColor;
        }

        protected virtual void OnInactiveGlowColorChanged()
        {
            if (this.IsInitializing || this.DesignMode || this.FormShadow == null)
                return;
            this.FormShadow.InactiveGlowColor = this.InactiveGlowColor;
        }

        private bool IsOnePixelBorder
        {
            get
            {
                return this.FormPainter != null && (double)this.FormPainter.GetSkinFrameLeft().Size.MinSize.Width <= 1.0 * (double)this.GetScaleFactor().Width;
            }
        }

        protected virtual FormBorderEffect GetFormBorderEffect()
        {
            return this.FormBorderEffect == FormBorderEffect.Default ? (this.FormPainter == null || !this.IsOnePixelBorder || this.SuppressFormShadowShowing() ? FormBorderEffect.None : FormBorderEffect.Shadow) : (this.FormBorderEffect != FormBorderEffect.None && this.SuppressFormShadowShowing() ? FormBorderEffect.None : this.FormBorderEffect);
        }

        protected bool SuppressFormShadowShowing()
        {
            return !this.DesignMode && (this.FormBorderStyle == FormBorderStyle.Sizable || this.FormBorderStyle == FormBorderStyle.SizableToolWindow) && (this.FormPainter == null || !this.FormPainter.IsAllowNCDraw) && DevExpress.Utils.Drawing.Helpers.NativeMethods.IsWindow10;
        }

        private void UpdateFormBorderEffect()
        {
            if (this.DesignMode)
                return;
            if (this.GetFormBorderEffect() != FormBorderEffect.None && !this.IsMdiChild && this.Parent == null)
            {
                this.InitFormShadow();
                if (this.Owner == null)
                    return;
                this.FormShadow.SetOwner(this.Owner.Handle);
            }
            else
                this.ReleaseFormShadow();
        }

        protected DevExpress.Utils.FormShadow.FormShadow FormShadow
        {
            get
            {
                return this.formShadowCore;
            }
        }

        private void InitFormShadow()
        {
            if (this.FormShadow == null)
            {
                this.formShadowCore = this.CreateFormShadow();
                this.FormShadow.Form = (Control)this;
            }
            this.FormShadow.BeginUpdate();
            this.FormShadow.ActiveGlowColor = this.activeGlowColorCore;
            this.FormShadow.InactiveGlowColor = this.inactiveGlowColorCore;
            this.FormShadow.IsGlow = this.GetFormBorderEffect() == FormBorderEffect.Glow;
            this.FormShadow.AllowResizeViaShadows = this.IsOnePixelBorder && this.IsSizableWindow;
            this.FormShadow.EndUpdate();
        }

        protected virtual bool IsSizableWindow
        {
            get
            {
                return this.FormBorderStyle == FormBorderStyle.Sizable || this.FormBorderStyle == FormBorderStyle.SizableToolWindow;
            }
        }

        private void ReleaseFormShadow()
        {
            if (this.FormShadow == null)
                return;
            this.FormShadow.Form = (Control)null;
            this.FormShadow.Dispose();
            this.formShadowCore = (DevExpress.Utils.FormShadow.FormShadow)null;
        }

        protected virtual DevExpress.Utils.FormShadow.FormShadow CreateFormShadow()
        {
            return (DevExpress.Utils.FormShadow.FormShadow)new XtraFormShadow();
        }

        public static bool ProcessSmartMouseWheel(Control control, MouseEventArgs e)
        {
            return MouseWheelHelper.ProcessSmartMouseWheel(control, e);
        }

        public static bool IsAllowSmartMouseWheel(Control control, MouseEventArgs e)
        {
            return MouseWheelHelper.IsAllowSmartMouseWheel(control, e);
        }

        public static bool IsAllowSmartMouseWheel(Control control)
        {
            return MouseWheelHelper.IsAllowSmartMouseWheel(control);
        }

        public new void SuspendLayout()
        {
            base.SuspendLayout();
            ++this.isInitializing;
        }

        private void CheckForceLookAndFeelChangedCore()
        {
            if (this.isInitializing != 0 || this.LayoutSuspendCountCore != (byte)1 || !this.shouldUpdateLookAndFeelOnResumeLayout)
                return;
            this.forceInitialized = true;
            try
            {
                this.OnLookAndFeelChangedCore();
            }
            finally
            {
                this.forceInitialized = false;
            }
        }

        public new void ResumeLayout()
        {
            this.ResumeLayout(true);
        }

        public new void ResumeLayout(bool performLayout)
        {
            if (this.isInitializing > 0)
                --this.isInitializing;
            if (this.isInitializing == 0)
            {
                if (this.delayedFontChanged)
                    this.OnFontChanged(EventArgs.Empty);
                this.CheckMinimumSize();
                this.CheckMaximumSize();
                this.CheckForceLookAndFeelChangedCore();
            }
            base.ResumeLayout(performLayout);
            if (this.IsInitializing)
                return;
            this.CheckMinimumSize();
            this.CheckMaximumSize();
        }

        private void CheckMaximumSize()
        {
            if (!this.maximumSize.HasValue)
                return;
            Size size = this.maximumSize.Value;
            if (!size.IsEmpty)
            {
                if (size.Width > 0)
                    size.Width = Math.Max(size.Width, this.Size.Width);
                if (size.Height > 0)
                    size.Height = Math.Max(size.Height, this.Size.Height);
                if (this.minimumSize.HasValue && !this.minimumSize.Value.IsEmpty)
                {
                    if (this.maximumSize.Value.Width == this.minimumSize.Value.Width)
                        size.Width = this.Size.Width;
                    if (this.maximumSize.Value.Height == this.minimumSize.Value.Height)
                        size.Height = this.Size.Height;
                }
            }
            this.maximumSize = new Size?();
            base.MaximumSize = size;
        }

        private void CheckMinimumSize()
        {
            if (!this.minimumSize.HasValue)
                return;
            Size size = this.minimumSize.Value;
            if (!size.IsEmpty)
            {
                if (size.Width > 0)
                    size.Width = Math.Min(size.Width, this.Size.Width);
                if (size.Height > 0)
                    size.Height = Math.Min(size.Height, this.Size.Height);
                if (this.maximumSize.HasValue && !this.maximumSize.Value.IsEmpty)
                {
                    if (this.maximumSize.Value.Width == this.minimumSize.Value.Width)
                        size.Width = this.Size.Width;
                    if (this.maximumSize.Value.Height == this.minimumSize.Value.Height)
                        size.Height = this.Size.Height;
                }
            }
            this.minimumSize = new Size?();
            base.MinimumSize = size;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (this.shouldUpdateLookAndFeelOnResumeLayout)
                this.CheckForceLookAndFeelChangedCore();
            base.OnLayout(levent);
            this.RaiseFormLayoutChanged(levent);
        }

        protected override void ScaleCore(float x, float y)
        {
            this.MaximumClientSize = new Size((int)Math.Round((double)this.MaximumClientSize.Width * (double)x), (int)Math.Round((double)this.MaximumClientSize.Height * (double)y));
            base.ScaleCore(x, y);
            this.MinimumClientSize = new Size((int)Math.Round((double)this.MinimumClientSize.Width * (double)x), (int)Math.Round((double)this.MinimumClientSize.Height * (double)y));
        }

        internal bool IsLayoutSuspendedCore
        {
            get
            {
                if (this.piLayout == (PropertyInfo)null)
                    this.piLayout = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.Instance | BindingFlags.NonPublic);
                return this.piLayout != (PropertyInfo)null && (bool)this.piLayout.GetValue((object)this, (object[])null);
            }
        }

        [Browsable(false)]
        protected internal byte LayoutSuspendCountCore
        {
            get
            {
                if (this.fiLayoutSuspendCount == (FieldInfo)null)
                    this.fiLayoutSuspendCount = typeof(Control).GetField("layoutSuspendCount", BindingFlags.Instance | BindingFlags.NonPublic);
                return this.fiLayoutSuspendCount != (FieldInfo)null ? (byte)this.fiLayoutSuspendCount.GetValue((object)this) : (byte)1;
            }
        }

        protected internal FormPainter FormPainter
        {
            get
            {
                return this.painter;
            }
        }

        Control IDXControl.Control
        {
            get
            {
                return (Control)this;
            }
        }

        void IDXControl.OnLookAndFeelStyleChanged(object sender)
        {
            this.defaultAppearance = (AppearanceDefault)null;
            this.CheckRightToLeft();
            this.OnAppearance_Changed(sender);
            this.OnLookAndFeelChangedCore();
            if (!this.EnableAcrylicAccent)
                return;
            this.SetAcrylicAccent(true);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.defaultAppearance = (AppearanceDefault)null;
            this.OnAppearance_Changed((object)null);
            this.OnFormBorderEffectChanged();
            base.OnParentChanged(e);
        }

        protected internal virtual bool IsRightToLeftCaption
        {
            get
            {
                return this.RightToLeft == RightToLeft.Yes;
            }
        }

        protected internal void SetLayoutDeferred()
        {
            typeof(Control).GetMethod("SetState", BindingFlags.Instance | BindingFlags.NonPublic, (Binder)null, new System.Type[2]
            {
        typeof (int),
        typeof (bool)
            }, (ParameterModifier[])null).Invoke((object)this, new object[2]
            {
        (object) 512,
        (object) true
            });
        }

        private void OnLookAndFeelChangedCore()
        {
            if (this.IsInitializing)
            {
                if (this.Visible && this.IsLayoutSuspendedCore)
                    this.SetLayoutDeferred();
                this.shouldUpdateLookAndFeelOnResumeLayout = true;
            }
            else
            {
                this.shouldUpdateLookAndFeelOnResumeLayout = false;
                bool flag = this.CheckUpdateSkinPainter();
                Size clientSize = this.ClientSize;
                this.OnMinimumClientSizeChanged();
                this.OnMaximumClientSizeChanged();
                FieldInfo field1 = typeof(Form).GetField("restoredWindowBounds", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field2 = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field3 = typeof(Form).GetField("restoreBounds", BindingFlags.Instance | BindingFlags.NonPublic);
                Rectangle rectangle1 = (Rectangle)field1.GetValue((object)this);
                Rectangle rectangle2 = (Rectangle)field3.GetValue((object)this);
                BoundsSpecified boundsSpecified = (BoundsSpecified)field2.GetValue((object)this);
                int width;
                int height;
                this.GetFormStateExWindowBoundsIsClientSize(out width, out height);
                int state = this.SaveFormStateWindowState();
                bool isNormal = this.SaveControlStateNormalState();
                if (flag)
                    this.Size = this.SizeFromClientSize(clientSize);
                if ((boundsSpecified & BoundsSpecified.Width) != BoundsSpecified.None && (boundsSpecified & BoundsSpecified.Height) != BoundsSpecified.None)
                    rectangle2.Size = this.SizeFromClientSize(rectangle1.Size);
                if (this.WindowState != FormWindowState.Normal && this.IsHandleCreated)
                {
                    field1.SetValue((object)this, (object)rectangle1);
                    field3.SetValue((object)this, (object)rectangle2);
                    this.SetFormStateExWindowBoundsIsClientSize(width, height);
                }
                if (this.IsMdiChild)
                {
                    this.RestoreFormStateWindowState(state);
                    this.RestoreControlStateNormalState(isNormal);
                }
                if (!this.IsHandleCreated)
                    return;
                this.UpdateFormBorderEffect();
            }
        }

        private bool SaveControlStateNormalState()
        {
            return ((int)typeof(Control).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)this) & 65536) != 0;
        }

        private void RestoreControlStateNormalState(bool isNormal)
        {
            FieldInfo field = typeof(Control).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
            int num = (int)field.GetValue((object)this);
            field.SetValue((object)this, (object)(isNormal ? num | 65536 : num & -65537));
        }

        private FieldInfo FormStateCoreField
        {
            get
            {
                if (this.formStateCoreField == (FieldInfo)null)
                    this.formStateCoreField = typeof(Form).GetField("formState", BindingFlags.Instance | BindingFlags.NonPublic);
                return this.formStateCoreField;
            }
        }

        private FieldInfo FormStateWindowActivatedField
        {
            get
            {
                if (this.formStateWindowActivated == (FieldInfo)null)
                    this.formStateWindowActivated = typeof(Form).GetField("FormStateIsWindowActivated", BindingFlags.Static | BindingFlags.NonPublic);
                return this.formStateWindowActivated;
            }
        }

        private int SaveFormStateWindowState()
        {
            return ((BitVector32)this.FormStateCoreField.GetValue((object)this))[(BitVector32.Section)typeof(Form).GetField("FormStateWindowState", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object)this)];
        }

        private void RestoreFormStateWindowState(int state)
        {
            BitVector32.Section index = (BitVector32.Section)typeof(Form).GetField("FormStateWindowState", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object)this);
            BitVector32 bitVector32 = (BitVector32)this.FormStateCoreField.GetValue((object)this);
            bitVector32[index] = state;
            this.FormStateCoreField.SetValue((object)this, (object)bitVector32);
        }

        private void GetFormStateExWindowBoundsIsClientSize(out int width, out int height)
        {
            FieldInfo field1 = typeof(Form).GetField("formStateEx", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo field3 = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.Static | BindingFlags.NonPublic);
            BitVector32.Section index1 = (BitVector32.Section)field2.GetValue((object)this);
            BitVector32.Section index2 = (BitVector32.Section)field3.GetValue((object)this);
            BitVector32 bitVector32 = (BitVector32)field1.GetValue((object)this);
            width = bitVector32[index1];
            height = bitVector32[index2];
        }

        private void SetFormStateExWindowBoundsIsClientSize(int width, int height)
        {
            FieldInfo field1 = typeof(Form).GetField("formStateEx", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo field3 = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.Static | BindingFlags.NonPublic);
            BitVector32.Section index1 = (BitVector32.Section)field2.GetValue((object)this);
            BitVector32.Section index2 = (BitVector32.Section)field3.GetValue((object)this);
            BitVector32 bitVector32 = (BitVector32)field1.GetValue((object)this);
            bitVector32[index1] = width;
            bitVector32[index2] = height;
            field1.SetValue((object)this, (object)bitVector32);
        }

        private bool IsFormStateClientSizeSet()
        {
            return ((BitVector32)this.FormStateCoreField.GetValue((object)this))[(BitVector32.Section)typeof(Form).GetField("FormStateSetClientSize", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object)this)] == 1;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.FormPainter != null && this.FormBorderStyle != FormBorderStyle.None && (this.ControlBox || !string.IsNullOrEmpty(this.Text)) && this.WindowState == FormWindowState.Normal)
                this.PatchClientSize();
            else if (this.ShouldPatchClientSize && this.WindowState != FormWindowState.Minimized)
                this.PatchClientSize();
            if (!this.IsRibbonForm)
                this.CalcFormBounds();
            base.OnSizeChanged(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.OnMinimumClientSizeChanged();
            this.OnMaximumClientSizeChanged();
            this.UpdateFormBorderEffect();
            if (this.IsHandleCreated)
                this.BeginInvoke((Delegate)(() => TouchKeyboardSupport.CheckEnableTouchSupport((Form)this)));
            this.CalcFormBounds();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TextMdiTab
        {
            get
            {
                return this.textMdiTab;
            }
            set
            {
                if (this.textMdiTab == value)
                    return;
                this.textMdiTab = value;
                this.OnTextMdiTabChanged();
            }
        }

        protected virtual void OnTextMdiTabChanged()
        {
            if (this.MdiParent == null)
                return;
            this.OnTextChanged(EventArgs.Empty);
        }

        protected Size ConstrainMinimumClientSize(Size value)
        {
            value.Width = Math.Max(0, value.Width);
            value.Height = Math.Max(0, value.Height);
            return value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MinimumClientSize
        {
            get
            {
                return this.minimumClientSize;
            }
            set
            {
                value = this.ConstrainMinimumClientSize(value);
                if (this.MinimumClientSize == value)
                    return;
                this.minimumClientSize = value;
                this.OnMinimumClientSizeChanged();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MaximumClientSize
        {
            get
            {
                return this.maximumClientSize;
            }
            set
            {
                if (this.MaximumClientSize == value)
                    return;
                this.maximumClientSize = value;
                this.OnMaximumClientSizeChanged();
            }
        }

        protected bool InMinimumClientSizeChanged { get; set; }

        protected virtual void OnMinimumClientSizeChanged()
        {
            if (this.IsInitializing)
                return;
            if (this.InMinimumClientSizeChanged)
                return;
            try
            {
                this.InMinimumClientSizeChanged = true;
                this.MinimumSize = this.GetConstrainSize(this.MinimumClientSize);
            }
            finally
            {
                this.InMinimumClientSizeChanged = false;
            }
        }

        protected bool InMaximumClientSizeChanged { get; set; }

        protected virtual void OnMaximumClientSizeChanged()
        {
            if (this.IsInitializing)
                return;
            if (this.InMaximumClientSizeChanged)
                return;
            try
            {
                this.InMaximumClientSizeChanged = true;
                this.MaximumSize = this.GetConstrainSize(this.MaximumClientSize);
            }
            finally
            {
                this.InMaximumClientSizeChanged = false;
            }
        }

        protected virtual Size GetConstrainSize(Size clientSize)
        {
            return clientSize == Size.Empty ? Size.Empty : this.SizeFromClientSize(clientSize);
        }

        protected virtual Size ClientSizeFromSize(Size formSize)
        {
            if (formSize == Size.Empty)
                return Size.Empty;
            Size size1 = this.SizeFromClientSize(Size.Empty);
            Size size2 = new Size(formSize.Width - size1.Width, formSize.Height - size1.Height);
            if (this.WindowState != FormWindowState.Maximized)
                return size2;
            DevExpress.Utils.Drawing.Helpers.NativeMethods.RECT rect = new DevExpress.Utils.Drawing.Helpers.NativeMethods.RECT(0, 0, size2.Width, size2.Height);
            if (this.painter != null)
                Taskbar.CorrectRECTByAutoHide(ref rect);
            return new Size(rect.Right, rect.Bottom);
        }

        [Localizable(true)]
        [DevExpressUtilsLocalizedDescription("XtraFormMinimumSize")]
        public override Size MinimumSize
        {
            get
            {
                return this.inScaleControl && this.minimumSize.HasValue ? this.minimumSize.Value : base.MinimumSize;
            }
            set
            {
                this.minimumSize = new Size?(value);
                if (this.IsInitializing || this.IsApplyingLocalizableResourcesInDesignTime)
                    return;
                Size maximumSize = this.MaximumSize;
                base.MinimumSize = value;
                if (!(maximumSize != this.MaximumSize))
                    return;
                this.MaximumClientSize = this.ClientSizeFromSize(this.MaximumSize);
            }
        }

        [DevExpressUtilsLocalizedDescription("XtraFormMaximumSize")]
        [Localizable(true)]
        public override Size MaximumSize
        {
            get
            {
                return this.inScaleControl && this.maximumSize.HasValue ? this.maximumSize.Value : base.MaximumSize;
            }
            set
            {
                this.maximumSize = new Size?(value);
                if (this.IsInitializing || this.IsApplyingLocalizableResourcesInDesignTime)
                    return;
                Size minimumSize = this.MinimumSize;
                base.MaximumSize = value;
                if (!(this.MinimumSize != minimumSize))
                    return;
                this.MinimumClientSize = this.ClientSizeFromSize(this.MinimumSize);
            }
        }

        protected override void OnMinimumSizeChanged(EventArgs e)
        {
            base.OnMinimumSizeChanged(e);
            this.MinimumClientSize = this.ClientSizeFromSize(this.MinimumSize);
        }

        private bool InMaximumSizeChanged { get; set; }

        protected override void OnMaximumSizeChanged(EventArgs e)
        {
            if (this.InMaximumSizeChanged)
                return;
            this.InMaximumSizeChanged = true;
            try
            {
                base.OnMaximumSizeChanged(e);
                this.MaximumClientSize = this.ClientSizeFromSize(this.MaximumSize);
            }
            finally
            {
                this.InMaximumSizeChanged = false;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual bool SupportAdvancedTitlePainting
        {
            get
            {
                return true;
            }
        }

        protected bool CheckUpdateSkinPainter()
        {
            SkinPaddingEdges skinPaddingEdges = (SkinPaddingEdges)null;
            Size clientSize = this.ClientSize;
            if (this.FormPainter != null && this.IsRibbonForm && (this.DesignMode && this.IsInitializing))
                skinPaddingEdges = this.FormPainter.Margins;
            bool flag = this.UpdateSkinPainter();
            if (skinPaddingEdges != null && this.FormPainter != null && !object.Equals((object)skinPaddingEdges, (object)this.FormPainter.Margins))
                this.ClientSize = clientSize;
            if (this.IsHandleCreated)
            {
                if (this.painter != null)
                    this.painter.ResetDefaultAppearance();
                if (flag)
                {
                    if (this.FormPainter == null || !this.FormPainter.IsAllowNCDraw)
                        this.EnableTheme();
                    else
                        this.DisableTheme();
                    if (!WXPPainter.Default.ThemesEnabled)
                    {
                        this.RecreateHandle();
                    }
                    else
                    {
                        FormPainter.ForceFrameChanged((Control)this);
                        this.ForceRefresh();
                    }
                    this.OnSkinPainterChanged();
                }
                if (this.FormPainter == null)
                    this.ForceRefresh();
            }
            return flag;
        }

        protected virtual void ForceRefresh()
        {
            this.Refresh();
        }

        protected virtual void OnSkinPainterChanged()
        {
        }

        void IDXControl.OnAppearanceChanged(object sender)
        {
            this.OnAppearance_Changed(sender);
        }

        bool ISupportLookAndFeel.IgnoreChildren
        {
            get
            {
                return false;
            }
        }

        protected void UpdateClientBackgroundBounds(int msg)
        {
            if (msg != 133 || this.WindowState == FormWindowState.Normal)
                return;
            this.CalcFormBounds();
        }

        protected virtual int ClientBackgroundTop
        {
            get
            {
                return 0;
            }
        }

        protected internal virtual bool IsRibbonForm
        {
            get
            {
                return false;
            }
        }

        private Rectangle GetFormXAddedRectangle()
        {
            return this.formBounds.Width < this.prevFormBounds.Width ? Rectangle.Empty : new Rectangle(this.prevFormBounds.Width, this.ClientBackgroundTop, Math.Abs(this.formBounds.Width - this.prevFormBounds.Width), Math.Abs(this.formBounds.Height));
        }

        private Rectangle GetFormYAddedRectangle()
        {
            return this.formBounds.Height < this.prevFormBounds.Height ? Rectangle.Empty : new Rectangle(0, this.prevFormBounds.Height, Math.Abs(this.formBounds.Width), Math.Abs(this.formBounds.Height - this.prevFormBounds.Height));
        }

        protected virtual void DrawClientBackgroundCore(GraphicsCache cache)
        {
            SkinElement element = CommonSkins.GetSkin(this.FormPainter.Provider)[CommonSkins.SkinForm];
            SkinElementInfo skinElementInfo1 = new SkinElementInfo(element, this.GetFormXAddedRectangle());
            ObjectPainter.DrawObject(cache, (ObjectPainter)SkinElementPainter.Default, (ObjectInfoArgs)skinElementInfo1);
            SkinElementInfo skinElementInfo2 = new SkinElementInfo(element, this.GetFormYAddedRectangle());
            ObjectPainter.DrawObject(cache, (ObjectPainter)SkinElementPainter.Default, (ObjectInfoArgs)skinElementInfo2);
        }

        protected bool IsRegionPainted
        {
            get
            {
                return this.FormPainter != null && this.FormPainter.RegionPainted;
            }
        }

        protected void CalcClippedRegion(Graphics g)
        {
            if (this.IsRegionPainted || this.Region == null)
                return;
            RectangleF bounds = this.Region.GetBounds(g);
            this.formBounds.Width = Math.Min(this.formBounds.Width, (int)((double)bounds.X + (double)bounds.Width));
            this.formBounds.Height = Math.Min(this.formBounds.Height, (int)((double)bounds.Y + (double)bounds.Height));
        }

        protected virtual void DrawClientBackground(Message msg)
        {
            this.boundsUpdated = false;
            if (this.FormPainter == null || this.formBounds == this.prevFormBounds)
                return;
            IntPtr dc = this.FormPainter.GetDC(msg);
            using (Graphics g = Graphics.FromHdc(dc))
            {
                using (GraphicsCache cache = new GraphicsCache(g))
                    this.DrawClientBackgroundCore(cache);
                this.CalcClippedRegion(g);
            }
            DevExpress.Utils.Drawing.Helpers.NativeMethods.ReleaseDC(this.Handle, dc);
        }

        protected bool CanDrawClientBackground
        {
            get
            {
                return (this.boundsUpdated || !this.IsRegionPainted) && !this.isFormPainted;
            }
        }

        protected virtual bool ShouldDrawClientBackground(int msg)
        {
            if (msg == 133)
                this.lockDrawingClientBackground = this.WindowState == FormWindowState.Maximized;
            if (!this.CanDrawClientBackground || this.IsMdiChild || this.lockDrawingClientBackground)
                return false;
            if (msg == 71 && this.WindowState == FormWindowState.Maximized)
            {
                this.lockDrawingClientBackground = true;
                return true;
            }
            return msg == 133 && this.WindowState != FormWindowState.Maximized;
        }

        protected internal bool IsMinimizedState(Rectangle bounds)
        {
            return this.WindowState == FormWindowState.Minimized && bounds.Location == this.minimizedFormLocation;
        }

        protected internal void CalcFormBounds()
        {
            if (this.IsMdiChild || !this.IsHandleCreated)
                return;
            DevExpress.Utils.Drawing.Helpers.NativeMethods.RECT lpRect = new DevExpress.Utils.Drawing.Helpers.NativeMethods.RECT();
            DevExpress.Utils.Drawing.Helpers.NativeMethods.GetWindowRect(this.Handle, ref lpRect);
            Rectangle bounds = lpRect.ToRectangle();
            if (this.IsMinimizedState(bounds))
                bounds = Rectangle.Empty;
            if (this.formBounds == bounds && (this.boundsUpdated || !this.IsRegionPainted))
                return;
            this.isFormPainted = false;
            this.prevFormBounds = this.formBounds;
            this.formBounds = bounds;
            if (!(this.prevFormBounds != this.formBounds))
                return;
            this.boundsUpdated = true;
        }

        protected override void WndProc(ref Message msg)
        {
            this.UpdateClientBackgroundBounds(msg.Msg);
            if (this.ShouldDrawClientBackground(msg.Msg))
            {
                this.DrawClientBackground(msg);
                if (this.FormPainter != null)
                    this.FormPainter.RegionPainted = true;
            }
            if (msg.Msg == 133 || msg.Msg == 15 || msg.Msg == 6)
                this.isFormPainted = true;
            if ((msg.Msg == 6 || msg.Msg == 134) && DevExpress.XtraEditors.XtraForm.SuppressDeactivation)
                msg.WParam = new IntPtr(1);
            if (msg.Msg == 526)
            {
                DXMouseEventArgs mouseHwheel = ControlHelper.GenerateMouseHWheel(ref msg, (Control)this);
                this.OnMouseWheel((MouseEventArgs)mouseHwheel);
                if (mouseHwheel.Handled)
                {
                    msg.Result = new IntPtr(1);
                    return;
                }
            }
            if (msg.Msg == 798)
                this.CheckUpdateSkinPainter();
            if (this.painter != null)
            {
                if (this.painter.DoWndProc(ref msg))
                {
                    if (!this.ShouldUpdateFrame(msg))
                        return;
                    FormPainter.ForceFrameChanged((Control)this);
                }
                else
                {
                    if (msg.Msg == 3 && this.FormPainter != null && (this.MdiParent == null && this.WindowState != FormWindowState.Maximized))
                        this.ShouldPatchClientSize = true;
                    if (msg.Msg == 71 && this.FormPainter != null && (this.MdiParent == null && this.WindowState != FormWindowState.Maximized))
                        this.ShouldPatchClientSize = true;
                    if (this.NeedActivateForm && msg.Msg == 133)
                        this.ForceActivateWindow(msg);
                    base.WndProc(ref msg);
                    CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, (object)this);
                }
            }
            else
            {
                base.WndProc(ref msg);
                this.ShouldPatchClientSize = false;
                CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, (object)this);
            }
        }

        protected bool ShouldUpdateFrame(Message msg)
        {
            return msg.Msg == 5 && this.IsRibbonForm && this.WindowState == FormWindowState.Maximized;
        }

        protected virtual bool NeedActivateForm
        {
            get
            {
                return false;
            }
        }

        protected virtual void ForceActivateWindow(Message msg)
        {
            this.ActivateWindow(msg.HWnd, 134, msg.WParam, msg.LParam);
        }

        [SecuritySafeCritical]
        protected void ActivateWindow(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            DevExpress.XtraEditors.XtraForm.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr DefWindowProc(
          IntPtr hWnd,
          int msg,
          IntPtr wParam,
          IntPtr lParam);

        private void DoBase(ref Message msg)
        {
            base.WndProc(ref msg);
        }

        protected bool IsCustomPainter
        {
            get
            {
                return this.painter != null;
            }
        }

        private bool ShouldPatchClientSize { get; set; }

        private Size SavedClientSize { get; set; }

        protected override void OnStyleChanged(EventArgs e)
        {
            if (this.FormPainter != null)
                this.ShouldPatchClientSize = true;
            this.SavedClientSize = this.ClientSize;
            try
            {
                if (this.UpdateSkinPainter())
                {
                    if (this.clientSizeSet)
                    {
                        this.ClientSize = this.ClientSize;
                        this.clientSizeSet = false;
                    }
                }
            }
            finally
            {
                this.ShouldPatchClientSize = false;
            }
            base.OnStyleChanged(e);
        }

        protected override void SetClientSizeCore(int x, int y)
        {
            this.clientSizeSet = false;
            this.UpdateSkinPainter();
            FieldInfo field1 = typeof(Control).GetField("clientWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Control).GetField("clientHeight", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field3 = typeof(Form).GetField("FormStateSetClientSize", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo field4 = typeof(Form).GetField("formState", BindingFlags.Instance | BindingFlags.NonPublic);
            if (this.IsCustomPainter && field1 != (FieldInfo)null && (field2 != (FieldInfo)null && field4 != (FieldInfo)null) && field3 != (FieldInfo)null)
            {
                this.clientSizeSet = true;
                this.Size = this.SizeFromClientSize(new Size(x, y));
                field1.SetValue((object)this, (object)x);
                field2.SetValue((object)this, (object)y);
                BitVector32.Section index = (BitVector32.Section)field3.GetValue((object)this);
                BitVector32 bitVector32 = (BitVector32)field4.GetValue((object)this);
                bitVector32[index] = 1;
                field4.SetValue((object)this, (object)bitVector32);
                this.OnClientSizeChanged(EventArgs.Empty);
                bitVector32[index] = 0;
                field4.SetValue((object)this, (object)bitVector32);
            }
            else
                base.SetClientSizeCore(x, y);
        }

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);
            if (this.IsCustomPainter)
                this.BeginInvoke((Delegate)new MethodInvoker(this.DrawMenuBarCore));
            if (this.prevMdiChild != null)
                this.prevMdiChild.TextChanged -= new EventHandler(this.OnMdiChildTextChanged);
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.TextChanged += new EventHandler(this.OnMdiChildTextChanged);
            this.prevMdiChild = this.ActiveMdiChild;
            if (this.IsCustomPainter)
                this.OnTextChanged(e);
            if (this.ActiveMdiChild != null)
                return;
            FormPainter.UpdateMdiClient(this.GetMdiClient());
        }

        private void DrawMenuBarCore()
        {
            DevExpress.Utils.Drawing.Helpers.NativeMethods.DrawMenuBar(this.Handle);
        }

        protected virtual void OnMdiChildTextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(e);
            FormPainter.InvalidateNC((Control)this);
        }

        protected override Size SizeFromClientSize(Size clientSize)
        {
            return !this.ControlBox && !this.AllowSkinForEmptyText && (this.Text == "" || this.Text == null) || this.painter == null ? base.SizeFromClientSize(clientSize) : this.painter.CalcSizeFromClientSize(clientSize);
        }

        protected override Rectangle GetScaledBounds(
          Rectangle bounds,
          SizeF factor,
          BoundsSpecified specified)
        {
            Rectangle scaledBounds = base.GetScaledBounds(bounds, factor, specified);
            if (!this.IsCustomPainter)
                return scaledBounds;
            Size size = this.SizeFromClientSize(Size.Empty);
            if (!this.GetStyle(ControlStyles.FixedWidth) && (specified & BoundsSpecified.Width) != BoundsSpecified.None)
            {
                int num = bounds.Width - size.Width;
                scaledBounds.Width = (int)Math.Round((double)num * (double)factor.Width) + size.Width;
            }
            if (!this.GetStyle(ControlStyles.FixedHeight) && (specified & BoundsSpecified.Height) != BoundsSpecified.None)
            {
                int num = bounds.Height - size.Height;
                scaledBounds.Height = (int)Math.Round((double)num * (double)factor.Height) + size.Height;
            }
            return scaledBounds;
        }

        protected internal SizeF GetScaleFactor()
        {
            return this.scaleFactor;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            this.inScaleControl = true;
            if (!this.IsCustomPainter)
            {
                base.ScaleControl(factor, specified);
                this.inScaleControl = false;
            }
            else
            {
                Size size1 = this.MinimumSize;
                Size size2 = this.MaximumSize;
                Size size3 = Size.Empty;
                try
                {
                    this.ShouldPatchClientSize = true;
                    this.Size = this.SizeFromClientSize(this.ClientSize);
                    this.ShouldPatchClientSize = true;
                    size3 = this.SizeFromClientSize(Size.Empty);
                    base.ScaleControl(factor, specified);
                }
                finally
                {
                    this.ShouldPatchClientSize = false;
                }
                if (size1 != Size.Empty)
                {
                    Size size4 = size1 - size3;
                    size1 = new Size((int)Math.Round((double)size4.Width * (double)factor.Width), (int)Math.Round((double)size4.Height * (double)factor.Height)) + size3;
                }
                if (size2 != Size.Empty)
                {
                    Size size4 = size2 - size3;
                    size2 = new Size((int)Math.Round((double)size4.Width * (double)factor.Width), (int)Math.Round((double)size4.Height * (double)factor.Height)) + size3;
                }
                this.MinimumSize = size1;
                this.MaximumSize = size2;
                this.inScaleControl = false;
            }
        }

        protected virtual Size PatchFormSizeInRestoreWindowBoundsIfNecessary(int width, int height)
        {
            if (this.WindowState == FormWindowState.Normal && this.WmWindowPosChangedCount > 0)
            {
                if (this.IsCustomPainter)
                {
                    try
                    {
                        if (((BoundsSpecified)typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)this) & BoundsSpecified.Size) != BoundsSpecified.None)
                        {
                            FieldInfo field1 = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.Static | BindingFlags.NonPublic);
                            FieldInfo field2 = typeof(Form).GetField("formStateEx", BindingFlags.Instance | BindingFlags.NonPublic);
                            FieldInfo field3 = typeof(Form).GetField("restoredWindowBounds", BindingFlags.Instance | BindingFlags.NonPublic);
                            if (field1 != (FieldInfo)null)
                            {
                                if (field2 != (FieldInfo)null)
                                {
                                    if (field3 != (FieldInfo)null)
                                    {
                                        Rectangle rectangle = (Rectangle)field3.GetValue((object)this);
                                        BitVector32.Section index = (BitVector32.Section)field1.GetValue((object)this);
                                        if (((BitVector32)field2.GetValue((object)this))[index] == 1)
                                        {
                                            width = rectangle.Width + this.painter.Margins.Left + this.painter.Margins.Right;
                                            height = rectangle.Height + this.painter.Margins.Top + this.painter.Margins.Bottom;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return new Size(width, height);
        }

        protected override void SetBoundsCore(
          int x,
          int y,
          int width,
          int height,
          BoundsSpecified specified)
        {
            Size size = this.CalcPreferredSizeCore(this.PatchFormSizeInRestoreWindowBoundsIfNecessary(width, height));
            base.SetBoundsCore(x, y, size.Width, size.Height, specified);
        }

        protected virtual Size CalcPreferredSizeCore(Size size)
        {
            return size;
        }

        protected override void CreateHandle()
        {
            bool flag = !this.creatingHandle;
            this.creatingHandle = true;
            if (flag && this.WindowState != FormWindowState.Minimized && !this.DesignMode)
                this.Size = this.SizeFromClientSize(this.ClientSize);
            if (!this.IsHandleCreated)
                base.CreateHandle();
            this.creatingHandle = false;
        }

        internal bool IsMaximizedBoundsSet
        {
            get
            {
                return !this.MaximumSize.IsEmpty || !this.MaximizedBounds.IsEmpty;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool IsSuspendRedraw
        {
            get
            {
                return this.lockRedraw != 0;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SuspendRedraw()
        {
            if (this.lockRedraw++ == 0)
                this.isWindowActive = this.painter != null && this.painter.IsWindowActive;
            this.suspendRedrawMdiParent = this.MdiParent as DevExpress.XtraEditors.XtraForm;
            if (this.suspendRedrawMdiParent == null)
                return;
            this.suspendRedrawMdiParent.SuspendRedraw();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResumeRedraw()
        {
            if (this.lockRedraw < 1)
                return;
            if (--this.lockRedraw == 0 && this.painter != null && this.painter.IsWindowActive != this.isWindowActive)
                this.painter.InvalidateNC();
            if (this.suspendRedrawMdiParent == null)
                return;
            this.suspendRedrawMdiParent.ResumeRedraw();
            if (this.suspendRedrawMdiParent.lockRedraw != 0)
                return;
            this.suspendRedrawMdiParent = (DevExpress.XtraEditors.XtraForm)null;
        }

        protected ControlHelper Helper
        {
            get
            {
                return this.helper;
            }
            set
            {
                if (this.Helper == value)
                    return;
                this.helper = value;
                this.OnHelperChanged();
            }
        }

        private void OnHelperChanged()
        {
            this.OnAppearance_Changed((object)this.Helper);
        }

        private bool ShouldSerializeAppearance()
        {
            return this.Appearance.ShouldSerialize();
        }

        private void ResetAppearance()
        {
            this.Appearance.Reset();
        }

        [Category("Appearance")]
        [DevExpressUtilsLocalizedDescription("XtraFormAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AppearanceObject Appearance
        {
            get
            {
                return this.Helper != null ? this.Helper.Appearance : AppearanceObject.Dummy;
            }
        }

        private bool ShouldSerializeLookAndFeel()
        {
            return this.LookAndFeel.ShouldSerialize();
        }

        [DevExpressUtilsLocalizedDescription("XtraFormLookAndFeel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        public FormUserLookAndFeel LookAndFeel
        {
            get
            {
                return this.Helper != null ? (FormUserLookAndFeel)this.Helper.LookAndFeel : (FormUserLookAndFeel)null;
            }
        }

        protected void OnAppearance_Changed(object sender)
        {
            this.defaultAppearance = (AppearanceDefault)null;
            if (this.IsHandleCreated)
            {
                this.OnBackColorChanged(EventArgs.Empty);
                this.Invalidate();
            }
            if (base.Font.Equals((object)this.GetFont()))
                return;
            this.useBaseFont = true;
            base.Font = this.GetFont();
            this.useBaseFont = false;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            if (this.isInitializing > 0)
                this.delayedFontChanged = true;
            this.delayedFontChanged = false;
            base.OnFontChanged(e);
        }

        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("XtraFormAllowMdiBar")]
        [DefaultValue(false)]
        public virtual bool AllowMdiBar
        {
            get
            {
                return this.allowMdiBar;
            }
            set
            {
                if (this.allowMdiBar == value)
                    return;
                this.allowMdiBar = value;
                if (this.FormPainter == null || !this.IsHandleCreated)
                    return;
                this.FormPainter.ForceFrameChanged();
                this.Refresh();
            }
        }

        [DevExpressUtilsLocalizedDescription("XtraFormCloseBox")]
        [DefaultValue(true)]
        [Browsable(false)]
        public virtual bool CloseBox
        {
            get
            {
                return this.closeBox;
            }
            set
            {
                if (this.CloseBox == value)
                    return;
                this.closeBox = value;
                if (this.FormPainter == null || !this.IsHandleCreated)
                    return;
                this.FormPainter.ForceFrameChanged();
                this.Refresh();
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [DXCategory("Appearance")]
        [DefaultValue("")]
        [DevExpressUtilsLocalizedDescription("XtraFormHtmlText")]
        public string HtmlText
        {
            get
            {
                return this.htmlText;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (this.HtmlText == value)
                    return;
                this.htmlText = value;
                ++this.lockTextChanged;
                try
                {
                    this.Text = StringPainter.Default.RemoveFormat(this.HtmlText);
                }
                finally
                {
                    --this.lockTextChanged;
                }
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        private bool ShouldSerializeText()
        {
            return string.IsNullOrEmpty(this.HtmlText) && !string.IsNullOrEmpty(this.Text);
        }

        [DXCategory("Behavior")]
        [RefreshProperties(RefreshProperties.All)]
        [DevExpressUtilsLocalizedDescription("XtraFormText")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (this.lockTextChanged == 0 && !this.IsInitializing)
                    this.htmlText = string.Empty;
                base.Text = value;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.lockTextChanged != 0)
                return;
            base.OnTextChanged(e);
        }

        [DevExpressUtilsLocalizedDescription("XtraFormHtmlImages")]
        [DXCategory("Appearance")]
        [DefaultValue(null)]
        public DevExpress.Utils.ImageCollection HtmlImages
        {
            get
            {
                return this.htmlImages;
            }
            set
            {
                if (this.htmlImages == value)
                    return;
                if (this.htmlImages != null)
                    this.htmlImages.Changed -= new EventHandler(this.OnHtmlImagesChanged);
                this.htmlImages = value;
                if (this.htmlImages == null)
                    return;
                this.htmlImages.Changed += new EventHandler(this.OnHtmlImagesChanged);
            }
        }

        private void OnHtmlImagesChanged(object sender, EventArgs e)
        {
        }

        [Browsable(false)]
        [DefaultValue(true)]
        [DevExpressUtilsLocalizedDescription("XtraFormAllowFormSkin")]
        [Category("Appearance")]
        public virtual bool AllowFormSkin
        {
            get
            {
                return this.allowFormSkin;
            }
            set
            {
                if (this.AllowFormSkin == value)
                    return;
                this.allowFormSkin = value;
                this.UpdateSkinPainter();
                if (!this.IsHandleCreated)
                    return;
                this.RecreateHandle();
            }
        }

        protected virtual object GetSkinPainterType()
        {
            return !this.GetAllowSkin() ? (object)null : (object)typeof(FormPainter);
        }

        protected bool UpdateSkinPainter()
        {
            return this.UpdateSkinPainter(false);
        }

        protected bool UpdateSkinPainter(bool fromHandleCreated)
        {
            bool flag = false;
            object skinPainterType = this.GetSkinPainterType();
            if (this.currentPainterType == skinPainterType || object.Equals(this.currentPainterType, skinPainterType))
            {
                if (this.painter != null)
                {
                    this.painter.ResetDefaultAppearance();
                    if (!fromHandleCreated)
                    {
                        this.painter.ResetRegionRect();
                        this.painter.ForceFrameChanged();
                    }
                }
                return false;
            }
            this.currentPainterType = this.IsInitializing ? (object)typeof(bool) : skinPainterType;
            if (this.painter != null)
            {
                this.painter.Dispose();
                this.painter = (FormPainter)null;
                flag = true;
            }
            if (skinPainterType != null)
            {
                if (!this.IsInitializing)
                {
                    this.painter = this.CreateFormBorderPainter();
                    this.painter.BaseWndProc += new FormPainter.WndProcMethod(this.DoBase);
                    flag = true;
                }
                else
                    this.shouldUpdateLookAndFeelOnResumeLayout = true;
            }
            return flag;
        }

        private void PatchClientSize()
        {
            this.SavedClientSize = this.ClientSizeFromSize(this.Size);
            FieldInfo field1 = typeof(Control).GetField("clientWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Control).GetField("clientHeight", BindingFlags.Instance | BindingFlags.NonPublic);
            field1.SetValue((object)this, (object)this.SavedClientSize.Width);
            field2.SetValue((object)this, (object)this.SavedClientSize.Height);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if (this.ShouldPatchClientSize)
                this.PatchClientSize();
            base.OnClientSizeChanged(e);
        }

        protected virtual FormPainter CreateFormBorderPainter()
        {
            return new FormPainter((Control)this, (ISkinProvider)this.LookAndFeel);
        }

        protected internal virtual bool AllowSkinForEmptyText
        {
            get
            {
                return false;
            }
        }

        protected virtual bool GetAllowMdiFormSkins()
        {
            return SkinManager.AllowMdiFormSkins;
        }

        protected virtual bool GetAllowSkin()
        {
            if (this.Disposing || this.IsDisposed || this.DesignMode && !UserLookAndFeelDefault.EnableDesignTimeFormSkin || (this.MdiParent != null && !this.GetAllowMdiFormSkins() || !this.AllowFormSkin) || !SkinManager.AllowFormSkins && (!this.DesignMode || this.DesignMode && !UserLookAndFeelDefault.EnableDesignTimeFormSkin) || this.FormBorderStyle == FormBorderStyle.None)
                return false;
            if (!this.ControlBox && !this.AllowSkinForEmptyText && (this.Text == "" || this.Text == null))
            {
                if (!this.IsInitializing || this.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
                    return false;
                this.shouldUpdateLookAndFeelOnResumeLayout = true;
            }
            return this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
        }

        [DllImport("USER32.dll")]
        private static extern bool DestroyMenu(IntPtr menu);

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenuCore(IntPtr hWnd, bool bRevert);

        [SecuritySafeCritical]
        internal static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert)
        {
            return DevExpress.XtraEditors.XtraForm.GetSystemMenuCore(hWnd, bRevert);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool EnableAcrylicAccent
        {
            get
            {
                return this.enableAcrylicAccent;
            }
            set
            {
                this.enableAcrylicAccent = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AcrylicAccentTintOpacity
        {
            get
            {
                return this.acrylicAccentTintOpacity;
            }
            set
            {
                this.acrylicAccentTintOpacity = value;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.UpdateSkinPainter(true);
            this.BeginInvoke((Delegate)new MethodInvoker(this.UpdateTheme));
            DevExpress.XtraEditors.XtraForm.GetSystemMenu(this.Handle, true);
            this.UpdateWindowThemeCore();
            if (!this.EnableAcrylicAccent)
                return;
            this.SetAcrylicAccent(true);
        }

        private void SetAcrylicAccent(bool focused = true)
        {
            if (!DCompositionSettings.IsWindowsSupportsAcrylic)
                return;
            Color formTransparencyKey = DCompositionSettings.CompositionFormTransparencyKey;
            List<ISupportDirectComposition> dcControls = ((DevExpress.XtraEditors.XtraForm.XFormControlCollection)this.Controls).DCControls;
            Color controlSkinBackColor = dcControls.Count > 0 ? dcControls[0].GetSkinBackColor() : Color.Empty;
            int num = 0;
            foreach (ISupportDirectComposition directComposition in dcControls)
            {
                if (directComposition.CompositionEnabled)
                    ++num;
                directComposition.SetBackColor(formTransparencyKey);
            }
            if (num <= 0)
                return;
            int opacity = focused ? this.AcrylicAccentTintOpacity : (int)byte.MaxValue;
            this.SetAcrylicAccentCore(controlSkinBackColor, opacity);
            this.TransparencyKey = formTransparencyKey;
        }

        private void SetAcrylicAccentCore(Color controlSkinBackColor, int opacity)
        {
            DWMProvider.SetWCA(this.Handle, AccentState.ACCENT_ENABLE_ACRYLIC, Color.FromArgb(opacity, controlSkinBackColor).ToArgb());
        }

        protected override void SetVisibleCore(bool value)
        {
            if (value && this.ShowingStrategy.AllowInitialize)
                this.ShowingStrategy.Initialize();
            base.SetVisibleCore(value);
        }

        protected internal bool DesignModeCore
        {
            get
            {
                return this.DesignMode;
            }
        }

        [SecuritySafeCritical]
        private static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList)
        {
            return DevExpress.XtraEditors.XtraForm.SetWindowThemeCore(hWnd, pszSubAppName, pszSubIdList);
        }

        [DllImport("uxtheme.dll", EntryPoint = "SetWindowTheme", CharSet = CharSet.Unicode)]
        private static extern int SetWindowThemeCore(
          IntPtr hWnd,
          string pszSubAppName,
          string pszSubIdList);

        protected virtual void UpdateWindowThemeCore()
        {
            if (!NativeVista.IsVista || this.DesignMode || this.FormPainter == null)
                return;
            DevExpress.XtraEditors.XtraForm.SetWindowTheme(this.Handle, "", "");
            this.themeEnabled = false;
        }

        internal virtual MdiClient GetMdiClient()
        {
            foreach (Control control in (ArrangedElementCollection)this.Controls)
            {
                if (control is MdiClient)
                    return control as MdiClient;
            }
            return (MdiClient)null;
        }

        private void EnableTheme()
        {
            if (this.themeEnabled)
                return;
            WXPPainter.Default.ResetWindowTheme((Control)this);
            this.Region = (Region)null;
            this.themeEnabled = true;
        }

        private void DisableTheme()
        {
            if (!this.ControlBox && !this.themeEnabled)
                return;
            WXPPainter.Default.DisableWindowTheme((Control)this);
            this.themeEnabled = false;
            this.UpdateWindowThemeCore();
        }

        private void UpdateTheme()
        {
            if (this.FormPainter != null && this.FormPainter.IsAllowNCDraw)
                this.DisableTheme();
            else
                this.EnableTheme();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                return this.GetCreateParams(base.CreateParams);
            }
        }

        protected virtual CreateParams GetCreateParams(CreateParams par)
        {
            if (this.IsDisposed || this.Disposing)
                return par;
            if (!this.GetAllowSkin())
                this.UpdateSkinPainter();
            if (this.creatingHandle && this.IsFormStateClientSizeSet() && this.IsCustomPainter)
            {
                par.Width = this.Width;
                par.Height = this.Height;
            }
            return par;
        }

        public override void ResetBackColor()
        {
            this.BackColor = Color.Empty;
        }

        [DevExpressUtilsLocalizedDescription("XtraFormBackColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                return this.GetColor(this.Appearance.BackColor, this.DefaultAppearance.BackColor);
            }
            set
            {
                if (this.GetDesigner() != null && this.GetDesigner().Loading)
                    return;
                this.Appearance.BackColor = value;
            }
        }

        protected Image GetSkinBackgroundImage()
        {
            if (this.GetSkin() == null)
                return (Image)null;
            return this.GetSkin().Image == null ? (Image)null : this.GetSkin().Image.Image;
        }

        protected SkinImageStretch GetSkinBackgroundImageStretch()
        {
            return this.GetSkin() == null || this.GetSkin().Image == null ? SkinImageStretch.NoResize : this.GetSkin().Image.Stretch;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DevExpressUtilsLocalizedDescription("XtraFormBackgroundImage")]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage != null ? base.BackgroundImage : this.GetSkinBackgroundImage();
            }
            set
            {
                if (value != null && value == this.GetSkinBackgroundImage())
                    value = (Image)null;
                base.BackgroundImage = value;
            }
        }

        protected bool ShouldSerializeBackgroundImageStore()
        {
            return base.BackgroundImage != null;
        }

        protected bool ShouldSerializeBackgroundImageLayoutStore()
        {
            return base.BackgroundImage != null;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Image BackgroundImageStore
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageLayout BackgroundImageLayoutStore
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DevExpressUtilsLocalizedDescription("XtraFormBackgroundImageLayout")]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                if (base.BackgroundImage == null && !this.IsInitializing)
                {
                    if (this.GetSkinBackgroundImageStretch() == SkinImageStretch.NoResize)
                        return ImageLayout.None;
                    if (this.GetSkinBackgroundImageStretch() == SkinImageStretch.Stretch)
                        return ImageLayout.Stretch;
                    if (this.GetSkinBackgroundImageStretch() == SkinImageStretch.Tile)
                        return ImageLayout.Tile;
                }
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        private IDesignerHost GetDesigner()
        {
            return this.Site == null ? (IDesignerHost)null : this.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
        }

        public override void ResetForeColor()
        {
            this.ForeColor = Color.Empty;
        }

        [DevExpressUtilsLocalizedDescription("XtraFormForeColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get
            {
                return LookAndFeelHelper.CheckTransparentForeColor((ISkinProvider)this.LookAndFeel, this.GetColor(this.Appearance.ForeColor, this.DefaultAppearance.ForeColor), (Control)null);
            }
            set
            {
                this.Appearance.ForeColor = value;
            }
        }

        [DevExpressUtilsLocalizedDescription("XtraFormFont")]
        public override Font Font
        {
            get
            {
                return this.useBaseFont ? base.Font : this.GetFont();
            }
            set
            {
                this.Appearance.Font = value;
            }
        }

        private Font GetFont()
        {
            AppearanceObject appearanceByOption = this.Appearance.GetAppearanceByOption(AppearanceObject.optUseFont);
            return appearanceByOption.Options.UseFont || this.DefaultAppearance.Font == null ? appearanceByOption.Font : this.DefaultAppearance.Font;
        }

        internal Font GetBaseFont()
        {
            return base.Font;
        }

        private Color GetColor(Color color, Color defColor)
        {
            return color == Color.Empty ? defColor : color;
        }

        protected AppearanceDefault DefaultAppearance
        {
            get
            {
                if (this.defaultAppearance == null)
                    this.defaultAppearance = this.CreateDefaultAppearance();
                return this.defaultAppearance;
            }
        }

        protected virtual AppearanceDefault CreateDefaultAppearance()
        {
            AppearanceDefault appearanceDefault = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
            if (this.LookAndFeel != null && this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
            {
                this.GetSkin().Apply(appearanceDefault, (ISkinProvider)this.LookAndFeel);
                this.CheckFormAppearanceAsMdiTabPage(appearanceDefault);
                LookAndFeelHelper.CheckColors((ISkinProvider)this.LookAndFeel, appearanceDefault, (Control)this);
            }
            return appearanceDefault;
        }

        protected void CheckFormAppearanceAsMdiTabPage(AppearanceDefault defaultAppearance)
        {
            SkinElement tabPane;
            if (!this.IsMdiTab(out tabPane) || tabPane == null)
                return;
            tabPane.ApplyForeColorAndFont(defaultAppearance);
            Color imageCenterColor = tabPane.Color.GetSolidImageCenterColor();
            if (!(imageCenterColor != Color.Empty))
                return;
            defaultAppearance.BackColor = imageCenterColor;
            Color imageCenterColor2 = tabPane.Color.GetSolidImageCenterColor2();
            if (!(imageCenterColor2 != Color.Empty))
                return;
            defaultAppearance.BackColor2 = imageCenterColor2;
            defaultAppearance.GradientMode = tabPane.Color.SolidImageCenterGradientMode;
        }

        protected bool IsMdiTab(out SkinElement tabPane)
        {
            tabPane = (SkinElement)null;
            MdiClient mdiClient = DevExpress.XtraEditors.XtraForm.GetMdiClient(this.MdiParent);
            if (mdiClient != null)
            {
                object clientSubclasser = DevExpress.XtraEditors.XtraForm.GetMdiClientSubclasser(mdiClient);
                if (clientSubclasser != null)
                {
                    if (clientSubclasser.GetType().Name.EndsWith("XtraMdiClientSubclasser"))
                        tabPane = TabSkins.GetSkin((ISkinProvider)this.LookAndFeel)[TabSkins.SkinTabPane];
                    if (clientSubclasser.GetType().Name.EndsWith("DocumentManagerMdiClientSubclasser"))
                        tabPane = DockingSkins.GetSkin((ISkinProvider)this.LookAndFeel)[DockingSkins.SkinDocumentGroupTabPane] ?? TabSkins.GetSkin((ISkinProvider)this.LookAndFeel)[TabSkins.SkinTabPane];
                    return true;
                }
            }
            return false;
        }

        private static MdiClient GetMdiClient(Form mdiParent)
        {
            return MdiClientSubclasserService.GetMdiClient(mdiParent);
        }

        private static object GetMdiClientSubclasser(MdiClient mdiClient)
        {
            return (object)MdiClientSubclasserService.FromMdiClient(mdiClient);
        }

        private static bool IsBarsAssembly(Assembly asm)
        {
            return string.Equals(asm.GetName().Name, "DevExpress.XtraBars.v17.2");
        }

        private SkinElement GetSkin()
        {
            return this.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin ? (SkinElement)null : CommonSkins.GetSkin((ISkinProvider)this.LookAndFeel)[CommonSkins.SkinForm];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.FormPainter != null && this.FormPainter.RequirePaint)
            {
                this.FormPainter.Draw(e);
                this.RaisePaintEvent((object)this, e);
            }
            else
                base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.FormPainter != null && this.FormPainter.RequirePaintBackground)
                this.FormPainter.DrawBackground(e);
            else
                base.OnPaintBackground(e);
        }

        bool IGlassForm.IsGlassForm
        {
            get
            {
                return this.CheckGlassForm();
            }
        }

        protected virtual bool CheckGlassForm()
        {
            return this.FormPainter == null;
        }

        internal CreateParams GetCreateParams()
        {
            return this.CreateParams;
        }

        bool ICustomDrawNonClientArea.IsCustomDrawNonClientArea
        {
            get
            {
                return this.CheckCustomDrawNonClientArea();
            }
        }

        protected virtual bool CheckCustomDrawNonClientArea()
        {
            return SkinManager.AllowFormSkins && !this.CheckGlassForm();
        }

        UserLookAndFeel ISupportLookAndFeel.LookAndFeel
        {
            get
            {
                return (UserLookAndFeel)this.LookAndFeel;
            }
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return (Control.ControlCollection)new DevExpress.XtraEditors.XtraForm.XFormControlCollection((Form)this);
        }

        internal int WmWindowPosChangedCount { get; set; }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return base.ShowDialog(DevExpress.XtraEditors.XtraForm.CheckOwner(owner));
        }

        private static IWin32Window CheckOwner(IWin32Window owner)
        {
            return owner is DevExpress.XtraEditors.XtraForm xtraForm && xtraForm.Location == ControlHelper.InvalidPoint ? (IWin32Window)((IEnumerable<Form>)xtraForm.OwnedForms).FirstOrDefault<Form>((Func<Form, bool>)(x => DevExpress.XtraEditors.XtraForm.IsAppropriateOwner(x))) : owner;
        }

        private static bool IsAppropriateOwner(Form condidateForm)
        {
            return true;
        }

        private class XFormControlCollection : Form.ControlCollection
        {
            internal XFormControlCollection(Form owner)
              : base(owner)
            {
                this.DCControls = new List<ISupportDirectComposition>();
            }

            private DevExpress.XtraEditors.XtraForm XForm
            {
                get
                {
                    return this.Owner as DevExpress.XtraEditors.XtraForm;
                }
            }

            internal List<ISupportDirectComposition> DCControls { get; set; }

            public override void Add(Control value)
            {
                try
                {
                    this.XForm.SuspendFormLayoutEvent();
                    if (this.XForm.EnableAcrylicAccent && value is ISupportDirectComposition directComposition)
                    {
                        this.DCControls.Add(directComposition);
                        directComposition.PrepareForComposition();
                    }
                    base.Add(value);
                }
                finally
                {
                    this.XForm.ResumeFormLayoutEvent(true);
                }
            }
        }
    }
}
