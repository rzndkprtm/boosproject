Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Action
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchAction")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchAction") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Action Access"

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
            Session("SearchAction") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Action Access"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Actions WHERE Id='" & lblId.Text & "'")
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
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Actions VALUES (NEWID(), @RoleId, @LevelId, @Page, @Action, @Description, @Active)", thisConn)
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

                    Session("ActionSearch") = txtSearch.Text
                    Response.Redirect("~/setting/general/action", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Actions SET RoleId=@RoleId, LevelId=@LevelId, Page=@Page, Action=@Action, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
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

                    Session("SearchAction") = txtSearch.Text
                    Response.Redirect("~/setting/general/action", False)
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
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Actions WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchAction") = txtSearch.Text
            Response.Redirect("~/setting/general/action", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchAction") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = " WHERE Actions.Id LIKE '%" & searchText.Trim() & "%' OR Actions.Page LIKE '%" & searchText.Trim() & "%' OR Actions.Action LIKE '%" & searchText.Trim() & "%' OR Actions.Description LIKE '%" & searchText.Trim() & "%' OR LoginRoles.Name LIKE '%" & searchText.Trim() & "%' OR LoginLevels.Name LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisString As String = String.Format("SELECT Actions.*, LoginRoles.Name AS RoleName, LoginLevels.Name AS LevelName, CASE WHEN Actions.Active=1 THEN 'Yes' WHEN Actions.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Actions LEFT JOIN LoginRoles ON Actions.RoleId=LoginRoles.Id LEFT JOIN LoginLevels ON Actions.LevelId=LoginLevels.Id {0} ORDER BY Actions.RoleId, Actions.LevelId, Actions.Page ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()

            btnAdd.Visible = PageAction("Add")
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
