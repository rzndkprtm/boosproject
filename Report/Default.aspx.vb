Imports System.Data.SqlClient

Partial Class Report_Default
    Inherits Page

    Dim reportClass As New ReportClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindCompany()

            gvCustomer.DataSource = Nothing : gvCustomer.DataBind()
            gvProduct.DataSource = Nothing : gvProduct.DataBind()
            gvFabric.DataSource = Nothing : gvFabric.DataBind()
            gvFabricColour.DataSource = Nothing : gvFabricColour.DataBind()
            gvChain.DataSource = Nothing : gvChain.DataBind()
            gvMotor.DataSource = Nothing : gvMotor.DataBind()
            gvBottom.DataSource = Nothing : gvBottom.DataBind()
            gvTube.DataSource = Nothing : gvTube.DataBind()

            btnGenerate.Visible = LoginAccess("Generate")
        End If
    End Sub

    Protected Sub btnGenerate_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/report/generate", False)
    End Sub

    Protected Sub gvProduct_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If ddlStatus.SelectedValue = "In Production" OrElse ddlStatus.SelectedValue = "Unsubmitted" Then
            If e.Row.RowType = DataControlRowType.Header OrElse
               e.Row.RowType = DataControlRowType.DataRow OrElse
               e.Row.RowType = DataControlRowType.Footer Then

                If e.Row.Cells.Count > 0 Then
                    e.Row.Cells(e.Row.Cells.Count - 1).Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub gvCustomer_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.Header OrElse e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.Cells.Count > 0 Then
                e.Row.Cells(0).Visible = False
                e.Row.Cells(1).Visible = False
            End If
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)

        gvCustomer.DataSource = Nothing : gvCustomer.DataBind()
        gvProduct.DataSource = Nothing : gvProduct.DataBind()
        gvFabric.DataSource = Nothing : gvFabric.DataBind()
        gvFabricColour.DataSource = Nothing : gvFabricColour.DataBind()
        gvChain.DataSource = Nothing : gvChain.DataBind()
        gvMotor.DataSource = Nothing : gvMotor.DataBind()
        gvBottom.DataSource = Nothing : gvBottom.DataBind()
        gvTube.DataSource = Nothing : gvTube.DataBind()
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
                Dim customers As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvCustomer.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_Customer", customers)
                gvCustomer.DataBind()

                Dim products As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvProduct.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_DesignType", products)
                gvProduct.DataBind()

                Dim fabrics As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvFabric.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_FabricType", fabrics)
                gvFabric.DataBind()

                Dim fabricColours As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvFabricColour.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_FabricColour", fabricColours)
                gvFabricColour.DataBind()

                Dim chains As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvChain.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_Chain", chains)
                gvChain.DataBind()

                Dim motors As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvMotor.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_Motor", motors)
                gvMotor.DataBind()

                Dim bottomColours As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvBottom.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_BottomColour", bottomColours)
                gvBottom.DataBind()

                Dim tubes As New List(Of SqlParameter) From {
                    New SqlParameter("@Status", ddlStatus.SelectedValue),
                    New SqlParameter("@StartDate", txtStartDate.Text),
                    New SqlParameter("@EndDate", txtEndDate.Text),
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                }
                gvTube.DataSource = reportClass.GetDataTableSP("sp_ReportOrder_TubeType", tubes)
                gvTube.DataBind()
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
        ddlCompany.Items.Clear() : ddlCompany.Enabled = True
        Try
            ddlCompany.DataSource = reportClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("ALL", ""))
            End If

            If Session("RoleName") = "Sales" Then
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

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
