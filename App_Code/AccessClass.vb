Imports System.Data
Imports System.Data.SqlClient

Public Class AccessClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetLoginAccess(roleId As String, levelId As String, page As String, action As String) As Boolean
        If String.IsNullOrEmpty(roleId) OrElse String.IsNullOrEmpty(levelId) OrElse
           String.IsNullOrEmpty(page) OrElse String.IsNullOrEmpty(action) Then Return False
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("sp_GetLoginAccess", thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    thisCmd.Parameters.AddWithValue("@RoleId", roleId)
                    thisCmd.Parameters.AddWithValue("@LevelId", levelId)
                    thisCmd.Parameters.AddWithValue("@Page", page)
                    thisCmd.Parameters.AddWithValue("@Action", action)
                    thisConn.Open()
                    Return Convert.ToInt32(thisCmd.ExecuteScalar()) = 1
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
End Class
