Imports System.Data
Imports System.Data.SqlClient

Partial Class Report
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Dim reportClass As New ReportClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindCompany()
            BindSubCompany(ddlCompany.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindSubCompany(ddlCompany.SelectedValue)
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "REPORT TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlFile.SelectedValue = "" Then
                MessageError(True, "FILE TYPE IS REQUIRED !")
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
                If ddlType.SelectedValue = "Blind Orders" Then
                    If ddlFile.SelectedValue = "PDF" Then
                        Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
                        Dim fileName As String = String.Format("{0}.pdf", todayString)
                        Dim pdfFilePath As String = Server.MapPath("~/File/Report/" & fileName)

                        reportClass.Blinds(pdfFilePath, ddlType.SelectedValue, txtStartDate.Text, txtEndDate.Text)

                        Response.ContentType = "application/pdf"
                        Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
                        Response.TransmitFile(pdfFilePath)
                        HttpContext.Current.ApplicationInstance.CompleteRequest()
                    End If

                    If ddlFile.SelectedValue = "Screen" Then
                        Dim whereCompanyDetail As String = ""
                        If Not String.IsNullOrEmpty(ddlSubCompany.SelectedValue) Then
                            whereCompanyDetail = " AND c.CompanyDetailId=@CompanyDetailId "
                        End If

                        Dim myCmd As New SqlCommand("Rpt_ProductionSummaryBlindsPivot")
                        myCmd.CommandType = CommandType.StoredProcedure

                        myCmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text))
                        myCmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text))
                        myCmd.Parameters.AddWithValue("@CompanyId", ddlCompany.SelectedValue)

                        If String.IsNullOrEmpty(ddlSubCompany.SelectedValue) Then
                            myCmd.Parameters.AddWithValue("@CompanyDetailId", DBNull.Value)
                        Else
                            myCmd.Parameters.AddWithValue("@CompanyDetailId", ddlSubCompany.SelectedValue)
                        End If

                        Dim ds As DataSet = reportClass.GetReportData(myCmd)
                        gvList.DataSource = ds
                        gvList.DataBind()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmit_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Companys ORDER BY Id ASC"
            If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                thisQuery = "SELECT * FROM Companys WHERE Id<>'1' ORDER BY Id ASC"
            End If
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer Service" Then
                thisQuery = "SELECT * FROM Companys WHERE Id='" & Session("CompanyId") & "' ORDER BY Id ASC"
            End If
            ddlCompany.DataSource = reportClass.GetListData(thisQuery)
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCompany.Enabled = False
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                ddlCompany.Enabled = True
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindCompany", ex.ToString()}
                MailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindSubCompany(companyId As String)
        ddlSubCompany.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlSubCompany.DataSource = reportClass.GetListData("SELECT * FROM CompanyDetails WHERE CompanyId='" & companyId & "'")
                ddlSubCompany.DataTextField = "Name"
                ddlSubCompany.DataValueField = "Id"
                ddlSubCompany.DataBind()

                If ddlSubCompany.Items.Count > 1 Then
                    ddlSubCompany.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlSubCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindSubCompany", ex.ToString()}
                mailingClass.WebError(dataMailing)
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


    Protected Sub gvList_RowCreated(sender As Object, e As GridViewRowEventArgs)
        If ddlType.SelectedValue = "Blind Orders" Then

            If e.Row.RowType = DataControlRowType.DataRow OrElse e.Row.RowType = DataControlRowType.Header Then
                e.Row.Cells(0).Visible = False

                For i As Integer = 0 To e.Row.Cells.Count - 1
                    If e.Row.Cells(i).Text = "CustomerName" Then
                        e.Row.Cells(i).Text = "Customer Name"
                    End If
                Next
            End If

        End If
    End Sub
End Class
