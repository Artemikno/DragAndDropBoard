Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Public Class Form1
    Private image As Image
    Private xOffset As Integer = 0
    Private yOffset As Integer = 0
    Private imageSize As Double = 1
    Private isMouseDown As Boolean
    Private mouseStartingPoint As Point
    Private isTheImageThere As Boolean
    Private didTheMouseMove As Boolean
    Private context As BufferedGraphicsContext
    Private buffer As BufferedGraphics
    Private zoomCursor = New Cursor(New IO.MemoryStream(My.Resources.zoomcursor_uncolored))
    Private WithEvents PictureBox1 As New DoublePanel
    Private bg As Bitmap
    Private listOfPins As New List(Of Pin)
    Private listOfImages As New List(Of BoardImage)
    Private listOfConnections As New List(Of Connection)
    Private startingPinTempValue As Pin
    Private isEditing = True
    Private imageEdited As BoardImage
    Private publicLastID As Integer = 0
    Private sawConnError As Boolean = False

    Class Object2D
        Property ID As Integer
        Property X As Integer
        Property Y As Integer

        Public Overrides Function ToString() As String
            Return String.Concat("Object2D#", X, "#", Y, "#", ID)
        End Function
    End Class

    Class Connection
        Property ID As Integer
        Property Color As Pen
        Property StartingLocation As Pin
        Property DestinationLocation As Pin
        Public Sub New(Color As Pen, StartingLocation As Pin, DestinationLocation As Pin)
            Me.Color = Color
            Me.StartingLocation = StartingLocation
            Me.DestinationLocation = DestinationLocation
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("Connection#", Color.Color.ToArgb(), "#", StartingLocation.ID, "#", DestinationLocation.ID, "#", ID)
        End Function
    End Class

    Class Pin
        Inherits Object2D
        Property Color As Brush
        Property Parent As BoardImage
        Public Sub New(Parent As BoardImage, Color As Brush, X As Integer, Y As Integer)
            Me.Parent = Parent
            Me.Color = Color
            Me.X = X
            Me.Y = Y
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("Pin#", X, "#", Y, "#", Parent.ID, "#", ID)
        End Function
    End Class

    Class BoardImage
        Inherits Object2D
        Property Image As Bitmap
        Property Sizedata As Size
        Public Sub New(Image As Bitmap, X As Integer, Y As Integer, ImageLocation As String, SizeData As Size)
            Me.Image = New Bitmap(Image) With {.Tag = ImageLocation}
            Me.X = X
            Me.Y = Y
            Me.Sizedata = SizeData
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("BoardImage#", X, "#", Y, "#", Image.Tag, "#", Sizedata.Height, "#", Sizedata.Width, "#", ID)
        End Function
    End Class

    Private Sub LoadFile()
        Dim myStream As Stream
        Dim openFileDialog1 As New OpenFileDialog With {
        .Filter = "save files|*.bsf",
        .FilterIndex = 1,
        .RestoreDirectory = True
    }

        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            myStream = openFileDialog1.OpenFile()
            If myStream IsNot Nothing Then
                Dim reader As New StreamReader(myStream, Encoding.Unicode)
                Dim loadedData As String = reader.ReadToEnd()
                reader.Close()

                ' Clear existing objects
                listOfPins.Clear()
                listOfConnections.Clear()
                listOfImages.Clear()

                ' Split the data into individual object strings
                Dim objectStrings As String() = loadedData.Split("|"c)

                ' Load BoardImages first
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("BoardImage#") Then
                        Dim data As String() = objStr.Substring(11).Split("#"c)
                        Dim tag As String = data(2)
                        Dim img As Bitmap = Nothing
                        If IO.File.Exists(tag) Then
                            img = New Bitmap(tag)
                            img.Tag = tag
                        Else
                            MessageBox.Show($"Image file not found: {tag}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Continue For
                        End If
                        Dim boardImage As New BoardImage(img, Integer.Parse(data(0)), Integer.Parse(data(1)), img.Tag, New Size(data(4), data(3))) With {
                        .ID = Integer.Parse(data(5))
                    }
                        listOfImages.Add(boardImage)
                    End If
                Next

                ' Load Pins
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("Pin#") Then
                        Dim data As String() = objStr.Substring(4).Split("#"c)
                        Dim parentID As Integer = Integer.Parse(data(2))
                        Dim parent As BoardImage = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfImages, parentID)
                        Dim pin As New Pin(parent, Brushes.Red, Integer.Parse(data(0)), Integer.Parse(data(1))) With {
                        .ID = Integer.Parse(data(3))
                    }
                        listOfPins.Add(pin)
                    End If
                Next

                ' Load Connections
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("Connection#") Then
                        Dim data As String() = objStr.Substring(11).Split("#"c)
                        Dim color As Pen = Pens.Red
                        Dim startPinID As Integer = Integer.Parse(data(1))
                        Dim destPinID As Integer = Integer.Parse(data(2))
                        Dim startPin As Pin = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfPins, startPinID)
                        Dim destPin As Pin = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfPins, destPinID)
                        Dim connection As New Connection(color, startPin, destPin) With {
                        .ID = Integer.Parse(data(3))
                    }
                        listOfConnections.Add(connection)
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub LoadFile(fp As String)
        Dim myStream As Stream
        myStream = File.OpenRead(fp)
        If myStream IsNot Nothing Then
            Dim reader As New StreamReader(myStream, Encoding.Unicode)
            Dim loadedData As String = reader.ReadToEnd()
            reader.Close()

            ' Clear existing objects
            listOfPins.Clear()
            listOfConnections.Clear()
            listOfImages.Clear()

            ' Split the data into individual object strings
            Dim objectStrings As String() = loadedData.Split("|"c)

            ' Load BoardImages first
            For Each objStr As String In objectStrings
                If objStr.StartsWith("BoardImage#") Then
                    Dim data As String() = objStr.Substring(11).Split("#"c)
                    Dim tag As String = data(2)
                    Dim img As Bitmap = Nothing
                    If IO.File.Exists(tag) Then
                        img = New Bitmap(tag)
                        img.Tag = tag
                    Else
                        MessageBox.Show($"Image file not found: {tag}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Continue For
                    End If
                    Dim boardImage As New BoardImage(img, Integer.Parse(data(0)), Integer.Parse(data(1)), img.Tag, New Size(data(4), data(3))) With {
                        .ID = Integer.Parse(data(5))
                    }
                    listOfImages.Add(boardImage)
                End If
            Next

            ' Load Pins
            For Each objStr As String In objectStrings
                If objStr.StartsWith("Pin#") Then
                    Dim data As String() = objStr.Substring(4).Split("#"c)
                    Dim parentID As Integer = Integer.Parse(data(2))
                    Dim parent As BoardImage = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfImages, parentID)
                    Dim pin As New Pin(parent, Brushes.Red, Integer.Parse(data(0)), Integer.Parse(data(1))) With {
                        .ID = Integer.Parse(data(3))
                    }
                    listOfPins.Add(pin)
                End If
            Next

            ' Load Connections
            For Each objStr As String In objectStrings
                If objStr.StartsWith("Connection#") Then
                    Dim data As String() = objStr.Substring(11).Split("#"c)
                    Dim color As Pen = Pens.Red
                    Dim startPinID As Integer = Integer.Parse(data(1))
                    Dim destPinID As Integer = Integer.Parse(data(2))
                    Dim startPin As Pin = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfPins, startPinID)
                    Dim destPin As Pin = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfPins, destPinID)
                    Dim connection As New Connection(color, startPin, destPin) With {
                        .ID = Integer.Parse(data(3))
                    }
                    listOfConnections.Add(connection)
                End If
            Next
        End If
    End Sub

    Function IterateThroughListOfObject2DToFindOneWithMatchingID(list As IEnumerable(Of Object2D), idToFind As Integer)
        For Each obj As Object2D In list
            If obj.ID = idToFind Then
                Return obj
            End If
        Next
        Throw New KeyNotFoundException($"Object2D was not found (by ID) with ID {idToFind} in a list of Object2D")
    End Function

    Private Sub CenteredZoom(zoomFactor As Double)
        If Math.Sign(zoomFactor) = -1 Then
            imageSize /= Math.Abs(zoomFactor)
        Else
            imageSize *= zoomFactor
        End If
        PictureBox1.Invalidate()
    End Sub

    Private Sub Save()
        Dim myStream As Stream
        Dim saveFileDialog1 As New SaveFileDialog With {
            .Filter = "save files|*.bsf",
            .FilterIndex = 1,
            .RestoreDirectory = True
        }

        Dim listOfString = ""
        For Each obj As Pin In listOfPins
            listOfString = String.Concat(listOfString, obj.ToString(), "|")
        Next
        For Each obj As Connection In listOfConnections
            listOfString = String.Concat(listOfString, obj.ToString(), "|")
        Next
        For Each obj As BoardImage In listOfImages
            listOfString = String.Concat(listOfString, obj.ToString(), "|")
        Next
        listOfString.TrimEnd("|")

        If saveFileDialog1.ShowDialog() = DialogResult.OK Then
            myStream = saveFileDialog1.OpenFile()
            If (myStream IsNot Nothing) Then
                For Each write As Byte In Encoding.Unicode.GetBytes(listOfString)
                    myStream.WriteByte(write)
                Next
                myStream.Close()
            End If
        End If
    End Sub

    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles MyBase.DragEnter
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles MyBase.DragDrop
        Dim files As Array = e.Data.GetData(DataFormats.FileDrop)
        If Not IsDBNull(files) Then
            Try
                isTheImageThere = True
                If ToolStripComboBox1.Text = "Unlimited" Or ToolStripComboBox1.Text = "" Then
                    image = New Bitmap(files(0).ToString())
                Else
                    image = New Bitmap(files(0).ToString())
                    image = New Bitmap(image, Integer.Parse(ToolStripComboBox1.Text), Integer.Parse(ToolStripComboBox1.Text))
                End If
                image.Tag = files(0)
                PictureBox1.Invalidate()
            Catch ex As Exception
                Try
                    isTheImageThere = False
                    LoadFile(files(0))
                    PictureBox1.Invalidate()
                Catch ex1 As Exception
                End Try
            End Try
        End If
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        If isTheImageThere Then
            listOfImages.Add(New BoardImage(image, 0, 0, image.Tag, image.Size) With {.ID = publicLastID})
            publicLastID += 1
            image.Dispose()
            isTheImageThere = False
        End If
        buffer.Graphics.Clear(PictureBox1.BackColor)
        buffer.Graphics.DrawImage(bg, 0, 0, PictureBox1.Width, PictureBox1.Height)
        DrawAdvancedGraphics(buffer.Graphics)
        'If isTheImageThere Then 'Not usable
        'buffer.Graphics.DrawImage(image, CInt(PictureBox1.Width \ 2 - image.Width * imageSize \ 2 + xOffset), CInt(PictureBox1.Height \ 2 - image.Height * imageSize \ 2 + yOffset), CInt(image.Width * imageSize), CInt(image.Height * imageSize))
        'else
        'buffer.Graphics.DrawString(String.Concat("The image", vbNewLine, "will be here"), New Font("Comic Sans MS", CSng(16 * imageSize), FontStyle.Bold), Brushes.Black, CInt(PictureBox1.Width / 2 - 132.5208 * imageSize / 2 + xOffset), CInt(PictureBox1.Height / 2 - 62.125 * imageSize / 2 + yOffset))
        'End If
        buffer.Graphics.DrawString(String.Concat(String.Concat("X: ", xOffset), String.Concat(" Y: ", yOffset), String.Concat(" Zoom: ", imageSize)), New Font("Comic Sans MS", 16, FontStyle.Regular), Drawing.Brushes.Black, 0, (PictureBox1.Size.Height - 32.39583))
        buffer.Render(e.Graphics)
    End Sub

    Private Sub SaveTiledBackground()
        Dim bgTemp As New Bitmap(Me.Size.Width, Me.Size.Height)
        Dim bgTempGraphics As Graphics = Graphics.FromImage(bgTemp)
        For i As Integer = 0 To Math.Ceiling(Me.Size.Width / 168)
            For i2 As Integer = 0 To Math.Ceiling(Me.Size.Height / 168)
                bgTempGraphics.DrawImage(My.Resources.board, i * 168, i2 * 168, 168, 168)
                bgTempGraphics.DrawString("", New Font("Comic Sans MS", 16, FontStyle.Regular), Drawing.Brushes.Red, i * 168, i2 * 168)
            Next
        Next
        bg = bgTemp
        bgTemp = Nothing
        bgTempGraphics = Nothing
    End Sub

    Private Sub DrawAdvancedGraphics(g As Graphics)
        For Each img As BoardImage In listOfImages
            g.DrawImage(img.Image, CInt(img.X + xOffset + imageSize / 2), CInt(img.Y + yOffset + imageSize / 2), CInt(img.Sizedata.Width * imageSize), CInt(img.Sizedata.Height * imageSize))
        Next
        For Each conn As Connection In listOfConnections
            If conn.StartingLocation IsNot Nothing AndAlso conn.DestinationLocation IsNot Nothing Then
                g.DrawLine(conn.Color, conn.StartingLocation.X + 10 + xOffset, conn.StartingLocation.Y + 10 + yOffset, conn.DestinationLocation.X + 10 + xOffset, conn.DestinationLocation.Y + 10 + yOffset)
            Else
                If Not sawConnError Then
                    MessageBox.Show("Connection drawing error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    sawConnError = True
                End If
            End If
        Next

        For Each pin As Pin In listOfPins
            g.FillEllipse(pin.Color, pin.X + xOffset, pin.Y + yOffset, CInt(20 * imageSize), CInt(20 * imageSize))
        Next
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        isMouseDown = True
        mouseStartingPoint.X = e.X
        mouseStartingPoint.Y = e.Y
        didTheMouseMove = False
        Me.Cursor = zoomCursor
        If isEditing Then
            For Each pin As Pin In listOfPins
                If Not Rectangle.Intersect(New Rectangle(pin.X, pin.Y, 20, 20), New Rectangle(e.Location, New Size(0, 0))).IsEmpty Then
                    startingPinTempValue = pin
                End If
            Next
            If startingPinTempValue Is Nothing Then
                For Each img As BoardImage In listOfImages
                    If Not Rectangle.Intersect(New Rectangle(img.X, img.Y, img.Image.Width, img.Image.Height), New Rectangle(e.Location, New Size(0, 0))).IsEmpty Then
                        imageEdited = img
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If isMouseDown Then
            If Not (mouseStartingPoint.X = e.X And mouseStartingPoint.Y = e.Y) Then
                If Not isEditing Then
                    Me.Cursor = Cursors.SizeAll
                    xOffset += (e.X - mouseStartingPoint.X)
                    yOffset += (e.Y - mouseStartingPoint.Y)
                    mouseStartingPoint.X = e.X
                    mouseStartingPoint.Y = e.Y
                ElseIf imageEdited IsNot Nothing Then
                    Me.Cursor = Cursors.SizeAll
                    imageEdited.X += (e.X - mouseStartingPoint.X)
                    imageEdited.Y += (e.Y - mouseStartingPoint.Y)
                    For Each pin As Pin In listOfPins
                        If pin.Parent.Equals(imageEdited) Then
                            pin.X += (e.X - mouseStartingPoint.X)
                            pin.Y += (e.Y - mouseStartingPoint.Y)
                        End If
                    Next
                    mouseStartingPoint.X = e.X
                    mouseStartingPoint.Y = e.Y
                End If
                didTheMouseMove = True
                PictureBox1.Invalidate()
            End If
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        isMouseDown = False
        If Not didTheMouseMove And isEditing Then
            For Each img As BoardImage In listOfImages
                If Not Rectangle.Intersect(New Rectangle(img.X, img.Y, img.Image.Width, img.Image.Height), New Rectangle(e.Location, New Size(0, 0))).IsEmpty Then
                    listOfPins.Add(New Pin(img, Brushes.Red, e.X, e.Y) With {.ID = publicLastID})
                    publicLastID += 1
                End If
            Next
        Else
            If isEditing Then
                For Each pin As Pin In listOfPins
                    If Not Rectangle.Intersect(New Rectangle(pin.X, pin.Y, 20, 20), New Rectangle(e.Location, New Size(0, 0))).IsEmpty Then
                        listOfConnections.Add(New Connection(Pens.Red, startingPinTempValue, pin) With {.ID = publicLastID})
                        publicLastID += 1
                    End If
                Next
            End If
        End If
        imageEdited = Nothing
        startingPinTempValue = Nothing
        PictureBox1.Invalidate()
    End Sub

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize
        If PictureBox1.Width > 0 And PictureBox1.Height > 0 Then
            If buffer IsNot Nothing Then
                buffer.Dispose()
            End If
            If context IsNot Nothing Then
                buffer = context.Allocate(PictureBox1.CreateGraphics(), PictureBox1.DisplayRectangle)
                buffer.Graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                buffer.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            End If
        End If
        SaveTiledBackground()
        PictureBox1.Invalidate()
    End Sub

    Private Sub InToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InToolStripMenuItem.Click
        CenteredZoom(1.1)
        PictureBox1.Invalidate()
    End Sub

    Private Sub OutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OutToolStripMenuItem.Click
        CenteredZoom(-1.1)
        PictureBox1.Invalidate()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ToolStripComboBox1.Text = "256"
        SaveTiledBackground()
        PictureBox1.Dock = DockStyle.Fill
        Me.Controls.Add(PictureBox1)
        context = BufferedGraphicsManager.Current
        If PictureBox1.Width > 0 And PictureBox1.Height > 0 Then
            If buffer IsNot Nothing Then
                buffer.Dispose()
            End If
            buffer = context.Allocate(PictureBox1.CreateGraphics(), PictureBox1.DisplayRectangle)
            buffer.Graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            buffer.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
        End If
        Dim args As String() = Environment.GetCommandLineArgs()

        If args.Length > 1 Then
            Dim filePath As String = args(1)
            Try
                isTheImageThere = True
                image = Image.FromFile(filePath)
                PictureBox1.Invalidate()
            Catch ex As Exception
                Try
                    isTheImageThere = False
                    LoadFile(filePath)
                    PictureBox1.Invalidate()
                Catch ex1 As Exception
                End Try
            End Try
        End If
    End Sub

    Private Sub PictureBox1_Scroll(sender As Object, e As ScrollEventArgs) Handles PictureBox1.Scroll
        imageSize += e.NewValue - e.OldValue
        PictureBox1.Invalidate()
    End Sub

    Private Sub ResetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem.Click
        xOffset = 0
        yOffset = 0
        PictureBox1.Invalidate()
    End Sub

    Private Sub ResetToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem1.Click
        imageSize = 1
        PictureBox1.Invalidate()
    End Sub

    Private Sub ToggleModesMoveEditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleModesMoveEditToolStripMenuItem.Click
        isEditing = Not isEditing
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Save()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        LoadFile()
        PictureBox1.Invalidate()
    End Sub
End Class
