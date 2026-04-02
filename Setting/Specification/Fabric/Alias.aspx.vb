Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Alias
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtSearch.Text = Session("SearchFabricAlias")

            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchFabricAlias") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "Fabrics"
            titleProcess.InnerText = "Add Fabric Alias"

            BindFabric(lblType.Text)

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchFabricAlias") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "FabricColours"
            titleProcess.InnerText = "Add Fabric Colour Alias"

            BindFabric(lblType.Text)

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            Session("SearchFabricAlias") = txtSearch.Text
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"


                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricAlias WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    lblType.Text = myData("Type").ToString()
                    BindFabric(lblType.Text)
                    titleProcess.InnerText = "Edit Alias"

                    ddlFirstId.SelectedValue = myData("FirstID").ToString()
                    ddlSecondId.SelectedValue = myData("SecondID").ToString()

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

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlFirstId.SelectedValue = "" Then
                MessageError_Process(True, "NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlSecondId.SelectedValue = "" Then
                MessageError_Process(True, "NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM FabricAlias ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO FabricAlias VALUES (@Id, @Type, @FirstId, @SecondId)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Type", lblType.Text)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricAlias", thisId, Session("LoginId").ToString(), "Alias Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricAlias") = txtSearch.Text
                    Response.Redirect("~/setting/specification/fabric/alias", False)
                End If

                If lblAction.Text = "Edit" Then
                    Dim stringUpdate As String = String.Format("UPDATE FabricAlias SET FirstId=@FirstId, SecondId=@SecondId WHERE Id=@Id", lblType.Text)
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(stringUpdate, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricAlias", lblId.Text, Session("LoginId").ToString(), "Alias Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricAlias") = txtSearch.Text
                    Response.Redirect("~/setting/specification/fabric/alias", False)
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

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM FabricAlias WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='FabricAlias' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchFabricAlias") = txtSearch.Text
            Response.Redirect("~/setting/specification/fabric/alias", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchFabricAlias") = String.Empty
        Try
            Dim stringSearch As String = String.Empty
            If Not searchText = "" Then
                stringSearch = "WHERE FF.Name LIKE '%" & searchText & "%' OR FC1.Name LIKE '%" & searchText & "%' OR SF.Name LIKE '%" & searchText & "%' OR FC2.Name LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT FA.*, COALESCE(FF.Name, FC1.Name) AS FirstName, COALESCE(SF.Name, FC2.Name) AS SecondName FROM FabricAlias FA LEFT JOIN Fabrics FF ON FA.FirstId=FF.Id AND FA.Type='Fabrics' LEFT JOIN Fabrics SF ON FA.SecondId=SF.Id AND FA.Type='Fabrics' LEFT JOIN FabricColours FC1 ON FA.FirstId=FC1.Id AND FA.Type='FabricColours' LEFT JOIN FabricColours FC2 ON FA.SecondId = FC2.Id AND FA.Type = 'FabricColours' {0} ORDER BY FA.Id ASC", stringSearch)
            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
            btnAddColour.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindFabric(type As String)
        ddlFirstId.Items.Clear()
        ddlSecondId.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(type) Then
                Dim thisQuery As String = String.Format("SELECT * FROM {0} ORDER BY Name ASC", type)

                ddlFirstId.DataSource = settingClass.GetDataTable(thisQuery)
                ddlFirstId.DataTextField = "Name"
                ddlFirstId.DataValueField = "Id"
                ddlFirstId.DataBind()

                ddlSecondId.DataSource = settingClass.GetDataTable(thisQuery)
                ddlSecondId.DataTextField = "Name"
                ddlSecondId.DataValueField = "Id"
                ddlSecondId.DataBind()

                If ddlFirstId.Items.Count > 0 Then
                    ddlFirstId.Items.Insert(0, New ListItem("", ""))
                End If
                If ddlSecondId.Items.Count > 0 Then
                    ddlSecondId.Items.Insert(0, New ListItem("", ""))
                End If
            End If

        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
