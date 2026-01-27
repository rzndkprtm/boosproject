Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomer")

            BindCompany()
            ddlCompany.SelectedValue = Session("CompanyCustomer")
            BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchCustomer") = txtSearch.Text
        Session("CompanyCustomer") = ddlCompany.SelectedValue
        Response.Redirect("~/setting/customer/add", False)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            Session("SearchCustomer") = txtSearch.Text
            Session("CompanyCustomer") = ddlCompany.SelectedValue

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/setting/customer/detail?customerid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId = '" + dataId + "' ORDER BY ActionDate ASC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLog_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active=0 WHERE Id=@Id UPDATE CustomerLogins SET Active=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchCustomer") = txtSearch.Text
            Session("CompanyCustomer") = ddlCompany.SelectedValue
            Response.Redirect("~/setting/customer", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDelete_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String, activeText As String)
        Session("SearchCustomer") = String.Empty
        Session("CompanyCustomer") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Active", ddlActive.SelectedValue),
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@CompanyText", If(String.IsNullOrEmpty(companyText), CType(DBNull.Value, Object), companyText)),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@CompanyId", Session("CompanyId")),
                New SqlParameter("@LoginId", Session("LoginId")),
                New SqlParameter("@CustomerId", Session("CustomerId")),
                New SqlParameter("@LevelName", Session("LevelName"))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_CustomerList", params)

            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
            gvList.Columns(2).Visible = PageAction("Visible Debtor Code") ' DEBTOR CODE
            gvList.Columns(4).Visible = PageAction("Visible Company") ' COMPANY
            gvList.Columns(5).Visible = PageAction("Visible Company Detail") ' COMPANY DETAIL
            gvList.Columns(6).Visible = PageAction("Visible Area") ' AREA
            gvList.Columns(7).Visible = PageAction("Visible Operator") ' OPERATOR
            gvList.Columns(8).Visible = PageAction("Visible Cash Sale") ' CASH SALE
            gvList.Columns(9).Visible = PageAction("Visible On Stop") ' ON STOP
            gvList.Columns(10).Visible = PageAction("Visible Active") ' ACTIVE

            btnAdd.Visible = PageAction("Add")
            ddlActive.Visible = PageAction("Active")
            divCompany.Visible = PageAction("Filter Company")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Companys WHERE Id<>'1' ORDER BY Id ASC"
            If Session("RoleName") = "Developer" Then
                thisQuery = "SELECT * FROM Companys ORDER BY Id ASC"
            End If

            ddlCompany.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindCompany", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

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
