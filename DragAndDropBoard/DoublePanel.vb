Public Class DoublePanel
    Inherits Panel
    Protected Overrides Property DoubleBuffered As Boolean
        Get
            Return True
        End Get
        Set(value As Boolean)
            Return
        End Set
    End Property
    Protected Overrides Sub InitLayout()
        MyBase.InitLayout()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer, True)
        Me.UpdateStyles()
    End Sub
End Class
