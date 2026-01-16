Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Bottom_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchBottom")

            BindCompanyDetailSort()

            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchBottom") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
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

    Protected Sub ddlCompanyDetail_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchBottom") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='Bottoms' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "BOTTOM NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim design As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    If Not String.IsNullOrEmpty(item.Selected) Then
                        design += item.Value & ","
                    End If
                End If
            Next
            If design = "" Then
                MessageError_Process(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim company As String = String.Empty
            For Each item As ListItem In lbCompany.Items
                If item.Selected Then
                    If Not String.IsNullOrEmpty(item.Selected) Then
                        company += item.Value & ","
                    End If
                End If
            Next
            If company = "" Then
                MessageError_Process(True, "COMPANY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim designType As String = design.Remove(design.Length - 1).ToString()
                Dim companyDetail As String = company.Remove(company.Length - 1).ToString()

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Bottoms ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Bottoms VALUES (@Id, @Name, @DesignId, @CompanyDetailId, @Description, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Bottoms", thisId, Session("LoginId").ToString(), "Bottom Type Created"}
                settingClass.Logs(dataLog)

                Session("SearchBottom") = txtSearch.Text
                url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", thisId)
                Response.Redirect(url, False)
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
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Products WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String)
        Session("SearchBottom") = String.Empty
        Try
            Dim company As String = "WHERE EXISTS (SELECT 1 FROM STRING_SPLIT(Bottoms.CompanyDetailId, ',') companyArray WHERE LTRIM(RTRIM(companyArray.value)) = '" & companyText & "')"
            Dim search As String = String.Empty

            If Not searchText = "" Then
                search = "(Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%')"
            End If

            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Bottoms {0} {1} ORDER BY Name ASC", company, search)

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

    Protected Sub BindCompanyDetailSort()
        ddlCompanyDetail.Items.Clear()
        Try
            If Not Session("CompanyDetailId") = "" Then
                Dim thisString As String = "SELECT * FROM CompanyDetails ORDER BY Id ASC"

                If Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer Service" Then
                    thisString = "SELECT * FROM CompanyDetails WHERE CompanyId='" & Session("CompanyId") & "' ORDER BY Id ASC"
                End If

                ddlCompanyDetail.DataSource = settingClass.GetDataTable(thisString)
                ddlCompanyDetail.DataTextField = "Name"
                ddlCompanyDetail.DataValueField = "Id"
                ddlCompanyDetail.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        lbCompany.Items.Clear()
        Try
            lbCompany.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails ORDER BY Name ASC")
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

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindDesign(bottomId As String) As String
        If Not String.IsNullOrEmpty(bottomId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT Designs.Name AS DesignName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.DesignId, ',') AS splitArray LEFT JOIN Designs ON splitArray.VALUE=Designs.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY Designs.Id ASC")
            Dim hasil As String = String.Empty
            If Not myData.Rows.Count = 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If

            Return hasil.Remove(hasil.Length - 2).ToString()
        End If
        Return "Error"
    End Function

    Protected Function BindCompany(bottomId As String) As String
        If Not String.IsNullOrEmpty(bottomId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY CompanyDetails.Id ASC")
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

    Protected Function BindTextLog(logId As String) As String
        Return settingClass.getTextLog(logId)
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
