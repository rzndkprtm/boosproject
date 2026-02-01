Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Design
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
            txtSearch.Text = Session("SearchDesign")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchDesign") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Design Type"

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
        MessageError_Process(False, String.Empty)
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
            Session("SearchDesign") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Design Type"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Designs WHERE Id='" & lblId.Text & "'")

                    If myData Is Nothing Then Exit Sub

                    BindCompany(True)

                    txtName.Text = myData("Name").ToString()
                    ddlType.SelectedValue = myData("Type").ToString()
                    txtPage.Text = myData("Page").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    If Not myData("CompanyId").ToString() = "" Then
                        Dim companyArray() As String = myData("CompanyId").ToString().Split(",")
                        For Each i In companyArray
                            If Not String.IsNullOrEmpty(i) Then
                                Dim item = lbCompany.Items.FindByValue(i)
                                If item IsNot Nothing Then
                                    item.Selected = True
                                End If
                            End If
                        Next
                    End If

                    If Not myData("AppliesTo").ToString() = "" Then
                        Dim applyArray() As String = myData("AppliesTo").ToString().Split(",")
                        For Each i In applyArray
                            If Not String.IsNullOrEmpty(i) Then
                                Dim item = lbApplies.Items.FindByValue(i)
                                If item IsNot Nothing Then
                                    item.Selected = True
                                End If
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
                MessageError_Process(True, "NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError_Process(True, "TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim company As String = String.Empty
                Dim applyTo As String = String.Empty

                If Not lbCompany.SelectedValue = "" Then
                    company = String.Join(",", lbCompany.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                If Not lbApplies.SelectedValue = "" Then
                    applyTo = String.Join(",", lbApplies.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Designs ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Designs VALUES (@Id, @Name, @CompanyId, @Type, @Page, @AppliesTo, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@CompanyId", company)
                            myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@AppliesTo", applyTo)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            myCmd.ExecuteNonQuery()
                        End Using

                        Using myCmd As New SqlCommand("DECLARE @Id NVARCHAR(MAX)=@NewId; UPDATE CustomerProductAccess SET DesignId=CASE WHEN DesignId IS NULL OR LTRIM(RTRIM(DesignId))='' THEN @NewId WHEN ',' + DesignId + ',' LIKE '%,' + @NewId + ',%' THEN DesignId ELSE DesignId + ',' + @NewId END;", thisConn)
                            myCmd.Parameters.AddWithValue("@NewId", thisId)

                            myCmd.ExecuteNonQuery()
                        End Using

                        thisConn.Close()
                    End Using

                    dataLog = {"Designs", thisId, Session("LoginId").ToString(), "Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchDesign") = txtSearch.Text
                    Response.Redirect("~/setting/specification/designtype", False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Designs SET Name=@Name, CompanyId=@CompanyId, Type=@Type, Page=@Page, AppliesTo=@AppliesTo, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@CompanyId", company)
                            myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                            myCmd.Parameters.AddWithValue("@AppliesTo", applyTo)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Designs", lblId.Text, Session("LoginId").ToString(), "Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchDesign") = txtSearch.Text
                    Response.Redirect("~/setting/specification/designtype", False)
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
        Session("SearchDesign") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Id LIKE '%" & searchText.Trim() & "%' OR Name LIKE '%" & searchText.Trim() & "%' OR Page LIKE '%" & searchText.Trim() & "%' OR Description LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Designs {0} ORDER BY Name ASC", search)

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

    Protected Sub BindCompany(Optional isEdit As Boolean = False)
        lbCompany.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM Companys ORDER BY Name ASC"
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

    Protected Function GetCompanyName(designId As String) As String
        If Not String.IsNullOrEmpty(designId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT Companys.Alias AS CompanyName FROM Designs CROSS APPLY STRING_SPLIT(Designs.CompanyId, ',') AS companyArray LEFT JOIN Companys ON companyArray.VALUE=Companys.Id WHERE Designs.Id='" & designId & "' ORDER BY Companys.Id ASC")
            Dim hasil As String = String.Empty
            If myData.Rows.Count > 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += designName & ","
                Next
                Return hasil.Remove(hasil.Length - 1).ToString()
            Else
                Return String.Empty
            End If
        End If
        Return "Error"
    End Function

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
