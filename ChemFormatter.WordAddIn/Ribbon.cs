﻿

namespace ChemFormatter.WordAddIn
{
    public class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        private void Ribbon_Load(object sender, Microsoft.Office.Tools.Ribbon.RibbonUIEventArgs e)
        {
        }

        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ribbon));
            this.tabAddIns = this.Factory.CreateRibbonTab();
            this.groupChemFormatter = this.Factory.CreateRibbonGroup();
            this.buttonRDigitChanger = this.Factory.CreateRibbonButton();
            this.buttonChemFormula = this.Factory.CreateRibbonButton();
            this.buttonIonFormula = this.Factory.CreateRibbonButton();
            this.buttonChemName = this.Factory.CreateRibbonButton();
            this.buttonJournalReference = this.Factory.CreateRibbonButton();
            this.tabAddIns.SuspendLayout();
            this.groupChemFormatter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAddIns
            // 
            this.tabAddIns.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabAddIns.Groups.Add(this.groupChemFormatter);
            this.tabAddIns.Label = "TabAddIns";
            this.tabAddIns.Name = "tabAddIns";
            // 
            // groupChemFormatter
            // 
            this.groupChemFormatter.Items.Add(this.buttonRDigitChanger);
            this.groupChemFormatter.Items.Add(this.buttonChemFormula);
            this.groupChemFormatter.Items.Add(this.buttonIonFormula);
            this.groupChemFormatter.Items.Add(this.buttonChemName);
            this.groupChemFormatter.Items.Add(this.buttonJournalReference);
            this.groupChemFormatter.Label = "ChemFormatter";
            this.groupChemFormatter.Name = "groupChemFormatter";

            // 
            // buttonRDigitChanger
            // 
            {
                var im = CommonResourceManager.GetImage(CommonResourceManager.RDigitChangerImage);
                if (im != null)
                    this.buttonRDigitChanger.Image = im;
            }
            this.buttonRDigitChanger.Label = "Sub-digits Changer";
            this.buttonRDigitChanger.Name = "buttonRDigitChanger";
            this.buttonRDigitChanger.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler((sender, e) => Applyer.ButtonRDigitChanger_Click(sender, e));
            this.buttonRDigitChanger.ShowLabel = true;
            this.buttonRDigitChanger.ShowImage = true;
            // 
            // buttonChemFormula
            // 
            {
                var im = CommonResourceManager.GetImage(CommonResourceManager.ChemFormulaImage);
                if (im != null)
                    this.buttonChemFormula.Image = im;
            }
            this.buttonChemFormula.Label = "Chem Formula";
            this.buttonChemFormula.Name = "buttonChemFormula";
            this.buttonChemFormula.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler((sender, e) => Applyer.ButtonChemFormular_Click(sender, e));
            this.buttonChemFormula.ShowLabel = true;
            this.buttonChemFormula.ShowImage = true;
            // 
            // buttonIonFormula
            // 
            {
                var im = CommonResourceManager.GetImage(CommonResourceManager.IonFormulaImage);
                if (im != null)
                    this.buttonIonFormula.Image = im;
            }
            this.buttonIonFormula.Label = "Ion Formula";
            this.buttonIonFormula.Name = "buttonIonFormula";
            this.buttonIonFormula.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler((sender, e) => Applyer.ButtonIonFormular_Click(sender, e));
            this.buttonIonFormula.ShowLabel = true;
            this.buttonIonFormula.ShowImage = true;
            // 
            // buttonChemName
            // 
            {
                var im = CommonResourceManager.GetImage(CommonResourceManager.ChemNameImage);
                if (im != null)
                    this.buttonChemName.Image = im;
            }
            this.buttonChemName.Label = "Chem Name";
            this.buttonChemName.Name = "buttonChemName";
            this.buttonChemName.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler((sender, e) => Applyer.ButtonChemName_Click(sender, e));
            this.buttonChemName.ShowLabel = true;
            this.buttonChemName.ShowImage = true;
            // 
            // buttonJournalReference
            // 
            {
                var im = CommonResourceManager.GetImage(CommonResourceManager.JournalReferenceImage);
                if (im != null)
                    this.buttonJournalReference.Image = im;
            }
            this.buttonJournalReference.Label = "Journal Ref";
            this.buttonJournalReference.Name = "buttonJournalReference";
            this.buttonJournalReference.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler((sender, e) => Applyer.ButtonJournalReference_Click(sender, e));
            this.buttonJournalReference.ShowLabel = true;
            this.buttonJournalReference.ShowImage = true;

            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tabAddIns);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabAddIns.ResumeLayout(false);
            this.tabAddIns.PerformLayout();
            this.groupChemFormatter.ResumeLayout(false);
            this.groupChemFormatter.PerformLayout();
            this.ResumeLayout(false);
        }

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabAddIns;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupChemFormatter;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonRDigitChanger;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonChemFormula;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonIonFormula;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonChemName;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonJournalReference;
    }
    
    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
