Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Rework_Default
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
            If Session("RoleName") = "Customer" Then
                BindDataOrder(Session("CustomerId").ToString())
            End If
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim reworkId As String = String.Empty

            Dim isReworkOrderUnsubmit As Boolean = orderClass.IsReworkOrderUnsubmitted(ddlOrderId.SelectedValue)
            If isReworkOrderUnsubmit = True Then
                reworkId = orderClass.GetItemData("SELECT Id FROM OrderReworks WHERE HeaderId='" & ddlOrderId.SelectedValue & "' AND Status='Unsubmitted' AND Active=1")

                url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
                Response.Redirect(url, False)
                Exit Sub
            End If
            If isReworkOrderUnsubmit = False Then
                reworkId = orderClass.GetNewOrderReworkId()

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO OrderReworks VALUES (@Id, @HeaderId, NULL, 'Unsubmitted', '', @CreatedBy, GETDATE(), NULL, 1)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", reworkId)
                        thisCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        thisCmd.Parameters.AddWithValue("@HeaderId", ddlOrderId.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim directory As String = Server.MapPath(String.Format("~/File/Rework/{0}/", reworkId))
                If Not IO.Directory.Exists(directory) Then
                    IO.Directory.CreateDirectory(directory)
                End If

                dataLog = {"OrderReworks", reworkId, Session("LoginId").ToString(), "Order Rework Created"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Active=0 WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderReworks", thisId, Session("LoginId").ToString(), "Order Rework Deleted"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/rework")
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub BindDataOrder(search As String, status As String, active As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", search.Trim()),
                New SqlParameter("@Status", status),
                New SqlParameter("@CompanyId", If(Session("CompanyId"), DBNull.Value)),
                New SqlParameter("@RoleName", If(Session("RoleName"), DBNull.Value)),
                New SqlParameter("@LevelName", If(Session("LevelName"), DBNull.Value)),
                New SqlParameter("@CustomerId", If(Session("CustomerId"), DBNull.Value)),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@Active", active)
            }

            gvList.DataSource = orderClass.GetDataTableSP("sp_OrderReworks_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            aCreate.Visible = LoginAccess("Create")
            divActive.Visible = LoginAccess("Active")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub BindDataOrder(customerId As String)
        ddlOrderId.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                ddlOrderId.DataSource = orderClass.GetDataTable("SELECT Id, OrderId + ' (' + OrderNumber + ')' AS DataOrder FROM OrderHeaders WHERE CustomerId='" & customerId & "' AND (Status = 'Shipped Out' OR Status = 'Completed') AND Active=1")
                ddlOrderId.DataTextField = "DataOrder"
                ddlOrderId.DataValueField = "Id"
                ddlOrderId.DataBind()

                If ddlOrderId.Items.Count > 1 Then
                    ddlOrderId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlOrderId.Items.Clear()
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

    Protected Function GetOrderId(headerId As String) As String
        If Not String.IsNullOrEmpty(headerId) Then
            Return orderClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & headerId & "'")
        End If
        Return String.Empty
    End Function

    Protected Function VisibleDelete(status As String) As Boolean
        Try
            If Session("RoleName") = "Developer" Then Return True
            If status = "Unsubmitted" Then Return True
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Protected Function VisibleLog() As Boolean
        Try
            If Session("RoleName") = "Developer" Then Return True
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

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
