Imports System.Data

Partial Class Setting_General_Newsletter_Preview
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("id")) Then
            Response.Redirect("~/setting/general/newsletter", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("id").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub BindData(newsletterId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE Id='" & newsletterId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/general/newsletter", False)
                Exit Sub
            End If

            imgNewsletter.ImageUrl = thisData("Link").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
