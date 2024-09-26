Public Class Form1

    Public board As New List(Of List(Of space))
    Public minebtn As New List(Of space)
    Public maxNumOfMines As Integer = 10


    Public Sub generateRandomBoard(numOfRows As Integer, numofCols As Integer)
        board.Clear()
        Dim rand As New Random

        For i As Integer = 0 To numOfRows - 1
            board.Add(New List(Of space))
            For j As Integer = 0 To numofCols - 1
                'make it so that it has to equal the maxNumOfMines
                'Dim mine As Boolean = IIf(minebtn.Count <= maxNumOfMines, IIf(rand.Next(0, 2) = 1, True, False), False)
                Dim pbtn As space = New space With {
                    .isMine = False,
                    .rowTag = i,
                    .colTag = j
                }
                board(i).Add(pbtn)
            Next
        Next
        placeRemainingMines()
    End Sub

    Public Sub doAllMineCounts()
        For i As Integer = 0 To board.Count - 1
            For j As Integer = 0 To board(0).Count - 1
                board(i)(j).neighborMineCount = GetNeighborMineCount(i, j)
            Next
        Next
    End Sub

    Public Sub FillBoardPanel()
        If board.Count < 1 Then
            MsgBox("Board Data Structure Invalid Peinis")
        End If
        Dim cols As Integer = board(0).Count
        Dim rows As Integer = board.Count
        pnlBoard.Controls.Clear()
        Dim btnWidth As Integer = pnlBoard.Size.Width / cols
        Dim btnHeight As Integer = pnlBoard.Size.Height / rows
        Try
            For row As Integer = 0 To rows - 1
                For col As Integer = 0 To cols - 1
                    board(row)(col).Width = btnWidth
                    board(row)(col).Height = btnHeight
                    board(row)(col).Location = New Point(col * btnWidth, row * btnHeight)
                    board(row)(col).TabStop = False
                    pnlBoard.Controls.Add(board(row)(col))
                    AddHandler board(row)(col).MouseDown, AddressOf btnClicked
                Next
            Next
        Catch ex As ArgumentOutOfRangeException
            MsgBox("Index Out Of Range")
        End Try

    End Sub

    Public Sub checkForWin()
        Dim win As Boolean = True
        Dim doesntmatter As Boolean = False
        For i As Integer = 0 To board.Count - 1
            For j As Integer = 0 To board(i).Count - 1
                If board(i)(j).isMine = False Then
                    If board(i)(j).isRevealed = True Then
                        win = True
                    Else
                        win = False
                        doesntmatter = True
                    End If
                Else
                    win = False
                    doesntmatter = True
                End If
            Next
        Next
        If win = True AndAlso doesntmatter = False Then
            pnlBoard.Enabled = False
            MenuStrip1.Enabled = False
            Timer1.Stop()
            MsgBox("You Won!")
            Form2.Show()
            Dim placeHolderInteger As Integer = 0
            Integer.TryParse(lblTimer.Text, placeHolderInteger)
            If BeginnerToolStripMenuItem.Checked = True Then
                If placeHolderInteger > My.Settings.beginnerHS Then
                    Integer.TryParse(lblTimer.Text, My.Settings.beginnerHS)
                End If
            ElseIf IntermediateToolStripMenuItem.Checked = True Then
                If placeHolderInteger > My.Settings.intermediateHS Then
                    Integer.TryParse(lblTimer.Text, My.Settings.intermediateHS)
                End If
            ElseIf ExpertToolStripMenuItem.Checked = True Then
                If placeHolderInteger > My.Settings.expertHS Then
                    Integer.TryParse(lblTimer.Text, My.Settings.expertHS)
                End If
            End If
        End If
    End Sub

    Public Sub revealAllMines()
        For i As Integer = 0 To board.Count - 1
            For j As Integer = 0 To board(0).Count - 1
                If board(i)(j).isMine = True Then
                    board(i)(j).isRevealed = True
                End If
            Next
        Next
    End Sub

    Dim StartTheTimer As Boolean = False
    Public remainingMines As Integer = maxNumOfMines

    Private Sub btnClicked(btn As space, e As MouseEventArgs)

        If StartTheTimer = False Then
            If GameOver = True Then
                GameOver = False
            End If
            StartTheTimer = True
            Timer1.Start()
        End If

        If btn.isRevealed = False Then
            If e.Button.Equals(MouseButtons.Left) Then

                If btn.isMine = True AndAlso btn.isFlagged = False Then
                    pnlBoard.Enabled = False
                    MenuStrip1.Enabled = False
                    GameOver = True
                    revealAllMines()
                    If BeginnerToolStripMenuItem.Checked = True Then
                        My.Settings.lastDifficulty = "Beginner"
                    ElseIf IntermediateToolStripMenuItem.Checked = True Then
                        My.Settings.lastDifficulty = "Intermediate"
                    ElseIf ExpertToolStripMenuItem.Checked = True Then
                        My.Settings.lastDifficulty = "Expert"
                    End If
                    Timer1.Stop()
                End If
                btn.isRevealed = True
                If btn.isMine AndAlso btn.isFlagged = False Then
                    MsgBox("Game Over")
                End If
                If btn.isMine = False AndAlso btn.isFlagged = False Then
                    btn.Text = btn.neighborMineCount
                    If btn.neighborMineCount = 0 Then
                        getZeroes(btn.rowTag, btn.colTag)
                    End If
                End If

            ElseIf e.Button.Equals(MouseButtons.Right) Then
                If btn.isFlagged = False Then
                    btn.isFlagged = True
                    remainingMines -= 1
                    lblMinesFound.Text = remainingMines
                Else
                    btn.isFlagged = False
                    remainingMines += 1
                    lblMinesFound.Text = remainingMines
                End If
            End If

        Else
            Return
        End If

        checkForWin()

    End Sub

    Public GameOver As Boolean = False


    Public column, row As Integer


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.lastDifficulty = "Beginner" Then
            row = 9
            column = 9
            maxNumOfMines = 10
            BeginnerToolStripMenuItem.Checked = True
        ElseIf My.Settings.lastDifficulty = "Intermediate" Then
            row = 16
            column = 16
            IntermediateToolStripMenuItem.Checked = True
            maxNumOfMines = 40
        ElseIf My.Settings.lastDifficulty = "Expert" Then
            row = 30
            column = 16
            maxNumOfMines = 99
            ExpertToolStripMenuItem.Checked = True
        End If
        StartTheTimer = False
        generateRandomBoard(row, column)
        FillBoardPanel()
        doAllMineCounts()
        lblMinesFound.Text = maxNumOfMines


    End Sub

    Private Sub BeginnerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeginnerToolStripMenuItem.Click
        BeginnerToolStripMenuItem.Checked = True
        IntermediateToolStripMenuItem.Checked = False
        ExpertToolStripMenuItem.Checked = False
        column = 9
        row = 9
        maxNumOfMines = 10
        remainingMines = maxNumOfMines
        lblMinesFound.Text = remainingMines
        board.Clear()
        generateRandomBoard(column, row)
        FillBoardPanel()
        doAllMineCounts()
        pnlBoard.Enabled = True
        MenuStrip1.Enabled = True
        My.Settings.lastDifficulty = "Beginner"
        StartTheTimer = False
        Timer1.Stop()
    End Sub

    Private Sub IntermediateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntermediateToolStripMenuItem.Click
        BeginnerToolStripMenuItem.Checked = False
        IntermediateToolStripMenuItem.Checked = True
        ExpertToolStripMenuItem.Checked = False
        column = 16
        row = 16
        maxNumOfMines = 40
        remainingMines = maxNumOfMines
        lblMinesFound.Text = remainingMines
        board.Clear()
        generateRandomBoard(column, row)
        FillBoardPanel()
        doAllMineCounts()
        pnlBoard.Enabled = True
        MenuStrip1.Enabled = True
        My.Settings.lastDifficulty = "Intermediate"
        StartTheTimer = False
        Timer1.Stop()
    End Sub

    Private Sub ExpertToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExpertToolStripMenuItem.Click
        BeginnerToolStripMenuItem.Checked = False
        IntermediateToolStripMenuItem.Checked = False
        ExpertToolStripMenuItem.Checked = True
        column = 30
        row = 30
        maxNumOfMines = 99
        remainingMines = maxNumOfMines
        lblMinesFound.Text = remainingMines
        board.Clear()
        generateRandomBoard(column, row)
        FillBoardPanel()
        doAllMineCounts()
        pnlBoard.Enabled = True
        MenuStrip1.Enabled = True
        My.Settings.lastDifficulty = "Expert"
        StartTheTimer = False
        Timer1.Stop()
    End Sub

    Public counterOfMines As Integer

    Public Sub placeRemainingMines()
        'set minesplaced = 0
        'while minesplace < remaining
        'generate random row
        'generate random col
        'if its not a mine, make it a mine and increment mines placed


        Dim rand As New Random
        Dim remainingMinesToPlace As Integer = maxNumOfMines
        While remainingMinesToPlace > 0
            Dim i As Integer = rand.Next(0, row)
            Dim j As Integer = rand.Next(0, column)
            If remainingMinesToPlace > 0 AndAlso board(i)(j).isMine = False AndAlso board(i)(j).isRevealed = False Then
                board(i)(j).isMine = True
                remainingMinesToPlace -= 1

            End If
        End While
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        GameOver = True
        StartTheTimer = False
        lblTimer.Text = 0
        Time = 0
        board.Clear()
        generateRandomBoard(row, column)
        FillBoardPanel()
        doAllMineCounts()
        pnlBoard.Enabled = True
        MenuStrip1.Enabled = True
        GameOver = False
        lblMinesFound.Text = maxNumOfMines
    End Sub

    Function GetNeighborMineCount(row As Integer, col As Integer) As Integer
        Dim count As Integer = 0
        For i As Integer = -1 To 1
            For j As Integer = -1 To 1
                If i = 0 AndAlso j = 0 Then
                Else
                    If spaceInBounds(row + i, col + j) AndAlso board(row + i)(col + j).isMine Then
                        count += 1
                    End If
                End If
            Next
        Next
        Return count
    End Function

    Function getZeroes(row As Integer, col As Integer) As Integer
        For i As Integer = -1 To 1
            For j As Integer = -1 To 1
                If i = 0 AndAlso j = 0 Then
                Else
                    If spaceInBounds(row + i, col + j) AndAlso board(row + i)(col + j).isRevealed = False AndAlso board(row + i)(col + j).neighborMineCount = 0 Then
                        board(row + i)(col + j).isRevealed = True
                        getZeroes(row + i, col + j)
                    ElseIf spaceInBounds(row + i, col + j) AndAlso board(row + i)(col + j).isRevealed = False Then
                        board(row + i)(col + j).isRevealed = True
                    End If
                End If
            Next
        Next
    End Function

    Function spaceInBounds(row As Integer, col As Integer)
        Return row >= 0 AndAlso row < board.Count AndAlso col >= 0 AndAlso col < board(0).Count
    End Function
    Private Time As Integer = 0
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Time += 1
        lblTimer.Text = Time
    End Sub

