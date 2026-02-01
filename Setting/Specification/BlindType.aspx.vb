Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_BlindType
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchBlind")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchBlind") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Blind Type"

            BindDesign()
            BindCompany()

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

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchBlind") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Blind Type"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Blinds WHERE Id='" & lblId.Text & "'")

                    If myData Is Nothing Then Exit Sub

                    BindDesign()
                    BindCompany()

                    ddlDesign.SelectedValue = myData("DesignId").ToString()
                    txtName.Text = myData("Name").ToString()
                    txtAlias.Text = myData("Alias").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    If Not myData("CompanyDetailId").ToString() = "" Then
                        Dim companyArray() As String = myData("CompanyDetailId").ToString().Split(",")
                        For Each i In companyArray
                            If Not (i.Equals(String.Empty)) Then
                                lbCompany.Items.FindByValue(i).Selected = True
                            End If
                        Next
                    End If

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
            If txtName.Text = "" Then
                MessageError_Process(True, "BLIND TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlDesign.Text = "" Then
                MessageError_Process(True, "DESIGN NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lbCompany.SelectedValue = "" Then
                MessageError_Process(True, "COMPANY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim company As String = String.Join(",", lbCompany.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))

                Dim aliasName As String = txtAlias.Text.Trim()
                If String.IsNullOrEmpty(txtAlias.Text) Then
                    aliasName = txtName.Text.Trim()
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Blinds ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Blinds VALUES (@Id, @DesignId, @CompanyDetailId, @Name, @Alias, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                            myCmd.Parameters.AddWithValue("@CompanyDetailId", company)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", aliasName)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Blinds", thisId, Session("LoginId").ToString(), "Blind Type Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchBlind") = txtSearch.Text
                    Response.Redirect("~/setting/specification/blindtype", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Blinds SET DesignId=@DesignId, CompanyDetailId=@CompanyDetailId, Name=@Name, Alias=@Alias, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                            myCmd.Parameters.AddWithValue("@CompanyDetailId", company)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", aliasName)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Blinds", lblId.Text, Session("LoginId").ToString(), "Blind Type Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchBlind") = txtSearch.Text
                    Response.Redirect("~/setting/specification/blindtype", False)
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

    Protected Sub BindData(searchText As String)
        Session("SearchBlind") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Blinds.Id LIKE '%" & searchText & "%' OR Blinds.Name LIKE '%" & searchText & "%' OR Blinds.Alias LIKE '%" & searchText & "%' OR Blinds.Description LIKE '%" & searchText & "%' OR Designs.Name LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT Blinds.*, Designs.Name AS DesignName, CASE WHEN Blinds.Active=1 THEN 'Yes' WHEN Blinds.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Blinds LEFT JOIN Designs ON Blinds.DesignId = Designs.Id {0} ORDER BY Designs.Id, Blinds.Name ASC", search)

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

    Protected Sub BindDesign(Optional isEdit As Boolean = False)
        ddlDesign.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Designs WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM Designs ORDER BY Name ASC"
            End If
            ddlDesign.DataSource = settingClass.GetDataTable(thisString)
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany(Optional isEdit As Boolean = False)
        lbCompany.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM CompanyDetails WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM CompanyDetails ORDER BY Name ASC"
            End If
            lbCompany.DataSource = settingClass.GetDataTable(thisString)
            lbCompany.DataTextField = "Name"
            lbCompany.DataValueField = "Id"
            lbCompany.DataBind()

            If lbCompany.Items.Count > 0 Then
                lbCompany.Items.Insert(0, New ListItem("", ""))
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

    Protected Function BindCompanyDetail(blindId As String) As String
        If Not String.IsNullOrEmpty(blindId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Blinds CROSS APPLY STRING_SPLIT(Blinds.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Blinds.Id='" & blindId & "' ORDER BY CompanyDetails.Id ASC")
            Dim hasil As String = String.Empty
            If myData.Rows.Count > 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += designName & ", "
                Next
                Return hasil.Remove(hasil.Length - 2).ToString()
            Else
                Return String.Empty
            End If
        End If
        Return "Error"
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
