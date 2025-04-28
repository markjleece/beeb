// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    partial class EditTileDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTileDialog));
            colorPickerPanel = new Panel();
            selectedColorPanel = new Panel();
            pixelsPanel = new Panel();
            okButton = new Button();
            cancelButton = new Button();
            toolStrip = new ToolStrip();
            undoButton = new ToolStripButton();
            redoButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            freeformModeButton = new ToolStripButton();
            lineModeButton = new ToolStripButton();
            rectModeButton = new ToolStripButton();
            typeComboBox = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // colorPickerPanel
            // 
            colorPickerPanel.BorderStyle = BorderStyle.FixedSingle;
            colorPickerPanel.Location = new Point(12, 94);
            colorPickerPanel.Name = "colorPickerPanel";
            colorPickerPanel.Size = new Size(64, 256);
            colorPickerPanel.TabIndex = 0;
            colorPickerPanel.Paint += colorPickerPanel_Paint;
            colorPickerPanel.MouseClick += colorPickerPanel_MouseClick;
            // 
            // selectedColorPanel
            // 
            selectedColorPanel.BorderStyle = BorderStyle.FixedSingle;
            selectedColorPanel.Location = new Point(12, 46);
            selectedColorPanel.Name = "selectedColorPanel";
            selectedColorPanel.Size = new Size(64, 32);
            selectedColorPanel.TabIndex = 1;
            selectedColorPanel.Paint += selectedColorPanel_Paint;
            // 
            // pixelsPanel
            // 
            pixelsPanel.BorderStyle = BorderStyle.FixedSingle;
            pixelsPanel.Location = new Point(100, 46);
            pixelsPanel.Name = "pixelsPanel";
            pixelsPanel.Size = new Size(512, 512);
            pixelsPanel.TabIndex = 2;
            pixelsPanel.Paint += pixelsPanel_Paint;
            pixelsPanel.MouseDown += pixelsPanel_MouseDown;
            pixelsPanel.MouseEnter += pixelsPanel_MouseEnter;
            pixelsPanel.MouseLeave += pixelsPanel_MouseLeave;
            pixelsPanel.MouseMove += pixelsPanel_MouseMove;
            pixelsPanel.MouseUp += pixelsPanel_MouseUp;
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(500, 570);
            okButton.Name = "okButton";
            okButton.Size = new Size(112, 34);
            okButton.TabIndex = 5;
            okButton.Text = "Ok";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(382, 570);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // toolStrip
            // 
            toolStrip.ImageScalingSize = new Size(24, 24);
            toolStrip.Items.AddRange(new ToolStripItem[] { undoButton, redoButton, toolStripSeparator1, freeformModeButton, lineModeButton, rectModeButton, toolStripSeparator2, typeComboBox });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(634, 33);
            toolStrip.TabIndex = 9;
            toolStrip.Text = "toolStrip";
            // 
            // undoButton
            // 
            undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            undoButton.Image = (Image)resources.GetObject("undoButton.Image");
            undoButton.ImageTransparentColor = Color.Magenta;
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(34, 28);
            undoButton.Text = "Undo (Ctrl+Z)";
            undoButton.Click += undoButton_Click;
            // 
            // redoButton
            // 
            redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            redoButton.Image = (Image)resources.GetObject("redoButton.Image");
            redoButton.ImageTransparentColor = Color.Magenta;
            redoButton.Name = "redoButton";
            redoButton.Size = new Size(34, 28);
            redoButton.Text = "Redo (Ctrl+Y)";
            redoButton.Click += redoButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 33);
            // 
            // freeformModeButton
            // 
            freeformModeButton.CheckOnClick = true;
            freeformModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            freeformModeButton.Image = (Image)resources.GetObject("freeformModeButton.Image");
            freeformModeButton.ImageTransparentColor = Color.Magenta;
            freeformModeButton.Name = "freeformModeButton";
            freeformModeButton.Size = new Size(34, 28);
            freeformModeButton.Text = "Freeform";
            freeformModeButton.ToolTipText = "Freeform Mode";
            freeformModeButton.Click += freeformModeButton_Click;
            // 
            // lineModeButton
            // 
            lineModeButton.CheckOnClick = true;
            lineModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lineModeButton.Image = (Image)resources.GetObject("lineModeButton.Image");
            lineModeButton.ImageTransparentColor = Color.Magenta;
            lineModeButton.Name = "lineModeButton";
            lineModeButton.Size = new Size(34, 28);
            lineModeButton.Text = "Line";
            lineModeButton.ToolTipText = "Line Mode";
            lineModeButton.Click += lineModeButton_Click;
            // 
            // rectModeButton
            // 
            rectModeButton.CheckOnClick = true;
            rectModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            rectModeButton.Image = (Image)resources.GetObject("rectModeButton.Image");
            rectModeButton.ImageTransparentColor = Color.Magenta;
            rectModeButton.Name = "rectModeButton";
            rectModeButton.Size = new Size(34, 28);
            rectModeButton.Text = "Rectangle";
            rectModeButton.ToolTipText = "Rectangle Mode";
            rectModeButton.Click += rectModeButton_Click;
            // 
            // typeComboBox
            // 
            typeComboBox.CausesValidation = false;
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(121, 33);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 33);
            // 
            // EditTileDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(634, 617);
            Controls.Add(toolStrip);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(pixelsPanel);
            Controls.Add(selectedColorPanel);
            Controls.Add(colorPickerPanel);
            Name = "EditTileDialog";
            Text = "Edit Tile";
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel colorPickerPanel;
        private Panel selectedColorPanel;
        private Panel pixelsPanel;
        private Button okButton;
        private Button cancelButton;
        private ToolStrip toolStrip;
        private ToolStripButton undoButton;
        private ToolStripButton redoButton;
        private ToolStripButton freeformModeButton;
        private ToolStripButton lineModeButton;
        private ToolStripButton rectModeButton;
        private ToolStripComboBox typeComboBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
    }
}