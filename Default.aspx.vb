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
            secDefault.Visible = True

            If Session("CompanyId") = "3" Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Newsletters WHERE CompanyId='2' AND Status='Active'")

            If thisData Is Nothing Then
                secDefault.Visible = False
                Exit Sub
            End If

            Dim newsletterType As String = thisData("Type").ToString().Trim()
            Dim newsletterLink As String = thisData("Link").ToString().Trim()

            imgNewsletter.Visible = False
            litNewsletter.Visible = False

            Select Case newsletterType.ToUpper()
                Case "LINK"
                    litNewsletter.Text = "<iframe src=""" & Server.HtmlEncode(newsletterLink) & """ style=""width:100%; height:85vh; border:0;"" loading=""lazy""></iframe>"
                    litNewsletter.Visible = True

                Case "UPLOAD"
                    imgNewsletter.ImageUrl = newsletterLink
                    imgNewsletter.Visible = True

                Case Else
                    secDefault.Visible = False
            End Select

        Catch ex As Exception
        End Try
    End Sub
End Class