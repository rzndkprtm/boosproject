Imports System.Data.SqlClient

Public Class ActionClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetActionAccess(RoleId As String, LevelId As String, Page As String, Action As String) As Boolean
        Dim result As Boolean = False
        If String.IsNullOrEmpty(RoleId) OrElse String.IsNullOrEmpty(LevelId) OrElse String.IsNullOrEmpty(Page) OrElse String.IsNullOrEmpty(Action) Then Return False

        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("SELECT * FROM Actions WHERE RoleId=@RoleId AND LevelId=@LevelId AND Page=@Page AND Action=@Action AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@RoleId", RoleId)
                    myCmd.Parameters.AddWithValue("@LevelId", LevelId)
                    myCmd.Parameters.AddWithValue("@Page", Page)
                    myCmd.Parameters.AddWithValue("@Action", Action)

                    thisConn.Open()
                    Dim obj = myCmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        result = True
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function
End Class
