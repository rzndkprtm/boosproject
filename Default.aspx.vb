Imports System.Data

Partial Class _Default
    Inherits Page
    
    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Protected Sub BindData()
        Try
            If Session("CompanyId") = "3" OrElse Session("CompanyId") = "5" Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            secDefault.Visible = True
            secNewsletter.Visible = False

            If String.IsNullOrEmpty(Session("CompanyId").ToString()) OrElse Session("CompanyId") = "2" Then
                secDefault.Visible = False
                secNewsletter.Visible = True
                Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE CompanyId='2' AND Active=1")
                If thisData Is Nothing Then
                    Exit Sub
                End If
                imgNewsletter.ImageUrl = thisData("Link").ToString()
            End If
        Catch ex As Exception
            If Session("RoleName") = "Developer" Then
                lblError.Text = ex.ToString()
            End If
        End Try
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