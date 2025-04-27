// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor.ux
{
    partial class SelectLayoutDialog
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
            cancelButton = new Button();
            okayButton = new Button();
            layoutSelectorPanel = new Panel();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.CausesValidation = false;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(550, 210);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 11;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okayButton
            // 
            okayButton.DialogResult = DialogResult.OK;
            okayButton.Location = new Point(668, 210);
            okayButton.Name = "okayButton";
            okayButton.Size = new Size(112, 34);
            okayButton.TabIndex = 10;
            okayButton.Text = "Ok";
            okayButton.UseVisualStyleBackColor = true;
            okayButton.Click += okayButton_Click;
            // 
            // layoutSelectorPanel
            // 
            layoutSelectorPanel.BackColor = SystemColors.ControlLightLight;
            layoutSelectorPanel.BorderStyle = BorderStyle.FixedSingle;
            layoutSelectorPanel.Location = new Point(12, 12);
            layoutSelectorPanel.Name = "layoutSelectorPanel";
            layoutSelectorPanel.Size = new Size(768, 192);
            layoutSelectorPanel.TabIndex = 12;
            layoutSelectorPanel.Paint += layoutSelectorPanel_Paint;
            layoutSelectorPanel.MouseClick += layoutSelectorPanel_MouseClick;
            // 
            // EditLayoutDialog
            // 
            AcceptButton = okayButton;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(791, 256);
            Controls.Add(layoutSelectorPanel);
            Controls.Add(cancelButton);
            Controls.Add(okayButton);
            Name = "EditLayoutDialog";
            Text = "Select Layout";
            ResumeLayout(false);
        }

        #endregion

        private Button cancelButton;
        private Button okayButton;
        private Panel layoutSelectorPanel;
    }
}