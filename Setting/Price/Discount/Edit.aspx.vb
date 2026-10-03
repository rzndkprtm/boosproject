Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Discount_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/discount", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("productdiscountid")) Then
            Response.Redirect("~/setting/price/discount", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("productdiscountid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataProduct(ddlType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlProduct.SelectedValue = "" Then
                MessageError(True, "PRODUCT IS REQUIRED !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If
            If txtDiscount.Text = "" Then
                MessageError(True, "DISCOUNT IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceProductDiscounts SET Type=@Type, Method=@Method, DataId=@DataId, Discount=@Discount, Description=@Description, Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DataId", ddlProduct.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceProductDiscounts", lblId.Text, Session("LoginId").ToString(), "Price Product Discount Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/discount", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/discount", False)
    End Sub

    Protected Sub BindData(productDiscountId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceProductDiscounts WHERE Id='" & productDiscountId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/price/discount", False)
                Exit Sub
            End If

            BindDataProduct(myData("Type").ToString())

            ddlType.SelectedValue = myData("Type").ToString()
            ddlProduct.SelectedValue = myData("DataId").ToString()
            ddlMethod.SelectedValue = myData("Method").ToString()
            Dim discount As Decimal
            If myData("Discount") IsNot DBNull.Value AndAlso Decimal.TryParse(myData("Discount").ToString(), discount) Then
                txtDiscount.Text = discount.ToString("0.##")
            Else
                txtDiscount.Text = ""
            End If
            txtDescription.Text = myData("Description").ToString()
            ddlStatus.SelectedValue = myData("Status").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataProduct(type As String)
        ddlProduct.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(type) Then
                Dim thisQuery As String = String.Empty
                If type = "Designs" Then
                    thisQuery = "SELECT Id, Name FROM Designs WHERE Active=1"
                End If
                If type = "Blinds" Then
                    thisQuery = "SELECT Id, Name FROM Blinds WHERE Active=1"
                End If

                If Not String.IsNullOrEmpty(thisQuery) Then
                    ddlProduct.DataSource = settingClass.GetDataTable(thisQuery)
                    ddlProduct.DataTextField = "Name"
                    ddlProduct.DataValueField = "Id"
                    ddlProduct.DataBind()

                    If ddlProduct.Items.Count > 0 Then
                        ddlProduct.Items.Insert(0, New ListItem("", ""))
                    End If
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
