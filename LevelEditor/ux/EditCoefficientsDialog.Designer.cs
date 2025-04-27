// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace DaleksLevelEditor.ux
{
    partial class EditCoefficientsDialog
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
            label1 = new Label();
            enemyWeaponStrengthComboBox = new ComboBox();
            label2 = new Label();
            laserStrengthComboBox = new ComboBox();
            label3 = new Label();
            laserHealthDrainComboBox = new ComboBox();
            label4 = new Label();
            jewelHealthGainComboBox = new ComboBox();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(208, 25);
            label1.TabIndex = 0;
            label1.Text = "Enemy Weapon Strength";
            // 
            // enemyWeaponStrengthComboBox
            // 
            enemyWeaponStrengthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            enemyWeaponStrengthComboBox.FormattingEnabled = true;
            enemyWeaponStrengthComboBox.Location = new Point(233, 12);
            enemyWeaponStrengthComboBox.Name = "enemyWeaponStrengthComboBox";
            enemyWeaponStrengthComboBox.Size = new Size(226, 33);
            enemyWeaponStrengthComboBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 51);
            label2.Name = "label2";
            label2.Size = new Size(124, 25);
            label2.TabIndex = 0;
            label2.Text = "Laser Strength";
            // 
            // laserStrengthComboBox
            // 
            laserStrengthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            laserStrengthComboBox.FormattingEnabled = true;
            laserStrengthComboBox.Location = new Point(233, 51);
            laserStrengthComboBox.Name = "laserStrengthComboBox";
            laserStrengthComboBox.Size = new Size(226, 33);
            laserStrengthComboBox.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 93);
            label3.Name = "label3";
            label3.Size = new Size(155, 25);
            label3.TabIndex = 0;
            label3.Text = "Laser Health Drain";
            // 
            // laserHealthDrainComboBox
            // 
            laserHealthDrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            laserHealthDrainComboBox.FormattingEnabled = true;
            laserHealthDrainComboBox.Location = new Point(233, 90);
            laserHealthDrainComboBox.Name = "laserHealthDrainComboBox";
            laserHealthDrainComboBox.Size = new Size(226, 33);
            laserHealthDrainComboBox.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 132);
            label4.Name = "label4";
            label4.Size = new Size(149, 25);
            label4.TabIndex = 0;
            label4.Text = "Jewel Health Gain";
            // 
            // jewelHealthGainComboBox
            // 
            jewelHealthGainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            jewelHealthGainComboBox.FormattingEnabled = true;
            jewelHealthGainComboBox.Location = new Point(233, 129);
            jewelHealthGainComboBox.Name = "jewelHealthGainComboBox";
            jewelHealthGainComboBox.Size = new Size(226, 33);
            jewelHealthGainComboBox.TabIndex = 4;
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(347, 180);
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
            cancelButton.Location = new Point(229, 180);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // EditCoefficientsDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(471, 226);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(jewelHealthGainComboBox);
            Controls.Add(label4);
            Controls.Add(laserHealthDrainComboBox);
            Controls.Add(label3);
            Controls.Add(laserStrengthComboBox);
            Controls.Add(label2);
            Controls.Add(enemyWeaponStrengthComboBox);
            Controls.Add(label1);
            Name = "EditCoefficientsDialog";
            Text = "Edit Coefficients";
            Load += EditCoefficients_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox enemyWeaponStrengthComboBox;
        private Label label2;
        private ComboBox laserStrengthComboBox;
        private Label label3;
        private ComboBox laserHealthDrainComboBox;
        private Label label4;
        private ComboBox jewelHealthGainComboBox;
        private Button okButton;
        private Button cancelButton;
    }
}