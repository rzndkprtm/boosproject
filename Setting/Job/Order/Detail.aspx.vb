Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Job_Order_Detail
    Inherits Page

    Dim jobClass As New JobClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/job/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderjobid")) Then
            Response.Redirect("~/setting/job/order", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("orderjobid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindJobSheet(lblId.Text)
            BindData(lblId.Text, ddlJobSheet.SelectedValue)
        End If
    End Sub

    Protected Sub ddlJobSheet_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(lblId.Text, ddlJobSheet.SelectedValue)
    End Sub

    Protected Sub BindJobSheet(orderJobId As String)
        ddlJobSheet.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(orderJobId) Then
                ddlJobSheet.DataSource = jobClass.GetDataTable("SELECT OrderJobDetails.JobSheetId AS JobSheetId, JobSheets.Name AS JobSheetName FROM OrderJobDetails INNER JOIN JobSheets ON OrderJobDetails.JobSheetId=JobSheets.Id WHERE OrderJobDetails.OrderJobId='" & orderJobId & "' GROUP BY OrderJobDetails.JobSheetId, JobSheets.Name ORDER BY JobSheets.Name ASC")
                ddlJobSheet.DataTextField = "JobSheetName"
                ddlJobSheet.DataValueField = "JobSheetId"
                ddlJobSheet.DataBind()

                If ddlJobSheet.Items.Count > 0 Then
                    ddlJobSheet.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlJobSheet.Items.Clear()
        End Try
    End Sub

    Protected Sub BindData(orderJobId As String, jobSheetId As String)
        gvList.DataSource = Nothing : gvList.DataBind()
        Try
            Dim thisData As DataRow = jobClass.GetDataRow("SELECT OrderJobs.*, CASE WHEN OrderJobs.Active=1 THEN 'Yes' ELSE 'No' END AS DataActive, OrderHeaders.OrderId AS OrderId, Logins.FullName AS ConvertBy FROM OrderJobs LEFT JOIN OrderHeaders ON OrderJobs.HeaderId=OrderHeaders.Id LEFT JOIN Logins ON OrderJobs.CreatedBy=Logins.Id where OrderJobs.Id='" & orderJobId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/job/order", False)
                Exit Sub
            End If

            lblOrderId.Text = thisData("OrderId").ToString()
            lblJobNumber.Text = thisData("JobNumber").ToString()
            lblWorkOrder.Text = thisData("WorkOrder").ToString()
            lblJobNote.Text = thisData("JobNote").ToString()
            lblActive.Text = thisData("DataActive").ToString()
            lblConverted.Text = "Convert By : " & thisData("ConvertBy").ToString() & " on " & Convert.ToDateTime(thisData("CreatedDate")).ToString("dd MMM yyyy HH:mm")

            If Not String.IsNullOrWhiteSpace(orderJobId) AndAlso Not String.IsNullOrWhiteSpace(jobSheetId) Then
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@OrderJobId", orderJobId),
                    New SqlParameter("@JobSheetId", jobSheetId)
                }
                gvList.DataSource = jobClass.GetDataTableSP("sp_OrderJobs_Detail_List", params)
                gvList.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerHtml = message
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
