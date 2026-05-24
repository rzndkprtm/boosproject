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
        End If
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/order/rework/detail?reworkid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                    End If
                End Try
            ElseIf e.CommandName = "ToOrder" Then
                MessageError(False, String.Empty)
                Try
                    Dim url As String = String.Format("~/order/detail?orderid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
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
        Session("reworkId") = String.Empty
        Session("headerId") = String.Empty
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

            Dim thisData As DataTable = orderClass.GetDataTableSP("sp_GetOrderListReworks", params)
            gvList.DataSource = thisData
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
