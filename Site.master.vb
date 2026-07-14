Imports System.Data
Imports System.Data.SqlClient

Partial Public Class SiteMaster
    Inherits MasterPage

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        '
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If IsHeartbeatRequest() Then Exit Sub
            If IsAjaxRequest() Then Exit Sub

            CheckSessions()

            If Not IsPostBack Then
                MyLoad()
                BindListNavigation()
            End If
        Catch ex As Exception
            Session.Clear()
            Response.Redirect("~/account/login", False)
            Context.ApplicationInstance.CompleteRequest()
        End Try
    End Sub

    Protected Sub CheckSessions()
        Try
            If Session("IsLoggedIn") Is Nothing OrElse Session("IsLoggedIn") = False Then
                HandleRedirectLogin()
                Exit Sub
            End If

            Dim sessionId As String = ""

            If Request.Cookies("deviceId") Is Nothing Then
                HandleRedirectLogin()
                Exit Sub
            End If

            sessionId = Request.Cookies("deviceId").Value

            Dim checkData As DataRow = settingClass.GetDataRow("SELECT 1 FROM Sessions WHERE Id='" & UCase(sessionId) & "' AND LoginId='" & Session("LoginId") & "'")

            If checkData Is Nothing Then
                HandleRedirectLogin()
                Exit Sub
            End If
        Catch ex As Exception
            HandleRedirectLogin()
        End Try
    End Sub

    Protected Sub HandleRedirectLogin()
        Session.Clear()
        Response.Redirect("~/account/login", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Protected Function IsAjaxRequest() As Boolean
        Return Request.Headers("X-Requested-With") = "XMLHttpRequest"
    End Function

    Protected Function IsHeartbeatRequest() As Boolean
        Return Request.Url.AbsolutePath.ToLower().Contains("updatesession")
    End Function

    Protected Sub linkLogout_Click(sender As Object, e As EventArgs)
        Dim sessionId As String = String.Empty

        If Request.Cookies("deviceId") IsNot Nothing Then
            sessionId = Request.Cookies("deviceId").Value
            settingClass.DeleteSession(sessionId)
        End If

        Session.Clear()
        Response.Redirect("~/account/login", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Protected Sub MyLoad()
        Try
            Dim loginId As String = Session("LoginId")

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@LoginId", loginId)
            }
            Dim myData As DataRow = settingClass.GetDataRowSP("sp_Logins_Profile", params)
            If myData Is Nothing Then
                HandleRedirectLogin()
                Exit Sub
            End If

            Session("CustomerId") = myData("CustomerId").ToString()
            Session("CustomerLevel") = myData("CustomerLevel").ToString()
            Session("RoleId") = myData("RoleId").ToString()
            Session("LevelId") = myData("LevelId").ToString()
            Session("FullName") = myData("FullName").ToString()
            Session("PersonalEmail") = myData("Email").ToString()
            Session("ResetLogin") = myData("ResetLogin")
            Session("PriceAccess") = myData("PriceAccess").ToString()
            Session("CompanyId") = myData("CompanyId").ToString()
            Session("CompanyDetailId") = myData("CompanyDetailId").ToString()
            Session("RoleName") = myData("RoleName").ToString()
            Session("LevelName") = myData("LevelName").ToString()
            Session("CompanyName") = myData("CompanyName").ToString()

            Dim loginEmail As String = myData("Email").ToString()
            Dim loginActive As Boolean = myData("Active")
            Dim roleName As String = myData("RoleName").ToString()
            Dim roleActive As Boolean = myData("RoleActive")
            Dim levelActive As Boolean = myData("LevelActive")
            Dim resetLogin As Integer = Convert.ToInt32(myData("ResetLogin"))

            Dim companyActive As Integer = If(IsDBNull(myData("CompanyActive")), -1, Convert.ToInt32(myData("CompanyActive")))
            Dim customerActive As Integer = If(IsDBNull(myData("CustomerActive")), -1, Convert.ToInt32(myData("CustomerActive")))

            Dim path As String = Request.Url.AbsolutePath.ToLower()

            If loginActive = False AndAlso Not path.Contains("/boos/nonactive") Then
                Response.Redirect("~/boos/nonactive", False)
                Context.ApplicationInstance.CompleteRequest()
                Exit Sub
            End If

            If roleActive = False AndAlso Not path.Contains("/boos/nonactive") Then
                Response.Redirect("~/boos/nonactive", False)
                Context.ApplicationInstance.CompleteRequest()
                Exit Sub
            End If

            If levelActive = False AndAlso Not path.Contains("/boos/nonactive") Then
                Response.Redirect("~/boos/nonactive", False)
                Context.ApplicationInstance.CompleteRequest()
                Exit Sub
            End If

            If customerActive = 0 AndAlso Not path.Contains("/boos/nonactive") Then
                Response.Redirect("~/boos/nonactive", False)
                Context.ApplicationInstance.CompleteRequest()
                Exit Sub
            End If

            If companyActive = 0 AndAlso Not path.Contains("/boos/maintenance") Then
                Response.Redirect("~/boos/maintenance", False)
                Context.ApplicationInstance.CompleteRequest()
                Exit Sub
            End If

            If resetLogin = 1 AndAlso Not path.StartsWith("/account/password") Then
                Response.Redirect("~/account/password", False)
                Context.ApplicationInstance.CompleteRequest()
                Return
            End If

            If roleName = "Customer" Then
                If String.IsNullOrEmpty(loginEmail) AndAlso Not Request.AppRelativeCurrentExecutionFilePath.ToLower() = "~/account/default.aspx" Then
                    Response.Redirect("~/account/?uid=" & Session("LoginId").ToString(), False)
                    Exit Sub
                End If
            End If

            imgLogo.ImageUrl = "~/Assets/images/logo/general.jpg?v=1.0.0"
            If Session("CompanyId") = "2" Then
                imgLogo.ImageUrl = "~/Assets/images/logo/jpmdirect.jpg?v=1.0.0"
            ElseIf Session("CompanyId") = "3" Then
                imgLogo.ImageUrl = "~/Assets/images/logo/bigblinds.png?v=1.0.0"
            ElseIf Session("CompanyId") = "4" Then
                imgLogo.ImageUrl = "~/Assets/images/logo/sunlight.jpg?v=1.0.0"
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("sp_Logins_Update_LastLogin", thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = loginId
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            HandleRedirectLogin()
        End Try
    End Sub

    Protected Sub BindListNavigation()
        Try
            liOldOrder.Visible = False
            liGuide.Visible = False
            liTicket.Visible = False
            liExport.Visible = False
            liReport.Visible = False
            liSales.Visible = False
            liStocks.Visible = False
            liQuotation.Visible = False

            liSetting.Visible = False
            liDashboard.Visible = False
            liGeneral.Visible = False

            liLogin.Visible = False
            liLoginRole.Visible = False
            liLoginLevel.Visible = False
            liLoginAccess.Visible = False

            liCustomer.Visible = False
            liSpecification.Visible = False
            liTubeType.Visible = False
            liControlType.Visible = False
            liColourType.Visible = False
            liJob.Visible = False
            liPrice.Visible = False
            liDatabase.Visible = False
            liXero.Visible = False
            liAKZero.Visible = False
            liLog.Visible = False

            If Session("RoleName") = "Developer" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liTicket.Visible = True
                liExport.Visible = True
                liReport.Visible = True
                liSales.Visible = True
                liStocks.Visible = True

                liSetting.Visible = True
                liDashboard.Visible = True
                liGeneral.Visible = True

                liLogin.Visible = True
                liLoginRole.Visible = True
                liLoginLevel.Visible = True
                liLoginAccess.Visible = True

                liCustomer.Visible = True
                liSpecification.Visible = True
                liTubeType.Visible = True : liControlType.Visible = True : liColourType.Visible = True
                liJob.Visible = True
                liPrice.Visible = True
                liDatabase.Visible = True
                liXero.Visible = True
                liAKZero.Visible = True
                liLog.Visible = True
            End If
            If Session("RoleName") = "IT" Then
                liOldOrder.Visible = True
                liExport.Visible = True
                liReport.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True
                liSales.Visible = True
                liStocks.Visible = True

                liSetting.Visible = True
                liDashboard.Visible = True
                liGeneral.Visible = True
                liLogin.Visible = True
                liCustomer.Visible = True
                liSpecification.Visible = True
                liJob.Visible = True
                liPrice.Visible = True
                liDatabase.Visible = True
                liXero.Visible = True
                liAKZero.Visible = True
                liLog.Visible = True
            End If
            If Session("RoleName") = "Factory Office" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liReport.Visible = True
                liTicket.Visible = True
                liSales.Visible = True
                liStocks.Visible = True

                liSetting.Visible = True
                liDashboard.Visible = True
                liGeneral.Visible = True
                liLogin.Visible = True
                liCustomer.Visible = True
                liSpecification.Visible = True
                liJob.Visible = True
                liPrice.Visible = True
                liDatabase.Visible = True
                liXero.Visible = True
                liAKZero.Visible = True
                liLog.Visible = True
            End If
            If Session("RoleName") = "Account" Then
                liOldOrder.Visible = True
                liSales.Visible = True
                liTicket.Visible = True

                liSetting.Visible = True
                liCustomer.Visible = True
                liPrice.Visible = True
                liXero.Visible = True
            End If
            If Session("RoleName") = "Sales" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liStocks.Visible = True
                liTicket.Visible = True

                If Session("LevelName") = "Leader" Then
                    liSales.Visible = True
                    liReport.Visible = True
                End If

                liSetting.Visible = True
                liCustomer.Visible = True
                liPrice.Visible = True
                liXero.Visible = True
            End If
            If Session("RoleName") = "Data Entry" Then
                liOldOrder.Visible = True
                liStocks.Visible = True
                liReport.Visible = True
                liTicket.Visible = True
            End If
            If Session("RoleName") = "Customer" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liStocks.Visible = True
                liQuotation.Visible = True
            End If
        Catch ex As Exception
            HandleRedirectLogin()
        End Try
    End Sub
End Class