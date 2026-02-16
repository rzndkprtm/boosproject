Imports System.Data
Imports System.Data.SqlClient

Partial Class Report_Default
    Inherits Page

    Dim reportClass As New ReportClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtStartDate.Text = Now.ToString("yyyy-mm-dd")
            txtEndDate.Text = Now.ToString("yyyy-mm-dd")

            BindCompany()
            BindCustomer(ddlCustomer.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCustomer()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlStatus.SelectedValue = "" Then
                MessageError(True, "STATUS IS REQUIRED !")
                Exit Sub
            End If

            If txtStartDate.Text = "" Then
                MessageError(True, "START DATE IS REQUIRED !")
                Exit Sub
            End If
            If txtEndDate.Text = "" Then
                MessageError(True, "END DATE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                BindData(ddlStatus.SelectedValue, txtStartDate.Text, txtEndDate.Text)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub BindData(status As String, dStart As Date, dEnd As Date)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Status", status),
                New SqlParameter("@DateFrom", dStart),
                New SqlParameter("@DateTo", dEnd)
            }

            Dim thisData As DataTable = reportClass.GetDataTableSP("sp_TotalItemsPerDesign", params)

            gvList.DataSource = thisData
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        ddlCompany.Enabled = True
        Try
            Dim thisString As String = "SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC"
            ddlCompany.DataSource = reportClass.GetDataTable(thisString)
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" Then
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

    Protected Sub BindCustomer(Optional company As String = "")
        Try
            Dim thisString As String = "SELECT * FROM Customers WHERE Active=1 ORDER BY Name ASC"
            If Not String.IsNullOrEmpty(company) Then
                thisString = "SELECT * FROM Customers WHERE CompanyId = '" & company & "' ORDER BY Name ASC"
            End If

            ddlCustomer.DataSource = reportClass.GetDataTable(thisString)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

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
