// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Windows.Forms;

namespace LevelEditor
{
    partial class LevelEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditorForm));
            tileSelectorPanel = new TileSelectorPanel();
            tileGridPanel = new TileGridPanel();
            toolStrip = new ToolStrip();
            newButton = new ToolStripButton();
            openButton = new ToolStripButton();
            saveButton = new ToolStripButton();
            saveAsButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            undoButton = new ToolStripButton();
            redoButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            editSettingsButton = new ToolStripButton();
            selectLayoutButton = new ToolStripButton();
            editPaletteButton = new ToolStripButton();
            editTileButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            freeformModeButton = new ToolStripButton();
            lineModeButton = new ToolStripButton();
            rectModeButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            zoomInButton = new ToolStripButton();
            zoomOutButton = new ToolStripButton();
            helpButton = new ToolStripButton();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // tileSelectorPanel
            // 
            tileSelectorPanel.Location = new Point(12, 42);
            tileSelectorPanel.Name = "tileSelectorPanel";
            tileSelectorPanel.Size = new Size(130, 1110);
            tileSelectorPanel.TabIndex = 4;
            tileSelectorPanel.Paint += TileSelectorPanel_Paint;
            tileSelectorPanel.MouseClick += TileSelectorPanel_MouseClick;
            tileSelectorPanel.MouseDoubleClick += TileSelectorPanel_MouseDoubleClick;
            // 
            // tileGridPanel
            // 
            tileGridPanel.AutoScroll = true;
            tileGridPanel.BorderStyle = BorderStyle.FixedSingle;
            tileGridPanel.Location = new Point(158, 42);
            tileGridPanel.Name = "tileGridPanel";
            tileGridPanel.Size = new Size(1715, 1110);
            tileGridPanel.TabIndex = 5;
            tileGridPanel.Paint += TileGridPanel_Paint;
            tileGridPanel.MouseDown += TileGridPanel_MouseDown;
            tileGridPanel.MouseEnter += TileGridPanel_MouseEnter;
            tileGridPanel.MouseLeave += TileGridPanel_MouseLeave;
            tileGridPanel.MouseMove += TileGridPanel_MouseMove;
            tileGridPanel.MouseUp += TileGridPanel_MouseUp;
            tileGridPanel.MouseWheel += TileGridPanel_MouseWheel;
            // 
            // toolStrip
            // 
            toolStrip.ImageScalingSize = new Size(24, 24);
            toolStrip.Items.AddRange(new ToolStripItem[] { newButton, openButton, saveButton, saveAsButton, toolStripSeparator1, undoButton, redoButton, toolStripSeparator3, editSettingsButton, selectLayoutButton, editPaletteButton, editTileButton, toolStripSeparator2, freeformModeButton, lineModeButton, rectModeButton, toolStripSeparator4, zoomInButton, zoomOutButton, helpButton });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1885, 33);
            toolStrip.TabIndex = 7;
            toolStrip.Text = "toolStrip";
            // 
            // newButton
            // 
            newButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newButton.Image = (Image)resources.GetObject("newButton.Image");
            newButton.ImageTransparentColor = Color.Magenta;
            newButton.Name = "newButton";
            newButton.Size = new Size(34, 28);
            newButton.Text = "New";
            newButton.Click += NewButton_Click;
            // 
            // openButton
            // 
            openButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openButton.Image = (Image)resources.GetObject("openButton.Image");
            openButton.ImageTransparentColor = Color.Magenta;
            openButton.Name = "openButton";
            openButton.Size = new Size(34, 28);
            openButton.Text = "Open";
            openButton.Click += OpenButton_Click;
            // 
            // saveButton
            // 
            saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveButton.Image = (Image)resources.GetObject("saveButton.Image");
            saveButton.ImageTransparentColor = Color.Magenta;
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(34, 28);
            saveButton.Text = "Save";
            saveButton.ToolTipText = "Save (Ctrl+S)";
            saveButton.Click += SaveButton_Click;
            // 
            // saveAsButton
            // 
            saveAsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveAsButton.Image = (Image)resources.GetObject("saveAsButton.Image");
            saveAsButton.ImageTransparentColor = Color.Magenta;
            saveAsButton.Name = "saveAsButton";
            saveAsButton.Size = new Size(34, 28);
            saveAsButton.Text = "Save As";
            saveAsButton.Click += SaveAsButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 33);
            // 
            // undoButton
            // 
            undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            undoButton.Image = (Image)resources.GetObject("undoButton.Image");
            undoButton.ImageTransparentColor = Color.Magenta;
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(34, 28);
            undoButton.Text = "Undo";
            undoButton.ToolTipText = "Undo (Ctrl+Z)";
            undoButton.Click += UndoButton_Click;
            // 
            // redoButton
            // 
            redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            redoButton.Image = (Image)resources.GetObject("redoButton.Image");
            redoButton.ImageTransparentColor = Color.Magenta;
            redoButton.Name = "redoButton";
            redoButton.Size = new Size(34, 28);
            redoButton.Text = "Redo";
            redoButton.ToolTipText = "Redo (Ctrl+Y)";
            redoButton.Click += RedoButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 33);
            // 
            // editSettingsButton
            // 
            editSettingsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            editSettingsButton.Image = (Image)resources.GetObject("editSettingsButton.Image");
            editSettingsButton.ImageTransparentColor = Color.Magenta;
            editSettingsButton.Name = "editSettingsButton";
            editSettingsButton.Size = new Size(34, 28);
            editSettingsButton.Text = "Select Layout";
            editSettingsButton.ToolTipText = "Edit Settings";
            editSettingsButton.Click += EditSettingsButton_Click;
            // 
            // selectLayoutButton
            // 
            selectLayoutButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            selectLayoutButton.Image = (Image)resources.GetObject("selectLayoutButton.Image");
            selectLayoutButton.ImageTransparentColor = Color.Magenta;
            selectLayoutButton.Name = "selectLayoutButton";
            selectLayoutButton.Size = new Size(34, 28);
            selectLayoutButton.Text = "Select Layout";
            selectLayoutButton.ToolTipText = "Select Layout";
            selectLayoutButton.Click += SelectLayoutButton_Click;
            // 
            // editPaletteButton
            // 
            editPaletteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            editPaletteButton.Image = (Image)resources.GetObject("editPaletteButton.Image");
            editPaletteButton.ImageTransparentColor = Color.Magenta;
            editPaletteButton.Name = "editPaletteButton";
            editPaletteButton.Size = new Size(34, 28);
            editPaletteButton.Text = "Edit Palette";
            editPaletteButton.ToolTipText = "Edit Palette";
            editPaletteButton.Click += PaletteButton_Click;
            // 
            // editTileButton
            // 
            editTileButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            editTileButton.Image = (Image)resources.GetObject("editTileButton.Image");
            editTileButton.ImageTransparentColor = Color.Magenta;
            editTileButton.Name = "editTileButton";
            editTileButton.Size = new Size(34, 28);
            editTileButton.Text = "Edit Tile";
            editTileButton.ToolTipText = "Edit Tile";
            editTileButton.Click += EditTileButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 33);
            // 
            // freeformModeButton
            // 
            freeformModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            freeformModeButton.Image = (Image)resources.GetObject("freeformModeButton.Image");
            freeformModeButton.ImageTransparentColor = Color.Magenta;
            freeformModeButton.Name = "freeformModeButton";
            freeformModeButton.Size = new Size(34, 28);
            freeformModeButton.Text = "Freeform";
            freeformModeButton.ToolTipText = "Freeform Mode";
            freeformModeButton.Click += FreeformModeButton_Click;
            // 
            // lineModeButton
            // 
            lineModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lineModeButton.Image = (Image)resources.GetObject("lineModeButton.Image");
            lineModeButton.ImageTransparentColor = Color.Magenta;
            lineModeButton.Name = "lineModeButton";
            lineModeButton.Size = new Size(34, 28);
            lineModeButton.Text = "Line";
            lineModeButton.ToolTipText = "Line Mode";
            lineModeButton.Click += LineModeButton_Click;
            // 
            // rectModeButton
            // 
            rectModeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            rectModeButton.Image = (Image)resources.GetObject("rectModeButton.Image");
            rectModeButton.ImageTransparentColor = Color.Magenta;
            rectModeButton.Name = "rectModeButton";
            rectModeButton.Size = new Size(34, 28);
            rectModeButton.Text = "Rectangle";
            rectModeButton.ToolTipText = "Rectangle Mode";
            rectModeButton.Click += RectModeButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 33);
            // 
            // zoomInButton
            // 
            zoomInButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            zoomInButton.Image = (Image)resources.GetObject("zoomInButton.Image");
            zoomInButton.ImageTransparentColor = Color.Magenta;
            zoomInButton.Name = "zoomInButton";
            zoomInButton.Size = new Size(34, 28);
            zoomInButton.Text = "Zoom In";
            zoomInButton.ToolTipText = "Zoom In (Ctrl +)";
            zoomInButton.Click += ZoomInButton_Click;
            // 
            // zoomOutButton
            // 
            zoomOutButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            zoomOutButton.Image = (Image)resources.GetObject("zoomOutButton.Image");
            zoomOutButton.ImageTransparentColor = Color.Magenta;
            zoomOutButton.Name = "zoomOutButton";
            zoomOutButton.Size = new Size(34, 28);
            zoomOutButton.Text = "Zoom Out (Ctrl (-)";
            zoomOutButton.ToolTipText = "Zoom Out (Ctrl -)";
            zoomOutButton.Click += ZoomOutButton_Click;
            // 
            // helpButton
            // 
            helpButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            helpButton.Image = (Image)resources.GetObject("helpButton.Image");
            helpButton.ImageTransparentColor = Color.Magenta;
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(34, 28);
            helpButton.Text = "toolStripButton1";
            helpButton.ToolTipText = "Help";
            helpButton.Click += HelpButton_Click;
            // 
            // LevelEditorForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1885, 1164);
            Controls.Add(toolStrip);
            Controls.Add(tileGridPanel);
            Controls.Add(tileSelectorPanel);
            Name = "LevelEditorForm";
            Text = "Level Editor";
            FormClosing += LevelEditorForm_FormClosing;
            Load += LevelEditorForm_Load;
            Resize += LevelEditorForm_Resize;
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TileSelectorPanel tileSelectorPanel;
        private TileGridPanel tileGridPanel;
        private ToolStrip toolStrip;
        private ToolStripButton newButton;
        private ToolStripButton openButton;
        private ToolStripButton saveButton;
        private ToolStripButton saveAsButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton undoButton;
        private ToolStripButton redoButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton freeformModeButton;
        private ToolStripButton lineModeButton;
        private ToolStripButton rectModeButton;
        private ToolStripButton selectLayoutButton;
        private ToolStripButton editPaletteButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton editTileButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton zoomInButton;
        private ToolStripButton zoomOutButton;
        private ToolStripButton helpButton;
        private ToolStripButton editSettingsButton;
    }
}