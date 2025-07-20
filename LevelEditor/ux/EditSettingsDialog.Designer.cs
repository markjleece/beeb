// --------------------------------------------------------------
// An Adventure In Time - A Doctor Who fan game for the BBC Micro
// Model B
//
// Copyright (C) 2025  Mark John Leece
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA  02110-1301, USA.
// --------------------------------------------------------------

namespace LevelEditor.ux
{
    partial class EditSettingsDialog
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
            enemyTypeComboBox = new ComboBox();
            enemyTypeLabel = new Label();
            enemyWeaponStrengthLabel = new Label();
            enemyWeaponStrengthComboBox = new ComboBox();
            laserStrengthLabel = new Label();
            laserStrengthComboBox = new ComboBox();
            laserHealthDrainLabel = new Label();
            laserHealthDrainComboBox = new ComboBox();
            jewelHealthGainLabel = new Label();
            jewelHealthGainComboBox = new ComboBox();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            // 
            // enemyTypeComboBox
            // 
            enemyTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            enemyTypeComboBox.FormattingEnabled = true;
            enemyTypeComboBox.Location = new Point(242, 13);
            enemyTypeComboBox.Name = "enemyTypeComboBox";
            enemyTypeComboBox.Size = new Size(232, 33);
            enemyTypeComboBox.TabIndex = 0;
            // 
            // enemyTypeLabel
            // 
            enemyTypeLabel.AutoSize = true;
            enemyTypeLabel.Location = new Point(14, 16);
            enemyTypeLabel.Name = "enemyTypeLabel";
            enemyTypeLabel.Size = new Size(208, 25);
            enemyTypeLabel.TabIndex = 0;
            enemyTypeLabel.Text = "Enemy Weapon Strength";
            // 
            // enemyWeaponStrengthLabel
            // 
            enemyWeaponStrengthLabel.AutoSize = true;
            enemyWeaponStrengthLabel.Location = new Point(12, 55);
            enemyWeaponStrengthLabel.Name = "enemyWeaponStrengthLabel";
            enemyWeaponStrengthLabel.Size = new Size(208, 25);
            enemyWeaponStrengthLabel.TabIndex = 0;
            enemyWeaponStrengthLabel.Text = "Enemy Weapon Strength";
            // 
            // enemyWeaponStrengthComboBox
            // 
            enemyWeaponStrengthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            enemyWeaponStrengthComboBox.FormattingEnabled = true;
            enemyWeaponStrengthComboBox.Location = new Point(242, 52);
            enemyWeaponStrengthComboBox.Name = "enemyWeaponStrengthComboBox";
            enemyWeaponStrengthComboBox.Size = new Size(232, 33);
            enemyWeaponStrengthComboBox.TabIndex = 0;
            // 
            // laserStrengthLabel
            // 
            laserStrengthLabel.AutoSize = true;
            laserStrengthLabel.Location = new Point(12, 94);
            laserStrengthLabel.Name = "laserStrengthLabel";
            laserStrengthLabel.Size = new Size(124, 25);
            laserStrengthLabel.TabIndex = 0;
            laserStrengthLabel.Text = "Laser Strength";
            // 
            // laserStrengthComboBox
            // 
            laserStrengthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            laserStrengthComboBox.FormattingEnabled = true;
            laserStrengthComboBox.Location = new Point(242, 91);
            laserStrengthComboBox.Name = "laserStrengthComboBox";
            laserStrengthComboBox.Size = new Size(232, 33);
            laserStrengthComboBox.TabIndex = 0;
            // 
            // laserHealthDrainLabel
            // 
            laserHealthDrainLabel.AutoSize = true;
            laserHealthDrainLabel.Location = new Point(12, 133);
            laserHealthDrainLabel.Name = "laserHealthDrainLabel";
            laserHealthDrainLabel.Size = new Size(155, 25);
            laserHealthDrainLabel.TabIndex = 0;
            laserHealthDrainLabel.Text = "Laser Health Drain";
            // 
            // laserHealthDrainComboBox
            // 
            laserHealthDrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            laserHealthDrainComboBox.FormattingEnabled = true;
            laserHealthDrainComboBox.Location = new Point(242, 130);
            laserHealthDrainComboBox.Name = "laserHealthDrainComboBox";
            laserHealthDrainComboBox.Size = new Size(232, 33);
            laserHealthDrainComboBox.TabIndex = 0;
            // 
            // jewelHealthGainLabel
            // 
            jewelHealthGainLabel.AutoSize = true;
            jewelHealthGainLabel.Location = new Point(12, 172);
            jewelHealthGainLabel.Name = "jewelHealthGainLabel";
            jewelHealthGainLabel.Size = new Size(149, 25);
            jewelHealthGainLabel.TabIndex = 0;
            jewelHealthGainLabel.Text = "Jewel Health Gain";
            // 
            // jewelHealthGainComboBox
            // 
            jewelHealthGainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            jewelHealthGainComboBox.FormattingEnabled = true;
            jewelHealthGainComboBox.Location = new Point(242, 169);
            jewelHealthGainComboBox.Name = "jewelHealthGainComboBox";
            jewelHealthGainComboBox.Size = new Size(232, 33);
            jewelHealthGainComboBox.TabIndex = 0;
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(362, 208);
            okButton.Name = "okButton";
            okButton.Size = new Size(112, 34);
            okButton.TabIndex = 0;
            okButton.Text = "Ok";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(244, 208);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // EditSettingsDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(486, 254);
            Controls.Add(enemyTypeLabel);
            Controls.Add(enemyTypeComboBox);
            Controls.Add(enemyWeaponStrengthLabel);
            Controls.Add(enemyWeaponStrengthComboBox);
            Controls.Add(laserStrengthLabel);
            Controls.Add(laserStrengthComboBox);
            Controls.Add(laserHealthDrainLabel);
            Controls.Add(laserHealthDrainComboBox);
            Controls.Add(jewelHealthGainLabel);
            Controls.Add(jewelHealthGainComboBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Name = "EditSettingsDialog";
            Text = "Edit Settings";
            Load += EditCoefficients_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label enemyWeaponStrengthLabel;
        private ComboBox enemyWeaponStrengthComboBox;
        private Label laserStrengthLabel;
        private ComboBox laserStrengthComboBox;
        private Label laserHealthDrainLabel;
        private ComboBox laserHealthDrainComboBox;
        private Label jewelHealthGainLabel;
        private ComboBox jewelHealthGainComboBox;
        private Button okButton;
        private Button cancelButton;
        private ComboBox enemyTypeComboBox;
        private Label enemyTypeLabel;
    }
}