End Class

Public Class space

    Inherits Button

    Public Property rowTag As Integer = 0
    Public Property colTag As Integer = 0

    Public Property isMine As Boolean

    Private _neighborMineCount As Integer = 0
    Private _isRevealed As Boolean = False

    Private _isFlagged As Boolean = False

    Public Property neighborMineCount As Integer
        Get
            Return _neighborMineCount
        End Get
        Set(value As Integer)
            If value < 0 OrElse value > 8 Then
                Throw New InvalidMineCountException("Invalid mine count given for neighborMineCount")
            Else
                _neighborMineCount = value
            End If
        End Set
    End Property

    Public Property isRevealed As Boolean
        Get
            Return _isRevealed
        End Get
        Set(value As Boolean)
            If value = True AndAlso isFlagged = False Then
                _isRevealed = True
                If isMine = True Then
                    Me.BackgroundImageLayout = BackgroundImageLayout.Zoom
                    Me.BackgroundImage = My.Resources.mine
                Else

                    Me.Text = Me.neighborMineCount.ToString
                End If
            End If
        End Set
    End Property



    Public Property isFlagged As Boolean
        Set(value As Boolean)
            If value = True AndAlso isRevealed = False Then
                _isFlagged = True
                Me.BackgroundImageLayout = BackgroundImageLayout.Zoom
                Me.BackgroundImage = My.Resources.RedFlag
            End If
            If value = False Then
                _isFlagged = False
                Me.BackgroundImageLayout = BackgroundImageLayout.Zoom
                Me.BackgroundImage = Nothing
            End If
        End Set
        Get
            Return _isFlagged
        End Get
    End Property

    'Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
    '    If e.Equals(MouseButtons.Left) Then
    '        Me.isRevealed = True
    '        MyBase.OnMouseClick(e)
    '        If Me.isMine = True Then
    '            Form1.pnlBoard.Enabled = False
    '            Form1.btnReset.Enabled = False
    '            Form1.BeginnerToolStripMenuItem.Enabled = False
    '            Form1.IntermediateToolStripMenuItem.Enabled = False
    '            Form1.ExpertToolStripMenuItem.Enabled = False
    '            MsgBox("Game Over")
    '        End If
    '    ElseIf e.Equals(MouseButtons.Right) Then
    '        If Me.isFlagged = True Then
    '            Me.isFlagged = False
    '        Else
    '            Me.isFlagged = True
    '        End If
    '    End If
    '
    'End Sub

End Class

Public Class InvalidMineCountException
    Inherits Exception

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class