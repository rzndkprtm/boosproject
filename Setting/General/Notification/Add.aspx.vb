Imports System.Data.SqlClient

Partial Class Setting_General_Notification_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/notification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindLoginRole()
            BindCompany()
            BindLoginId(ddlLoginRole.SelectedValue, ddlCompany.SelectedValue)
            VisibleCompany(ddlLoginRole.SelectedValue)
        End If
    End Sub

    Protected Sub ddlLoginRole_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindLoginId(ddlLoginRole.SelectedValue, ddlCompany.SelectedValue)
        VisibleCompany(ddlLoginRole.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindLoginId(ddlLoginRole.SelectedValue, ddlCompany.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlLoginRole.SelectedValue = "" Then
                MessageError(True, "ROLE IS REQUIRED !")
                Exit Sub
            End If
            If txtStartDate.Text = "" Then
                MessageError(True, "START DATE IS REQUIRED !")
                Exit Sub
            End If
            If txtEndDate.Text = "" Then
                MessageError(True, "END DATE IS REQUIRED !")
                Exit Sub
            End If
            If txtTitle.Text = "" Then
                MessageError(True, "TITLE IS REQUIRED !")
                Exit Sub
            End If

            Dim htmlContent As String = fieldMessage.Value
            If htmlContent = "" Then
                MessageError(True, "MESSAGE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Notifications ORDER BY Id DESC")

                If Not ddlLoginRole.SelectedValue = "8" Then ddlCompany.SelectedValue = ""

                Dim loginId As String = String.Empty
                If Not String.IsNullOrEmpty(lbLoginId.SelectedValue) Then
                    loginId = String.Join(",", lbLoginId.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If
                If String.IsNullOrEmpty(lbLoginId.SelectedValue) Then
                    If Not String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        loginId = settingClass.GetItemData("SELECT STRING_AGG(CAST(Logins.Id AS VARCHAR), ',') AS Ids FROM Logins LEFT JOIN Customers ON Logins.CustomerId=Customers.Id WHERE Logins.RoleId='" & ddlLoginRole.SelectedValue & "' AND Customers.CompanyId='" & ddlCompany.SelectedValue & "'")
                    End If
                    If String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        loginId = settingClass.GetItemData("SELECT STRING_AGG(CAST(Id AS VARCHAR), ',') AS Ids FROM Logins WHERE RoleId='" & ddlLoginRole.SelectedValue & "'")
                    End If
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Notifications VALUES (@Id, @RoleId, @CompanyId, @LoginId, @Title, @Message, @StartDate, @EndDate, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@RoleId", ddlLoginRole.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)
                        myCmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Message", htmlContent)
                        myCmd.Parameters.AddWithValue("@StartDate", txtStartDate.Text.Trim())
                        myCmd.Parameters.AddWithValue("@EndDate", txtEndDate.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Notifications", thisId, Session("LoginId").ToString(), "Notification Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/general/notification", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/notification", False)
    End Sub

    Protected Sub BindLoginRole()
        ddlLoginRole.Items.Clear()
        Try
            ddlLoginRole.DataSource = settingClass.GetDataTable("SELECT * FROM LoginRoles ORDER BY Name ASC")
            ddlLoginRole.DataTextField = "Name"
            ddlLoginRole.DataValueField = "Id"
            ddlLoginRole.DataBind()

            If ddlLoginRole.Items.Count > 0 Then
                ddlLoginRole.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlLoginRole.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT * FROM Companys ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindLoginId(roleId As String, companyId As String)
        lbLoginId.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(roleId) Then
                Dim thisString As String = "SELECT Logins.* FROM Logins WHERE Logins.RoleId='" & roleId & "' ORDER BY Logins.UserName ASC"
                If Not String.IsNullOrEmpty(companyId) Then
                    thisString = "SELECT Logins.* FROM Logins LEFT JOIN Customers ON Logins.CustomerId=Customers.Id WHERE Logins.RoleId='" & roleId & "' AND Customers.CompanyId='" & companyId & "' ORDER BY Logins.UserName ASC"
                End If

                lbLoginId.DataSource = settingClass.GetDataTable(thisString)
                lbLoginId.DataTextField = "FullName"
                lbLoginId.DataValueField = "Id"
                lbLoginId.DataBind()

                If lbLoginId.Items.Count > 0 Then
                    lbLoginId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbLoginId.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub VisibleCompany(roleId As String)
        divCompany.Visible = False
        If roleId = "8" Then divCompany.Visible = True
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
