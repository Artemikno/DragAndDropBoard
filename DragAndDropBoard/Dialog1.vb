Imports System.Windows.Forms

Public Class Dialog1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ColorPicker.ShowDialog() = DialogResult.OK Then
            My.Settings.ColorNotes = ColorPicker.chosenColor
            My.Settings.Save()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ColorPicker.ShowDialog() = DialogResult.OK Then
            My.Settings.ColorConn = ColorPicker.chosenColor
            My.Settings.Save()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ColorPicker.ShowDialog() = DialogResult.OK Then
            My.Settings.ColorPins = ColorPicker.chosenColor
            My.Settings.Save()
        End If
    End Sub
End Class
