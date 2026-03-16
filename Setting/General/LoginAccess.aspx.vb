Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_General_LoginAccess
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTab") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not Session("selectedTab") = "" Then
            selected_tab.Value = Session("selectedTab").ToString()
        End If

        If Not IsPostBack Then
            MessageError_Role(False, String.Empty)
            MessageError_Level(False, String.Empty)

            BindDataRole()
            BindDataLevel()
        End If
    End Sub

    Protected Sub gvListRole_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Session("selectedTab") = "list-role"
        MessageError_Role(False, String.Empty)
        Try
            gvListRole.PageIndex = e.NewPageIndex
            BindDataRole()
        Catch ex As Exception
            MessageError_Role(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Role(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvListRole_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Session("selectedTab") = "list-role"
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    lblType.Text = "Role"
                    titleProcess.InnerText = "Edit Role Access"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM LoginRoles WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtName.Text = myData("Name").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddRole_Click(sender As Object, e As EventArgs)
        Session("selectedTab") = "list-role"
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "Role"
            titleProcess.InnerText = "Add Role Access"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListLevel_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Session("selectedTab") = "list-level"
        MessageError_Level(False, String.Empty)
        Try
            gvListLevel.PageIndex = e.NewPageIndex
            BindDataLevel()
        Catch ex As Exception
            MessageError_Level(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Level(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvListLevel_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Session("selectedTab") = "list-level"
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    lblType.Text = "Level"
                    titleProcess.InnerText = "Edit Level Access"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM LoginLevels WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtName.Text = myData("Name").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddLevel_Click(sender As Object, e As EventArgs)
        Session("selectedTab") = "list-level"
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "Level"
            titleProcess.InnerText = "Add Level Access"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "ROLE NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisString As String = String.Empty
                Dim thisLog As String = String.Empty

                If lblAction.Text = "Add" Then
                    Dim thisId As String = String.Empty

                    If lblType.Text = "Role" Then
                        thisId = settingClass.CreateId("SELECT TOP 1 Id FROM LoginRoles ORDER BY Id DESC")
                        thisScript = "INSERT INTO LoginRoles VALUES (@Id, @Name, @Description, @Active)"
                        thisLog = "LoginRoles"
                    End If
                    If lblType.Text = "Level" Then
                        thisId = settingClass.CreateId("SELECT TOP 1 Id FROM LoginLevels ORDER BY Id DESC")
                        thisString = "INSERT INTO LoginLevels VALUES (@Id, @Name, @Description, @Active)"
                        thisLog = "LoginLevels"
                    End If

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(thisString, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {thisLog, thisId, Session("LoginId").ToString(), "Created"}
                    settingClass.Logs(dataLog)

                    Response.Redirect("~/setting/general/loginaccess", False)
                End If

                If lblAction.Text = "Edit" Then
                    If lblType.Text = "Role" Then
                        thisString = "UPDATE LoginRoles SET Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id"
                        thisLog = "LoginRoles"
                    End If
                    If lblType.Text = "Level" Then
                        thisString = "UPDATE LoginLevels SET Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id"
                        thisLog = "LoginLevels"
                    End If

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(thisString, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {thisLog, lblId.Text, Session("LoginId").ToString(), "Updated"}
                    settingClass.Logs(dataLog)

                    Response.Redirect("~/setting/general/loginaccess", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub BindDataRole()
        Try
            gvListRole.DataSource = settingClass.GetDataTable("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM LoginRoles ORDER BY Name ASC")
            gvListRole.DataBind()
            gvListRole.Columns(1).Visible = PageAction("Visible ID")

            btnAddRole.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError_Role(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Role(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataLevel()
        Try
            gvListLevel.DataSource = settingClass.GetDataTable("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM LoginLevels ORDER BY Name ASC")
            gvListLevel.DataBind()
            gvListLevel.Columns(1).Visible = PageAction("Visible ID")

            btnAddLevel.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError_Level(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Level(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError_Role(visible As Boolean, message As String)
        divErrorRole.Visible = visible : msgErrorRole.InnerText = message
    End Sub

    Protected Sub MessageError_Level(visible As Boolean, message As String)
        divErrorLevel.Visible = visible : msgErrorLevel.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
