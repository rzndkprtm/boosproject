Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_Specification_Fabric_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty


    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabFabric") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If
        If String.IsNullOrEmpty(Request.QueryString("fabricid")) Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not Session("selectedTabFabric") = "" Then
            selected_tab.Value = Session("selectedTabFabric").ToString()
        End If

        lblId.Text = Request.QueryString("fabricid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Colour(False, String.Empty)
            MessageError_Process(False, String.Empty)

            BindData(lblId.Text)
            BindDataColour(lblId.Text, txtSearchColour.Text)
        End If
    End Sub

    Protected Sub btnEditFabric_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/specification/fabric/edit?fabricid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim newStatus As String = ddlNewStatus.SelectedValue

            If newStatus = "" Then
                url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
                Response.Redirect(url, False)
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Status=@Status WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@Status", newStatus)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Fabric Type : {0}", newStatus)
            dataLog = {"Fabrics", lblId.Text, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Dim detailData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricColours WHERE FabricId='" & lblId.Text & "' AND Status='" & lblStatus.Text & "'")
            If Not detailData.Rows.Count = 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim detailId As String = detailData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", detailId)
                            myCmd.Parameters.AddWithValue("@Status", newStatus)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)

                    dataLog = {"FabricColours", detailId, Session("LoginId").ToString(), changeDesc}
                    settingClass.Logs(dataLog)
                Next
            End If

            Dim aliasData As DataRow = settingClass.GetDataRow("SELECT SecondId FROM FabricAlias WHERE Type='Fabrics' AND FirstId='" & lblId.Text & "'")
            If aliasData IsNot Nothing Then
                Dim aliasId As String = aliasData(0).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Status=@Status WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", aliasId)
                        myCmd.Parameters.AddWithValue("@Status", newStatus)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                changeDesc = String.Format("Change Status Fabric Type : {0}", newStatus)
                dataLog = {"Fabrics", aliasId, Session("LoginId").ToString(), changeDesc}
                settingClass.Logs(dataLog)

                Dim detailAliasData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricColours WHERE FabricId='" & aliasId & "' AND Status='" & lblStatus.Text & "'")
                If Not detailAliasData.Rows.Count = 0 Then
                    For i As Integer = 0 To detailAliasData.Rows.Count - 1
                        Dim detailId As String = detailAliasData.Rows(i)("Id").ToString()

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", detailId)
                                myCmd.Parameters.AddWithValue("@Status", newStatus)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)

                        dataLog = {"FabricColours", detailId, Session("LoginId").ToString(), changeDesc}
                        settingClass.Logs(dataLog)
                    Next
                End If
            End If

            url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSearchColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        BindDataColour(lblId.Text, txtSearchColour.Text)
    End Sub

    Protected Sub gvListColour_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabFabric") = "list-colour"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessColour(); };"
                Try
                    lblIdColour.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Fabric Colour"

                    divStatusColour.Visible = False

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricColours WHERE Id='" & lblIdColour.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtBoeId.Text = myData("BoeId").ToString()
                    ddlFactoryColour.SelectedValue = myData("Factory").ToString()
                    txtNameColour.Text = myData("Colour").ToString()
                    txtWidthColour.Text = myData("Width").ToString()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessColour(); };"
        Session("selectedTabFabric") = "list-colour"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Fabric Colour"

            divStatusColour.Visible = True

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessColour_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessColour(); };"
        Try
            If txtNameColour.Text = "" Then
                MessageError_Process(True, "COLOUR IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim fabricColourName As String = String.Format("{0} {1}", lblName.Text, txtNameColour.Text.Trim())

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM FabricColours ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO FabricColours VALUES (@Id, @FabricId, @BoeId, @Factory, @Name, @Colour, @Width, @Status)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@FabricId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Factory", ddlFactoryColour.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Name", fabricColourName)
                            myCmd.Parameters.AddWithValue("@Colour", txtNameColour.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidthColour.Text)
                            myCmd.Parameters.AddWithValue("@Status", ddlStatusColour.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricColours", thisId, Session("LoginId").ToString(), "Fabric Colour Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours Set BoeId=@BoeId, Factory=@Factory, Name=@Name, Colour=@Colour, Width=@Width WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdColour.Text)
                            myCmd.Parameters.AddWithValue("@FabricId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Factory", ddlFactoryColour.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Name", fabricColourName)
                            myCmd.Parameters.AddWithValue("@Colour", txtNameColour.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidthColour.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricColours", lblIdColour.Text, Session("LoginId").ToString(), "Fabric Colour Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessColour", thisScript, True)
        End Try
    End Sub

    Protected Sub btnChangeStatusColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        Session("selectedTabFabric") = "list-colour"
        Try
            Dim thisId As String = txtIdStatusColour.Text
            Dim newStatus As String = ddlNewStatusColour.SelectedValue
            Dim oldStatus As String = ddlOldStatusColour.SelectedValue

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Status", newStatus)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Fabric Colour : {0}", newStatus)
            dataLog = {"FabricColours", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Dim aliasData As DataRow = settingClass.GetDataRow("SELECT SecondId FROM FabricAlias WHERE Type='FabricColours' AND FirstId='" & thisId & "'")
            If aliasData IsNot Nothing Then
                Dim aliasId As String = aliasData(0).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", aliasId)
                        myCmd.Parameters.AddWithValue("@Status", newStatus)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)
                dataLog = {"FabricColours", aliasId, Session("LoginId").ToString(), changeDesc}
                settingClass.Logs(dataLog)
            End If

            url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Colour(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Colour(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(fabricId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Fabrics WHERE Id='" & fabricId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/fabric", False)
                Exit Sub
            End If

            lblName.Text = thisData("Name").ToString()
            lblType.Text = thisData("Type").ToString()
            lblGroup.Text = thisData("Group").ToString()

            Dim norailroad As Integer = Convert.ToInt32(thisData("NoRailRoad"))
            lblNoRailRoad.Text = "Error"
            If norailroad = 1 Then lblNoRailRoad.Text = "Yes"
            If norailroad = 0 Then lblNoRailRoad.Text = "No"

            lblStatus.Text = thisData("Status").ToString()

            lblDesignName.Text = String.Empty
            If Not thisData("DesignId").ToString() = "" Then
                Dim designArray() As String = thisData("DesignId").ToString().Split(",")

                Dim designName As String = String.Empty
                For Each i In designArray
                    Dim thisName As String = settingClass.GetItemData("SELECT Name FROM Designs WHERE Id='" & i & "'")
                    designName &= thisName & ", "
                Next

                lblDesignName.Text = designName.Remove(designName.Length - 2).ToString()
            End If

            lblTubeType.Text = String.Empty
            If Not thisData("TubeId").ToString() = "" Then
                Dim tubeArray() As String = thisData("TubeId").ToString().Split(",")

                Dim tubeName As String = String.Empty
                For Each i In tubeArray
                    Dim thisName As String = settingClass.GetItemData("SELECT Name FROM ProductTubes WHERE Id='" & i & "'")
                    tubeName &= thisName & ", "
                Next

                lblTubeType.Text = tubeName.Remove(tubeName.Length - 2).ToString()
            End If

            lblCompanyDetailName.Text = String.Empty
            If Not thisData("CompanyDetailId").ToString() = "" Then
                Dim companyArray() As String = thisData("CompanyDetailId").ToString().Split(",")

                Dim companyDetailName As String = String.Empty
                For Each i In companyArray
                    Dim thisName As String = settingClass.GetItemData("SELECT Name FROM CompanyDetails WHERE Id='" & i & "'")
                    companyDetailName += thisName & ", "
                Next

                lblCompanyDetailName.Text = companyDetailName.Remove(companyDetailName.Length - 2).ToString()
            End If

            btnEditFabric.Visible = PageAction("Edit")
            aChangeStatus.Visible = PageAction("Change Status")
            btnAddColour.Visible = PageAction("Add Colour")
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataColour(fabricId As String, searchText As String)
        Try
            Dim stringFabricId As String = "WHERE FabricId='" & fabricId & "'"
            Dim stringSearch As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                stringSearch = "AND (BoeId LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Colour LIKE '%" & searchText & "%' OR Status LIKE '%" & searchText & "%')"
            End If
            Dim thisString As String = String.Format("SELECT * FROM FabricColours {0} {1} ORDER BY Colour ASC", stringFabricId, stringSearch)

            gvListColour.DataSource = settingClass.GetDataTable(thisString)
            gvListColour.DataBind()
            gvListColour.Columns(1).Visible = PageAction("Visible ID Detail")
            gvListColour.Columns(4).Visible = PageAction("Visible Name Detail")
        Catch ex As Exception
            MessageError_Colour(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError_Colour(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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
