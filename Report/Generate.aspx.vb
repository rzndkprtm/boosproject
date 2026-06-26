Imports System.Data
Imports System.Data.SqlClient

Partial Class Report_Generate
    Inherits Page

    Dim reportClass As New ReportClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/report/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDataType.SelectedValue = "Job Order" OrElse ddlDataType.SelectedValue = "Customer (Order)" Then
                If txtStartDate.Text = "" Then
                    MessageError(True, "START DATE IS REQUIRED !")
                    Exit Sub
                End If
                If txtEndDate.Text = "" Then
                    MessageError(True, "END DATE IS REQUIRED !")
                    Exit Sub
                End If
            End If

            If ddlDataType.SelectedValue = "Job Order" Then
                If ddlFileType.SelectedValue = "PDF" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim pdfBytes As Byte() = reportClass.JobOrderPDF(company, txtStartDate.Text, txtEndDate.Text)

                    Response.Clear()
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_JOB_ORDER.pdf")
                    Response.BinaryWrite(pdfBytes)
                    Response.Flush()
                    Response.End()
                End If
                If ddlFileType.SelectedValue = "EXCEL" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim excelBytes As Byte() = reportClass.JobOrderExcel(company, txtStartDate.Text, txtEndDate.Text)

                    Response.Clear()
                    Response.ClearContent()
                    Response.ClearHeaders()

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    Response.AddHeader("content-disposition", "attachment; filename=REPORT_JOB_ORDER.xlsx")
                    Response.BinaryWrite(excelBytes)

                    Response.Flush()
                    Response.SuppressContent = True

                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                End If
                If ddlFileType.SelectedValue = "CSV" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim csvBytes As Byte() = reportClass.JobOrderCSV(company, txtStartDate.Text, txtEndDate.Text)

                    Response.Clear()
                    Response.ContentType = "text/csv"
                    Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_JOB_ORDER.csv")
                    Response.BinaryWrite(csvBytes)
                    Response.Flush()
                    Response.End()
                End If
            End If

            If ddlDataType.SelectedValue = "Customer (List)" Then
                If ddlFileType.SelectedValue = "PDF" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim pdfBytes As Byte() = reportClass.CustomerListPDF(company, Session("RoleName").ToString())

                    Response.Clear()
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_CUSTOMER_LIST.pdf")
                    Response.BinaryWrite(pdfBytes)
                    Response.Flush()
                    Response.End()
                End If
                If ddlFileType.SelectedValue = "EXCEL" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim excelBytes As Byte() = reportClass.CustomerListExcel(company, Session("RoleName").ToString())

                    Response.Clear()
                    Response.ClearContent()
                    Response.ClearHeaders()

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    Response.AddHeader("content-disposition", "attachment; filename=REPORT_CUSTOMER_LIST.xlsx")
                    Response.BinaryWrite(excelBytes)

                    Response.Flush()
                    Response.SuppressContent = True

                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                End If
                If ddlFileType.SelectedValue = "CSV" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim csvBytes As Byte() = reportClass.CustomerListCSV(company, Session("RoleName").ToString())

                    Response.Clear()
                    Response.ContentType = "text/csv"
                    Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_CUSTOMER_LIST.csv")
                    Response.BinaryWrite(csvBytes)
                    Response.Flush()
                    Response.End()
                End If
            End If

            If ddlDataType.SelectedValue = "Customer (Order)" Then
                If ddlFileType.SelectedValue = "PDF" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim paramsPivot As New List(Of SqlParameter) From {
                        New SqlParameter("@Status", "In Production"),
                        New SqlParameter("@StartDate", txtStartDate.Text),
                        New SqlParameter("@EndDate", txtEndDate.Text),
                        New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(company), CType(DBNull.Value, Object), company))}

                    Dim dt As DataTable = reportClass.GetDataTableSP("sp_ReportPerCustomer", paramsPivot)

                    Dim pdfBytes As Byte() = reportClass.CustomerOrderPDF(dt, "REPORT CUSTOMER ORDER")

                    Response.Clear()
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_CUSTOMER_ORDER.pdf")
                    Response.BinaryWrite(pdfBytes)
                    Response.Flush()
                    Response.End()
                End If
                If ddlFileType.SelectedValue = "EXCEL" Then
                    MessageError(True, "UNDER MAINTENANCE !")
                    Exit Sub
                    'Dim company As String = ddlCompany.SelectedValue
                    'If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    'Dim paramsPivot As New List(Of SqlParameter) From {
                    '    New SqlParameter("@Status", "In Production"),
                    '    New SqlParameter("@StartDate", txtStartDate.Text),
                    '    New SqlParameter("@EndDate", txtEndDate.Text),
                    '    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(company), CType(DBNull.Value, Object), company))
                    '}

                    'Dim dt As DataTable = reportClass.GetDataTableSP("sp_ReportPerCustomer", paramsPivot)
                    'Dim excelBytes As Byte() = reportClass.CustomerOrderExcel(dt, "REPORT PER CUSTOMER")

                    'Response.Clear()
                    'Response.Buffer = True
                    'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    'Response.AddHeader("Content-Disposition", "attachment; filename=REPORT_PER_CUSTOMER.xlsx")
                    'Response.BinaryWrite(excelBytes)
                    'Response.Flush()
                    'HttpContext.Current.ApplicationInstance.CompleteRequest()
                End If
                If ddlFileType.SelectedValue = "CSV" Then
                    MessageError(True, "UNDER MAINTENANCE !")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/report", False)
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear() : ddlCompany.Enabled = True
        Try
            ddlCompany.DataSource = reportClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("ALL", "ALL"))
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
