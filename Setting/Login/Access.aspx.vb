Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Login_Access
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/login", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchAccess")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchAccess") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Login Access"

            BindRole()
            BindLevel()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchAccess") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Login Access"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM LoginAccess WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindRole()
                    BindLevel()

                    ddlRoleId.SelectedValue = myData("RoleId").ToString()
                    ddlLevelId.SelectedValue = myData("LevelId").ToString()
                    txtPage.Text = myData("Page").ToString()
                    txtAction.Text = myData("Action").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlRoleId.SelectedValue = "" Then
                MessageError_Process(True, "ROLE NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlLevelId.SelectedValue = "" Then
                MessageError_Process(True, "LEVEL ACCESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtPage.Text = "" Then
                MessageError_Process(True, "PAGE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtAction.Text = "" Then
                MessageError_Process(True, "ACTION IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO LoginAccess VALUES (NEWID(), @RoleId, @LevelId, @Page, @Action, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@RoleId", ddlRoleId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLevelId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Action", txtAction.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Session("SearchAccess") = txtSearch.Text
                    Response.Redirect("~/setting/login/access", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE LoginAccess SET RoleId=@RoleId, LevelId=@LevelId, Page=@Page, Action=@Action, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@RoleId", ddlRoleId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLevelId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Action", txtAction.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Session("SearchAccess") = txtSearch.Text
                    Response.Redirect("~/setting/login/access", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM LoginAccess WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchAccess") = txtSearch.Text
            Response.Redirect("~/setting/login/access", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchAccess") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = " WHERE LoginAccess.Id LIKE '%" & searchText.Trim() & "%' OR LoginAccess.Page LIKE '%" & searchText.Trim() & "%' OR LoginRoles.Name LIKE '%" & searchText.Trim() & "%' OR LoginLevels.Name LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisString As String = String.Format("SELECT LoginAccess.*, LoginRoles.Name AS RoleName, LoginLevels.Name AS LevelName, CASE WHEN LoginAccess.Active=1 THEN 'Yes' WHEN LoginAccess.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM LoginAccess LEFT JOIN LoginRoles ON LoginAccess.RoleId=LoginRoles.Id LEFT JOIN LoginLevels ON LoginAccess.LevelId=LoginLevels.Id {0} ORDER BY LoginAccess.RoleId, LoginAccess.LevelId, LoginAccess.Page ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindRole()
        ddlRoleId.Items.Clear()
        Try
            ddlRoleId.DataSource = settingClass.GetDataTable("SELECT * FROM LoginRoles ORDER BY Name ASC")
            ddlRoleId.DataTextField = "Name"
            ddlRoleId.DataValueField = "Id"
            ddlRoleId.DataBind()

            If ddlRoleId.Items.Count > 1 Then
                ddlRoleId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindLevel()
        ddlLevelId.Items.Clear()
        Try
            ddlLevelId.DataSource = settingClass.GetDataTable("SELECT * FROM LoginLevels ORDER BY Name ASC")
            ddlLevelId.DataTextField = "Name"
            ddlLevelId.DataValueField = "Id"
            ddlLevelId.DataBind()

            If ddlLevelId.Items.Count > 1 Then
                ddlLevelId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
