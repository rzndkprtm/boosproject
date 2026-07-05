Imports System.Data.SqlClient

Partial Class Setting_Database_View_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database/view/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrWhiteSpace(txtQuery.Text) Then
                Dim viewName As String = txtName.Text.Trim().Replace("]", "]]")

                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using thisCheck As New SqlCommand("SELECT COUNT(*) FROM sys.views WHERE name = @Name", thisConn)
                        thisCheck.Parameters.AddWithValue("@Name", viewName)
                        If CInt(thisCheck.ExecuteScalar()) > 0 Then
                            MessageError(True, "SUDAH ADA !")
                            Exit Sub
                        End If
                    End Using

                    Dim sql As String = "CREATE VIEW [dbo].[" & viewName & "] AS " & vbCrLf & txtQuery.Text
                    Using thisCmd As New SqlCommand(sql, thisConn)
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/setting/database/view", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/database/view", False)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
