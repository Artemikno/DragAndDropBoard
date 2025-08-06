Imports System.CodeDom.Compiler
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.IO
Imports System.Net.WebRequestMethods
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
    Private boardImg As New Bitmap(512, 512)
    Private listOfPins As New List(Of Pin)
    Private listOfImages As New List(Of BoardImage)
    Private listOfNotes As New List(Of Note)
    Private listOfConnections As New List(Of Connection)
    Private startingPinTempValue As Pin
    Private isEditing = True
    Private imageEdited As Object2D
    Private publicLastID As Integer = 0
    Private sawConnError As Boolean = True
    Private isDeletingSomthing = False
    'Private rand As New Random()
    Private main = "Pin#825#159#8#14#Red|Pin#984#132#11#15#Red|Pin#433#366#0#19#Red|Pin#644#388#17#20#Red|Pin#526#513#18#22#Red|Pin#645#576#24#26#Red|Pin#550#666#28#29#Red|Pin#845#315#5#32#Red|Connection#Red#15#14#16|Connection#Red#20#19#21|Connection#Red#22#20#23|Connection#Red#22#26#27|Connection#Red#29#26#30|Connection#Red#14#22#31|Connection#Red#32#22#33|Note#368#305#Hi!#128#128#0#SkyBlue|Note#160#34#/\
 l
 l#128#128#3#SkyBlue|Note#197#85#Click here
to delete#128#128#4#SkyBlue|Note#752#279#To add an
image do
what the
title says#128#128#5#SkyBlue|Note#406#42#/\
 l
 l#128#128#6#SkyBlue|Note#437#84#When you put
