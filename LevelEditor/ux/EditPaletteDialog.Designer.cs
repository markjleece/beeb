// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor.ux
{
    partial class EditPaletteDialog
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
            label2 = new Label();
            label1 = new Label();
            paletteLabel = new Label();
            colorSelector_0 = new ComboBox();
            colorSelector_3 = new ComboBox();
            colorSelector_2 = new ComboBox();
            colorSelector_1 = new ComboBox();
            cancelButton = new Button();
            okayButton = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label2.Location = new Point(210, 165);
            label2.Name = "label2";
            label2.Size = new Size(98, 25);
            label2.TabIndex = 18;
            label2.Text = "foreground";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label1.Location = new Point(210, 51);
            label1.Name = "label1";
            label1.Size = new Size(105, 25);
            label1.TabIndex = 19;
            label1.Text = "background";
            // 
            // paletteLabel
            // 
            paletteLabel.AutoSize = true;
            paletteLabel.Location = new Point(22, 20);
            paletteLabel.Name = "paletteLabel";
            paletteLabel.Size = new Size(111, 25);
            paletteLabel.TabIndex = 17;
            paletteLabel.Text = "Select colors";
            // 
            // colorSelector_0
            // 
            colorSelector_0.DrawMode = DrawMode.OwnerDrawFixed;
            colorSelector_0.DropDownStyle = ComboBoxStyle.DropDownList;
            colorSelector_0.FormattingEnabled = true;
            colorSelector_0.Location = new Point(22, 48);
            colorSelector_0.Name = "colorSelector_0";
            colorSelector_0.Size = new Size(182, 32);
            colorSelector_0.TabIndex = 13;
            colorSelector_0.DrawItem += colorSelector_DrawItem;
            // 
            // colorSelector_3
            // 
            colorSelector_3.DrawMode = DrawMode.OwnerDrawFixed;
            colorSelector_3.DropDownStyle = ComboBoxStyle.DropDownList;
            colorSelector_3.FormattingEnabled = true;
            colorSelector_3.Location = new Point(22, 165);
            colorSelector_3.Name = "colorSelector_3";
            colorSelector_3.Size = new Size(182, 32);
            colorSelector_3.TabIndex = 14;
            colorSelector_3.DrawItem += colorSelector_DrawItem;
            // 
            // colorSelector_2
            // 
            colorSelector_2.DrawMode = DrawMode.OwnerDrawFixed;
            colorSelector_2.DropDownStyle = ComboBoxStyle.DropDownList;
            colorSelector_2.FormattingEnabled = true;
            colorSelector_2.Location = new Point(22, 126);
            colorSelector_2.Name = "colorSelector_2";
            colorSelector_2.Size = new Size(182, 32);
            colorSelector_2.TabIndex = 15;
            colorSelector_2.DrawItem += colorSelector_DrawItem;
            // 
            // colorSelector_1
            // 
            colorSelector_1.DrawMode = DrawMode.OwnerDrawFixed;
            colorSelector_1.DropDownStyle = ComboBoxStyle.DropDownList;
            colorSelector_1.FormattingEnabled = true;
            colorSelector_1.Location = new Point(22, 87);
            colorSelector_1.Name = "colorSelector_1";
            colorSelector_1.Size = new Size(182, 32);
            colorSelector_1.TabIndex = 16;
            colorSelector_1.DrawItem += colorSelector_DrawItem;
            // 
            // cancelButton
            // 
            cancelButton.CausesValidation = false;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(110, 230);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okayButton
            // 
            okayButton.DialogResult = DialogResult.OK;
            okayButton.Location = new Point(228, 230);
            okayButton.Name = "okayButton";
            okayButton.Size = new Size(112, 34);
            okayButton.TabIndex = 11;
            okayButton.Text = "Ok";
            okayButton.UseVisualStyleBackColor = true;
            okayButton.Click += okayButton_Click;
            // 
            // EditPaletteDialog
            // 
            AcceptButton = okayButton;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(356, 275);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(paletteLabel);
            Controls.Add(colorSelector_0);
            Controls.Add(colorSelector_3);
            Controls.Add(colorSelector_2);
            Controls.Add(colorSelector_1);
            Controls.Add(cancelButton);
            Controls.Add(okayButton);
            Name = "EditPaletteDialog";
            Text = "Edit Palette";
            Load += EditPaletteDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private Label paletteLabel;
        private ComboBox colorSelector_0;
        private ComboBox colorSelector_3;
        private ComboBox colorSelector_2;
        private ComboBox colorSelector_1;
        private Button cancelButton;
        private Button okayButton;
    }
}