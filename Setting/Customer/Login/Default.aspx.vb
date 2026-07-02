Imports System.Data.SqlClient

Partial Class Setting_Customer_Login_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            txtSearch.Text = Session("SearchCustomerLogin")

            MessageError(False, String.Empty)
            MessageError_Send(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchCustomerLogin") = txtSearch.Text
        Response.Redirect("~/setting/customer/login/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchCustomerLogin") = txtSearch.Text
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtActiveId.Text

            Dim active As Integer = 1
            If txtActiveStatus.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Active=@Active WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Active", active)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Login Has Been Activated"
            If active = 0 Then activeDesc = "Login Has Been Deactivated"

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Session("SearchCustomerLogin") = txtSearch.Text
            Response.Redirect("~/setting/customer/login", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSend_Click(sender As Object, e As EventArgs)
        MessageError_Send(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSend(); };"
        Try
            Dim thisId As String = txtSendId.Text
            Dim thisEmail As String = txtSendEmail.Text

            If String.IsNullOrEmpty(thisEmail) Then
                MessageError_Send(True, "EMAIL ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSend", thisScript, True)
                Exit Sub
            End If

            Dim isValidEmail As Boolean = False
            Try
                Dim addr As New Net.Mail.MailAddress(thisEmail)
                isValidEmail = (addr.Address = thisEmail)
            Catch
                isValidEmail = False
            End Try

            If Not isValidEmail Then
                MessageError_Send(True, "PLEASE ENTER A VALID EMAIL ADDRESS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSend", thisScript, True)
                Exit Sub
            End If

            If thisEmail = Session("PersonalEmail") Then
                MessageError_Send(True, "YOU DO NOT HAVE THE AUTHORITY TO HAVE THESE LOGIN CREDENTIALS SENT TO YOUR EMAIL ADDRESS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSend", thisScript, True)
                Exit Sub
            End If

            Dim mailingClass As New MailingClass
            mailingClass.PersonalLogin(thisId, thisEmail, Session("LoginId").ToString())

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Send Personal Login"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerLogin") = txtSearch.Text
            Response.Redirect("~/setting/customer/login", False)
        Catch ex As Exception
            MessageError_Send(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Send(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showSend", thisScript, True)
        End Try
    End Sub

    Protected Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtChangePassword.Text) Then
                Dim thisId As String = txtChangePasswordId.Text
                Dim newPassword As String = settingClass.Encrypt(txtChangePassword.Text)

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Password", newPassword)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Change Password Login"}
                settingClass.Logs(dataLog)

                Session("SearchCustomerLogin") = txtSearch.Text
                Response.Redirect("~/setting/customer/login", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetPassword_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtResetPasswordId.Text
            Dim newPassword As String = settingClass.Encrypt(txtResetPasswordNew.Text)

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, ResetLogin=1 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Password", newPassword)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Login Reset Password"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerLogin") = txtSearch.Text
            Response.Redirect("~/setting/customer/login", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerLogin") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerLogins_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Function DencryptPassword(password As String) As String
        Dim result As String = settingClass.Decrypt(password)
        Return result
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Send(visible As Boolean, message As String)
        divErrorSend.Visible = visible : msgErrorSend.InnerText = message
    End Sub

    Protected Function VisibleSend(active As Integer) As Boolean
        If active = 1 Then Return True
        Return False
    End Function

    Protected Function TextActive(active As Boolean) As String
        Dim result As String = "Enable"
        If active = True Then : Return "Disable" : End If
        Return result
    End Function

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
