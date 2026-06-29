Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Order_EditCosting
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("boos")) Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("boos").ToString()
        Dim queryRow As DataRow = orderClass.GetDataRow("SELECT QueryString FROM OrderActionContext WHERE Id='" & lblId.Text & "'")
        If queryRow Is Nothing Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        Dim httpUtility = Web.HttpUtility.ParseQueryString(queryRow(0).ToString())
        lblHeaderId.Text = Convert.ToString(httpUtility("headerid"))
        lblItemId.Text = Convert.ToString(httpUtility("itemid"))

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblHeaderId.Text, lblItemId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using tran As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Using delFinal As New SqlCommand("DELETE FROM OrderCostings WHERE ItemId=@ItemId AND Type='Final'", thisConn, tran)
                            delFinal.Parameters.Add("@ItemId", SqlDbType.Int).Value = lblItemId.Text
                            delFinal.ExecuteNonQuery()
                        End Using

                        For Each row As GridViewRow In gvList.Rows
                            If row.RowType = DataControlRowType.DataRow Then
                                Dim costingId As String = gvList.DataKeys(row.RowIndex).Values("Id").ToString()

                                Dim txtNewSellPrice As TextBox = CType(row.FindControl("txtNewSellPrice"), TextBox)
                                Dim newSell As Decimal = 0
                                Decimal.TryParse(txtNewSellPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, newSell)

                                Dim txtNewBuyPrice As TextBox = CType(row.FindControl("txtNewBuyPrice"), TextBox)
                                Dim newBuy As Decimal = 0
                                Decimal.TryParse(txtNewBuyPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, newBuy)

                                Using thisCmd As New SqlCommand("UPDATE OrderCostings SET SellPrice=@SellPrice, BuyPrice=@BuyPrice WHERE Id=@Id", thisConn, tran)
                                    thisCmd.Parameters.AddWithValue("@Id", costingId)
                                    thisCmd.Parameters.Add("@SellPrice", SqlDbType.Decimal).Value = newSell
                                    thisCmd.Parameters.Add("@BuyPrice", SqlDbType.Decimal).Value = newBuy
                                    thisCmd.ExecuteNonQuery()
                                End Using
                            End If
                        Next

                        Dim buyPrice As Decimal = 0
                        Dim sellPrice As Decimal = 0

                        Using cmdSum As New SqlCommand("SELECT ISNULL(SUM(CASE WHEN Type='Base' THEN BuyPrice WHEN Type='Discount' THEN -BuyPrice WHEN Type='Surcharge' THEN BuyPrice ELSE 0 END),0) AS TotalBuy, ISNULL(SUM(CASE WHEN Type='Base' THEN SellPrice WHEN Type='Discount' THEN -SellPrice WHEN Type='Surcharge' THEN SellPrice ELSE 0 END),0) AS TotalSell FROM OrderCostings WHERE ItemId=@ItemId", thisConn, tran)
                            cmdSum.Parameters.Add("@ItemId", SqlDbType.Int).Value = lblItemId.Text

                            Using rd = cmdSum.ExecuteReader()
                                If rd.Read() Then
                                    buyPrice = Convert.ToDecimal(rd("TotalBuy"))
                                    sellPrice = Convert.ToDecimal(rd("TotalSell"))
                                End If
                            End Using
                        End Using

                        Dim dataCosting As Object() = {lblHeaderId.Text, lblItemId.Text, 0, "Final", "Final Cost This Item", buyPrice, sellPrice}
                        orderClass.OrderCostings(dataCosting)

                        dataLog = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Price"}
                        orderClass.Logs(dataLog)
                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            Dim salesClass As New SalesClass
            salesClass.RefreshData(lblCompanyId.Text)

            Response.Redirect(String.Format("~/order/detail?orderid={0}", lblHeaderId.Text), False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect(String.Format("~/order/detail?orderid={0}", lblHeaderId.Text), False)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim costingArray As Object() = {lblHeaderId.Text, lblItemId.Text, ddlAddItem.SelectedValue, "Surcharge", txtAddDescription.Text, txtAddBuyPrice.Text, txtAddSellPrice.Text}
            orderClass.OrderCostings(costingArray)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            dataLog = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Price"}
            orderClass.Logs(dataLog)

            Response.Redirect(String.Format("~/order/editcosting?boos={0}", lblId.Text), False)
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

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            dataLog = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Price"}
            orderClass.Logs(dataLog)

            Response.Redirect(String.Format("~/order/editcosting?boos={0}", lblId.Text), False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnNote_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim costingArray As Object() = {lblHeaderId.Text, lblItemId.Text, 0, "Note", txtNote.Text, 0, 0}
            orderClass.OrderCostings(costingArray)
            orderClass.FinalCostItem(lblHeaderId.Text, lblItemId.Text)

            dataLog = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Price | Add Note"}
            orderClass.Logs(dataLog)

            Response.Redirect(String.Format("~/order/editcosting?boos={0}", lblId.Text), False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(headerId As String, itemId As String)
        Try
            Dim headerData As DataRow = orderClass.GetDataRow("SELECT OrderHeaders.*, Customers.CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order/", False)
                Exit Sub
            End If
            lblOrderStatus.Text = headerData("Status").ToString()
            lblCompanyId.Text = headerData("CompanyId").ToString()

            hTitle.InnerText = orderClass.GetItemData("SELECT ISNULL(CONVERT(VARCHAR(200), OrderDetails.Room), '') + ' - ' + ISNULL(CONVERT(VARCHAR(200), Products.Name), '') FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@ItemId", lblItemId.Text),
                New SqlParameter("@CompanyId", lblCompanyId.Text)
            }
            gvList.DataSource = orderClass.GetDataTableSP("sp_GetOrderCostingByItem", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = False
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

    Protected Function BindCurrency() As String
        Try
            If lblCompanyId.Text = "3" Then Return "Rp"
            Return "$"
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function

    Protected Function BindDescription(designName As String, description As String, orderNote As String) As String
        Try
            If designName = "Service" AndAlso Not String.IsNullOrEmpty(orderNote) Then
                Return description & "<br />" & orderNote
            End If
            Return description
        Catch ex As Exception
            Return "ERROR"
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
