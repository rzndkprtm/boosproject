Imports System.Data
Imports System.Data.SqlClient

Public Class ActionClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetActionAccess(RoleId As String, LevelId As String, Page As String, Action As String) As Boolean
        If String.IsNullOrEmpty(RoleId) OrElse String.IsNullOrEmpty(LevelId) OrElse
           String.IsNullOrEmpty(Page) OrElse String.IsNullOrEmpty(Action) Then Return False
        Try
            Using thisConn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("sp_GetActionAccess", thisConn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@RoleId", RoleId)
                    cmd.Parameters.AddWithValue("@LevelId", LevelId)
                    cmd.Parameters.AddWithValue("@Page", Page)
                    cmd.Parameters.AddWithValue("@Action", Action)

                    thisConn.Open()
                    Return Convert.ToInt32(cmd.ExecuteScalar()) = 1
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
End Class
