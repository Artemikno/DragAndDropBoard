Imports System.Windows.Forms

Public Class ColorPicker
    Property chosenColor As Color

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.chosenColor = Color.FromName(ListBox1.SelectedItem)
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ColorPicker_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each color As String In [Enum].GetNames(GetType(KnownColor))
            ListBox1.Items.Add(color)
        Next
    End Sub
End Class
