Imports System.Data.SqlClient

Partial Class Setting_Login_User_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/login", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchLoginUser")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchLoginUser") = txtSearch.Text
        Response.Redirect("~/setting/login/user/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
        Session("SearchLoginUser") = txtSearch.Text
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindData(txtSearch.Text)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdActive.Text

            Dim active As Integer = 1
            If txtActive.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Login Has Been Activated"
            If active = 0 Then activeDesc = "Login Has Been Deactivated"

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Session("SearchLoginUser") = txtSearch.Text
            Response.Redirect("~/setting/login/user", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logins WHERE Id=@Id; UPDATE Logs SET ActionBy=NULL WHERE ActionBy=@Id; UPDATE OrderHeaders SET CreatedBy=NULL WHERE CreatedBy=@Id; DELETE FROM Sessions WHERE LoginId=@Id; UPDATE Notifications SET LoginId = LTRIM(RTRIM(REPLACE(',' + LoginId + ',', ',' + @Id + ',', ',' ))) WHERE ',' + LoginId + ',' LIKE '%,' + @Id + ',%'; DELETE FROM NotificationLogs WHERE LoginId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Login Deleted"}
            settingClass.Logs(dataLog)

            Session("SearchLoginUser") = txtSearch.Text
            Response.Redirect("~/setting/login/user", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtChangePassword.Text) Then
                Dim thisId As String = txtIdChangePassword.Text
                Dim newPassword As String = settingClass.Encrypt(txtChangePassword.Text)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Change Password Login"}
                settingClass.Logs(dataLog)

                Session("SearchLoginUser") = txtSearch.Text
                Response.Redirect("~/setting/login/user", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetPass_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdResetPass.Text
            Dim newPassword As String = settingClass.Encrypt(txtNewResetPass.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, ResetLogin=1 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Password", newPassword)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            '

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Login Reset Password"}
            settingClass.Logs(dataLog)

            Session("SearchLoginUser") = txtSearch.Text
            Response.Redirect("~/setting/login/user", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchLoginUser") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), "", searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString())
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_LoginList", params)
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
    End Sub

    Protected Function DencryptPassword(password As String) As String
        Dim result As String = settingClass.Decrypt(password)
        Return result
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

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
