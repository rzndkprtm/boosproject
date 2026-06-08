Imports System.Data.SqlClient

Partial Class Setting_Database_Function_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database/function/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("spname")) Then
            Response.Redirect("~/setting/database/function/", False)
            Exit Sub
        End If

        txtName.Text = Request.QueryString("spname").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtName.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtQuery.Text) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(txtQuery.Text, thisConn)
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/setting/database/function", False)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/database/function", False)
    End Sub

    Protected Sub BindData(functionName As String)
        Try
            Dim query As String = "SELECT OBJECT_DEFINITION(OBJECT_ID('" & functionName.Replace("'", "''") & "'))"

            Dim spText As String = settingClass.GetItemData(query)
            If Not String.IsNullOrEmpty(spText) Then
                spText = Regex.Replace(spText, "\bCREATE\s+PROCEDURE\b", "ALTER PROCEDURE", RegexOptions.IgnoreCase)
                spText = Regex.Replace(spText, "\bCREATE\s+PROC\b", "ALTER PROCEDURE", RegexOptions.IgnoreCase)

                txtQuery.Text = spText
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
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
