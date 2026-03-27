Imports System.Globalization

Partial Class Sales_Default
    Inherits Page

    Dim salesClass As New SalesClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()

            ddlCompany.SelectedValue = Session("SearchSalesCompany")
            BindData(ddlCompany.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlCompany.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlCompany.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlCompany.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            Session("SearchSalesCompany") = ddlCompany.SelectedValue
            If e.CommandName = "Refresh" Then
                Dim companyId As String = salesClass.GetItemData("SELECT CompanyId FROM Sales WHERE Id='" & dataId & "'")

                salesClass.RefreshData(companyId)
                Response.Redirect("~/sales", False)
            End If
        End If
    End Sub

    Protected Sub BindData(companyId As String)
        gvList.DataSource = Nothing
        gvList.DataBind()
        Session("SearchSalesCompany") = String.Empty
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                gvList.DataSource = salesClass.GetDataTable("SELECT * FROM Sales WHERE CompanyId='" & companyId & "' ORDER BY SummaryDate DESC")
                gvList.DataBind()
                gvList.Columns(1).Visible = PageAction("Visible ID")
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        ddlCompany.Enabled = True
        Try
            ddlCompany.DataSource = salesClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                ddlCompany.SelectedValue = Session("CompanyId").ToString()
                ddlCompany.Enabled = False
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function BindPrice(price As Decimal) As String
        If price > 0 Then
            If ddlCompany.SelectedValue = "3" Then
                Return String.Format("Rp {0}", price.ToString("N2", idIDR))
            End If
            Return String.Format("$ {0}", price.ToString("N2", enUS))
        End If
        Return "$ 0.00"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
