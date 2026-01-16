Partial Class Setting_UpdateSession
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim keys As List(Of String) = Session.Keys.Cast(Of String)().ToList()
        Dim updatedSessions As New List(Of String)

        For Each key As String In keys
            If Session(key) IsNot Nothing Then
                Session(key) = Session(key)
                updatedSessions.Add(key)
            End If
        Next
    End Sub
End Class
