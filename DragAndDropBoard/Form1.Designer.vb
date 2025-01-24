<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ZoomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OffsetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleModesMoveEditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripComboBox()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ZoomToolStripMenuItem, Me.OffsetToolStripMenuItem, Me.DeleteModeToolStripMenuItem, Me.ToggleModesMoveEditToolStripMenuItem, Me.ToolStripComboBox1, Me.ToolStripTextBox1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(800, 27)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.SaveAsToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 23)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.OpenToolStripMenuItem.Text = "Open..."
        '
        'SaveAsToolStripMenuItem
        '
        Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
        Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.SaveAsToolStripMenuItem.Text = "Save As..."
        '
        'ZoomToolStripMenuItem
        '
        Me.ZoomToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InToolStripMenuItem, Me.OutToolStripMenuItem, Me.ResetToolStripMenuItem1})
        Me.ZoomToolStripMenuItem.Name = "ZoomToolStripMenuItem"
        Me.ZoomToolStripMenuItem.Size = New System.Drawing.Size(51, 23)
        Me.ZoomToolStripMenuItem.Text = "Zoom"
        '
        'InToolStripMenuItem
        '
        Me.InToolStripMenuItem.Name = "InToolStripMenuItem"
        Me.InToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
        Me.InToolStripMenuItem.Text = "In"
        '
        'OutToolStripMenuItem
        '
        Me.OutToolStripMenuItem.Name = "OutToolStripMenuItem"
        Me.OutToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
        Me.OutToolStripMenuItem.Text = "Out"
        '
        'ResetToolStripMenuItem1
        '
        Me.ResetToolStripMenuItem1.Name = "ResetToolStripMenuItem1"
        Me.ResetToolStripMenuItem1.Size = New System.Drawing.Size(102, 22)
        Me.ResetToolStripMenuItem1.Text = "Reset"
        '
        'OffsetToolStripMenuItem
        '
        Me.OffsetToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ResetToolStripMenuItem})
        Me.OffsetToolStripMenuItem.Name = "OffsetToolStripMenuItem"
        Me.OffsetToolStripMenuItem.Size = New System.Drawing.Size(51, 23)
        Me.OffsetToolStripMenuItem.Text = "Offset"
        '
        'ResetToolStripMenuItem
        '
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        Me.ResetToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
        Me.ResetToolStripMenuItem.Text = "Reset"
        '
        'DeleteModeToolStripMenuItem
        '
        Me.DeleteModeToolStripMenuItem.Name = "DeleteModeToolStripMenuItem"
        Me.DeleteModeToolStripMenuItem.Size = New System.Drawing.Size(86, 23)
        Me.DeleteModeToolStripMenuItem.Text = "Delete mode"
        '
        'ToggleModesMoveEditToolStripMenuItem
        '
        Me.ToggleModesMoveEditToolStripMenuItem.Name = "ToggleModesMoveEditToolStripMenuItem"
        Me.ToggleModesMoveEditToolStripMenuItem.Size = New System.Drawing.Size(165, 23)
        Me.ToggleModesMoveEditToolStripMenuItem.Text = "Toggle modes (Move / Edit)"
        '
        'ToolStripComboBox1
        '
        Me.ToolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripComboBox1.Items.AddRange(New Object() {"Unlimited", "32", "64", "128", "256", "512", "1024"})
        Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
        Me.ToolStripComboBox1.Size = New System.Drawing.Size(121, 23)
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append
        Me.ToolStripTextBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ToolStripTextBox1.Items.AddRange(New Object() {"AliceBlue", "AntiqueWhite", "Aqua", "Aquamarine", "Azure", "Beige", "Bisque", "Black", "BlanchedAlmond", "Blue", "BlueViolet", "Brown", "BurlyWood", "CadetBlue", "Chartreuse", "Chocolate", "Coral", "CornflowerBlue", "Cornsilk", "Crimson", "Cyan", "DarkBlue", "DarkCyan", "DarkGoldenrod", "DarkGray", "DarkGreen", "DarkKhaki", "DarkMagenta", "DarkOliveGreen", "DarkOrange", "DarkOrchid", "DarkRed", "DarkSalmon", "DarkSeaGreen", "DarkSlateBlue", "DarkSlateGray", "DarkTurquoise", "DarkViolet", "DeepPink", "DeepSkyBlue", "DimGray", "DodgerBlue", "Firebrick", "FloralWhite", "ForestGreen", "Fuchsia", "Gainsboro", "GhostWhite", "Gold", "Goldenrod", "Gray", "Green", "GreenYellow", "Honeydew", "HotPink", "IndianRed", "Indigo", "Ivory", "Khaki", "Lavender", "LavenderBlush", "LawnGreen", "LemonChiffon", "LightBlue", "LightCoral", "LightCyan", "LightGoldenrodYellow", "LightGray", "LightGreen", "LightPink", "LightSalmon", "LightSeaGreen", "LightSkyBlue", "LightSlateGray", "LightSteelBlue", "LightYellow", "Lime", "LimeGreen", "Linen", "Magenta", "Maroon", "MediumAquamarine", "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSeaGreen", "MediumSlateBlue", "MediumSpringGreen", "MediumTurquoise", "MediumVioletRed", "MidnightBlue", "MintCream", "MistyRose", "Moccasin", "NavajoWhite", "Navy", "OldLace", "Olive", "OliveDrab", "Orange", "OrangeRed", "Orchid", "PaleGoldenrod", "PaleGreen", "PaleTurquoise", "PaleVioletRed", "PapayaWhip", "PeachPuff", "Peru", "Pink", "Plum", "PowderBlue", "Purple", "RebeccaPurple", "Red", "RosyBrown", "RoyalBlue", "SaddleBrown", "Salmon", "SandyBrown", "SeaGreen", "SeaShell", "Sienna", "Silver", "SkyBlue", "SlateBlue", "SlateGray", "Snow", "SpringGreen", "SteelBlue", "Tan", "Teal", "Thistle", "Tomato", "Transparent", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke", "Yellow", "YellowGreen"})
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(100, 23)
        Me.ToolStripTextBox1.Text = "Red"
        Me.ToolStripTextBox1.ToolTipText = "Color Name"
        '
        'Form1
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "Form1"
        Me.Text = "Please drag an image / save file here"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ZoomToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ResetToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents OffsetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ResetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToggleModesMoveEditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripComboBox1 As ToolStripComboBox
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteModeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripTextBox1 As ToolStripComboBox
End Class