an image
it will
resize#128#128#7#SkyBlue|Note#741#71#To add a
note click
somwhere
empty#128#128#8#SkyBlue|Note#889#45#To add a
new line
type /nｌ#128#128#11#SkyBlue|Note#572#291#To save press
File and
Save As...#128#128#17#SkyBlue|Note#425#478#To add a pin 
click on a 
note or image#128#128#18#SkyBlue|Note#619#475#To connect pins 
drag from one 
pin to another!#128#128#24#SkyBlue|Note#453#651#I will
add more#128#128#28#SkyBlue|"

    Class Object2D
        Property ID As Integer
        Property X As Integer
        Property Y As Integer
        Property DeleteMe As Boolean

        Public Overrides Function ToString() As String
            Return String.Concat("Object2D#", X, "#", Y, "#", ID)
        End Function
    End Class

    Class Connection
        Property ID As Integer
        Property Color As Pen
        Property StartingLocation As Pin
        Property DestinationLocation As Pin
        Public Sub New(Color As String, StartingLocation As Pin, DestinationLocation As Pin)
            Me.Color = New Pen(Drawing.Color.FromName(Color))
            Me.StartingLocation = StartingLocation
            Me.DestinationLocation = DestinationLocation
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("Connection#", Color.Color.Name, "#", StartingLocation.ID, "#", DestinationLocation.ID, "#", ID)
        End Function
    End Class

    Class Pin
        Inherits Object2D
        Property Color As Brush
        Property Parent As Object2D
        Public Sub New(Parent As Object2D, Color As String, X As Integer, Y As Integer)
            Me.Parent = Parent
            Me.Color = New Pen(Drawing.Color.FromName(Color)).Brush
            Me.X = X
            Me.Y = Y
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("Pin#", X, "#", Y, "#", Parent.ID, "#", ID, "#", New Pen(Color).Color.Name)
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

    Class Note
        Inherits Object2D
        Property Color As Brush
        Property Note As String
        Property Sizedata As Size
        Public Sub New(Note As String, X As Integer, Y As Integer, SizeData As Size, color As String)
            If Note.Equals("") Then
                Throw New InvalidOperationException("Invalid condition for creating this object.")
                Return
            End If
            Me.Note = Note
            Me.X = X
            Me.Y = Y
            Me.Sizedata = SizeData
            Me.Color = New Pen(Drawing.Color.FromName(color)).Brush
        End Sub

        Public Overrides Function ToString() As String
            Return String.Concat("Note#", X, "#", Y, "#", Note, "#", Sizedata.Height, "#", Sizedata.Width, "#", ID, "#", New Pen(Color).Color.Name)
        End Function

        Public Function GetRandomSolidBrush(rand As Random) As SolidBrush
            Dim randomColor As Color = Drawing.Color.FromArgb(rand.Next(16) * 16, rand.Next(16) * 16, rand.Next(16) * 16)
            Return New SolidBrush(randomColor)
        End Function
    End Class

    Public Sub LoadFile()
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
                listOfNotes.Clear()

                ' Split the data into individual object strings
                Dim objectStrings As String() = loadedData.Split("|"c)

                ' Load BoardImages first
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("BoardImage#") Then
                        Dim data As String() = objStr.Substring(11).Split("#"c)
                        Dim tag As String = data(2)
                        Dim img As Bitmap
                        If IO.File.Exists(tag) Then
                            img = New Bitmap(tag) With {
                                .Tag = tag
                            }
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

                ' Load Notes
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("Note#") Then
                        Dim data As String() = objStr.Substring(5).Split("#"c)
                        Dim pin As New Note(data(2), Integer.Parse(data(0)), Integer.Parse(data(1)), New Size(data(4), data(3)), data(6)) With {
                            .ID = Integer.Parse(data(5))
                        }
                        listOfNotes.Add(pin)
                    End If
                Next

                ' Load Pins
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("Pin#") Then
                        Dim data As String() = objStr.Substring(4).Split("#"c)
                        Dim parentID As Integer = Integer.Parse(data(2))
                        Dim parent As BoardImage = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfImages, parentID)
                        Dim pin As New Pin(parent, data(4), Integer.Parse(data(0)), Integer.Parse(data(1))) With {
                            .ID = Integer.Parse(data(3))
                        }
                        listOfPins.Add(pin)
                    End If
                Next

                ' Load Connections
                For Each objStr As String In objectStrings
                    If objStr.StartsWith("Connection#") Then
                        Dim data As String() = objStr.Substring(11).Split("#"c)
                        Dim color As String = data(0)
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

    Public Sub LoadFile(fp As String)
        Dim myStream As FileStream
        myStream = IO.File.OpenRead(fp)
        If myStream IsNot Nothing Then
            Dim reader As New StreamReader(myStream, Encoding.Unicode)
            Dim loadedData As String = reader.ReadToEnd()
            reader.Close()

            ' Clear existing objects
            listOfPins.Clear()
            listOfConnections.Clear()
            listOfImages.Clear()
            listOfNotes.Clear()

            ' Split the data into individual object strings
            Dim objectStrings As String() = loadedData.Split("|"c)

            ' Load BoardImages first
            For Each objStr As String In objectStrings
                If objStr.StartsWith("BoardImage#") Then
                    Dim data As String() = objStr.Substring(11).Split("#"c)
                    Dim tag As String = data(2)
                    Dim img As Bitmap
                    If IO.File.Exists(tag) Then
                        img = New Bitmap(tag) With {
                            .Tag = tag
                        }
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

            ' Load Notes
            For Each objStr As String In objectStrings
                If objStr.StartsWith("Note#") Then
                    Dim data As String() = objStr.Substring(5).Split("#"c)
                    Dim pin As New Note(data(2), Integer.Parse(data(0)), Integer.Parse(data(1)), New Size(data(4), data(3)), data(6)) With {
                            .ID = Integer.Parse(data(5))
                        }
                    listOfNotes.Add(pin)
                End If
            Next

            ' Load Pins
            For Each objStr As String In objectStrings
                If objStr.StartsWith("Pin#") Then
                    Dim data As String() = objStr.Substring(4).Split("#"c)
                    Dim parentID As Integer = Integer.Parse(data(2))
                    Dim parent As BoardImage = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfImages, parentID)
                    Dim pin As New Pin(parent, data(4), Integer.Parse(data(0)), Integer.Parse(data(1))) With {
                        .ID = Integer.Parse(data(3))
                    }
                    listOfPins.Add(pin)
                End If
            Next

            ' Load Connections
            For Each objStr As String In objectStrings
                If objStr.StartsWith("Connection#") Then
                    Dim data As String() = objStr.Substring(11).Split("#"c)
                    Dim color As String = data(0)
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

    Public Sub LoadFile(objectStrings As String())
        listOfPins.Clear()
        listOfConnections.Clear()
        listOfImages.Clear()
        listOfNotes.Clear()

        ' Load BoardImages first
        For Each objStr As String In objectStrings
            If objStr.StartsWith("BoardImage#") Then
                Dim data As String() = objStr.Substring(11).Split("#"c)
                Dim tag As String = data(2)
                Dim img As Bitmap
                If IO.File.Exists(tag) Then
                    img = New Bitmap(tag) With {
                        .Tag = tag
                    }
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

        ' Load Notes
        For Each objStr As String In objectStrings
            If objStr.StartsWith("Note#") Then
                Dim data As String() = objStr.Substring(5).Split("#"c)
                Dim pin As New Note(data(2), Integer.Parse(data(0)), Integer.Parse(data(1)), New Size(data(4), data(3)), data(6)) With {
                        .ID = Integer.Parse(data(5))
                    }
                listOfNotes.Add(pin)
            End If
        Next

        Dim listOfConnectibleObjects = New List(Of Object2D)
        listOfImages.ForEach(Sub(obj) listOfConnectibleObjects.Add(obj))
        listOfNotes.ForEach(Sub(obj) listOfConnectibleObjects.Add(obj))

        ' Load Pins
        For Each objStr As String In objectStrings
            If objStr.StartsWith("Pin#") Then
                Dim data As String() = objStr.Substring(4).Split("#"c)
                Dim parentID As Integer = Integer.Parse(data(2))
                Dim parent As Object2D = IterateThroughListOfObject2DToFindOneWithMatchingID(listOfConnectibleObjects, parentID)
                Dim pin As New Pin(parent, data(4), Integer.Parse(data(0)), Integer.Parse(data(1))) With {
                    .ID = Integer.Parse(data(3))
                }
                listOfPins.Add(pin)
            End If
        Next

        ' Load Connections
        For Each objStr As String In objectStrings
            If objStr.StartsWith("Connection#") Then
                Dim data As String() = objStr.Substring(11).Split("#"c)
                Dim color As String = data(0)
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
        For Each obj As Note In listOfNotes
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
            listOfImages.Add(New BoardImage(image, 0 + 100 + xOffset, 0 + 100 + yOffset, image.Tag, image.Size) With {.ID = publicLastID})
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

    Public Function GetOppositeSolidBrush(originalBrush As SolidBrush) As SolidBrush
        Dim originalColor As Color = originalBrush.Color
        Dim oppositeColor As Color = Color.FromArgb(255 - originalColor.R, 255 - originalColor.G, 255 - originalColor.B)
        Return New SolidBrush(oppositeColor)
    End Function

    Private Sub SaveTiledBackground()
        Dim bgTemp As New Bitmap(Me.Size.Width, Me.Size.Height)
        Dim bgTempGraphics As Graphics = Graphics.FromImage(bgTemp)
        For i As Integer = 0 To Math.Ceiling(Me.Size.Width / 512)
            For i2 As Integer = 0 To Math.Ceiling(Me.Size.Height / 512)
                Try
                    bgTempGraphics.DrawImageUnscaled(boardImg, i * 512, i2 * 512)
                Catch
                    bgTempGraphics.DrawString("Error!", New Font("Comic Sans MS", 16, FontStyle.Regular), Drawing.Brushes.Red, i * 512, i2 * 512)
                End Try
                bgTempGraphics.DrawString("", New Font("Comic Sans MS", 16, FontStyle.Regular), Drawing.Brushes.Red, i * 512, i2 * 512)
            Next
        Next
        bg = bgTemp.Clone()
        bgTemp.Dispose()
        bgTempGraphics.Dispose()
    End Sub

    Private Sub PrebuildBoard()
        Dim boardTemp As Bitmap = boardImg
        Dim g As Graphics = Graphics.FromImage(boardTemp)
        For x As Integer = 0 To 16
            For y As Integer = 0 To 16
                g.DrawImage(My.Resources.board, x * 32, y * 32, 32, 32)
            Next
        Next
        boardImg = boardTemp.Clone()
        boardTemp.Dispose()
        g.Dispose()
    End Sub

    Private Sub DrawAdvancedGraphics(g As Graphics)
        For Each img As BoardImage In listOfImages
            g.DrawImage(img.Image, CInt(img.X + xOffset + imageSize / 2), CInt(img.Y + yOffset + imageSize / 2), CInt(img.Sizedata.Width * imageSize), CInt(img.Sizedata.Height * imageSize))
        Next
        For Each note As Note In listOfNotes
            g.FillRectangle(note.Color, CInt(note.X + xOffset + imageSize / 2), CInt(note.Y + yOffset + imageSize / 2), CInt(note.Sizedata.Width * imageSize), CInt(note.Sizedata.Height * imageSize))
            g.DrawString(note.Note, New Font("Comic Sans MS", 16, FontStyle.Regular), GetOppositeSolidBrush(note.Color), note.X + xOffset, note.Y + yOffset)
        Next
        For Each conn As Connection In listOfConnections.ToArray()
            If conn.StartingLocation IsNot Nothing And conn.DestinationLocation IsNot Nothing And listOfPins.Exists(Function(val As Pin) val.Equals(conn.StartingLocation)) And listOfPins.Exists(Function(val As Pin) val.Equals(conn.DestinationLocation)) Then
                'g.DrawLine(conn.Color, conn.StartingLocation.X + 10 + xOffset, conn.StartingLocation.Y + 10 + yOffset, conn.DestinationLocation.X + 10 + xOffset, conn.DestinationLocation.Y + 10 + yOffset)
                g.DrawCurve(conn.Color, {
                    New PointF(conn.StartingLocation.X + 10 + xOffset, conn.StartingLocation.Y + 10 + yOffset),
                    New PointF((conn.StartingLocation.X + 10 + xOffset + conn.DestinationLocation.X + 10 + xOffset) / 2, Math.Max(conn.StartingLocation.Y + 10 + yOffset, conn.DestinationLocation.Y + 10 + yOffset) + 50),
                    New PointF(conn.DestinationLocation.X, conn.DestinationLocation.Y)
                }, 0, 2, 0.5F)
            Else
                If Not sawConnError Then
                    MessageBox.Show(String.Concat("Connection drawing error!", Environment.NewLine, "Removing all connections with errors!"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    sawConnError = True
                End If
                listOfConnections.Remove(conn)
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
        If Not isDeletingSomthing Then
            Me.Cursor = zoomCursor
        End If
        If isEditing Then
            For Each pin As Pin In listOfPins
                If Not Rectangle.Intersect(New Rectangle(pin.X, pin.Y, 20, 20), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                    startingPinTempValue = pin
                End If
            Next
            If startingPinTempValue Is Nothing Then
                For Each img As BoardImage In listOfImages
                    If Not Rectangle.Intersect(New Rectangle(img.X, img.Y, img.Sizedata.Width, img.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        imageEdited = img
                    End If
                Next
                For Each note As Note In listOfNotes
                    If Not Rectangle.Intersect(New Rectangle(note.X, note.Y, note.Sizedata.Width, note.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        imageEdited = note
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
            If Not isDeletingSomthing Then
                Me.Cursor = Cursors.Default
            Else
                Me.Cursor = Cursors.No
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        isMouseDown = False
        If isDeletingSomthing Then
            If Not didTheMouseMove Then
                For Each obj As Pin In listOfPins
                    If Not Rectangle.Intersect(New Rectangle(obj.X, obj.Y, 20, 20), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        listOfPins.Remove(obj)
                        PictureBox1.Invalidate()
                        Return
                    End If
                Next
                For Each obj As Note In listOfNotes
                    If Not Rectangle.Intersect(New Rectangle(obj.X, obj.Y, obj.Sizedata.Width, obj.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        listOfNotes.Remove(obj)
                        For Each chi As Pin In listOfPins.ToArray()
                            If chi.Parent.Equals(obj) Then
                                listOfPins.Remove(chi)
                            End If
                        Next
                        PictureBox1.Invalidate()
                        Return
                    End If
                Next
                For Each obj As BoardImage In listOfImages
                    If Not Rectangle.Intersect(New Rectangle(obj.X, obj.Y, obj.Sizedata.Width, obj.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        listOfImages.Remove(obj)
                        For Each chi As Pin In listOfPins.ToArray()
                            If chi.Parent.Equals(obj) Then
                                listOfPins.Remove(chi)
                            End If
                        Next
                        PictureBox1.Invalidate()
                        Return
                    End If
                Next
            End If
            Return
        End If
        If Not didTheMouseMove And isEditing Then
            Dim imgnumber As Integer = 0
            Dim imgimg As Object2D
            For Each img As BoardImage In listOfImages
                If Not Rectangle.Intersect(New Rectangle(img.X, img.Y, img.Sizedata.Width, img.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                    imgnumber += 1
                    imgimg = img
                End If
            Next
            For Each img As Note In listOfNotes
                If Not Rectangle.Intersect(New Rectangle(img.X, img.Y, img.Sizedata.Width, img.Sizedata.Height), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                    imgnumber += 1
                    imgimg = img
                End If
            Next
            If imgnumber = 1 Then
                listOfPins.Add(New Pin(imgimg, ToolStripTextBox1.Text, e.X + xOffset, e.Y + yOffset) With {.ID = publicLastID})
                publicLastID += 1
            ElseIf imgnumber = 0 Then
                Dim it = InputBox("Enter the note text:", "DragAndDropBoard").Replace("/nl", Environment.NewLine)
                If Not it.Equals("") Then
                    If ToolStripComboBox1.Text = "Unlimited" Then
                        listOfNotes.Add(New Note(it, e.X + xOffset, e.Y + yOffset, New Size(128, 128), ToolStripTextBox1.Text) With {.ID = publicLastID})
                    Else
                        listOfNotes.Add(New Note(it, e.X + xOffset, e.Y + yOffset, New Size(Integer.Parse(ToolStripComboBox1.Text), Integer.Parse(ToolStripComboBox1.Text)), ToolStripTextBox1.Text) With {.ID = publicLastID})
                    End If
                    publicLastID += 1
                End If
            End If
        Else
            If isEditing Then
                For Each pin As Pin In listOfPins
                    If Not Rectangle.Intersect(New Rectangle(pin.X, pin.Y, 20, 20), New Rectangle(e.Location.X - xOffset, e.Location.Y - yOffset, 0, 0)).IsEmpty Then
                        Dim i = 0
                        For Each conn As Connection In listOfConnections
                            If conn.StartingLocation.Equals(startingPinTempValue) And conn.DestinationLocation.Equals(pin) Then
                                i += 1
                                listOfConnections.Remove(conn)
                            End If
                        Next
                        If i = 0 Then
                            listOfConnections.Add(New Connection(ToolStripTextBox1.Text, startingPinTempValue, pin) With {.ID = publicLastID})
                            publicLastID += 1
                        End If
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
            buffer?.Dispose()
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
        PrebuildBoard()
        SaveTiledBackground()
        PictureBox1.Dock = DockStyle.Fill
        Me.Controls.Add(PictureBox1)
        context = BufferedGraphicsManager.Current
        If PictureBox1.Width > 0 And PictureBox1.Height > 0 Then
            buffer?.Dispose()
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
        Else
            LoadFile(main.Split("|"c))
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

    Private Sub DeleteModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteModeToolStripMenuItem.Click
        isDeletingSomthing = Not isDeletingSomthing
    End Sub
End Class
