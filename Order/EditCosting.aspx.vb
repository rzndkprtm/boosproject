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
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("boos")) Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("boos").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
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

                                Using cmd As New SqlCommand("UPDATE OrderCostings SET SellPrice=@SellPrice, BuyPrice=@BuyPrice WHERE Id=@Id", thisConn, tran)
                                    cmd.Parameters.AddWithValue("@Id", costingId)
                                    cmd.Parameters.Add("@SellPrice", SqlDbType.Decimal).Value = newSell
                                    cmd.Parameters.Add("@BuyPrice", SqlDbType.Decimal).Value = newBuy
                                    cmd.ExecuteNonQuery()
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

    Protected Sub BindData(id As String)
        Try
            Dim queryString As String = orderClass.GetItemData("SELECT QueryString FROM OrderActionContext WHERE Id='" & id & "'")
            If String.IsNullOrEmpty(queryString) Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim httpUtility = Web.HttpUtility.ParseQueryString(queryString)
            lblHeaderId.Text = Convert.ToString(httpUtility("headerid"))
            lblItemId.Text = Convert.ToString(httpUtility("itemid"))

            Dim headerData As DataRow = orderClass.GetDataRow("SELECT OrderHeaders.*, Customers.CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & lblHeaderId.Text & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order/", False)
                Exit Sub
            End If
            lblOrderStatus.Text = headerData("Status").ToString()
            lblCompanyId.Text = headerData("CompanyId").ToString()

            hTitle.InnerText = orderClass.GetItemData("SELECT ISNULL(CONVERT(VARCHAR(200), OrderDetails.Room), '') + ' - ' + ISNULL(CONVERT(VARCHAR(200), Products.Name), '') FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.Id = '" & lblItemId.Text & "'")

            Dim thisQuery As String = "SELECT *, FORMAT(BuyPrice, 'C', 'en-US') AS BuyPricing, FORMAT(SellPrice, 'C', 'en-US') AS SellPricing FROM OrderCostings WHERE ItemId='" & lblItemId.Text & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
            If lblCompanyId.Text = "3" Then
                thisQuery = "SELECT *, FORMAT(BuyPrice, 'C', 'id-ID') AS BuyPricing, FORMAT(SellPrice, 'C', 'en-US') AS SellPricing FROM OrderCostings WHERE ItemId='" & lblItemId.Text & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
            End If
            gvList.DataSource = orderClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = False

            divNote.Visible = False
            Dim itemNote As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Designs.Name AS DesignName, Designs.Type AS DesignType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderDetails.Id='" & lblItemId.Text & "' AND OrderDetails.HeaderId='" & lblHeaderId.Text & "'")
            If Not itemNote Is Nothing Then
                Dim designName As String = itemNote("DesignName").ToString()
                Dim designType As String = itemNote("DesignType").ToString()
                Dim note As String = itemNote("Notes").ToString()
                If designType = "Services" Then
                    divNote.Visible = True
                    lblNote.Text = String.Format("* {0}", note)
                End If
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
