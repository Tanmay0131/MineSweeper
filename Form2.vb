Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Form1.BeginnerToolStripMenuItem.Checked = True Then
            lblDifficulty.Text = "Beginner"
        ElseIf Form1.IntermediateToolStripMenuItem.Checked = True Then
            lblDifficulty.Text = "Intermediate"
        ElseIf Form1.ExpertToolStripMenuItem.Checked = True Then
            lblDifficulty.Text = "Expert"
        End If
        lblTime.Text = Form1.lblTimer.Text
    End Sub
End Class