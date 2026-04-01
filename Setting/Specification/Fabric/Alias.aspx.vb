Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_Specification_Fabric_Alias
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
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Colour(False, String.Empty)

            txtSearch.Text = Session("SearchFabricAlias")
            txtSearchColour.Text = Session("SearchFabricColourAlias")

            BindData(txtSearch.Text)
            BindDataColour(txtSearchColour.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("selectedTab") = "list-fabric"

        Session("SearchFabricAlias") = txtSearch.Text
        Session("SearchFabricColourAlias") = txtSearchColour.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "FabricAlias"
            titleProcess.InnerText = "Add Fabric Alias"

            BindFabric("Fabrics")

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
        Session("selectedTab") = "list-fabriccolour"

        Session("SearchFabricAlias") = txtSearch.Text
        Session("SearchFabricColourAlias") = txtSearchColour.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            lblType.Text = "FabricColourAlias"
            titleProcess.InnerText = "Add Fabric Colour Alias"

            BindFabric("FabricColours")

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

    Protected Sub btnSearchColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        BindDataColour(txtSearchColour.Text)
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            Session("SearchFabricAlias") = txtSearch.Text
            Session("SearchFabricColourAlias") = txtSearchColour.Text

            Session("selectedTab") = "list-fabric"
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    lblType.Text = "FabricAlias"
                    titleProcess.InnerText = "Edit Fabric Alias"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricAlias WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindFabric("Fabrics")

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

    Protected Sub gvListColour_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            Session("SearchFabricAlias") = txtSearch.Text
            Session("SearchFabricColourAlias") = txtSearchColour.Text

            Session("selectedTab") = "list-fabriccolour"
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    lblType.Text = "FabricColourAlias"
                    titleProcess.InnerText = "Edit Fabric Colour Alias"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricColourAlias WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindFabric("FabricColours")

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
                    Dim thisId As String = settingClass.CreateId(String.Format("SELECT TOP 1 Id FROM {0} ORDER BY Id DESC", lblType.Text))

                    Dim stringInsert As String = String.Format("INSERT INTO {0} VALUES (@Id, @FirstId, @SecondId)", lblType.Text)

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(stringInsert, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {lblType.Text, thisId, Session("LoginId").ToString(), "Alias Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricAlias") = txtSearch.Text
                    Session("SearchFabricColourAlias") = txtSearchColour.Text
                    Response.Redirect("~/setting/specification/fabric/alias", False)
                End If

                If lblAction.Text = "Edit" Then
                    Dim stringUpdate As String = String.Format("UPDATE {0} SET FirstId=@FirstId, SecondId=@SecondId WHERE Id=@Id", lblType.Text)
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand(stringUpdate, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {lblType.Text, lblId.Text, Session("LoginId").ToString(), "Alias Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricAlias") = txtSearch.Text
                    Session("SearchFabricColourAlias") = txtSearchColour.Text
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
        Session("selectedTab") = "list-fabric"
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
            Session("SearchFabricColourAlias") = txtSearchColour.Text
            Response.Redirect("~/setting/specification/fabric/alias", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDeleteColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        Session("selectedTab") = "list-fabriccolour"
        Try
            Dim thisId As String = txtIdDeleteColour.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM FabricColourAlias WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='FabricColourAlias' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchFabricAlias") = txtSearch.Text
            Session("SearchFabricColourAlias") = txtSearchColour.Text
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
                stringSearch = "WHERE FirstFabric.Name LIKE '%" & searchText & "%' OR SecondFabric.Name LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT FabricAlias.*, FirstFabric.Name AS FirstName, SecondFabric.Name AS SecondName FROM FabricAlias LEFT JOIN Fabrics FirstFabric ON FabricAlias.FirstId = FirstFabric.Id LEFT JOIN Fabrics SecondFabric ON FabricAlias.SecondId = SecondFabric.Id {0} ORDER BY FabricAlias.Id ASC", stringSearch)
            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataColour(searchText As String)
        Session("SearchFabricColourAlias") = String.Empty
        Try
            Dim stringSearch As String = String.Empty
            If Not searchText = "" Then
                stringSearch = "WHERE FirstFabric.Name LIKE '%" & searchText & "%' OR SecondFabric.Name LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT FabricColourAlias.*, FirstFabric.Name AS FirstName, SecondFabric.Name AS SecondName FROM FabricColourAlias LEFT JOIN FabricColours FirstFabric ON FabricColourAlias.FirstId = FirstFabric.Id LEFT JOIN FabricColours SecondFabric ON FabricColourAlias.SecondId = SecondFabric.Id {0} ORDER BY FabricColourAlias.Id ASC", stringSearch)
            gvListColour.DataSource = settingClass.GetDataTable(thisString)
            gvListColour.DataBind()
            gvListColour.Columns(1).Visible = PageAction("Visible ID")

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

    Protected Sub MessageError_Colour(visible As Boolean, message As String)
        divErrorColour.Visible = visible : msgErrorColour.InnerText = message
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
