Imports System.Data
Imports System.Data.SqlClient

Public Class ActionClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetActionAccess(roleId As String, levelId As String, page As String, action As String) As Boolean
        If String.IsNullOrEmpty(roleId) OrElse String.IsNullOrEmpty(levelId) OrElse
           String.IsNullOrEmpty(page) OrElse String.IsNullOrEmpty(action) Then Return False
        Try
            Using thisConn As New SqlConnection(myConn)
                Using cmd As New SqlCommand("sp_GetActionAccess", thisConn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@RoleId", roleId)
                    cmd.Parameters.AddWithValue("@LevelId", levelId)
                    cmd.Parameters.AddWithValue("@Page", page)
                    cmd.Parameters.AddWithValue("@Action", action)

                    thisConn.Open()
                    Return Convert.ToInt32(cmd.ExecuteScalar()) = 1
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
End Class
