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

            txtStartDate.Text = Now.ToString("dd-mm-yyyy")
            txtEndDate.Text = Now.ToString("dd-mm-yyyy")

            BindCompany()
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If ddlStatus.SelectedValue = "In Production" Then

            If e.Row.RowType = DataControlRowType.Header OrElse
               e.Row.RowType = DataControlRowType.DataRow OrElse
               e.Row.RowType = DataControlRowType.Footer Then

                If e.Row.Cells.Count > 0 Then
                    e.Row.Cells(e.Row.Cells.Count - 1).Visible = False
                End If

            End If

        End If
    End Sub

    Protected Sub gvBlindsPivot_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.Header OrElse e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.Cells.Count > 0 Then
                e.Row.Cells(0).Visible = False
            End If

            If e.Row.Cells.Count > 3 Then
                e.Row.Cells(3).Visible = False
            End If
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        gvList.DataSource = Nothing
        gvList.DataBind()

        gvBlindsPivot.DataSource = Nothing
        gvBlindsPivot.DataBind()

        Try
            If txtStartDate.Text = "" Then
                MessageError(True, "START DATE IS REQUIRED !")
                Exit Sub
            End If
            If txtEndDate.Text = "" Then
                MessageError(True, "END DATE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim paramsItem As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", ddlCompany.SelectedValue)
                }
                gvList.DataSource = reportClass.GetDataTableSP("sp_ReportPerDesign", paramsItem)
                gvList.DataBind()

                Dim paramsPivot As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", ddlCompany.SelectedValue)
                }
                gvBlindsPivot.DataSource = reportClass.GetDataTableSP("sp_ReportPerCustomer", paramsPivot)
                gvBlindsPivot.DataBind()
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

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        ddlCompany.Enabled = True
        Try
            ddlCompany.DataSource = reportClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
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
