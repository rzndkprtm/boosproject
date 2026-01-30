Imports System.Data

Partial Class Order_Printing
    Inherits Page

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim dataMailing As Object() = Nothing
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
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

            Dim headerData As DataRow = orderClass.GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & lblHeaderId.Text & "'")
            If headerData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            lblOrderId.InnerText = headerData("OrderId").ToString()
            lblOrderNumber.InnerText = headerData("OrderNumber").ToString()
            lblOrderName.InnerText = headerData("OrderName").ToString()

            Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.Name AS ProductName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & lblItemId.Text & "' AND OrderDetails.Active=1")
            If detailData Is Nothing Then
                Response.Redirect("~/order/detail?orderid='" & lblHeaderId.Text & "'", False)
                Exit Sub
            End If

            descProduct.InnerHtml = String.Format("{0}, {1}", detailData("Room").ToString(), detailData("ProductName").ToString())
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
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
End Class
