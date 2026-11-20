Imports System.Data
Imports System.Data.SqlClient

Partial Class Schedule_Detail
    Inherits Page

    Dim scheduleClass As New ShipmentScheduleClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/schedule", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("scheduleid")) Then
            Response.Redirect("~/schedule", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("scheduleid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataOrder(lblId.Text, txtSearch.Text)
        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        BindOrderDropdown()
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(lblId.Text, txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindDataOrder(lblId.Text, txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindDataOrder(lblId.Text, txtSearch.Text)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnAddOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = scheduleClass.CreateShipmentScheduleDetailId()

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO ShipmentScheduleDetails VALUES (@Id, @ScheduleId, @HeaderId, 'Active')", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@ScheduleId", lblId.Text)
                    thisCmd.Parameters.AddWithValue("@HeaderId", ddlOrderId.SelectedValue)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"ShipmentScheduleDetails", thisId, Session("LoginId").ToString(), "Created"}
            scheduleClass.Logs(dataLog)

            url = String.Format("~/schedule/detail?scheduleid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE ShipmentScheduleDetails SET Status='Deleted' WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"ShipmentScheduleDetails", thisId, Session("LoginId").ToString(), "Deleted"}
            scheduleClass.Logs(dataLog)

            url = String.Format("~/schedule/detail?scheduleid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataOrder(scheduleId As String, searchText As String)
        Try
            Dim headerData As DataRow = scheduleClass.GetDataRow("SELECT * FROM ShipmentSchedules WHERE Id='" & scheduleId & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/schedule", False)
                Exit Sub
            End If

            lblShipmentNumber.Text = headerData("ShipmentNumber").ToString()
            lblDestinationAirport.Text = headerData("DestinationAirport").ToString()
            If Not String.IsNullOrEmpty(headerData("ETD").ToString()) Then
                lblETD.Text = Convert.ToDateTime(headerData("ETD")).ToString("dd MMM yyyy")
            End If
            If Not String.IsNullOrEmpty(headerData("ETA").ToString()) Then
                lblETA.Text = Convert.ToDateTime(headerData("ETA")).ToString("dd MMM yyyy")
            End If

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@ShipmentScheduleId", scheduleId),
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText))
            }
            gvList.DataSource = scheduleClass.GetDataTableSP("sp_ShipmentScheduleDetails_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindOrderDropdown()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT OrderHeaders.Id, OrderHeaders.OrderId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN Companys ON Customers.CompanyId=Companys.Id WHERE OrderHeaders.Active=1 AND (OrderHeaders.Status='In Production' OR OrderHeaders.Status='On Hold') AND Companys.Id<>'3' AND NOT EXISTS (SELECT 1 FROM ShipmentScheduleDetails WHERE ShipmentScheduleDetails.HeaderId=OrderHeaders.Id AND ShipmentScheduleDetails.Status='Active') ORDER BY OrderHeaders.OrderId DESC", thisConn)

                    thisConn.Open()

                    ddlOrderId.Items.Clear()
                    ddlOrderId.Items.Add(New ListItem("-- SELECT ORDER --", ""))

                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            ddlOrderId.Items.Add(New ListItem(dr("OrderId").ToString(), dr("Id").ToString()))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
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
