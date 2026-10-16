Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.InteropServices.ComTypes

Partial Class Report_Generate
    Inherits Page

    Dim reportClass As New ReportClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/report", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
            BindProduct()
            BindForm(ddlDataType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDataType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindForm(ddlDataType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDataType.SelectedValue = "OrderHeaders" Then
                If txtStartDate.Text = "" Then
                    MessageError(True, "START DATE IS REQUIRED !")
                    Exit Sub
                End If
                If txtEndDate.Text = "" Then
                    MessageError(True, "END DATE IS REQUIRED !")
                    Exit Sub
                End If
            End If
            If ddlDataType.SelectedValue = "OrderDetails" Then
                If txtStartDate.Text = "" Then
                    MessageError(True, "START DATE IS REQUIRED !")
                    Exit Sub
                End If
                If txtEndDate.Text = "" Then
                    MessageError(True, "END DATE IS REQUIRED !")
                    Exit Sub
                End If
            End If

            If ddlDataType.SelectedValue = "OrderHeaders" Then
                If ddlFileType.SelectedValue = "PDF" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim pdfBytes As Byte() = reportClass.OrderHeaderPDF(company, txtStartDate.Text, txtEndDate.Text)

                    Dim companyName As String = "ALL"
                    If Not String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        companyName = reportClass.GetItemData("SELECT ALIAS FROM Companys WHERE Id='" & ddlCompany.SelectedValue & "'")
                    End If

                    Dim fileName As String = "REPORT_ORDER_" & companyName.Replace(" ", "_") & "_" & txtStartDate.ToString("yyyy-MM-dd") & "_TO_" & txtEndDate.ToString("yyyy-MM-dd") & ".pdf"

                    Response.Clear()
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
                    Response.BinaryWrite(pdfBytes)
                    Response.Flush()
                    Response.End()
                End If
                If ddlFileType.SelectedValue = "EXCEL" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim reportClass As New ReportClass
                    Dim excelBytes As Byte() = reportClass.JobOrderExcel(company, txtStartDate.Text, txtEndDate.Text)

                    Dim companyName As String = "ALL"
                    If Not String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        companyName = reportClass.GetItemData("SELECT ALIAS FROM Companys WHERE Id='" & ddlCompany.SelectedValue & "'")
                    End If

                    Dim fileName As String = "REPORT_ORDER_" & companyName.Replace(" ", "_") & "_" & txtStartDate.ToString("yyyy-MM-dd") & "_TO_" & txtEndDate.ToString("yyyy-MM-dd") & ".pdf"

                    Response.Clear()
                    Response.ClearContent()
                    Response.ClearHeaders()

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    Response.AddHeader("content-disposition", "attachment; filename=" & fileName)
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

                    Dim companyName As String = "ALL"
                    If Not String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        companyName = reportClass.GetItemData("SELECT ALIAS FROM Companys WHERE Id='" & ddlCompany.SelectedValue & "'")
                    End If

                    Dim fileName As String = "REPORT_ORDER_" & companyName.Replace(" ", "_") & "_" & txtStartDate.ToString("yyyy-MM-dd") & "_TO_" & txtEndDate.ToString("yyyy-MM-dd") & ".pdf"

                    Response.Clear()
                    Response.ContentType = "text/csv"

                    Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
                    Response.BinaryWrite(csvBytes)
                    Response.Flush()
                    Response.End()
                End If
            End If

            If ddlDataType.SelectedValue = "OrderDetails" Then
                If ddlFileType.SelectedValue = "PDF" Then
                    Dim company As String = ddlCompany.SelectedValue
                    If ddlCompany.SelectedValue = "ALL" Then company = String.Empty

                    Dim designType As String = String.Empty
                    If Not lbProduct.SelectedValue = "" Then
                        designType = String.Join(",", lbProduct.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                    End If

                    Dim reportClass As New ReportClass
                    Dim pdfBytes As Byte() = reportClass.OrderDetailPDF(company, designType, txtStartDate.Text, txtEndDate.Text)

                    Dim companyName As String = "ALL"
                    If Not String.IsNullOrEmpty(ddlCompany.SelectedValue) Then
                        companyName = reportClass.GetItemData("SELECT ALIAS FROM Companys WHERE Id='" & ddlCompany.SelectedValue & "'")
                    End If

                    Dim fileName As String = "REPORT_ORDER_PRODUCT_" & companyName.Replace(" ", "_") & "_" & txtStartDate.ToString("yyyy-MM-dd") & "_TO_" & txtEndDate.ToString("yyyy-MM-dd") & ".pdf"

                    Response.Clear()
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
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
            ddlCompany.DataSource = reportClass.GetDataTable("SELECT * FROM Companys WHERE Status='Active' ORDER BY Name ASC")
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

    Protected Sub BindProduct()
        lbProduct.Items.Clear()
        Try
            lbProduct.DataSource = reportClass.GetDataTable("SELECT Id, Name FROM Designs")
            lbProduct.DataTextField = "Name"
            lbProduct.DataValueField = "Id"
            lbProduct.DataBind()

            If lbProduct.Items.Count > 0 Then
                lbProduct.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbProduct.Items.Clear()
        End Try
    End Sub

    Protected Sub BindForm(dataType As String)
        Try
            divProduct.Visible = False
            If Not String.IsNullOrEmpty(dataType) Then
                If dataType = "OrderDetails" Then
                    divProduct.Visible = True
                End If
            End If
        Catch ex As Exception

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
