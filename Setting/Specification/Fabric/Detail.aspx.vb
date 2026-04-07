Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

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

        lblId.Text = Request.QueryString("fabricid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Process(False, String.Empty)

            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnEditFabric_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/specification/fabric/edit?fabricid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Fabric Colour"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblIdColour.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Fabric Colour"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricColours WHERE Id='" & lblIdColour.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtBoeId.Text = myData("BoeId").ToString()
                    ddlFactoryColour.SelectedValue = myData("Factory").ToString()
                    txtNameColour.Text = myData("Colour").ToString()
                    txtWidthColour.Text = myData("Width").ToString()

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
            If txtNameColour.Text = "" Then
                MessageError_Process(True, "COLOUR IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim fabricColourName As String = String.Format("{0} {1}", lblName.Text, txtNameColour.Text.Trim())
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM FabricColours ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO FabricColours VALUES (@Id, @FabricId, @BoeId, @Factory, @Name, @Colour, @Width, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
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
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim active As Integer = 1
            If lblActive.Text = "Yes" Then active = 0

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Fabric Has Been Activated"
            If active = 0 Then activeDesc = "Fabric Has Been Deactivated"

            Dim dataLog As Object() = {"Fabrics", lblId.Text, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Dim aliasData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricAlias WHERE Type='Fabrics' AND FirstId='" & lblId.Text & "'")
            If aliasData.Rows.Count > 0 Then
                For i As Integer = 0 To aliasData.Rows.Count - 1
                    Dim aliasId As String = aliasData.Rows(i)("SecondId").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", aliasId)
                            myCmd.Parameters.AddWithValue("@Active", active)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Fabrics", aliasId, Session("LoginId").ToString(), activeDesc}
                    settingClass.Logs(dataLog)
                Next
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

    Protected Sub btnActiveColour_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdActiveColour.Text

            Dim active As Integer = 1
            If txtActiveColour.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Fabric Colour Has Been Activated"
            If active = 0 Then activeDesc = "Fabric Colour Has Been Deactivated"

            Dim dataLog As Object() = {"FabricColours", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Dim aliasData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricAlias WHERE Type='FabricColours' AND FirstId='" & thisId & "'")
            If aliasData.Rows.Count > 0 Then
                For i As Integer = 0 To aliasData.Rows.Count - 1
                    Dim aliasId As String = aliasData.Rows(i)("SecondId").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", aliasId)
                            myCmd.Parameters.AddWithValue("@Active", active)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Fabrics", aliasId, Session("LoginId").ToString(), activeDesc}
                    settingClass.Logs(dataLog)
                Next
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

            Dim active As Integer = Convert.ToInt32(thisData("Active"))
            lblActive.Text = "Error"
            If active = 1 Then lblActive.Text = "Yes"
            If active = 0 Then lblActive.Text = "No"

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
            aActive.Visible = PageAction("Active")
            btnAddColour.Visible = PageAction("Add Colour")

            gvList.DataSource = settingClass.GetDataTable("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM FabricColours WHERE FabricId='" & fabricId & "' ORDER BY Colour ASC")
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function TextActive(active As String) As String
        If active = "Yes" Then Return "Deactivate"
        Return "Activate"
    End Function

    Protected Function TextActiveColour(active As Boolean) As String
        If active = True Then Return "Deactivate"
        Return "Activate"
    End Function

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
